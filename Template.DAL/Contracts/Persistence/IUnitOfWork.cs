using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Core.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task Save();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        //ICustomerRepository CustomerRepository { get; }
        public ICustomerRepository GetCustomerRepository();
    }
}
