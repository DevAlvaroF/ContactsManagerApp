using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
	public class PersonsRepository : IPersonsRepository
	{
		private readonly ApplicationDbContext _db;
		private readonly ILogger<PersonsRepository> _logger;

		public PersonsRepository(ApplicationDbContext context, ILogger<PersonsRepository> logger)
		{
			_db = context;
			_logger = logger;
		}
		public async Task<Person> AddPerson(Person person)
		{
			_logger.LogInformation("Async AddPerson repository method reached");

			_db.Persons.Add(person);
			await _db.SaveChangesAsync();
			return person;
		}

		public async Task<bool> DeletePersonByPersonId(Guid PersonId)
		{
			_db.Persons.RemoveRange(_db.Persons.Where(person => person.PersonId == PersonId));

			int rowsDeleted = await _db.SaveChangesAsync();

			return rowsDeleted > 0;

		}

		public async Task<List<Person>> GetAllPersons()
		{
			return await _db.Persons.Include("Country").ToListAsync();
		}

		public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
		{
			return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
		}

		public async Task<Person?> GetPersonByPersonId(Guid personId)
		{
			return await _db.Persons.Include("Country").FirstOrDefaultAsync(tmp => tmp.PersonId == personId);
		}

		public async Task<Person> UpdatePerson(Person person)
		{
			Person? matchedPerson = await GetPersonByPersonId(person.PersonId);
			if (matchedPerson == null)
				return person;

			// Attached modified person to person database
			_db.Persons.Attach(matchedPerson);

			//Modifed on attached person
			matchedPerson.PersonName = person.PersonName;
			matchedPerson.Email = person.Email;
			matchedPerson.DateOfBirth = person.DateOfBirth;
			matchedPerson.Gender = person.Gender;
			matchedPerson.CountryId = person.CountryId;
			matchedPerson.Address = person.Address;
			matchedPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

			// Save Changes
			await _db.SaveChangesAsync();

			return matchedPerson;

		}
	}
}
