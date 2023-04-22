using ContactsManager.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactsManager.Filters.ActionFilters
{
	public class PersonsListActionFilter : IActionFilter
	{
		private readonly ILogger<PersonsListActionFilter> _logger;

		public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
		{
			_logger = logger;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{

			_logger.LogWarning("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

			// Convert returned controller into it's class
			PersonsController personsController = (PersonsController)context.Controller;

			// Access action method arguments passed from OnActionsExecuting method
			IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

			if (parameters != null)
			{
				if (parameters.ContainsKey("searchBy"))
				{
					personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
				}

				if (parameters.ContainsKey("searchString"))
				{
					personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
				}

				if (parameters.ContainsKey("sortBy"))
				{
					personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
				}
				else
				{
					personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
				}

				if (parameters.ContainsKey("sortOrder"))
				{
					personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
				}
				else
				{
					personsController.ViewData["CurrentSortOrder"] = SortOrderOptions.ASC.ToString();
				}
			}

			// Supply Search Parameters
			personsController.ViewBag.SearchFields = new Dictionary<string, string>()
			{
				{ nameof(PersonResponse.PersonName), "Person Name" },
				{ nameof(PersonResponse.Email), "Email" },
				{ nameof(PersonResponse.DateOfBirth), "Date of Birth" },
				{ nameof(PersonResponse.Age), "Age" },
				{ nameof(PersonResponse.Gender), "Gender" },
				{ nameof(PersonResponse.Country), "Country" },
				{ nameof(PersonResponse.Address), "Address" },
				{ nameof(PersonResponse.ReceiveNewsLetters), "Receive News Letters" },
			};
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			_logger.LogWarning("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

			// Passing action arguments from executing to executed
			context.HttpContext.Items["arguments"] = context.ActionArguments;

			// Data binding values chckec
			IDictionary<string, object?> param = context.ActionArguments;

			// Verify
			if (param.ContainsKey("searchBy"))
			{
				// Validate seachBy parameter value
				string? searchBy = Convert.ToString(param["searchBy"]);
				if (!string.IsNullOrEmpty(searchBy))
				{
					var searchByOptions = new List<string>()
					{
						nameof(PersonResponse.PersonName),
						nameof(PersonResponse.Email),
						nameof(PersonResponse.DateOfBirth),
						nameof(PersonResponse.Gender),
						nameof(PersonResponse.CountryId),
						nameof(PersonResponse.Address),
					};

					// Reset searchBy parameter value
					if (searchByOptions.Any(temp => temp == searchBy) == false)
					{
						_logger.LogInformation("searchBy actual value {searchBye}", searchBy);
						context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
						_logger.LogInformation("searchBy updated value {searchBye}", Convert.ToString(param["searchBy"]));
					}
				}
			}
		}
	}
}
