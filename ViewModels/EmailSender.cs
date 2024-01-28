using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Assignment.ViewModels
{
	public class EmailSender : IEmailSender
	{
		public async Task SendEmailAsync(string email, string subject, string message)
		{
			var smtpClient = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				Credentials = new NetworkCredential("acejobagency442@gmail.com", "sqkl cwfh jzoe mqnd"),
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress("acejobagency442@gmail.com", "AceJobAgency"),
				Subject = subject,
				Body = message,
				IsBodyHtml = true,
			};

			mailMessage.To.Add(email);

			await smtpClient.SendMailAsync(mailMessage);
		}
	}
}