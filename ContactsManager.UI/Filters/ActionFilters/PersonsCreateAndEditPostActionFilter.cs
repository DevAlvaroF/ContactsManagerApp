using ContactsManager.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ContactsManager.Filters.ActionFilters
{
	public class PersonsCreateAndEditFilterFactoryAttribute : Attribute, IFilterFactory
	{
		public bool IsReusable => false;

		public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
		{
			var customFilter = serviceProvider.GetRequiredService<PersonsCreateAndEditPostActionFilter>();

			return customFilter;
		}
	}
	public class PersonsCreateAndEditPostActionFilter : IAsyncActionFilter
	{
		private readonly ICountriesGetterService _countriesGetterService;
		private readonly ILogger<PersonsCreateAndEditPostActionFilter> _logger;

		public PersonsCreateAndEditPostActionFilter(ICountriesGetterService countriesGetterService, ILogger<PersonsCreateAndEditPostActionFilter> logger)
		{
			_countriesGetterService = countriesGetterService;
			_logger = logger;
		}
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			// Before ======================================
			if (context.Controller is PersonsController personsController)
			{

				// Validate Binding
				if (!personsController.ModelState.IsValid)
				{
					// Get countries fro dropdown
					List<CountryResponse> allCountries = await _countriesGetterService.GetAllCountries();

					// Get Error values
					personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(val => val.Errors).Select(e => e.ErrorMessage).ToList();

					personsController.ViewBag.AllCountries = allCountries.Select(tmp => new SelectListItem()
					{
						Text = tmp.CountryName,
						Value = tmp.CountryId.ToString(),
					});

					var personRequest = context.ActionArguments["personRequest"];

					// Short circuit
					context.Result = personsController.View(personRequest);
				}
				else
				{
					await next(); // If model is valid Action Method/next filter is executed

				}
			}
			else
			{
				await next(); // If model is valid Action Method/next filter is executed
			}

			// After ==========================================

		}
	}
}
