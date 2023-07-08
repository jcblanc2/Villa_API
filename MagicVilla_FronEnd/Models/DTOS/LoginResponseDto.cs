namespace MagicVilla_FronEnd.Models.DTOS
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
}
