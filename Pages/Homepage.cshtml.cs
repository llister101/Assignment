using Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Assignment.Pages
{
    [Authorize]
    public class HomepageModel : PageModel
    {

        private readonly AuthDbContext _context;
        private readonly ILogger<HomepageModel> _logger;
        private readonly SignInManager<ApplicationMember> _signInManager;

        public HomepageModel(AuthDbContext context, ILogger<HomepageModel> logger, SignInManager<ApplicationMember> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
        }
        public IActionResult OnGet()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("Login");
            }
            else
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("HiddenWow");

                List<ApplicationMember> members = _context.Users.ToList();
                var useremail = User.FindFirstValue(ClaimTypes.Email);
                var user = members.Where(member => member.Email == useremail).FirstOrDefault();
                ViewData["Email"] = user.Email;
                ViewData["DateOfBirth"] = user.DateOfBirth;
                ViewData["FirstName"] = user.FirstName;
                ViewData["LastName"] = user.LastName;
                ViewData["Gender"] = user.Gender;
                ViewData["NRIC"] = protector.Unprotect(user.NRIC);
                ViewData["WhoAmI"] = user.WhoAmI;
                ViewData["Resume"] = user.Resume;

                return Page();
            }
        }

        public IActionResult OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("heloo");
                return RedirectToPage("ChangePassword");
            }
            return Page();
        }
    }
}
