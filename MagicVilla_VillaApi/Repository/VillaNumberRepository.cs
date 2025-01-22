using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.IRepository;
using MagicVilla_VillaApi.Models;

namespace MagicVilla_VillaApi.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;
        public VillaNumberRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(VillaNumber villaNumber)
        {
            villaNumber.UpdatedDate = DateTime.Now;
            _db.Update(villaNumber);
            await _db.SaveChangesAsync();
        }
    }
}
