namespace MagicVilla_FronEnd.Models.DTOS
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
