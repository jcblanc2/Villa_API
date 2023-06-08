using MagicVilla_API.Model;
using MagicVilla_API.Models;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        public Task UpdateAsync(VillaNumber villaNumber);
    }
}
