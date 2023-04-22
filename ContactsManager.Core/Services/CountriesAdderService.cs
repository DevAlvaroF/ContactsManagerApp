using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountriesAdderService : ICountriesAdderService
	{
		// Private Field
		private readonly ICountriesRepository _countriesRepository;

		public CountriesAdderService(ICountriesRepository countriesRepository)
		{
			_countriesRepository = countriesRepository;
		}

		public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
		{
			if (countryAddRequest == null)
				throw new ArgumentNullException(nameof(countryAddRequest));
			if (countryAddRequest.CountryName == null)
				throw new ArgumentException(nameof(countryAddRequest.CountryName));

			if (await _countriesRepository.GetCountryByName(countryAddRequest.CountryName) != null)
				throw new ArgumentException("Country name already exists");

			// Convert CountryAddRequestToCountry
			Country country = countryAddRequest.ToCountry();

			//Generate new Guid
			country.CountryId = Guid.NewGuid();

			// Add Country to Database
			await _countriesRepository.AddCountry(country);

			// Convert Country to Response
			return country.ToCountryReponse();
		}

		public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
		{
			// Create memory stream to store excel file
			MemoryStream memoryStream = new MemoryStream();
			int countriesInserted = 0;

			await formFile.CopyToAsync(memoryStream);

			using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
			{
				// Read worksheet (should follow template)
				ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

				// Get filled rows
				int rowCount = workSheet.Dimension.Rows;

				for (int i = 2; i <= rowCount; i++)
				{
					string? cellValue = Convert.ToString(workSheet.Cells[i, 1].Value);
					if (!string.IsNullOrEmpty(cellValue))
					{
						string? countryName = cellValue;

						// Search Values of existing countries
						Country? matchedCountry = await _countriesRepository.GetCountryByName(countryName);
						bool countryExists = matchedCountry != null;

						if (!countryExists)
						{
							// Create country add request
							Country country = new Country()
							{
								CountryName = countryName,
								// Key is provided by database
								//CountryId = Guid.NewGuid(),
							};

							// Adds to Element to Database Commands
							await _countriesRepository.AddCountry(country);

							countriesInserted++;
						}
					}

				}
			}

			return countriesInserted;
		}
	}
}