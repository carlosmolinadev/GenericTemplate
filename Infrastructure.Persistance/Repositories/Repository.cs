using Microsoft.EntityFrameworkCore;
using Template.Core.Application.Contracts.Persistence;


namespace Template.Infrastructure.Persistance.Repositories
{
    public class Repository<T> : IRepositoryEntity<T> where T : class
    {
        protected readonly TemplateDbContext _dbContext;

        public Repository(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            T? t = await _dbContext.Set<T>().FindAsync(id);
            return t;
        }

        public async virtual Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async virtual Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size)
        {
            return await _dbContext.Set<T>().Skip((page - 1) * size).Take(size).AsNoTracking().ToListAsync();
        }

        public async virtual Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public virtual void UpdateAsync(T entity)
        {   
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async virtual Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _dbContext.Set<T>().Remove(entity);
            }
        }
    }
}
