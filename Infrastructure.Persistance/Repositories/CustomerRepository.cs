using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Template.Core.Application.Contracts.Persistence;

namespace Template.Infrastructure.Persistance.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepositoryEntity
    {
        public CustomerRepository(TemplateDbContext dbContext) : base(dbContext)
        {
        }

        public Task CustomImplementation(Customer customer)
        {
            base._dbContext.Set<Customer>().AddAsync(customer);
            base._dbContext.SaveChangesAsync();
            return Task.CompletedTask;
        }
    }
}
