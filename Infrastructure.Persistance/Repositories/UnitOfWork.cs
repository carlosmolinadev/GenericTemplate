﻿using Core.Contracts.Persistence;
using Persistance;

namespace Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWorkEntity
    {
        private readonly TemplateDbContext _context;
        public readonly ICustomerRepositoryEntity _customerRepository;

        public UnitOfWork(TemplateDbContext context, ICustomerRepositoryEntity customerRepository)
        {
            _context = context;
            _customerRepository = customerRepository;
        }

        public ICustomerRepositoryEntity CustomerRepository => _customerRepository;

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public IRepositoryEntity<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_context);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}


//private CustomerRepository _customer;
//public Repository<Customer>? CustomerRepository
//{
//    get
//    {
//        if(_customer == null)
//        {
//            _customer = new CustomerRepository(_context);
//        }
//        return _customer;
//    }
//}