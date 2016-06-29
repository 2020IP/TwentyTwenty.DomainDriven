using System;
using System.Linq;

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
            var obj = new T();

            var events = _eventStore.GetEventsForAggregate(id);
            
            if (!events.Any())
            {
                throw new AggregateNotFoundException();   
            }

            obj.LoadChangesFromHistory(events.Select(e => e.Data));
            return obj;
        }
    }
}
