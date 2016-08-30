using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public class EventSourcingRepository : IEventSourcingRepository
    {
        private readonly IEventStore _eventStore;

        public EventSourcingRepository(IEventStore eventStore)
        {
            if (eventStore == null)
            {
                throw new ArgumentNullException(nameof(eventStore));
            }
            _eventStore = eventStore;
        }

        public void Save<T>(T aggregate, int? expectedVersion = null) where T : EventSourcingAggregateRoot<T>, new()
        {
            var changes = aggregate.GetUncommittedChanges();
            _eventStore.SaveEvents(aggregate.Id, changes, expectedVersion);
            aggregate.MarkChangesAsCommitted();
        }

        public T GetById<T>(Guid id) where T : EventSourcingAggregateRoot<T>, new()
        {
            var events = _eventStore.GetEventsForAggregate(id);
            return Replay<T>(events);
        }

        public async Task SaveAsync<T>(T aggregate, int? expectedVersion = default(int?)) where T : EventSourcingAggregateRoot<T>, new()
        {
            var changes = aggregate.GetUncommittedChanges();
            await _eventStore.SaveEventsAsync(aggregate.Id, changes, expectedVersion);
            aggregate.MarkChangesAsCommitted();
        }

        public async Task<T> GetByIdAsync<T>(Guid id) where T : EventSourcingAggregateRoot<T>, new()
        {
            var events = await _eventStore.GetEventsForAggregateAsync(id);            
            return Replay<T>(events);
        }

        private T Replay<T>(IEnumerable<IEventDescriptor> events) where T : EventSourcingAggregateRoot<T>, new()
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
