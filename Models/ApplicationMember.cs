﻿using Microsoft.AspNetCore.Identity;

namespace Assignment.Models
{
	public class ApplicationMember : IdentityUser
	{
        public string PasswordHashed2 { get; set; }
		public DateTime MinimumPasswordAge { get; set; }
		public DateTime MaximumPasswordAge { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string NRIC {  get; set; }
		public string DateOfBirth {  get; set; }
		public string Resume {  get; set; }		
		public string WhoAmI { get; set; }
		public string SessionId { get; set; }
	}
}
