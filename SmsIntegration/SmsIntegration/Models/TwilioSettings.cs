namespace SmsIntegration.Models
{
    public class TwilioSettings
    {
        public int Id { get; set; }
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string MyPhoneNumber { get; set; }
        public string TwilioPhoneNumber { get; set; }
    }
}
