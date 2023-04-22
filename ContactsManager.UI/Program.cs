using ContactsManager.Middleware;
using ContactsManager.StartupExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// Configure Serilog as the default Logger
// =====================================================
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
	// Allowing serilog to read the configuration settings "appsettings.json" from builtin IConfiguration and allowing it to read current app's services and maken them available
	loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services);
});

// ----------------------------------------------------
//				Custom Services Class (cleanness)
// ----------------------------------------------------
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// =====================================================
// Exception page for development
// =====================================================
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	// Built-in Exception Handler to custom created page
	app.UseExceptionHandler("/error");

	// Created one
	app.UseExceptionHandlingMiddleware();
}

// ----------------------------------------------------
//				Request HTTPS and force it
// add also "http://localhost:52789" in launchSettings.json
// firefox doesn't work well in "Developer" environment
// Workaround: https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-7.0&tabs=visual-studio%2Clinux-ubuntu#trust-ff
// ----------------------------------------------------
//app.UseHsts();
//app.UseHttpsRedirection();

// ----------------------------------------------------
//				Create Application Pipeline
// ----------------------------------------------------

// Enabling Endpoint completion log (adds extra log message when response is completed)
app.UseSerilogRequestLogging();



// =====================================================
// Enable Http Logging (to log get and post requests)
// =====================================================
app.UseHttpLogging();

// =====================================================
//			Rotativa binaries for PDF Printing
// =====================================================
if (builder.Environment.IsEnvironment("Test") == false)
{
	Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "RotativaBinaries");
}

app.UseStaticFiles();

app.UseRouting(); // Identifying action method based on rout
app.UseAuthentication(); // Reads authentication cookie (after routing)
app.UseAuthorization(); // Validates if we can access with 
app.MapControllers(); // Executes filter pipeline (action + filters)

// Conventional Global routing (gets substituted if attribute routing
// used (fail safe, not really used)
app.UseEndpoints(endpoints =>
{
	// Admin Area Conventional Routing
	endpoints.MapControllerRoute(
		name: "areas",
		pattern: "{area:exists}/{controller=Home}/{action=Index}" // e.g Admin/Home/Index
	);

	// Normal Action Methods Conventional Routing
	endpoints.MapControllerRoute(
		name: "default",
		pattern: "{controller}/{action}"
		);
});

app.Run();

// =====================================================
//				For Integration Testing
// =====================================================
public partial class Program { } // To access auto-generated Program class programatically from the rest of the application (used because we are using top-level statements)