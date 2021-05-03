using System.Collections.Generic;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public abstract class EventPublishingAggregateRoot<TId> : Entity<TId>, IEventPublishingAggregateRoot<TId>
    {
        protected readonly List<IDomainEvent> _changes = new List<IDomainEvent>();

        public virtual IEnumerable<IDomainEvent> GetUnpublishedEvents() => _changes;

        public virtual bool HasUnpublishedEvents => _changes.Count > 0;

        public virtual void MarkEventsAsPublished() => _changes.Clear();

        protected virtual void AddEvent(IDomainEvent @event) => _changes.Add(@event);
    }
}