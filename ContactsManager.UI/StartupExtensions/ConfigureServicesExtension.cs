using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
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
			services.AddTransient<ResponseHeaderActionFilter>(); // global filter
			services.AddTransient<PersonsCreateAndEditPostActionFilter>(); // global filter

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


				// Anti Forgery globally for POST Action methods
				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

			});

			// =======================================
			// DbContext service for EntityFramework on MS SQL
			// =======================================
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				// Consider Moving to SQLite
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});

			// ===================================================
			// Add identity to the website with 2 IdentityEntities
			// ===================================================
			services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
			{
				options.Password.RequiredLength = 5;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = true;
				options.Password.RequireDigit = false;
				options.Password.RequiredUniqueChars = 3;

			}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>().AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();


			// ===================================================
			// Enforcing authorization policy (user must be autheticated)
			// for all the action methods
			// ===================================================

			services.AddAuthorization(options =>
			{
				options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

				// Custom Policy
				options.AddPolicy("CustomBlockIfLogin", policy =>
				{
					policy.RequireAssertion(context =>
					{
						// If not authenticated or no identity
						if (context.User.Identity == null || (!context.User.Identity.IsAuthenticated))
						{
							// Visible: return true
							return true;
						}

						// Authenticated
						// Invisible: return false
						return false;


					});
				});
			});

			// ===================================================
			// When identity cookie not valid/not supplied
			// the fallback method goes to the following path
			// ===================================================

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/account/login";
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
