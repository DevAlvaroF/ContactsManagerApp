using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.AuthorizationFilters
{
	public class TokenAuthorizationFilter : IAsyncAuthorizationFilter
	{
		public readonly ILogger<TokenAuthorizationFilter> _logger;

		public TokenAuthorizationFilter(ILogger<TokenAuthorizationFilter> logger)
		{
			_logger = logger;
		}

		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			_logger.LogCritical("{FilterName}.{MethodName} - Result Filter (ONLY)", nameof(TokenAuthorizationFilter), nameof(OnAuthorizationAsync));

			if (context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") == false)
			{
				context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);

				return;
			}
			if (context.HttpContext.Request.Cookies["Auth-key"] != "A100")
			{
				context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
				return;
			}
			await Task.FromResult(0);
		}
	}
}
