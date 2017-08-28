using Marten;
using System;
using System.Threading.Tasks;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEventPublishingRepository : IEventPublishingRepository<Guid>
    {
        private readonly IDocumentSession _session;
        private readonly IEventPublisher _publisher;

        public MartenEventPublishingRepository(IDocumentSession session, IEventPublisher publisher)
        {
            _session = session;
            _publisher = publisher;
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
            await _session.SaveChangesAsync().ConfigureAwait(false);
            aggregate.MarkEventsAsPublished();
            return aggregate;
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
    }
}