namespace ServiceContracts
{
	/// <summary>
	/// Represents the business logic for manipulating Persons entity
	/// </summary>
	public interface IPersonsDeleterService
	{
		/// <summary>
		/// Delete a person based on the given person id
		/// </summary>
		/// <param name="personId">PersonID to delete</param>
		/// <returns>Returns true if delete successful, otherwise false</returns>
		Task<bool> DeletePerson(Guid? personId);

	}
}
