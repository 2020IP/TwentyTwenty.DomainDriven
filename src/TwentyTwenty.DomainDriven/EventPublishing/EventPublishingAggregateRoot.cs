using System;
using System.Collections.Generic;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public abstract class EventPublishingAggregateRoot<TAggregate> : Entity<Guid>, IAggregateRoot<Guid>
        where TAggregate : EventPublishingAggregateRoot<TAggregate>
    {
        private readonly List<IDomainEvent> _changes = new List<IDomainEvent>();

        public IEnumerable<IDomainEvent> GetUnpublishedEvents()
        {
            return _changes;
        }

        public void MarkEventsAsPublished()
        {
            _changes.Clear();
        }

        protected void AddEvent(IDomainEvent @event)
        {
            _changes.Add(@event);
        }
    }
}