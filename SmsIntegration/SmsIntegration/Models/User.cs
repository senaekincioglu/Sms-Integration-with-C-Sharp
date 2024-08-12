namespace SmsIntegration.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string VerificationCode { get; set; } // SMS kodunu saklamak için
        public bool IsVerified { get; set; } // Kullanıcının doğrulanıp doğrulanmadığını kontrol etmek için
    }
}
