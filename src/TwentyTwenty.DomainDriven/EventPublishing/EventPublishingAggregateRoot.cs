using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public abstract class EventPublishingAggregateRoot<TId> : Entity<TId>, IEventPublishingAggregateRoot<TId>
    {
        protected readonly List<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();
        protected virtual void AddEvent(IDomainEvent @event) => _uncommittedEvents.Add(@event);
        
        [IgnoreDataMember]
        public virtual bool HasUncommittedEvents => _uncommittedEvents.Count > 0;
        public virtual IList<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents;
        public virtual void ClearUncommittedEvents() => _uncommittedEvents.Clear();
    }
}