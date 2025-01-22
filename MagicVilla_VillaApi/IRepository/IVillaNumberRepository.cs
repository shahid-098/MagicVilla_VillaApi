using MagicVilla_VillaApi.Models;

namespace MagicVilla_VillaApi.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        public Task UpdateAsync(VillaNumber villaNumber);
    }
}
