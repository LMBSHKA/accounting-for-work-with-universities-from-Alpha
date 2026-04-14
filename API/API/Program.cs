using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		builder.Services.AddOpenApi();

		var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
			?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

		builder.Services.AddDbContext<AppDbContext>(options =>
			options.UseNpgsql(connectionString));

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
		}

		app.UseHttpsRedirection();
		app.UseAuthorization();
		app.MapControllers();

		app.Run();
	}
}