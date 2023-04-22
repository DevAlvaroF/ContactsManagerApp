using ServiceContracts.DTO;
using ServiceContracts.Enums;
namespace ServiceContracts
{
	/// <summary>
	/// Represents the business logic for manipulating Persons entity
	/// </summary>
	public interface IPersonsSorterService
	{
		/// <summary>
		/// Returns sorted list of persons
		/// </summary>
		/// <param name="allPerson">Represents list of persons to sort</param>
		/// <param name="sortBy">Name of the property (key), based on which persons should be sorted</param>
		/// <param name="sortOder">ASC or DESC</param>
		/// <returns>Sorted list of PersonResponse</returns>
		Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

	}
}
