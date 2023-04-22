using ContactsManager.Controllers;
using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ContactsManager.UI.Controllers
{
	[Route("[controller]/[action]")]

	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

		[HttpGet]
		[Authorize("CustomBlockIfLogin")] // Alternative: [AllowAnonymous]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[Authorize("CustomBlockIfLogin")]
		public async Task<IActionResult> Register(RegisterDTO registerDTO)
		{
			// Check for validation errors (might not happen with client side validation but is possible)
			if (ModelState.IsValid == false)
			{
				// Cycle through errors and pass them to ViewBag
				ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(temp => temp.ErrorMessage);
				return View(registerDTO);

			}

			// TODO: Implement extension ToApplicationUser on the RegisterDTO

			// Convert DTO to ApplicationUser manually
			ApplicationUser user = new ApplicationUser()
			{
				PersonName = registerDTO.PersonName,
				Email = registerDTO.Email,
				PhoneNumber = registerDTO.Phone,
				UserName = registerDTO.Email
			};

			// User will be tried to be added to database
			IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

			if (result.Succeeded)
			{
				// Create Admin role if required
				if (registerDTO.UserType == UserTypeOptions.Admin)
				{
					// Create role if not created
					if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
					{
						ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.Admin.ToString() };

						// Create role
						await _roleManager.CreateAsync(applicationRole);
					}

					// Add user to role
					await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
				}

				if (registerDTO.UserType == UserTypeOptions.User)
				{
					// Create role if not created
					if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
					{
						ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.User.ToString() };

						// Create role
						await _roleManager.CreateAsync(applicationRole);
					}

					// Add user to role
					await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
				}

				//Sign in and creates cookie dissapears soon
				await _signInManager.SignInAsync(user, isPersistent: false);



				// Redirect to Home if Success
				return RedirectToAction(nameof(PersonsController.Index), "Persons");
			}

			// Catch errors 
			foreach (IdentityError error in result.Errors)
			{
				// Add model error to Model State to print with Tag Helpers
				ModelState.AddModelError("Register", error.Description);
			}

			// Cycle through errors and pass them to ViewBag
			//ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(temp => temp.ErrorMessage);

			return View(registerDTO);


		}

		[HttpGet]
		[Authorize("CustomBlockIfLogin")]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[Authorize("CustomBlockIfLogin")]
		public async Task<IActionResult> Login(LoginDTO loginDTO, string ReturnUrl)
		{
			// Check for validation errors (might not happen with client side validation but is possible)
			if (ModelState.IsValid == false)
			{
				// Cycle through errors and pass them to ViewBag
				ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(temp => temp.ErrorMessage);

				return View(loginDTO);

			}

			// User will be tried to be logged in with password
			var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				// Check user type
				ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
				if (user != null)
				{
					if (await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
					{

						// TO DO: Work on the AdminArea design
						//return RedirectToAction("Index", "Home", new { area = "AdminArea" });
						return RedirectToAction(nameof(PersonsController.Index), "Persons");
					}
				}

				if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
				{
					return LocalRedirect(ReturnUrl);
				}
				// Redirect to Home if Success
				return RedirectToAction(nameof(PersonsController.Index), "Persons");
			}

			// Push errors
			ModelState.AddModelError("Login", "The user or password can't be found");

			return View(loginDTO);

		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction(nameof(PersonsController.Index), "Persons");
		}

		[Authorize("CustomBlockIfLogin")]
		public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
		{
			ApplicationUser User = await _userManager.FindByEmailAsync(email);

			if (User == null)
			{
				return Json(true);
			}
			return Json(false);
		}

	}
}
