using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public abstract class EventPublishingRepository<TId> : IEventPublishingRepository<TId>
    {
        private readonly IEventPublisher _eventPublisher;

        public EventPublishingRepository(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public abstract Task<T> GetById<T>(TId id, CancellationToken token = default) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        public abstract Task Save<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        public abstract Task Save<T>(IEnumerable<T> aggregates, CancellationToken token = default) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        public abstract Task Delete<T>(T aggregate, CancellationToken token = default) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        protected async Task PublishEvents<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<TId>, new()
        {
            if (_eventPublisher != null)
            {
                var uncommittedEvents = aggregate.GetUncommittedEvents();
                foreach (var uncommittedEvent in uncommittedEvents)
                {
                    await _eventPublisher.Publish(uncommittedEvent, uncommittedEvent.GetType(), token);
                }
            }

            aggregate.ClearUncommittedEvents();
        }

        protected async Task PublishEvents<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<TId>, new()
        {
            foreach (var aggregate in aggregates)
            {
                await PublishEvents(aggregate, token);
            }
        }
    }
}