using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using System.Reflection;
using Xunit.Abstractions;
namespace CRUDTests
{
	public class PersonServiceTest
	{
		private readonly IPersonsGetterService _personGetterService;
		private readonly IPersonsAdderService _personAdderService;
		private readonly IPersonsUpdaterService _personUpdaterService;
		private readonly IPersonsSorterService _personSorterService;
		private readonly IPersonsDeleterService _personDeleterService;
		// Testing Helpers
		private readonly ITestOutputHelper _outputHelper;
		private readonly IFixture _fixture;

		// Repository
		private readonly IPersonsRepository _personsRepositoryMock;
		private readonly Mock<IPersonsRepository> _personRepositoryMocker;

		public PersonServiceTest(ITestOutputHelper testOutputHelper)
		{
			// 0) Fixture object to create dummy data
			_fixture = new Fixture();

			// 1) Create Mockers and MockRepository
			_personRepositoryMocker = new Mock<IPersonsRepository>();
			_personsRepositoryMock = _personRepositoryMocker.Object;

			// 2) Countries service requires dbContext with DI, we supply MoqdbContext for each service along with context
			_outputHelper = testOutputHelper;

			var diagnosticContext = new Mock<IDiagnosticContext>();
			var loggerContext = new Mock<ILogger<PersonsGetterService>>();

			_personGetterService = new PersonsGetterService(_personsRepositoryMock, loggerContext.Object, diagnosticContext.Object);
			_personAdderService = new PersonsAdderService(_personsRepositoryMock, loggerContext.Object, diagnosticContext.Object);
			_personDeleterService = new PersonsDeleterService(_personsRepositoryMock, loggerContext.Object, diagnosticContext.Object);
			_personUpdaterService = new PersonsUpdaterService(_personsRepositoryMock, loggerContext.Object, diagnosticContext.Object);
			_personSorterService = new PersonsSorterService(_personsRepositoryMock, loggerContext.Object, diagnosticContext.Object);
		}

		#region AddPerson

		[Fact]
		// Test 1: When we supply null values as PersonAddRequest, it should throw ArgumentNullException
		public async Task AddPerson_NullPerson_ToBeArgumentNullException()
		{
			// Arrange ---------------------------------
			PersonAddRequest? personRequest = null;

			// Act --------------------------------------------
			Func<Task> actionFunc = async () =>
			{
				await _personAdderService.AddPerson(personRequest);
			};

			// Assert -----------------------------------------
			await actionFunc.Should().ThrowAsync<ArgumentNullException>();
		}


		[Fact]
		// Test 2: When we suplly person name as null values in the PersonAddRequest, it should throw ArgumentException
		public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
		{
			// Arrange -----------------------------------------
			PersonAddRequest? personRequestDummy = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

			Person personDummy = personRequestDummy.ToPerson();

			_personRepositoryMocker.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(personDummy);

			// Act ---------------------------------

			Func<Task> actionFunc = async () =>
			{
				await _personAdderService.AddPerson(personRequestDummy);
			};

			// Assert ---------------------------------------
			await actionFunc.Should().ThrowAsync<ArgumentException>();
		}

		[Fact]
		// Test 3: When we supply proper person details, it should insert the person in the persons list and return an object of PersonResponse with a newly generated PersonId
		public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
		{
			// Arrange --------------------------------

			// Dummy data with Autofixture
			PersonAddRequest? personAddRequestDummy = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();


			// Mock repository Fakes the AddPerson() in IPersonsRepository. Expects to receive any type of Person but it will always return the Dummy
			Person personDummy = personAddRequestDummy.ToPerson();
			PersonResponse personResponseDummy = personDummy.ToPersonResponse();

			_personRepositoryMocker.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(personDummy);

			// Act -------------------------------------
			PersonResponse personResponseActual = await _personAdderService.AddPerson(personAddRequestDummy);
			personResponseDummy.PersonId = personResponseActual.PersonId;

			// Assert ------------------------------------
			personResponseActual.PersonId.Should().NotBe(Guid.Empty);
			personResponseActual.Should().Be(personResponseDummy);
		}
		#endregion

		#region GetPersonByPersonId

