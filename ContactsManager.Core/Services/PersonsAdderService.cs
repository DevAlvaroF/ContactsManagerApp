using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
	public class PersonsAdderService : IPersonsAdderService
	{
		private readonly IPersonsRepository _personsRepository;
		private readonly ILogger<PersonsGetterService> _logger;
		private readonly IDiagnosticContext _diagnosticContext;
		public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
		{
			// Add repository abstract layer
			_personsRepository = personsRepository;
			_logger = logger;
			_diagnosticContext = diagnosticContext;
		}

		public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			// Model Validations
			ValidationHelper.ModelValidation(request);

			Person person = request.ToPerson();

			person.PersonId = Guid.NewGuid();

			_ = await _personsRepository.AddPerson(person);

			return person.ToPersonResponse();
		}
	}
}

