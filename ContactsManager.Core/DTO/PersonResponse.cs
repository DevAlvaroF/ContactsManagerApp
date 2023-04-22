using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
	public class PersonResponse
	{
		public Guid PersonId { get; set; }
		public string? PersonName { get; set; }
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Gender { get; set; }
		public Guid? CountryId { get; set; }
		public string? Country { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }
		public double? Age { get; set; }

		/// <summary>
		/// Compares the current object data with the paramters object
		/// </summary>
		/// <param name="obj">The PersonResponse Object to compare</param>
		/// <returns>true of false, indicating wether all person details matched</returns>
		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (obj.GetType() != typeof(PersonResponse)) return false;

			PersonResponse otherPerson = ((PersonResponse)obj);

			return (otherPerson.PersonId == this.PersonId && otherPerson.PersonName == this.PersonName &&
				otherPerson.Email == this.Email &&
				otherPerson.Address == this.Address &&
				otherPerson.Gender == this.Gender &&
				otherPerson.ReceiveNewsLetters == this.ReceiveNewsLetters &&
				otherPerson.CountryId == this.CountryId);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"Person {PersonName} - PersonId: {PersonId}";
		}

		public PersonUpdateRequest ToPersonUpdateRequest()
		{
			PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
			{
				PersonId = this.PersonId,
				PersonName = this.PersonName,
				Email = this.Email,
				DateOfBirth = this.DateOfBirth,
				CountryId = this.CountryId,
				Address = this.Address,
				ReceiveNewsLetters = this.ReceiveNewsLetters,
			};
			if (this.Gender != null) personUpdateRequest.Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), this.Gender, true);

			return personUpdateRequest;

		}
	}

	public static class PersonExtensions
	{
		/// <summary>
		/// An extension method to convert an object fo Person class into PersonResponse class
		/// </summary>
		/// <param name="person">The Person object to convert</param>
		/// <returns>Returns the converted PersonResponse object</returns>
		public static PersonResponse ToPersonResponse(this Person person)
		{
			return new PersonResponse()
			{
				PersonId = person.PersonId,
				PersonName = person.PersonName,
				Email = person.Email,
				CountryId = person.CountryId,
				DateOfBirth = person.DateOfBirth,
				Gender = person.Gender,
				Address = person.Address,
				ReceiveNewsLetters = person.ReceiveNewsLetters,
				Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
				Country = person.Country?.CountryName
			};
		}

	}
}
