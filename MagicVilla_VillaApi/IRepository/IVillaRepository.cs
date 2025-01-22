using MagicVilla_VillaApi.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaApi.IRepository
{
    public interface IVillaRepository : IRepository<Villa>
    {
        public Task UpdateAsync(Villa entity);
    }
}
