namespace Assignment.Models
{
	public class AuditLog
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string ActivityDetails { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
