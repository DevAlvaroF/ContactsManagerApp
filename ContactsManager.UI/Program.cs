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
app.UseRouting();
app.MapControllers();

app.Run();

// =====================================================
//				For Integration Testing
// =====================================================
public partial class Program { } // To access auto-generated Program class programatically from the rest of the application (used because we are using top-level statements)