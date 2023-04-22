using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;

namespace Services
{
	public class PersonsDeleterService : IPersonsDeleterService
	{
		private readonly IPersonsRepository _personsRepository;
		private readonly ILogger<PersonsGetterService> _logger;
		private readonly IDiagnosticContext _diagnosticContext;
		public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
		{
			// Add repository abstract layer
			_personsRepository = personsRepository;
			_logger = logger;
			_diagnosticContext = diagnosticContext;
		}

		public async Task<bool> DeletePerson(Guid? personId)
		{
			if (personId == null) throw new ArgumentNullException(nameof(personId));
			Person? personMatch = await _personsRepository.GetPersonByPersonId(personId.Value);

			if (personMatch == null) return false;

			_ = await _personsRepository.DeletePersonByPersonId(personMatch.PersonId);

			return true;
		}


	}
}

