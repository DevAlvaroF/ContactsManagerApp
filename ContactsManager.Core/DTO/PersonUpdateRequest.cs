using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{/// <summary>
 /// DTO Class for Updating a Person through a PersonId Guid
 /// </summary>
	public class PersonUpdateRequest
	{
		[Required(ErrorMessage = "Person ID can't be empty")]
		public Guid PersonId { get; set; }

		[Required(ErrorMessage = "{0} can't be empty")]
		public string? PersonName { get; set; }
		[Required(ErrorMessage = "{0} can't be empty")]
		[EmailAddress(ErrorMessage = "{0} should be valid")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }
		[DataType(DataType.Date)]
		public DateTime? DateOfBirth { get; set; }
		public GenderOptions? Gender { get; set; }
		[Required(ErrorMessage = "Please select a country")]
		public Guid? CountryId { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }

		/// <summary>
		/// Converts the current object of PersonAddRequest into Person type
		/// </summary>
		/// <returns>Person Object</returns>
		public Person ToPerson()
		{
			return new Person()
			{
				PersonId = this.PersonId,
				PersonName = this.PersonName,
				Email = this.Email,
				DateOfBirth = this.DateOfBirth,
				Gender = this.Gender.ToString(),
				Address = this.Address,
				ReceiveNewsLetters = this.ReceiveNewsLetters,
				CountryId = this.CountryId,
			};
		}
	}
}
