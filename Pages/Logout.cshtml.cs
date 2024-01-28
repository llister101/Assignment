using Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    {
		private readonly ILogger _logger;
		private readonly SignInManager<ApplicationMember> signInManager;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly AuthDbContext _context;
		public LogoutModel(AuthDbContext context, SignInManager<ApplicationMember> signInManager, IHttpContextAccessor httpContextAccessor, ILogger<LogoutModel> logger)
		{
			this.signInManager = signInManager;
			_contextAccessor = httpContextAccessor;
			_logger = logger;
			_context = context;
		}
		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostLogoutAsync()
		{
			_contextAccessor.HttpContext.Session.Clear();
			_contextAccessor.HttpContext.Session.Remove(".AspNetCore.Session");
			_contextAccessor.HttpContext.Session.Remove("AuthToken");
			_contextAccessor.HttpContext.Session.Clear();
			await _contextAccessor.HttpContext.Session.CommitAsync();
			if (Request.Cookies[".AspNetCore.Session"] != null)
			{
				_contextAccessor.HttpContext.Response.Cookies.Delete(".AspNetCore.Session");
			}
			if (Request.Cookies["AuthToken"] != null)
			{
				_contextAccessor.HttpContext.Response.Cookies.Delete("AuthToken");
			}
			var user = _context.Users.ToList().Where(member => member.Email == (User.FindFirstValue(ClaimTypes.Email))).FirstOrDefault();
			_context.AuditLogs.Add(new AuditLog
			{
				UserId = user.Id,
				ActivityDetails = "User has successfully logged out.",
				Timestamp = DateTime.Now,
			});
			user.SessionId = "";
			await _context.SaveChangesAsync();
			await signInManager.SignOutAsync();
			return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Homepage");
		}
	}
}