		// Null personId should return null
		[Fact]
		public async Task GetPersonByPersonId_NullPersonId_ToBeNull()
		{
			// Arrange
			Guid? personId = null;

			// Act
			PersonResponse? personResponseActual = await _personGetterService.GetPersonByPersonId(personId);

			// Assert
			personResponseActual.Should().BeNull();
		}

		// Correct Details should returna Person with a generated Id
		[Fact]
		public async Task GetPersonByPersonId_WithPersonId_ToBeSuccessful()
		{
			// Arrange -------------------------------------------

			Person personDummy = _fixture.Build<Person>().With(temp => temp.Email, "email@email.com").With(temp => temp.Country, null as Country).Create();

			PersonResponse personResponseDummy = personDummy.ToPersonResponse();

			_personRepositoryMocker.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(personDummy);

			// Act --------------------------------------------
			PersonResponse? personResponseByIdActual = await _personGetterService.GetPersonByPersonId(personDummy.PersonId);

			// Assert ------------------------------------------
			personResponseDummy.Should().Be(personResponseByIdActual);
		}
		#endregion

		#region GetAllPersons

		// Get all persons should return an empty list by default
		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			_personRepositoryMocker.Setup(temp => temp.GetAllPersons()).ReturnsAsync(new List<Person>());

			// Act
			List<PersonResponse> personResponseListActual = await _personGetterService.GetAllPersons();

			// Assert
			personResponseListActual.Should().BeEmpty();
		}


		// If you add many persons, the same persons should be returned
		[Fact]
		public async Task GetAllPersons_WithFewPersons_ToBeSuccesful()
		{
			// Arrange ---------------------------------------
			List<Person> personsListDummy = new List<Person>() {
				_fixture.Build<Person>().With(temp => temp.Email, "olivia@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email2@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email3@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email4@gmail.com").With(temp=>temp.Country, null as Country).Create(),
			};

			// Add Person Dummy data
			List<PersonResponse> personResponseListDummy = personsListDummy.Select(temp => temp.ToPersonResponse()).ToList();

			_outputHelper.WriteLine("Expected");
			foreach (PersonResponse personResponse in personResponseListDummy)
			{
				_outputHelper.WriteLine(personResponse.ToString());
			}

			_personRepositoryMocker.Setup(temp => temp.GetAllPersons()).ReturnsAsync(personsListDummy);

			// Act ------------------------------------------------
			List<PersonResponse> allPersonsReponseList = await _personGetterService.GetAllPersons();

			// Print Expected Results
			_outputHelper.WriteLine("Actual");
			foreach (PersonResponse person_response_Get in allPersonsReponseList)
			{
				_outputHelper.WriteLine(person_response_Get.ToString());
			}

			// Assert
			allPersonsReponseList.Should().BeEquivalentTo(personResponseListDummy);
		}
		#endregion

		#region GetFilteredPersons

		// If the search text is empty and search by is "PersonName", its should return all persons
		[Fact]
		public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
		{
			// Arrange-------------------------------------- -
			List<Person> personsListDummy = new List<Person>() {
				_fixture.Build<Person>().With(temp => temp.Email, "olivia@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email2@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email3@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email4@gmail.com").With(temp=>temp.Country, null as Country).Create(),
			};

			// Add Person Dummy data
			List<PersonResponse> personResponseListDummy = personsListDummy.Select(temp => temp.ToPersonResponse()).ToList();

			// Print Expected Results
			_outputHelper.WriteLine("Expected");
			foreach (PersonResponse person_response in personResponseListDummy)
			{
				_outputHelper.WriteLine(person_response.ToString());
			}

			_personRepositoryMocker.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(personsListDummy);


			// Act
			List<PersonResponse> personResponseListGetFilteredPersons = await _personGetterService.GetFilteredPersons(searchBy: nameof(PersonResponse.PersonName), "");

			// Print Expected Results
			_outputHelper.WriteLine("Actual");
			foreach (PersonResponse person_response_Get in personResponseListGetFilteredPersons)
			{
				_outputHelper.WriteLine(person_response_Get.ToString());
			}

			// Assert
			personResponseListGetFilteredPersons.Should().BeEquivalentTo(personResponseListDummy);
		}

