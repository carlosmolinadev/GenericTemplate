﻿using GloboTicket.TicketManagement.Domain.Common;
using Marques.EFCore.SnakeCase;
using Microsoft.EntityFrameworkCore;
using Template.Core.Application.Contracts;
using Template.Domain.Entities;

namespace Template.Infrastructure.Persistance
{
    public class TemplateDbContext : DbContext
    {
        private readonly ILoggedInUserService? _loggedInUserService;

        //public GloboTicketDbContext(DbContextOptions<GloboTicketDbContext> options)
        //   : base(options)
        //{
        //}

        public TemplateDbContext(DbContextOptions<TemplateDbContext> options, ILoggedInUserService loggedInUserService)
            : base(options)
        {
            _loggedInUserService = loggedInUserService;
        }

        public DbSet<Customer> Customer { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ToSnakeCase();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateDbContext).Assembly);
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = _loggedInUserService.UserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        entry.Entity.LastModifiedBy = _loggedInUserService.UserId;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
