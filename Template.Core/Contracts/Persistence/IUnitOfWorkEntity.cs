using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Core.Application.Contracts.Persistence
{
    public interface IUnitOfWorkEntity : IDisposable
    {
        Task Save();
        IRepositoryEntity<TEntity> GetRepository<TEntity>() where TEntity : class;

        ICustomerRepositoryEntity CustomerRepository { get; }
    }
}
