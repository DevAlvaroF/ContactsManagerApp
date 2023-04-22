using Serilog;

namespace ContactsManager.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		// ===============================================
		// ILogger for log into Serilog and IDiagnosticContext
		// so we can write diagnostic details into de diagnostic
		// context that can be written into serilog
		// ===============================================

		private readonly ILogger<ExceptionHandlingMiddleware> _logger;
		private readonly IDiagnosticContext _diagnosticContext;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IDiagnosticContext diagnosticContext)
		{
			// Represents subsequent middleware
			_next = next;
			_diagnosticContext = diagnosticContext;
			_logger = logger;
		}

		public async Task Invoke(HttpContext httpContext)
		{

			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					// Log into Serilog
					_logger.LogError("{MiddlewareInnerExceptionType}: {MiddlewareInnerExceptionMessage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);

					// Pass info to View
					httpContext.Items["Custom-Error"] = $"(InnerException) ~ {ex.InnerException.GetType().ToString()}: {ex.InnerException.Message}";
				}
				else
				{
					// Log into Serilog
					_logger.LogError("{MiddlewareExceptionType}: {MiddlewareExceptionMessage}", ex.GetType().ToString(), ex.Message);

					// Pass info to View
					httpContext.Items["Custom-Error"] = $"{ex.GetType().ToString()}: {ex.Message}";
				}

				// Write in the website for the user
				// Not needed if view generated

				//httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
				//await httpContext.Response.WriteAsync("Error occured");

				// Extends existing stack trace



				throw;
			}
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class ExceptionHandlingMiddlewareExtensions
	{
		public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
