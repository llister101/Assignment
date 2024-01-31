using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Web;

namespace Assignment.Pages
{
	[AutoValidateAntiforgeryToken]
	public class ChangePasswordModel : PageModel
	{
		[BindProperty]
		public string oldPassword { get; set; }
		[BindProperty]
		public string Password { get; set; }
		[BindProperty]
		public string confirmPassword { get; set; }
		private readonly AuthDbContext _context;
		private readonly IHttpContextAccessor _contextAccessor;
		private UserManager<ApplicationMember> _userManager { get; set; }
		private SignInManager<ApplicationMember> _signInManager { get; set; }
		private ILogger<ChangePasswordModel> _logger { get; set; }

		public ChangePasswordModel(AuthDbContext context, UserManager<ApplicationMember> userManager, SignInManager<ApplicationMember> signInManager, IHttpContextAccessor contextAccessor, ILogger<ChangePasswordModel> logger)
		{
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
			_contextAccessor = contextAccessor;
			_logger = logger;
		}
		public IActionResult OnGet()
		{
			var email = _contextAccessor.HttpContext.Session.GetString("Email");
			_logger.LogInformation(email);
			_logger.LogInformation(User.FindFirstValue(ClaimTypes.Email));
			if (email == null && User.FindFirstValue(ClaimTypes.Email) == null)
			{
				return RedirectToPage("Login");
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var email = _contextAccessor.HttpContext.Session.GetString("Email");
				ApplicationMember user;
				if (email != null)
				{
					user = _context.Users.ToList().Where(member => member.Email == email).FirstOrDefault();
				}
				else
				{
					user = _context.Users.ToList().Where(member => member.Email == User.FindFirstValue(ClaimTypes.Email)).FirstOrDefault();
				}
				var passwordcheck = await _signInManager.CheckPasswordSignInAsync(user, Password, false);
				var oldpasswordcheck = await _signInManager.CheckPasswordSignInAsync(user, oldPassword, false);
				var passwordcheck2 = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHashed2, Password);
				if (!oldpasswordcheck.Succeeded)
				{
					ModelState.AddModelError("", "Your password is wrong.");
				}
				else
				{
					if (user.MinimumPasswordAge > DateTime.Now)
					{
						ModelState.AddModelError("", "Password has been changed recently, please wait a day after you last changed it.");
					}
					else if (passwordcheck.Succeeded || passwordcheck2 != PasswordVerificationResult.Failed)
					{
						ModelState.AddModelError("", "Your password can't be the same as your previous 2.");
					}
					else if (Password != confirmPassword)
					{
						ModelState.AddModelError("", "Your password and confirm password aren't the same.");
					}
					else
					{
						var token = await _userManager.GeneratePasswordResetTokenAsync(user);
						user.PasswordHashed2 = user.PasswordHash;
						var result = await _userManager.ResetPasswordAsync(user, token, HttpUtility.HtmlEncode(Password));
						if (result.Succeeded)
						{
							_context.AuditLogs.Add(new AuditLog
							{
								UserId = user.Id,
								ActivityDetails = "User has changed their password.",
								Timestamp = DateTime.Now,
							});
							user.MinimumPasswordAge = DateTime.Now.AddDays(1);
							user.MaximumPasswordAge = DateTime.Now.AddDays(120);
							_contextAccessor.HttpContext.Session.Remove("Email");
							await _context.SaveChangesAsync();
							return RedirectToPage("Homepage");
						}
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}
					}
				}
			}
			return Page();
		}
	}
}
