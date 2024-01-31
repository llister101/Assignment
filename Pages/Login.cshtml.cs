using Assignment.Models;
using Assignment.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics.Eventing.Reader;

namespace Assignment.Pages
{
	[AutoValidateAntiforgeryToken]
	public class LoginModel : PageModel
	{
		[BindProperty]
		public Login LModel { get; set; }
		private readonly SignInManager<ApplicationMember> signInManager;
		private readonly ILogger<LoginModel> _logger;
		private readonly IHttpContextAccessor _contextAccessor;
		public LoginModel(SignInManager<ApplicationMember> signInManager, ILogger<LoginModel> logger, IHttpContextAccessor contextAccessor)
		{
			this.signInManager = signInManager;
			_logger = logger;
			_contextAccessor = contextAccessor;
		}
		public void OnGet()
		{
			if (_contextAccessor.HttpContext.Session.GetString("ModelError") != null)
			{
				ModelState.AddModelError("", _contextAccessor.HttpContext.Session.GetString("ModelError"));
			}
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
					}
				}

				return result;
			}
			catch (WebException ex)
			{
				throw ex;
			}
		}

		public async Task<IActionResult> OnPostAsync(string submitButton)
			{
				if (ModelState.IsValid)
				{
					if (!ValidateCaptcha())
					{
						if (submitButton == "button1")
						{
							_contextAccessor.HttpContext.Session.SetString("Email",LModel.Email);
							return RedirectToPage("ForgetPassword");
						}
						else if (submitButton == "button2") {
							var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);
							if (identityResult.Succeeded)
							{
								_contextAccessor.HttpContext.Session.Remove("ModelError");
								return RedirectToPage("TwoFactorEnabled");
							}
							else if (identityResult.IsLockedOut)
							{
								ModelState.AddModelError("", "Account has been locked out, please try again in 20 minutes");
							}
							else
							{
								ModelState.AddModelError("", "Username or password is incorrect");
							}
						}
					}
					else
					{
						ModelState.AddModelError("", "Captcha verification failed, please try again.");
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
