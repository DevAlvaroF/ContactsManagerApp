using ContactsManager.Filters.ActionFilters;
using ContactsManager.Filters.AuthorizationFilters;
using ContactsManager.Filters.ResourceFilters;
using ContactsManager.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactsManager.Controllers
{
	[Route("persons")]
	//[TypeFilter(typeof(HandleExceptionFilter))]
	public class PersonsController : Controller
	{
		private readonly IPersonsGetterService _personsGetterService;
		private readonly IPersonsAdderService _personsAdderService;
		private readonly IPersonsSorterService _personsSorterService;
		private readonly IPersonsUpdaterService _personsUpdaterService;
		private readonly IPersonsDeleterService _personsDeleterService;

		private readonly ICountriesGetterService _countriesGetterService;
		private readonly ILogger<PersonsController> _logger;

		public PersonsController(IPersonsGetterService personsGetterService, IPersonsAdderService personsAdderService,
IPersonsSorterService personsSorterService, IPersonsUpdaterService personsUpdaterService, IPersonsDeleterService personsDeleterService, ICountriesGetterService countriesGetterService, ILogger<PersonsController> logger)
		{
			_logger = logger;

			// Countries Services
			_countriesGetterService = countriesGetterService;

			// Persons Services
			_personsGetterService = personsGetterService;
			_personsGetterService = personsGetterService;
			_personsAdderService = personsAdderService;
			_personsSorterService = personsSorterService;
			_personsUpdaterService = personsUpdaterService;
			_personsDeleterService = personsDeleterService;
		}

		[Route("index")]
		[Route("/")]
		[TypeFilter(typeof(PersonsListActionFilter))]
		[ResponseHeaderFilterFactory("X-MethodKey", "Custom-Value")]
		[TypeFilter(typeof(PersonsListResultFilter))]
		public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
		{
			List<PersonResponse> personsList = await _personsGetterService.GetAllPersons();

			// Searching
			if (searchBy != null)
			{
				List<PersonResponse> filteredPersons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);
				personsList = filteredPersons;
			}

			// Sorting
			personsList = await _personsSorterService.GetSortedPersons(personsList, sortBy, sortOrder);

			return View(personsList);
		}


		[Route("create")]
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			// Get All Countries
			List<CountryResponse> allCountries = await _countriesGetterService.GetAllCountries();

			// Convert to SelectLiist item to implement with TagHelpers
			ViewBag.AllCountries = allCountries.Select(tmp => new SelectListItem()
			{
				Text = tmp.CountryName,
				Value = tmp.CountryId.ToString(),
			});

			return View();
		}

		[HttpPost]
		[Route("create")]
		[PersonsCreateAndEditFilterFactory]
		[TypeFilter(typeof(FeatureDisableResourceFilter), Arguments = new object[] { false })]
		public async Task<IActionResult> Create(PersonAddRequest personRequest)
		{
			await Task<IActionResult>.FromResult(0);

			// Add Person (discarding response)
			_ = _personsAdderService.AddPerson(personRequest);

			// Redirect to Index() action method through another request
			return RedirectToAction("Index", "Persons");

		}

		[HttpGet]
		[Route("edit/{personId}")]
		[TypeFilter(typeof(TokenResultFilter))] // Simulates addition of login cookkie
		public async Task<IActionResult> Edit(Guid personId)
		{

			PersonResponse? personFromId = await _personsGetterService.GetPersonByPersonId(personId);

			if (personFromId == null) return RedirectToAction("Index", "Persons");

			// Convert person to update Request with extension method
			PersonUpdateRequest personUpdateRequest = personFromId.ToPersonUpdateRequest();

			// Get All Countries
			List<CountryResponse> allCountries = await _countriesGetterService.GetAllCountries();
			// Convert to SelectList item to implement with TagHelpers
			ViewBag.AllCountries = allCountries.Select(tmp => new SelectListItem()
			{
				Text = tmp.CountryName,
				Value = tmp.CountryId.ToString(),
			});

			return View(personUpdateRequest);
		}

		[HttpPost]
		[Route("edit/{personId}")]
		[PersonsCreateAndEditFilterFactory]
		[TypeFilter(typeof(TokenAuthorizationFilter))] // Verifies cookie

		public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
		{
			PersonResponse? personById = await _personsGetterService.GetPersonByPersonId(personRequest.PersonId);

			if (personById == null) return RedirectToAction("Index", "Persons");

			// Update Person
			_ = await _personsUpdaterService.UpdatePerson(personRequest);
			return RedirectToAction("Index", "Persons");

		}

		[HttpGet]
		[Route("delete/{personId}")]
		public async Task<IActionResult> Delete(Guid personId)
		{
			// Check if person exists
			PersonResponse? personFromId = await _personsGetterService.GetPersonByPersonId(personId);

			// If person does not exist, redirect to all persons
			if (personFromId == null) return RedirectToAction("Index", "Persons");

			return View(personFromId);
		}

		[HttpPost]
		[Route("delete/{personId}")]
		public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
		{
			// Get person model with model binding
			PersonResponse? personById = await _personsGetterService.GetPersonByPersonId(personUpdateRequest.PersonId);

			if (personById == null) return RedirectToAction("Index", "Persons");

			// Update Person
			bool deleteResult = await _personsDeleterService.DeletePerson(personById.PersonId);
			if (deleteResult)
			{
				return RedirectToAction("Index", "Persons");
			}

			// If failed
			ViewBag.Errors = new List<string>() { "Failed to delete person, PersonId does not exist" };
			return View(personById);
		}

		[Route("pdf")]
		public async Task<IActionResult> PersonsPDF()
		{
			// ViewBag for Column Headers
			// Supply Search Parameters
			ViewBag.SearchFields = new Dictionary<string, string>()
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

			// Get all persons to render Table
			List<PersonResponse> allPersons = await _personsGetterService.GetAllPersons();

			// Return Rotativa PDFView
			return new ViewAsPdf("PersonsPDF", allPersons, ViewData)
			{
				PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
			};
		}

		[Route("csv")]
		public async Task<IActionResult> PersonsCSV()
		{
			// Get MemoryStream
			MemoryStream memoryStream = await _personsGetterService.GetPersonsCSV();

			return File(memoryStream, "application/octet-stream", "persons_export.csv");
		}

		[Route("excel")]
		public async Task<IActionResult> PersonsExcel()
		{
			// Get MemoryStream
			MemoryStream memoryStream = await _personsGetterService.GetPersonsExcel();

			return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons_export.xlsx");
		}
	}
}
