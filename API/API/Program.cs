using API.OpenApi;
using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Abstractions.Projects;
using Application.Authentication.Services;
using Application.Projects.Services;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Scalar.AspNetCore;
using System.Text;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		//Cors
		builder.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(builder =>
			{
				builder.SetIsOriginAllowed(origin => true)
					   .AllowAnyMethod()
					   .AllowAnyHeader()
					   .AllowCredentials();
			});
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
