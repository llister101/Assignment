using Assignment.Models;
using Assignment.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assignment.Pages
{
    public class RegisterModel : PageModel
	{ 
    private UserManager<ApplicationMember> userManager { get; }
	private SignInManager<ApplicationMember> signInManager { get; }
	private readonly RoleManager<IdentityRole> roleManager;
	[BindProperty]
	public Register RModel { get; set; }
	public RegisterModel(UserManager<ApplicationMember> userManager,
	SignInManager<ApplicationMember> signInManager,
	RoleManager<IdentityRole> roleManager)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
		this.roleManager = roleManager;
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
				WhoAmI = RModel.WhoAmI

			};
			//Create the Member role if NOT exist
			IdentityRole role = await roleManager.FindByIdAsync("Member");
			if (role == null)
			{
				IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Member"));
				if (!result2.Succeeded)
				{
					ModelState.AddModelError("", "Create role member failed");
				}
			}

			var result = await userManager.CreateAsync(user, RModel.Password);
			if (result.Succeeded)
			{
				//Add users to Member Role
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
