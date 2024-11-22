namespace WebApi.Models
{
    public class MailjetSettings
    {
        public required string ApiKey { get; set; }
        public required string ApiSecret { get; set; }
        public required string SenderEmail { get; set; }
        public required string SenderName { get; set; }
    }
}
