using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOS;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IUserRepository 
    {
        bool IsUnique(string username);
        Task<LoginResponseDto> Login(LoginDto loginDto);
        Task<LocalUser> Register(RegisterationDto registerationDto);
    }
}
