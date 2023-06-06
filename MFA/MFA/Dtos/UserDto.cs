namespace MFA.Dtos
{
    public class UserDto
    {
        public string  Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Id { get; set; }
        public bool IsTwoFactor { get; set; }
    }
}
