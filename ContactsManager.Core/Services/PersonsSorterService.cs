using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Reflection;

namespace Services
{
	public class PersonsSorterService : IPersonsSorterService
	{
		private readonly IPersonsRepository _personsRepository;
		private readonly ILogger<PersonsGetterService> _logger;
		private readonly IDiagnosticContext _diagnosticContext;
		public PersonsSorterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
		{
			// Add repository abstract layer
			_personsRepository = personsRepository;
			_logger = logger;
			_diagnosticContext = diagnosticContext;
		}

		public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
		{
			_logger.LogInformation("GetSortedPersons of PersonsService has been reached");
			await Task.FromResult(0);

			if (string.IsNullOrEmpty(sortBy)) return allPersons;

			// Sorts Ascending by default
			List<PersonResponse> sortedFinal = allPersons.OrderBy(temp =>
			{
				PropertyInfo? propertyInfo = typeof(PersonResponse).GetProperty(sortBy);
				string? propertyValue = Convert.ToString(propertyInfo?.GetValue(temp, null));

				return !string.IsNullOrEmpty(propertyValue) ? propertyValue : "";

			}, StringComparer.OrdinalIgnoreCase).ToList();

			// If DESC passed, we reverse
			if (sortOrder == SortOrderOptions.DESC) sortedFinal.Reverse();

			return sortedFinal;
		}

	}
}

