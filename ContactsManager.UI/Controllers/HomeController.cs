using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers
{
	public class HomeController : Controller
	{
		[Route("error")]
		public IActionResult Error()
		{
			// Exception details
			IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			if (exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null)
			{
				// Accesing message of Exception
				HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
				ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message + HttpContext.Items?["Custom-Error"];

			}
			return View();
		}
	}
}
