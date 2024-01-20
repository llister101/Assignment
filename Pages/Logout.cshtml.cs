using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assignment.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<ApplicationMember> signInManager;
		public LogoutModel(SignInManager<ApplicationMember> signInManager)
		{
			this.signInManager = signInManager;
		}
		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostLogoutAsync()
		{
			await signInManager.SignOutAsync();
			return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Homepage");
		}
	}
}
