using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests
{
	public class CountriesServiceTest
	{
		private readonly ICountriesAdderService _countriesAdderService;
		private readonly ICountriesGetterService _countriesGetterService;
		// Testing Helpers
		private readonly IFixture _fixture;

		// To Mock Repository
		private readonly ICountriesRepository _countriesRepositoryMock;
		private readonly Mock<ICountriesRepository> _countriesRepositoryMocker;

		public CountriesServiceTest()
		{
			// 0) Fixture object to create dummy data
			_fixture = new Fixture();

			// 1) Create Mockers and MockRepository
			_countriesRepositoryMocker = new Mock<ICountriesRepository>();
			_countriesRepositoryMock = _countriesRepositoryMocker.Object;

			// 2) Countries service requires ICountriesRepository with DI, we supply the Mock one
			_countriesAdderService = new CountriesAdderService(_countriesRepositoryMock);
			_countriesGetterService = new CountriesGetterService(_countriesRepositoryMock);
		}

		#region AddCountry

		//When CountryAddRequest is null, it should throw ArgumentNullException
		[Fact]
		public async Task AddCountry_NullCountry_ThrosArgumentNullException()
		{
			//Arrange
			CountryAddRequest? request = null;

			//Act
			var action = async () =>
			{
				await _countriesAdderService.AddCountry(request);
			};

			//Assert
			await action.Should().ThrowAsync<ArgumentNullException>();
		}


		//When the CountryName is null, it should throw ArgumentException
		[Fact]
		public async Task AddCountry_CountryNameIsNull_ThrowsArgumentException()
		{
			//Arrange
			CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
			 .With(temp => temp.CountryName, null as string)
			 .Create();

			//Act
			var action = async () =>
			{
				await _countriesAdderService.AddCountry(request);
			};

			//Assert
			await action.Should().ThrowAsync<ArgumentException>();
		}


		//When the CountryName is duplicate, it should throw ArgumentException
		[Fact]
		public async Task AddCountry_DuplicateCountryName()
		{
			//Arrange
			CountryAddRequest? requestDummy = _fixture.Build<CountryAddRequest>()
			 .With(temp => temp.CountryName).Create();

			Country countryDummy = requestDummy.ToCountry();


			// Mock
			_countriesRepositoryMocker.Setup(temp => temp.GetCountryByName(It.IsAny<string>())).ReturnsAsync(countryDummy);

			_countriesRepositoryMocker.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(countryDummy);

			//Act
			var action = async () =>
			{
				await _countriesAdderService.AddCountry(requestDummy);
			};

			//Assert
			await action.Should().ThrowAsync<ArgumentException>();
		}


		//When you supply proper country name, it should insert (add) the country to the existing list of countries
		[Fact]
		public async Task AddCountry_ProperCountryDetails()
		{
			//Arrange
			//Arrange
			CountryAddRequest? requestDummy = _fixture.Build<CountryAddRequest>().Create();

			Country countryDummy = requestDummy.ToCountry();

			CountryResponse countryResponseDummy = countryDummy.ToCountryReponse();

			// Mock
			_countriesRepositoryMocker.Setup(temp => temp.GetCountryByName(It.IsAny<string>())).ReturnsAsync(null as Country);
			_countriesRepositoryMocker.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(countryDummy);

			//Act
			CountryResponse countryResponse = await _countriesAdderService.AddCountry(requestDummy);
			countryResponseDummy.CountryId = countryResponse.CountryId;

			//Assert
			countryResponse.CountryId.Should().NotBe(Guid.Empty);
			countryResponse.Should().BeEquivalentTo(countryResponseDummy);
		}

		#endregion


		#region GetAllCountries

		[Fact]
		//The list of countries should be empty by default (before adding any countries)
		public async Task GetAllCountries_EmptyList_EmptyList()
		{
			// Arrange ====================================================================================
			// Mock
			_countriesRepositoryMocker.Setup(temp => temp.GetAllCountries()).ReturnsAsync(new List<Country>());

			//Act
			List<CountryResponse> actual_country_response_list = await _countriesGetterService.GetAllCountries();

			//Assert
			actual_country_response_list.Should().BeEmpty();
		}


		[Fact]
		public async Task GetAllCountries_AddFewCountries_ToBeSuccesful()
		{
			// Arrange ====================================================================================
			List<Country> countryListDummy = new List<Country>() {
				_fixture.Build<Country>().With(temp=>temp.Persons,null as ICollection<Person>).Create(),
				_fixture.Build<Country>().With(temp=>temp.Persons,null as ICollection<Person>).Create()
			};

			List<CountryResponse> countryAddRequestListDummy = countryListDummy.Select(temp => temp.ToCountryReponse()).ToList();

			// Mock
			_countriesRepositoryMocker.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countryListDummy);

			//Act
			List<CountryResponse> actualCountryResponseList = await _countriesGetterService.GetAllCountries();

			//Assert
			actualCountryResponseList.Should().BeEquivalentTo(countryAddRequestListDummy);
		}
		#endregion


		#region GetCountryByCountryId

		[Fact]
		//If we supply null as CountryId, it should return null as CountryResponse
		public async Task GetCountryByCountryId_NullCountryId_Null()
		{
			//Arrange
			Guid? countryId = null;

			//Act
			CountryResponse? country_response_from_get_method = await _countriesGetterService.GetCountryByCountryId(countryId);


			//Assert
			country_response_from_get_method.Should().BeNull();
		}


		[Fact]
		//If we supply a valid country id, it should return the matching country details as CountryResponse object
		public async Task GetCountryByCountryId_ValidCountryId_ToBeSuccessful()
		{
			//Arrange
			Country countryDummy = _fixture.Build<Country>().With(temp => temp.Persons, null as ICollection<Person>).Create();
			CountryResponse countryResponseDummy = countryDummy.ToCountryReponse();

			_countriesRepositoryMocker.Setup(temp => temp.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(countryDummy);

			//Act
			CountryResponse? country_response_from_get = await _countriesGetterService.GetCountryByCountryId(countryDummy.CountryId);

			//Assert
			country_response_from_get.Should().Be(countryResponseDummy);
		}
		#endregion
	}
}
