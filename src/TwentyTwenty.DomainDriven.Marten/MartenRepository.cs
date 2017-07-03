using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenRepository : MartenEventStore, IEventSourcingRepository<Guid>
    {
        public MartenRepository(IDocumentSession session)
            : base(session) { }

        public T GetById<T>(Guid id) 
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
            => _session.Events.AggregateStream<T>(id);

        public Task<T> GetByIdAsync<T>(Guid id) 
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
            => _session.Events.AggregateStreamAsync<T>(id);

        public void Save<T>(T aggregate, int? expectedVersion = default(int?)) 
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
        {
            var changes = aggregate
                .GetUnpublishedEvents();
            
            SaveEvents(aggregate.Id, changes);

            aggregate.MarkEventsAsPublished();
        }

        public async Task SaveAsync<T>(T aggregate, int? expectedVersion = default(int?)) 
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
        {
            var changes = aggregate
                .GetUnpublishedEvents();
            
            await SaveEventsAsync(aggregate.Id, changes).ConfigureAwait(false);
            
            aggregate.MarkEventsAsPublished();
        }
    }
}