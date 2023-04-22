using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;
namespace ServiceContracts
{
	/// <summary>
	/// Represents business logic for manipulating Country entity
	/// </summary>
	public interface ICountriesAdderService
	{
		/// <summary>
		/// Add a country object to the list of countries
		/// </summary>
		/// <param name="countryAddRequest">Country to add</param>
		/// <returns>Returns the country object after adding it (including newly generated country id)</returns>
		Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

		/// <summary>
		/// Uploads countries form excel file into the database
		/// </summary>
		/// <param name="formFile">Excel file with list of countries</param>
		/// <returns>Returns number of countries added to the database</returns>
		Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
	}


}