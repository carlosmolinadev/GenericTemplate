using Npgsql;
using System.Data;
using System.Data.Common;
using Template.Core.Application.Contracts.Persistence;
using Template.Domain.Entities;

namespace Template.Infrastructure.Persistance.Dapper.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(DbConnection connection) : base(connection)
        {
        }

        public async Task CustomImplementation(Customer customer)
        {
            await base.AddAsync(customer);
        }
    }
}
