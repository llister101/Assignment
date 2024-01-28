using Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminsModel : PageModel
	{
		private readonly AuthDbContext _context;

		public AdminsModel(AuthDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> OnGet()
        {
			var user = _context.Users.ToList().Where(member => member.Email == (User.FindFirstValue(ClaimTypes.Email))).FirstOrDefault();
			_context.AuditLogs.Add(new AuditLog
			{
				UserId = user.Id,
				ActivityDetails = "Admin has viewed page.",
				Timestamp = DateTime.Now,
			});
			await _context.SaveChangesAsync();
			return Page();
		}
    }
}
