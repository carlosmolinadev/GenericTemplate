using Domain.Entities;
using Npgsql;
using System.Data;
using System.Data.Common;
using Template.Core.Application.Contracts.Persistence;

namespace Template.Infrastructure.Persistance.Dapper.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(NpgsqlConnection connection) : base(connection)
        {
        }

        public async Task CustomImplementation(Customer customer)
        {
            await base.AddAsync(customer);
        }
    }
}
