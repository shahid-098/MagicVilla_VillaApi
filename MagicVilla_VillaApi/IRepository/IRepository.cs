using MagicVilla_VillaApi.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaApi.IRepository
{
    public interface IRepository <T> where T : class
    {
        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties=null);
        public Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string? includeProperties = null);
        public Task CreateAsync(T entity);
        public Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
