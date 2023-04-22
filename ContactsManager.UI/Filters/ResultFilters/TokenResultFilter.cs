using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ResultFilters
{
	public class TokenResultFilter : IAsyncResultFilter
	{
		public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			context.HttpContext.Response.Cookies.Append("Auth-Key", "A100");
			await next();
		}
	}
}
