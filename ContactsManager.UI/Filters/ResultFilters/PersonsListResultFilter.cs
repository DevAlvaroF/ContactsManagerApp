using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ResultFilters
{
	public class PersonsListResultFilter : IAsyncResultFilter
	{
		public readonly ILogger<PersonsListResultFilter> _logger;

		public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
		{
			_logger = logger;
		}

		public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			_logger.LogCritical("{FilterName}.{MethodName} - Result Filter (before)", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

			context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy MM dd");


			await next();

			_logger.LogCritical("{FilterName}.{MethodName} - Result Filter (after)", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
		}
	}
}
