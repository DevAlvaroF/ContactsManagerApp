using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ExceptionFilters
{
	public class HandleExceptionFilter : IAsyncExceptionFilter
	{
		private readonly IHostEnvironment _hostEnvironment;
		private readonly ILogger<HandleExceptionFilter> _logger;

		public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IHostEnvironment hostEnvironment)
		{
			_logger = logger;
			_hostEnvironment = hostEnvironment;
		}

		public async Task OnExceptionAsync(ExceptionContext context)
		{
			await Task.FromResult(0);

			_logger.LogError("Exception Filter {FilterName}.{Method}\n{ExceptionType}\n{ExceptionMEssage}", nameof(HandleExceptionFilter), nameof(OnExceptionAsync), context.Exception.GetType().ToString(), context.Exception.Message);

			if (!_hostEnvironment.IsDevelopment())
			{
				// Custom Error View
				context.Result = new ContentResult() { Content = context.Exception.Message, StatusCode = 500 };
			}
			return;
		}
	}
}
