using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public class EventSourcingRepository<TId> : IEventSourcingRepository<TId>
    {
        private readonly IEventStore<TId> _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public EventSourcingRepository(IEventStore<TId> eventStore, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventPublisher = eventPublisher;
        }

        public virtual async Task<T> GetById<T>(TId id, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var streamEvents = await _eventStore.GetEventsForStream(id, token);
            return Replay<T>(streamEvents.Events, streamEvents.CurrentVersion);
        }

        public virtual async Task Save<T>(T aggregate, CancellationToken token = default)
           where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            _eventStore.AppendEvents(aggregate.Id, aggregate.GetUncommittedEvents());
            await _eventStore.SaveChanges(token);
            await PublishEvents(aggregate, token);
        }

        public virtual async Task SaveOptimistic<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var uncommittedEvents = aggregate.GetUncommittedEvents();
            _eventStore.AppendEvents(aggregate.Id, uncommittedEvents, aggregate.Version + uncommittedEvents.Count);
            await _eventStore.SaveChanges(token);
            await PublishEvents(aggregate, token);
        }

        public virtual async Task Save<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.AppendEvents(aggregate.Id, aggregate.GetUncommittedEvents());
            }

            await _eventStore.SaveChanges(token);
            await PublishEvents(aggregates, token);
        }

        public virtual async Task SaveOptimistic<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                var uncommittedEvents = aggregate.GetUncommittedEvents();
                _eventStore.AppendEvents(aggregate.Id, uncommittedEvents, aggregate.Version + uncommittedEvents.Count);
            }

            await _eventStore.SaveChanges(token);
            await PublishEvents(aggregates, token);
        }

        public virtual Task Archive<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            _eventStore.ArchiveStream(aggregate.Id);
            return _eventStore.SaveChanges(token);
        }

        public virtual Task Archive<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.ArchiveStream(aggregate.Id);
            }

            return _eventStore.SaveChanges(token);
        }

        protected async Task PublishEvents<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            if (_eventPublisher != null)
            {
                foreach (var uncommittedEvent in aggregate.GetUncommittedEvents())
                {
                    await _eventPublisher.Publish(uncommittedEvent, uncommittedEvent.GetType(), token);
                }
            }

            aggregate.ClearUncommittedEvents();
        }

        protected async Task PublishEvents<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                await PublishEvents(aggregate, token);
            }
        }

        private T Replay<T>(IEnumerable<IEventDescriptor> events, long currentVersion)
            where T : IEventSourcingAggregateRoot<TId>, new()
        {
            if (events is null || !events.Any())
            {
                throw new AggregateNotFoundException();
            }

            var aggregate = new T();
            aggregate.LoadChangesFromHistory(events.Select(e => e.Data), currentVersion);
            return aggregate;
        }

        public void Dispose()
            => _eventStore.Dispose();

        public ValueTask DisposeAsync()
            => _eventStore.DisposeAsync();
    }
}
