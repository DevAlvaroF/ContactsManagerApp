using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ResourceFilters
{
	public class FeatureDisableResourceFilter : IAsyncResourceFilter
	{
		public readonly ILogger<FeatureDisableResourceFilter> _logger;
		private bool _isDisabled;

		public FeatureDisableResourceFilter(ILogger<FeatureDisableResourceFilter> logger, bool isDisabled = true)
		{
			_logger = logger;
			_isDisabled = isDisabled;
		}
		public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
		{
			_logger.LogCritical("{FilterName}.{MethodName} - Result Filter (before)", nameof(FeatureDisableResourceFilter), nameof(OnResourceExecutionAsync));

			if (_isDisabled)
				context.Result = new NotFoundResult(); // 404
			else
				await next();

			_logger.LogCritical("{FilterName}.{MethodName} - Result Filter (after)", nameof(FeatureDisableResourceFilter), nameof(OnResourceExecutionAsync));
		}
	}
}
