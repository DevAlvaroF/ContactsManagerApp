using ServiceContracts.DTO;
namespace ServiceContracts
{
	/// <summary>
	/// Represents the business logic for manipulating Persons entity
	/// </summary>
	public interface IPersonsUpdaterService
	{
		/// <summary>
		/// Updates the specified person details based on the given Person Id
		/// </summary>
		/// <param name="personUpdateRequest">Person details to update, including PersonId</param>
		/// <returns>The person response after modification</returns>
		Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

	}
}
