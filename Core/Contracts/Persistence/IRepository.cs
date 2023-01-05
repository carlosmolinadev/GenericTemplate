namespace Core.Contracts.Persistence
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsynch();
        Task<int> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size);
    }
}
