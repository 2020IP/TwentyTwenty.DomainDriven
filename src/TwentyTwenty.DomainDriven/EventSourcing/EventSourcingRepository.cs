using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public class EventSourcingRepository<TId> : IEventSourcingRepository<TId>
    {
        private readonly IEventStore<TId> _eventStore;

        public EventSourcingRepository(IEventStore<TId> eventStore)
        {
            if (eventStore == null)
            {
                throw new ArgumentNullException(nameof(eventStore));
            }
            _eventStore = eventStore;
        }

        public void Save<T>(T aggregate, int? expectedVersion = null) 
            where T : IEventSourcingAggregateRoot<TId>, new()
        {
            var changes = aggregate.GetUnpublishedEvents();
            _eventStore.SaveEvents(aggregate.Id, changes, expectedVersion);
            aggregate.MarkEventsAsPublished();
        }

        public T GetById<T>(TId id) 
            where T : IEventSourcingAggregateRoot<TId>, new()
        {
            var events = _eventStore.GetEventsForAggregate(id);
            return Replay<T>(events);
        }

        public async Task SaveAsync<T>(T aggregate, int? expectedVersion = default(int?)) 
            where T : IEventSourcingAggregateRoot<TId>, new()
        {
            var changes = aggregate.GetUnpublishedEvents();
            await _eventStore.SaveEventsAsync(aggregate.Id, changes, expectedVersion);
            aggregate.MarkEventsAsPublished();;
        }

        public async Task<T> GetByIdAsync<T>(TId id) 
            where T : IEventSourcingAggregateRoot<TId>, new()
        {
            var events = await _eventStore.GetEventsForAggregateAsync(id);            
            return Replay<T>(events);
        }

        private T Replay<T>(IEnumerable<IEventDescriptor> events) 
            where T : IEventSourcingAggregateRoot<TId>, new()
        {
            var obj = new T();

            if (!events.Any())
            {
                throw new AggregateNotFoundException();   
            }

            obj.LoadChangesFromHistory(events.Select(e => e.Data));
            return obj;
        }
    }
}
