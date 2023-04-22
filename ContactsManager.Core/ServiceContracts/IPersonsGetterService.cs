using ServiceContracts.DTO;
namespace ServiceContracts
{
	/// <summary>
	/// Represents the business logic for manipulating Persons entity
	/// </summary>
	public interface IPersonsGetterService
	{
		/// <summary>
		/// Returns all persons
		/// </summary>
		/// <returns>Returns a list of objects of PersonResponse type</returns>
		Task<List<PersonResponse>> GetAllPersons();

		/// <summary>
		/// Get person based on the supplied PersonID
		/// </summary>
		/// <param name="personId"> Guid person ID to search</param>
		/// <returns>Returns PersonResponse matching the supplied PersonId</returns>
		Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

		/// <summary>
		/// Returns all person objects that mathces with the given search field and search string
		/// </summary>
		/// <param name="searchBy">Search field to search</param>
		/// <param name="searchString">Search string to search</param>
		/// <returns>All matching persons based on the search string</returns>
		Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

		/// <summary>
		/// Returns persons as CSV
		/// </summary>
		/// <returns>Persons data as CSV</returns>
		Task<MemoryStream> GetPersonsCSV();

		/// <summary>
		/// Returns persons as Excel file
		/// </summary>
		/// <returns>Returns the memory stream with Excel data of persons</returns>
		Task<MemoryStream> GetPersonsExcel();
	}
}
