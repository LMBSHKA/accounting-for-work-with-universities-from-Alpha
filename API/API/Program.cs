using API.OpenApi;
using Application.Abstractions.Authentication;
using Application.Abstractions.Discussions;
using Application.Abstractions.Iterations;
using Application.Abstractions.Meetings;
using Application.Abstractions.Persistence;
using Application.Abstractions.Projects;
using Application.Abstractions.Students;
using Application.Authentication.Services;
using Application.Discussions.Services;
using Application.Iterations.Services;
using Application.Meetings.Services;
using Application.Projects.Services;
using Application.Students.Services;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

internal class Program
{
	private const string CorsPolicyName = "FrontendCors";

	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		//builder.Services.AddCors(options =>
		//{
		//	options.AddPolicy(CorsPolicyName, policy =>
		//	{
		//		policy
		//			.WithOrigins(
		//				"http://localhost:5173",
		//				"http://127.0.0.1:5173",
		//				"https://front-web-accounting-alpha3-ptf0o2br9.vercel.app"
		//			)
		//			.AllowAnyMethod()
		//			.AllowAnyHeader()
		//			.AllowCredentials();
		//	});
		//});

		const string CorsPolicyName = "Frontend";

		builder.Services.AddCors(options =>
		{
			options.AddPolicy(CorsPolicyName, policy =>
			{
				policy
					.SetIsOriginAllowed(_ => true)
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials();
			});
		});

		builder.Services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders =
				ForwardedHeaders.XForwardedFor |
				ForwardedHeaders.XForwardedProto;

			options.KnownNetworks.Clear();
			options.KnownProxies.Clear();
		});

		builder.Services.AddControllers();

		var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
			?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

		builder.Services.AddDbContext<AppDbContext>(options =>
			options.UseNpgsql(connectionString));

		builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

		var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
			?? throw new InvalidOperationException($"Section '{JwtOptions.SectionName}' was not found.");

		var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

		builder.Services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = jwtOptions.Issuer,

					ValidateAudience = true,
					ValidAudience = jwtOptions.Audience,

					ValidateIssuerSigningKey = true,
					IssuerSigningKey = signingKey,

					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero,

					NameClaimType = "userId",
					RoleClaimType = "systemRole"
				};

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var authorizationHeader = context.Request.Headers.Authorization.ToString();

						if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
							authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
						{
							context.Token = authorizationHeader["Bearer ".Length..].Trim();
							return Task.CompletedTask;
						}

						if (context.Request.Cookies.TryGetValue(jwtOptions.CookieName, out var token) &&
							!string.IsNullOrWhiteSpace(token))
						{
							context.Token = token;
						}

						return Task.CompletedTask;
					}
				};
			});

		builder.Services.AddAuthorization();

		builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
		builder.Services.AddScoped<IProjectService, ProjectService>();
		builder.Services.AddScoped<IDiscussionService, DiscussionService>();
		builder.Services.AddScoped<IStudentService, StudentService>();
		builder.Services.AddScoped<IMeetingService, MeetingService>();
		builder.Services.AddScoped<IIterationService, IterationService>();

		builder.Services.AddOpenApi(options =>
		{
			options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
			options.AddOperationTransformer<AuthOperationTransformer>();

			options.AddDocumentTransformer((document, _, _) =>
			{
				document.Info = new Microsoft.OpenApi.Models.OpenApiInfo
				{
					Title = "Alpha API",
					Version = "v1",
					Description = "API для работы с проектами, командами, встречами и JWT-аутентификацией."
				};

				return Task.CompletedTask;
			});
		});

		var app = builder.Build();

		app.UseForwardedHeaders();

		app.UseCors(CorsPolicyName);

		app.MapOpenApi();

		app.MapScalarApiReference(options =>
		{
			options
				.WithTitle("Alpha API")
				.WithTheme(ScalarTheme.BluePlanet)
				.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
				.AddPreferredSecuritySchemes("Bearer");
		});

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}