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
            await _eventStore.CommitEvents(token);
            await PublishEvents(aggregate, token);
        }

        public virtual async Task SaveOptimistic<T>(T aggregate, CancellationToken token = default) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var uncommittedEvents = aggregate.GetUncommittedEvents();
            _eventStore.AppendEvents(aggregate.Id, uncommittedEvents, aggregate.Version + uncommittedEvents.Count);
            await _eventStore.CommitEvents(token);
            await PublishEvents(aggregate, token);
        }

        public virtual async Task Save<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.AppendEvents(aggregate.Id, aggregate.GetUncommittedEvents());
            }

            await _eventStore.CommitEvents(token);
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

            await _eventStore.CommitEvents(token);
            await PublishEvents(aggregates, token);
        }

        public virtual async Task Archive<T>(T aggregate, CancellationToken token = default) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            await _eventStore.ArchiveStream(aggregate.Id, token);
        }

        protected async Task PublishEvents<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            if (_eventPublisher != null)
            {
                var uncommittedEvents = aggregate.GetUncommittedEvents();
                foreach (var uncommittedEvent in uncommittedEvents)
                {
                    await _eventPublisher.Publish(uncommittedEvent, token);
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
            var aggregate = new T();

            if (!events.Any())
            {
                throw new AggregateNotFoundException();   
            }

            aggregate.LoadChangesFromHistory(events.Select(e => e.Data), currentVersion);
            return aggregate;
        }
    }
}
