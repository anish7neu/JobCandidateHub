using Application.Common.Interfaces;
using CleanArchitecture.Domain.ValueObjects;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Candidate> Candidates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {

                    case EntityState.Modified:
                    case EntityState.Added:

                        foreach (var property in entry.CurrentValues.Properties)
                        {
                            if (property.ClrType == typeof(string))
                            {
                                entry.CurrentValues[property] = (entry.CurrentValues[property] as string)?.Trim();
                            }
                        }
                        break;

                    default:
                        break;
                }

            }

            var result = await base.SaveChangesAsync(cancellationToken);

            //await DispatchEvents();

            return result;
        }

        //private async Task DispatchEvents()
        //{
        //    while (true)
        //    {
        //        var domainEventEntity = ChangeTracker.Entries<IHasDomainEvent>()
        //            .Select(x => x.Entity.DomainEvents)
        //            .SelectMany(x => x)
        //            .Where(domainEvent => !domainEvent.IsPublished)
        //            .FirstOrDefault();
        //        if (domainEventEntity == null) break;

        //        domainEventEntity.IsPublished = true;
        //        await _domainEventService.Publish(domainEventEntity);
        //    }
        //}
    }
}
