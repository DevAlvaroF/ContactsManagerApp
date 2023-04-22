using AutoFixture;
using ContactsManager.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests
{
	public class PersonsControllerTest
	{
		// Service Classes
		private readonly ICountriesAdderService _countriesAdderServiceMock;
		private readonly ICountriesGetterService _countriesGetterServiceMock;

		private readonly IPersonsGetterService _personsGetterServiceMock;
		private readonly IPersonsAdderService _personsAdderServiceMock;
		private readonly IPersonsSorterService _personsSorterServiceMock;
		private readonly IPersonsUpdaterService _personsUpdaterServiceMock;
		private readonly IPersonsDeleterService _personsDeleterServiceMock;



		// Mock of Persons Service Classes
		private readonly Mock<IPersonsGetterService> _personsGetterServiceMocker;
		private readonly Mock<IPersonsAdderService> _personsAdderServiceMocker;
		private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMocker;
		private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMocker;
		private readonly Mock<IPersonsSorterService> _personsSorterServiceMocker;


		// Mock of Countries Service Classes
		private readonly Mock<ICountriesAdderService> _countriesAdderServiceMocker;
		private readonly Mock<ICountriesGetterService> _countriesGetterServiceMocker;

		private readonly Mock<ILogger<PersonsController>> _loggerMocker;

		private readonly Fixture _fixture;

		public PersonsControllerTest()
		{
			// 0) Create Fixture
			_fixture = new Fixture();

			//1) Create Mock services
			_personsGetterServiceMocker = new Mock<IPersonsGetterService>();
			_personsGetterServiceMock = _personsGetterServiceMocker.Object;


			_personsAdderServiceMocker = new Mock<IPersonsAdderService>();
			_personsAdderServiceMock = _personsAdderServiceMocker.Object;

			_personsDeleterServiceMocker = new Mock<IPersonsDeleterService>();
			_personsDeleterServiceMock = _personsDeleterServiceMocker.Object;

			_personsUpdaterServiceMocker = new Mock<IPersonsUpdaterService>();
			_personsUpdaterServiceMock = _personsUpdaterServiceMocker.Object;

			_personsSorterServiceMocker = new Mock<IPersonsSorterService>();
			_personsSorterServiceMock = _personsSorterServiceMocker.Object;



			_countriesAdderServiceMocker = new Mock<ICountriesAdderService>();
			_countriesAdderServiceMock = _countriesAdderServiceMocker.Object;


			_countriesGetterServiceMocker = new Mock<ICountriesGetterService>();
			_countriesGetterServiceMock = _countriesGetterServiceMocker.Object;

			_loggerMocker = new Mock<ILogger<PersonsController>>();
		}
		#region Index

		[Fact]
		public async Task Index_ShouldReturnIndexViewWithPersonList()
		{
			// Arrange ===============================================
			List<PersonResponse> personResponseListDummy = _fixture.Create<List<PersonResponse>>();

			// Create controller with mock Services

			PersonsController personController = new PersonsController(_personsGetterServiceMock, _personsAdderServiceMock, _personsSorterServiceMock, _personsUpdaterServiceMock, _personsDeleterServiceMock, _countriesGetterServiceMock, _loggerMocker.Object);

			// Mock
			_personsGetterServiceMocker.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personResponseListDummy);

			_personsSorterServiceMocker.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponseListDummy);

			_personsGetterServiceMocker.Setup(temp => temp.GetAllPersons()).ReturnsAsync(personResponseListDummy);

			// Act ===============================================
			IActionResult viewIndex = await personController.Index(searchBy: _fixture.Create<string>(), searchString: _fixture.Create<string>(), sortBy: _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

			// Assert =============================================
			ViewResult viewResult = Assert.IsType<ViewResult>(viewIndex);

			// Get the Model returned by the view
			viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
			viewResult.ViewData.Model.Should().BeEquivalentTo(personResponseListDummy);
		}

		#endregion

		#region Create

		//[Fact]
		//public async Task Create_IfModelErrors_ReturnCreateView()
		//{
		//	// Arrange ===============================================
		//	PersonAddRequest personAddRequestDummy = _fixture.Create<PersonAddRequest>();

		//	PersonResponse personResponseDummy = _fixture.Create<PersonResponse>();

		//	List<CountryResponse> countryResponseListDummy = _fixture.Create<List<CountryResponse>>();

		//	// Create controller with mock Services
		//	PersonsController personController = new PersonsController(_countriesServiceMock, _personsServiceMock, _loggerMocker.Object);

		//	// Mock
		//	_countriesServiceMocker.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countryResponseListDummy);

		//	_personsServiceMocker.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponseDummy);

		//	// Act ===============================================
		//	personController.ModelState.AddModelError("Unit Testing", " Model Error");

		//	IActionResult viewCreate = await personController.Create(personAddRequestDummy);

		//	// Assert =============================================
		//	ViewResult viewResult = Assert.IsType<ViewResult>(viewCreate);

		//	// Get the Model returned by the view
		//	viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
		//	viewResult.ViewData.Model.Should().BeEquivalentTo(personAddRequestDummy);
		//}

		[Fact]
		public async Task Create_IfNoModelErrors_RedirectoToIndex()
		{
			// Arrange ===============================================
			PersonAddRequest personAddRequestDummy = _fixture.Create<PersonAddRequest>();

			PersonResponse personResponseDummy = _fixture.Create<PersonResponse>();

			List<CountryResponse> countryResponseListDummy = _fixture.Create<List<CountryResponse>>();

			// Create controller with mock Services
			PersonsController personController = new PersonsController(_personsGetterServiceMock, _personsAdderServiceMock, _personsSorterServiceMock, _personsUpdaterServiceMock, _personsDeleterServiceMock, _countriesGetterServiceMock, _loggerMocker.Object);

			// Mock
			_countriesGetterServiceMocker.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countryResponseListDummy);

			_personsAdderServiceMocker.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponseDummy);

			// Act ===============================================
			IActionResult viewCreate = await personController.Create(personAddRequestDummy);

			// Assert =============================================
			RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(viewCreate);

			// Get the Model returned by the view
			redirectResult.ActionName.Should().Be("Index");
			redirectResult.ControllerName.Should().Be("Persons");
		}
		#endregion

	}
}
