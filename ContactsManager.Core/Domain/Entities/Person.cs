using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
	/// <summary>
	/// Person Domain Model class
	/// </summary>
	public class Person
	{
		[Key]
		public Guid PersonId { get; set; }
		[StringLength(40)]
		public string? PersonName { get; set; }
		[StringLength(40)]
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		[StringLength(10)]
		public string? Gender { get; set; }
		public Guid? CountryId { get; set; }
		[StringLength(200)]
		public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }

		public string? TIN { get; set; }

		// This is the relation to the other entity (table)
		[ForeignKey("CountryId")]
		public virtual Country? Country { get; set; }

		public override string ToString()
		{
			return $"(Custom Generated) PersonId: {PersonId}, PersonName:{PersonName}, CountryName:{Country?.CountryName}, CountryId:{Country?.CountryId}";
		}
	}
}
