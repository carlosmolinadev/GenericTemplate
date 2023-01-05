namespace Core.Contracts.Persistence
{
    public interface IUnitOfWorkEntity : IDisposable
    {
        Task Save();
        IRepositoryEntity<TEntity> GetRepository<TEntity>() where TEntity : class;

        ICustomerRepositoryEntity CustomerRepository { get; }
    }
}
