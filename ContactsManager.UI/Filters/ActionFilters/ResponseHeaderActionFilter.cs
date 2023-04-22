using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ActionFilters
{
	public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public ResponseHeaderFilterFactoryAttribute(string key, string value)
		{
			Key = key;
			Value = value;

		}
		public bool IsReusable => false;

		public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
		{
			var customFilter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();



			customFilter.Key = Key;
			customFilter.Value = Value;

			return customFilter;
		}
	}
	public class ResponseHeaderActionFilter : IAsyncActionFilter
	{
		private readonly ILogger<ResponseHeaderActionFilter> _logger;
		public string? Key { get; set; }
		public string? Value { get; set; }

		public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
		{
			_logger = logger;
		}


		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			// Before
			_logger.LogWarning("{FilterName}.{MethodName} {Key} executed before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync), Key);

			await next();

			// After
			_logger.LogWarning("{FilterName}.{MethodName} {Key} executed after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync), Key);

			if (Key != null)
				context.HttpContext.Response.Headers[Key] = Value;
		}
	}
}
