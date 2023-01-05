﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Core.Application.Contracts.Persistence
{
    public interface ICustomerRepositoryEntity : IRepositoryEntity<Customer>
    {
        public Task CustomImplementation(Customer customer);
    }
}