		// After adding some persons, we will search on the person name with some search string. It should return the matching persons
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
		{
			// Arrange-------------------------------------- -
			List<Person> personsListDummy = new List<Person>() {
				_fixture.Build<Person>().With(temp => temp.Email, "olivia@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email2@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email3@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email4@gmail.com").With(temp=>temp.Country, null as Country).Create(),
			};

			// Add Person Dummy data
			List<PersonResponse> personResponseListDummy = personsListDummy.Select(temp => temp.ToPersonResponse()).ToList();

			// Print Expected Results
			_outputHelper.WriteLine("Expected");
			foreach (PersonResponse person_response in personResponseListDummy)
			{
				_outputHelper.WriteLine(person_response.ToString());
			}

			_personRepositoryMocker.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(personsListDummy);


			// Act
			List<PersonResponse> personResponseListGetFilteredPersons = await _personGetterService.GetFilteredPersons(searchBy: nameof(PersonResponse.PersonName), "sa");

			// Print Expected Results
			_outputHelper.WriteLine("Actual");
			foreach (PersonResponse person_response_Get in personResponseListGetFilteredPersons)
			{
				_outputHelper.WriteLine(person_response_Get.ToString());
			}

			// Assert
			personResponseListGetFilteredPersons.Should().BeEquivalentTo(personResponseListDummy);

		}
		#endregion

		#region GetSortedPersons
		// When we sort based on PersonName in DESC, it should return person list in descending order on PersonName
		[Fact]
		public async Task GetSortedPersons_ToBeSuccesful()
		{
			// Arrange-------------------------------------- -
			List<Person> personsListDummy = new List<Person>() {
				_fixture.Build<Person>().With(temp => temp.Email, "olivia@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email2@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email3@gmail.com").With(temp=>temp.Country, null as Country).Create(),
				_fixture.Build<Person>().With(temp => temp.Email, "email4@gmail.com").With(temp=>temp.Country, null as Country).Create(),
			};

			// Add Person Dummy data
			List<PersonResponse> personResponseListDummy = personsListDummy.Select(temp => temp.ToPersonResponse()).ToList();

			_personRepositoryMocker.Setup(x => x.GetAllPersons()).ReturnsAsync(personsListDummy);

			string sortBy = nameof(PersonResponse.PersonName);
			SortOrderOptions sortOrder = SortOrderOptions.DESC;


			// Print Expected Results
			_outputHelper.WriteLine("Expected");
			List<PersonResponse> sorted_expected = personResponseListDummy.OrderBy(temp =>
			{
				PropertyInfo? propertyInfo = typeof(PersonResponse).GetProperty(sortBy);
				string? propertyValue = Convert.ToString(propertyInfo?.GetValue(temp, null));

				return !string.IsNullOrEmpty(propertyValue) ? propertyValue : "";

			}, StringComparer.OrdinalIgnoreCase).ToList();

			// If DESC passed, we reverse
			if (sortOrder == SortOrderOptions.DESC) sorted_expected.Reverse();

			foreach (PersonResponse person_response in sorted_expected)
			{
				_outputHelper.WriteLine(person_response.ToString());
			}

			// Act --------------------------------------------------
			List<PersonResponse> allPersons = await _personGetterService.GetAllPersons();

			List<PersonResponse> personResponseListGetSortedPersons = await _personSorterService.GetSortedPersons(allPersons, sortBy, sortOrder);

			// Print Actual Results
			_outputHelper.WriteLine("Actual");
			foreach (PersonResponse person_response_Get in personResponseListGetSortedPersons)
			{
				_outputHelper.WriteLine(person_response_Get.ToString());
			}

			// Assert
			personResponseListGetSortedPersons.Should().BeInDescendingOrder(temp => temp.PersonName);

			personResponseListGetSortedPersons.Should().BeEquivalentTo(sorted_expected);

		}
		#endregion

