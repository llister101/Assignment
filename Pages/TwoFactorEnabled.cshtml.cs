using Assignment.Models;
using Assignment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment.Pages
{
	[AutoValidateAntiforgeryToken]
	public class TwoFactorEnabledModel : PageModel
	{
		private UserManager<ApplicationMember> _userManager { get; }
		private readonly IEmailSender _emailSender;
		private readonly AuthDbContext _context;
		private readonly ILogger<TwoFactorEnabledModel> _logger;
		private readonly IHttpContextAccessor _contextAccessor;
		[BindProperty]
		public string userValue{ get; set; }

		public TwoFactorEnabledModel(AuthDbContext context, UserManager<ApplicationMember> userManager, IEmailSender emailSender, IHttpContextAccessor contextAccessor, ILogger<TwoFactorEnabledModel> logger)
		{
			_userManager = userManager;
			_emailSender = emailSender;
			_context = context;
			_contextAccessor = contextAccessor;
			_logger = logger;
		}

		public async Task<IActionResult> OnGet()
		{
			var user = _context.Users.ToList().Where(member => member.Email == (User.FindFirstValue(ClaimTypes.Email))).FirstOrDefault();
			var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
			await _emailSender.SendEmailAsync(user.Email, "Your 2FA Code", $"Your 2FA code is: { code }");
			return Page();
		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var code = userValue;
				var user = _context.Users.ToList().Where(member => member.Email == (User.FindFirstValue(ClaimTypes.Email))).FirstOrDefault();
				var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
				if (isCodeValid) {
					string guid = Guid.NewGuid().ToString();
					_contextAccessor.HttpContext.Session.SetString("AuthToken", guid);
					_contextAccessor.HttpContext.Response.Cookies.Append("AuthToken", guid);
					if (user.SessionId != "" && user.SessionId != Request.Cookies["AuthToken"])
					{
						_contextAccessor.HttpContext.Session.SetString("ModelError", "Another session is being used.");
						return RedirectToPage("Login");
					}
					else
					{
						if (user.MaximumPasswordAge < DateTime.Now)
						{
							user.SessionId = guid;
							await _context.SaveChangesAsync();
							return RedirectToPage("ChangePassword");
						}
						else {
							_context.AuditLogs.Add(new AuditLog
							{
								UserId = user.Id,
								ActivityDetails = "User has successfully logged in.",
								Timestamp = DateTime.Now,
							});
							user.SessionId = guid;
							await _context.SaveChangesAsync();
							return RedirectToPage("Homepage");
						}
					}
				}
				else
				{
					ModelState.AddModelError("", "Verification Code is wrong.");
				}
			}
			return Page();
		}
	}
}
