using MagicVilla_API.Model;
using MagicVilla_API.Models.DTOS;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IVillaRepository : IRepository<Villa>
    {
        public Task UpdateAsync(Villa entity);
    }
}