		#region UpdatePerson
		// When supplied with a null PersonUpdateRequest, it should throw ArgumentNullException
		[Fact]
		public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
		{
			// Arrange
			PersonUpdateRequest? personUpdateRequest = null;

			//Act
			Func<Task> action = async () =>
			{
				await _personUpdaterService.UpdatePerson(personUpdateRequest);
			};

			//Assert
			await action.Should().ThrowAsync<ArgumentNullException>();

		}

		// If the Person Id is invalid, it should throw ArgumentException
		[Fact]
		public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
		{
			// Arrange
			PersonUpdateRequest? personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

			//Act
			Func<Task> action = async () =>
			{
				await _personUpdaterService.UpdatePerson(personUpdateRequest);
			};

			//Assert
			await action.Should().ThrowAsync<ArgumentException>();

		}

		// When PersonName is null, should throw ArgumentException
		[Fact]
		public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
		{
			// Arrange
			Person personDummy = _fixture.Build<Person>().With(temp => temp.Email, "email@gmail.com").With(temp => temp.Country, null as Country).With(temp => temp.PersonName, null as string).With(temp => temp.Gender, "Male").Create();


			PersonResponse personResponseDummy = personDummy.ToPersonResponse();

			PersonUpdateRequest personUpdateRequestDummy = personResponseDummy.ToPersonUpdateRequest();

			//Act
			Func<Task> action = async () =>
			{
				await _personUpdaterService.UpdatePerson(personUpdateRequestDummy);
			};

			//Assert
			await action.Should().ThrowAsync<ArgumentException>();

		}

		// A person will be added, and try to update the same name and email
		[Fact]
		public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
		{
			// Arrange
			Person personDummy = _fixture.Build<Person>().With(temp => temp.Email, "email@gmail.com").With(temp => temp.Country, null as Country).With(temp => temp.Gender, "Male").Create();

			PersonResponse personResponseDummy = personDummy.ToPersonResponse();

			PersonUpdateRequest personUpdateRequestDummy = personResponseDummy.ToPersonUpdateRequest();

			// Mock
			_personRepositoryMocker.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(personDummy);

			_personRepositoryMocker.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(personDummy);



			// Act --------------------------------------
			PersonResponse personModifiedResponse = await _personUpdaterService.UpdatePerson(personUpdateRequestDummy);


			// Print expected and Actual
			_outputHelper.WriteLine("Expected");
			_outputHelper.WriteLine(personResponseDummy.ToString());

			_outputHelper.WriteLine("-------------");

			_outputHelper.WriteLine("Actual");
			_outputHelper.WriteLine(personModifiedResponse?.ToString());

			// Assert
			personModifiedResponse.Should().Be(personResponseDummy);
		}
		#endregion

		#region DeletePerson
		// If you supply valid ID, deleteion should be true
		[Fact]
		public async Task DeletePerson_ValidPersonId_ToBeSuccessful()
		{
			// Arrange -------------------------------------------
			Person personDummy = _fixture.Build<Person>().With(temp => temp.Email, "email@gmail.com").With(temp => temp.Country, null as Country).With(temp => temp.Gender, "Male").Create();

			// Mock
			_personRepositoryMocker.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(personDummy);

			_personRepositoryMocker.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);

			bool deleteResult = await _personDeleterService.DeletePerson(personDummy.PersonId);

			// Assert
			// Print expected and Actual
			_outputHelper.WriteLine("Expected");
			_outputHelper.WriteLine("true");

			_outputHelper.WriteLine("-------------");

			_outputHelper.WriteLine("Actual");
			_outputHelper.WriteLine(deleteResult.ToString());


			//Assert
			deleteResult.Should().BeTrue();

		}

		// If you supply invalid ID, deleteion should be false
		[Fact]
		public async Task DeletePerson_InvalidPersonId()
		{

			bool deleteResult = await _personDeleterService.DeletePerson(Guid.NewGuid());

			// Assert
			// Print expected and Actual
			_outputHelper.WriteLine("Expected");
			_outputHelper.WriteLine("false");

			_outputHelper.WriteLine("-------------");

			_outputHelper.WriteLine("Actual");
			_outputHelper.WriteLine(deleteResult.ToString());

			deleteResult.Should().BeFalse();

		}
		#endregion
	}
}
