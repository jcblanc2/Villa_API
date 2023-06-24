using MagicVilla_FronEnd.Models.DTOS;

namespace MagicVilla_FronEnd.Services.IServices
{
    public interface IVillaNumberServices
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateVillaAsync<T>(VillaNumberCreateDto dto);
        Task<T> UpdateAsync<T>(VillaNumberUpdateDto dto);
        Task<T> RemoveAsync<T>(int id);
    }
}
