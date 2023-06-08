using MagicVilla_API.Model;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;

namespace MagicVilla_API.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public VillaNumberRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task UpdateAsync(VillaNumber entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _dbContext.VillasNumber.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
