using System;
using System.Collections.Generic;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public interface IEventPublishingAggregateRoot<TId> : IEntity<TId>, IAggregateRoot<TId>
    {
        IEnumerable<IDomainEvent> GetUnpublishedEvents();
        void MarkEventsAsPublished();
    }
}