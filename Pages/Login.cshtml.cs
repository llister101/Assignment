using Assignment.Models;
using Assignment.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Assignment.Pages
{
	public class LoginModel : PageModel
	{
		private readonly ILogger _logger;

		[BindProperty]
		public Login LModel { get; set; }
		private readonly SignInManager<ApplicationMember> signInManager;
		public LoginModel(SignInManager<ApplicationMember> signInManager, ILogger<LoginModel> logger)
		{
			_logger = logger;
			this.signInManager = signInManager;
		}
		public void OnGet()
		{
		}
		public bool ValidateCaptcha()
		{
			bool result = true;

			string captchaResponse = Request.Form["g-recaptcha-response"];

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create
		   ("https://www.google.com/recaptcha/api/siteverify?secret=6Le7LlcpAAAAAPnzta0OWQS5tBqRLeECE0DUUtei &response=" + captchaResponse);


			try
			{

				using (WebResponse wResponse = req.GetResponse())
				{
					using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
					{

						string jsonResponse = readStream.ReadToEnd();

						myObject jsonObject = JsonConvert.DeserializeObject<myObject>(jsonResponse);

						result = Convert.ToBoolean(jsonObject.success);
						_logger.LogInformation(jsonResponse.ToString());
					}
				}

				return result;
			}
			catch (WebException ex)
			{
				throw ex;
			}
		}

	public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				if (!ValidateCaptcha())
				{
					var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, false);
					if (identityResult.Succeeded)
					{
						//Create the security context
						var claims = new List<Claim> {
							new Claim(ClaimTypes.Name, "c@c.com"),
							new Claim(ClaimTypes.Email, "c@c.com"),
							new Claim("Department", "HR")
						};

						var i = new ClaimsIdentity(claims, "MyCookieAuth");
						ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
						await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
						return RedirectToPage("Homepage");
					}
				}
			}
			return Page();
		}
	}

	public class myObject
	{
		public string success { get; set; }
		public List<string> ErrorMessage { get; set; }
	}
}
