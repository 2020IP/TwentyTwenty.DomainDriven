using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public abstract class EventPublishingAggregateRoot<TId> : Entity<TId>, IEventPublishingAggregateRoot<TId>
    {
        protected readonly List<IDomainEvent> _changes = new List<IDomainEvent>();
        protected virtual void AddEvent(IDomainEvent @event) => _changes.Add(@event);
        
        [IgnoreDataMember]
        public virtual bool HasUnpublishedEvents => _changes.Count > 0;
        public virtual IEnumerable<IDomainEvent> GetUnpublishedEvents() => _changes;
        public virtual void MarkEventsAsPublished() => _changes.Clear();
    }
}