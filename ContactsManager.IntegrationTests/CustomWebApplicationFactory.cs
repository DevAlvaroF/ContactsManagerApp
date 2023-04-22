using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDTests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.UseEnvironment("Test");

			// Services additional to Program (Add or Remove)
			builder.ConfigureServices(services =>
			{
				// Remove SQL Database services to test on memory DbContext environment without compromising the db
				ServiceDescriptor? descriptor = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

				if (descriptor != null) services.Remove(descriptor);

				// Re-Add DbContext from Memory
				services.AddDbContext<ApplicationDbContext>(options =>
				options.UseInMemoryDatabase("DatabaseForTesting"));
			});
		}
	}
}
