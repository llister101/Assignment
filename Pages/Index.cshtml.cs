using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assignment.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var storedSessionId = HttpContext.Session.GetString("AuthToken");

            if (storedSessionId != Request.Cookies["AuthToken"])
            {
                return RedirectToPage("Login");

            }
            return Page();
        }
    }
}