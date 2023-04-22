using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO
{
	public class RegisterDTO
	{
		[Required]
		public string PersonName { get; set; }

		[Required]
		[EmailAddress]
		[Remote(action: "isEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already taken (Js)")]
		public string Email { get; set; }

		[Required]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Phone number can only contain numbers")]
		public string Phone { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords should match")]
		public string ConfirmPassword { get; set; }

		[Required]
		public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
	}
}
