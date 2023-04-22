using ServiceContracts.DTO;
namespace ServiceContracts
{
	/// <summary>
	/// Represents business logic for manipulating Country entity
	/// </summary>
	public interface ICountriesGetterService
	{

		/// <summary>
		/// Returns all coutrines from the list
		/// </summary>
		/// <returns></returns>
		Task<List<CountryResponse>> GetAllCountries();

		/// <summary>
		/// Returns the response of a country by ID
		/// </summary>
		/// <param name="countryId">CountryId (GUID) to search</param>
		/// <returns>Matching country as CountryResponse object</returns>
		Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);

	}


}