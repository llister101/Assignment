using Assignment.Models;
using Assignment.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;

namespace Assignment.Pages
{
	[AutoValidateAntiforgeryToken]
	public class RegisterModel : PageModel
	{ 
		private UserManager<ApplicationMember> userManager { get; }
		private SignInManager<ApplicationMember> signInManager { get; }
		private readonly RoleManager<IdentityRole> roleManager;
		[BindProperty]
		public Register RModel { get; set; }
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly AuthDbContext _context;
		public RegisterModel(UserManager<ApplicationMember> userManager,
		SignInManager<ApplicationMember> signInManager,
		RoleManager<IdentityRole> roleManager,
		IHttpContextAccessor httpContextAccessor,
		AuthDbContext context)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.roleManager = roleManager;
			_contextAccessor = httpContextAccessor;
			_context = context;
		}
		public void OnGet()
		{
		}

		//Save data into the database
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
				var protector = dataProtectionProvider.CreateProtector("HiddenWow");

				if (!Regex.IsMatch(RModel.Email, "^\\w+([.-]?\\w+)*@\\w+([.-]?\\w+)*(\\.\\w{2,3})+$"))
				{
				}
				if (!Regex.IsMatch(RModel.Password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*\\W)[a-zA-Z\\d\\W]{12,}$"))
				{
				}
				if(RModel.Password != RModel.ConfirmPassword){
				}
				if (!Regex.IsMatch(RModel.FirstName, "^[A-Za-z]*$"))
				{
				}
				if (!Regex.IsMatch(RModel.LastName, "^[A-Za-z]*$"))
				{
				}
				if (RModel.Gender == null)
				{
				}
				if (!Regex.IsMatch(RModel.NRIC, "^[A-Za-z][0-9]{7}[A-Za-z]$"))
				{
				}
				if ((DateTime.Parse(RModel.DateOfBirth) >= DateTime.Now))
				{
				}
				if (new string[] {".pdf", ".docx"}.Any(x => x.ToLowerInvariant() == Path.GetExtension(RModel.Resume)))
				{
				}
				if (RModel.WhoAmI == null)
				{
				}
				var user = new ApplicationMember()
				{
					UserName = RModel.Email,
					Email = RModel.Email,
					FirstName = RModel.FirstName,
					LastName = RModel.LastName,
					Gender = RModel.Gender,
					NRIC = protector.Protect(RModel.NRIC),
					DateOfBirth = RModel.DateOfBirth,
					Resume = RModel.Resume,
					WhoAmI = HttpUtility.HtmlEncode(RModel.WhoAmI)

				};

				//Create the Member role if NOT exist
				IdentityRole role = await roleManager.FindByIdAsync("Member");
				if (role == null)
				{
					await roleManager.CreateAsync(new IdentityRole("Member"));
				}

				var result = await userManager.CreateAsync(user, HttpUtility.HtmlEncode(RModel.Password));
				if (result.Succeeded)
				{
					string guid = Guid.NewGuid().ToString();
					_contextAccessor.HttpContext.Session.SetString("AuthToken", guid);
					_contextAccessor.HttpContext.Response.Cookies.Append("AuthToken", guid);
					var users = _context.Users.ToList().Where(member => member.Email == RModel.Email).FirstOrDefault();
					_context.AuditLogs.Add(new AuditLog
					{
						UserId = users.Id,
						ActivityDetails = "User has successfully signed up.",
						Timestamp = DateTime.Now,
					});
					user.MinimumPasswordAge = DateTime.Now.AddDays(1);
					user.MaximumPasswordAge = DateTime.Now.AddDays(120);
					await _context.SaveChangesAsync();
					result = await userManager.AddToRoleAsync(user, "Member");
					await signInManager.SignInAsync(user, false);
					return RedirectToPage("Homepage");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
		return Page();
		}
	}
}
