using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
	/// <summary>
	/// Represents data access logic for managing Persons Entity
	/// </summary>
	public interface IPersonsRepository
	{
		/// <summary>
		/// Adds person object to the data store
		/// </summary>
		/// <param name="person">Person object to add</param>
		/// <returns>Returns the person object after adding it to the table</returns>
		Task<Person> AddPerson(Person person);

		/// <summary>
		/// Returns all persons in the data store
		/// </summary>
		/// <returns>List of person objects from table</returns>
		Task<List<Person>> GetAllPersons();

		/// <summary>
		/// 
		/// Returns a person object based on the given person Id
		/// </summary>
		/// <param name="personId">PersonId (Guid) to search</param>
		/// <returns>A person object or null</returns>
		Task<Person?> GetPersonByPersonId(Guid personId);

		/// <summary>
		/// Returns all person objects based on the given expression
		/// </summary>
		/// <param name="predicate">LINQ expression to check</param>
		/// <returns>All matching persons with given condition</returns>
		Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

		/// <summary>
		/// Deletes a person object based on the person Id
		/// </summary>
		/// <param name="PersonId">Person Id (Guid) to delete</param>
		/// <returns>True if the deletion is successful, otherwise false</returns>
		Task<bool> DeletePersonByPersonId(Guid PersonId);

		/// <summary>
		/// Updates a person object (person name and other details) based on the given person object
		/// </summary>
		/// <param name="person">Person object to update</param>
		/// <returns>Updated person object</returns>
		Task<Person> UpdatePerson(Person person);


	}
}
