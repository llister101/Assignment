using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment.Pages
{
	public class ForgetPasswordModel : PageModel
	{
		private readonly AuthDbContext _context;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IEmailSender _emailSender;
		public ForgetPasswordModel(AuthDbContext context, IEmailSender emailSender, IHttpContextAccessor contextAccessor)
		{
			_context = context;
			_emailSender = emailSender;
			_contextAccessor = contextAccessor;
		}
		public async Task<IActionResult> OnGet()
        {
			var email = _contextAccessor.HttpContext.Session.GetString("Email");
			if (email == null)
			{
				return RedirectToPage("Login");
			}
			else
			{
				await _emailSender.SendEmailAsync(email, "Password Reset", "Click here to reset your password https://localhost:7278/ChangePassword");
				return Page();
			}
		}
    }
}
