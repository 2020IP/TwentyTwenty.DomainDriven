using System.Collections.Generic;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public interface IEventPublishingAggregateRoot<TId> : IEntity<TId>, IAggregateRoot<TId>
    {
        IEnumerable<IDomainEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();
        bool HasUncommittedEvents { get; }
    }
}