using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public virtual async Task<T> GetById<T>(TId id) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var events = await _eventStore.GetEventsForStream(id);            
            return Replay<T>(events);
        }

        public virtual async Task Save<T>(T aggregate, int? expectedVersion = default) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            var unpublishedEvents = aggregate.GetUnpublishedEvents();
            _eventStore.AppendEvents(aggregate.Id, unpublishedEvents, expectedVersion);
            await _eventStore.CommitEvents();

            //await PublishEvents(unpublishedEvents);
            aggregate.MarkEventsAsPublished();
        }

        public virtual async Task Save<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents());
            }

            await _eventStore.CommitEvents();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public virtual async Task SaveAndArchive<T>(T aggregate, int? expectedVersion = default) 
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            _eventStore.ArchiveStream(aggregate.Id);
            await _eventStore.CommitEvents();
            aggregate.MarkEventsAsPublished();
        }

        public virtual async Task SaveAndArchive<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                _eventStore.AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents());
                _eventStore.ArchiveStream(aggregate.Id);
            }

            await _eventStore.CommitEvents();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        // private async Task PublishEvents<T>(params T[] aggregates)
        //     where T : class, IEventSourcingAggregateRoot<TId>, new()
        // {
        //     if (_eventPublisher != null)
        //     {
        //         foreach (var aggregate in aggregates)
        //         {
        //             var unpublishedEvents = aggregate.GetUnpublishedEvents();
        //             foreach ( var unpub in unpubEvents)
        //             {
        //                 await context.Publish(unpub);
        //             }
        //             await _eventPublisher.Publish(unpublishedEvent, unpublishedEvent.GetType());
        //         }
        //     }
        // }

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
