using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace ContactsManager.Controllers
{
	[Route("countries")]
	public class CountriesController : Controller
	{
		private readonly ICountriesAdderService _countriesAdderService;

		public CountriesController(ICountriesAdderService countriesAdderService)
		{
			_countriesAdderService = countriesAdderService;
		}

		[Route("upload-excel")]
		[HttpGet]
		public IActionResult UploadExcel()
		{
			return View();
		}

		[Route("upload-excel")]
		[HttpPost]
		public async Task<IActionResult> UploadExcel(IFormFile excelFile)
		{
			if (excelFile == null || excelFile.Length == 0)
			{
				ViewBag.ErrorMessage = "Please Select a .xlsx file";
				return View();
			}
			if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
			{
				ViewBag.ErrorMessage = "Unsupported file extension";
				return View();
			}

			int affectedCountries = await _countriesAdderService.UploadCountriesFromExcelFile(excelFile);

			if (affectedCountries == 0)
			{
				ViewBag.ErrorMessage = "All countries exist already";
				return View();
			}

			ViewBag.Message = $"{affectedCountries} Countries Added to database";
			return View();
		}
	}
}
