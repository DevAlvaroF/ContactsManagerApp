using ServiceContracts.DTO;
namespace ServiceContracts
{
	/// <summary>
	/// Represents the business logic for manipulating Persons entity
	/// </summary>
	public interface IPersonsAdderService
	{
		/// <summary>
		/// Adds a new person into the list of persons
		/// </summary>
		/// <param name="request">Person to add</param>
		/// <returns>Returns the same person details in a Person Response</returns>
		Task<PersonResponse> AddPerson(PersonAddRequest? request);

	}
}
