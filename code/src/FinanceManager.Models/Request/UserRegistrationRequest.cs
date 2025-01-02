namespace FinanceManager.Models.Request
{
    public class UserRegistrationRequest
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEmailVerified { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public DateTime? LastPasswordReset { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public bool IsPhoneVerified { get; set; }
        public string PictureUrl { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string AppMetadata { get; set; }
        public string UserMetadata { get; set; }
    }
}
