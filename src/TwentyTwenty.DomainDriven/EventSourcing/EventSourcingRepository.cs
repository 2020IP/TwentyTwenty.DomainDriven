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
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        public void Save<T>(T aggregate, int? expectedVersion = null) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var changes = aggregate.GetUnpublishedEvents();
            _eventStore.AppendEvents(aggregate.Id, changes, expectedVersion);
            _eventStore.CommitEvents();
            aggregate.MarkEventsAsPublished();
        }

        public void Save<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates.DefaultIfEmpty())
            {
                Save(aggregate);
            }
        }

        public async Task SaveAsync<T>(T aggregate, int? expectedVersion = default) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var changes = aggregate.GetUnpublishedEvents();
            _eventStore.AppendEvents(aggregate.Id, changes, expectedVersion);
            await _eventStore.CommitEventsAsync();
            aggregate.MarkEventsAsPublished();;
        }

        public async Task SaveAsync<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates.DefaultIfEmpty())
            {
                await SaveAsync(aggregate);
            }
        }

        public T GetById<T>(TId id) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var events = _eventStore.GetEventsForAggregate(id);
            return Replay<T>(events);
        }

        public async Task<T> GetByIdAsync<T>(TId id) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
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
