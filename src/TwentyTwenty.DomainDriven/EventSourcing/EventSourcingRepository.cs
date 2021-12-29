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
            _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            _eventStore.CommitEvents();
            aggregate.MarkEventsAsPublished();
        }

        public async Task SaveAsync<T>(T aggregate, int? expectedVersion = default) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            await _eventStore.CommitEventsAsync();
            aggregate.MarkEventsAsPublished();
        }

        public void Save<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents());
            }

            _eventStore.CommitEvents();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public async Task SaveAsync<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents());
            }

            await _eventStore.CommitEventsAsync();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public void SaveAndArchive<T>(T aggregate, int? expectedVersion = null) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            _eventStore.ArchiveStream(aggregate.Id);
            _eventStore.CommitEvents();
            aggregate.MarkEventsAsPublished();
        }

        public async Task SaveAndArchiveAsync<T>(T aggregate, int? expectedVersion = default) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            _eventStore.ArchiveStream(aggregate.Id);
            await _eventStore.CommitEventsAsync();
            aggregate.MarkEventsAsPublished();
        }

        public void SaveAndArchive<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents());
                _eventStore.ArchiveStream(aggregate.Id);
            }

            _eventStore.CommitEvents();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public async Task SaveAndArchiveAsync<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents());
                _eventStore.ArchiveStream(aggregate.Id);
            }

            await _eventStore.CommitEventsAsync();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public T GetById<T>(TId id) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var events = _eventStore.GetEventsForStream(id);
            return Replay<T>(events);
        }

        public async Task<T> GetByIdAsync<T>(TId id) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var events = await _eventStore.GetEventsForStreamAsync(id);            
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
