using MagicVilla_FronEnd.Models.DTOS;
using System.Linq.Expressions;

namespace MagicVilla_FronEnd.Services.IServices
{
    public interface IVillaService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateVillaAsync<T>(VillaCreateDto dto);
        Task<T> UpdateAsync<T>(VillaUpdateDto dto);
        Task<T> RemoveAsync<T>(int id);
    }
}