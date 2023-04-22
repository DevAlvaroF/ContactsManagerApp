using ContactsManager.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace ContactsManager.StartupExtensions
{
	public static class ConfigureServicesExtension
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			// Add Services into IoC Container
			services.AddScoped<ICountriesAdderService, CountriesAdderService>();
			services.AddScoped<ICountriesGetterService, CountriesGetterService>();
			services.AddScoped<IPersonsGetterService, PersonsGetterService>();
			services.AddScoped<IPersonsAdderService, PersonsAdderService>();
			services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
			services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
			services.AddScoped<IPersonsSorterService, PersonsSorterService>();
			services.AddScoped<ICountriesRepository, CountriesRepository>();
			services.AddScoped<IPersonsRepository, PersonsRepository>();
			services.AddTransient<ResponseHeaderActionFilter>();
			services.AddTransient<PersonsCreateAndEditPostActionFilter>();

			// =======================================
			// Adds controllers as services (with filters factory for DI)
			// =======================================
			services.AddControllersWithViews(options =>
			{
				// --------------------------------------------------
				// Global filter that requires 2 arguments
				// --------------------------------------------------
				var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

				options.Filters.Add(new ResponseHeaderActionFilter(logger)
				{
					Key = "X-Global-State",
					Value = "Started"
				});

			});

			// =======================================
			// DbContext service for EntityFramework on MS SQL
			// =======================================
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				// Consider Moving to SQLite
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});




			// =======================================
			// HttpLogging Configuration for end of request messages with Serilog
			// =======================================
			services.AddHttpLogging(options =>
			{
				options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
			});

			return services;
		}
	}
}
