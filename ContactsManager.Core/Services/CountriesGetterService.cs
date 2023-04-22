using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountriesGetterService : ICountriesGetterService
	{
		// Private Field
		private readonly ICountriesRepository _countriesRepository;

		public CountriesGetterService(ICountriesRepository countriesRepository)
		{
			_countriesRepository = countriesRepository;
		}

		public async Task<List<CountryResponse>> GetAllCountries()
		{
			List<Country> allCountries = await _countriesRepository.GetAllCountries();

			List<CountryResponse> countryResponseList = allCountries.Select(country => country.ToCountryReponse()).ToList();

			return countryResponseList;
		}

		public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
		{
			if (countryId == null) return null;

			Country? finalCountry = await _countriesRepository.GetCountryById(countryId.Value);

			if (finalCountry == null) return null;

			return finalCountry.ToCountryReponse();
		}

	}
}