namespace MagicVilla_API.Models.DTOS
{
    public class LoginResponseDto
    {
        public LocalUser User { get; set; }
        public string Token { get; set; }
    }
}
