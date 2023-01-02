using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Entities;

namespace Template.Core.Application.Contracts.Persistence
{
    public interface ICustomerRepositoryEntity : IRepositoryEntity<Customer>
    {
        public Task CustomImplementation(Customer customer);
    }
}
