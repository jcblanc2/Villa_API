using MagicVilla_FronEnd.Models.DTOS;

namespace MagicVilla_FronEnd.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginDto loginDto);
        Task<T> RegisterAsync<T>(RegisterationDto registerationDto);
    }
}
