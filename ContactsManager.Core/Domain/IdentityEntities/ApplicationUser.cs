using Microsoft.AspNetCore.Identity;

namespace ContactsManager.Core.Domain.IdentityEntities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		// Custom added property
		public string? PersonName { get; set; }

	}
}
