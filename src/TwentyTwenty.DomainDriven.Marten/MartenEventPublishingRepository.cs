using Marten;
using System;
using System.Threading.Tasks;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEventPublishingRepository : IEventPublishingRepository<Guid>
    {
        private readonly IDocumentSession _session;

        public MartenEventPublishingRepository(IDocumentSession session)
        {
            _session = session;
        }

        public T Save<T>(T aggregate) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _session.Store(aggregate);
            _session.SaveChanges();
            aggregate.MarkEventsAsPublished();
            return aggregate;
        }

        public async Task<T> SaveAsync<T>(T aggregate) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _session.Store(aggregate);
            await _session.SaveChangesAsync();
            aggregate.MarkEventsAsPublished();
            return aggregate;
        }

        public void Save<T>(params T[] aggregates) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _session.Store(aggregates);
            _session.SaveChanges();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public async Task SaveAsync<T>(params T[] aggregates) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _session.Store(aggregates);
            await _session.SaveChangesAsync();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public T GetById<T>(Guid id) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            return _session.Load<T>(id);
        }

        public Task<T> GetByIdAsync<T>(Guid id) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            return _session.LoadAsync<T>(id);
        }

        public void Delete<T>(T aggregate) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _session.Delete(aggregate);
            _session.SaveChanges();
            aggregate.MarkEventsAsPublished();
        }

        public async Task DeleteAsync<T>(T aggregate) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _session.Delete(aggregate);
            await _session.SaveChangesAsync();
            aggregate.MarkEventsAsPublished();
        }
    }
}