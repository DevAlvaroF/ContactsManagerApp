using Entities;
using Exceptions;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
	public class PersonsUpdaterService : IPersonsUpdaterService
	{
		private readonly IPersonsRepository _personsRepository;
		private readonly ILogger<PersonsGetterService> _logger;
		private readonly IDiagnosticContext _diagnosticContext;
		public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
		{
			// Add repository abstract layer
			_personsRepository = personsRepository;
			_logger = logger;
			_diagnosticContext = diagnosticContext;
		}

		public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
		{
			if (personUpdateRequest == null) throw new ArgumentNullException(nameof(personUpdateRequest));
			// Validation
			ValidationHelper.ModelValidation(personUpdateRequest);

			// Get Person
			Person? matchedPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId);

			if (matchedPerson == null) throw new InvalidPersonIdException("Person ID doesn't exist");

			Person personToUpdate = personUpdateRequest.ToPerson();

			//matchedPerson.PersonName = personUpdateRequest.PersonName;
			//matchedPerson.Email = personUpdateRequest.Email;
			//matchedPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
			//matchedPerson.Gender = personUpdateRequest.Gender.ToString();
			//matchedPerson.CountryId = personUpdateRequest.CountryId;
			//matchedPerson.Address = personUpdateRequest.Address;
			//matchedPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

			_ = await _personsRepository.UpdatePerson(personToUpdate); // Update when property modification is modified

			return matchedPerson.ToPersonResponse();
		}

	}
}

