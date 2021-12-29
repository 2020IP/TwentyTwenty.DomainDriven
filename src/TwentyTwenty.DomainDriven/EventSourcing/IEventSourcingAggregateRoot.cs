using System.Collections.Generic;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventSourcingAggregateRoot<TId> : IEventPublishingAggregateRoot<TId>
    {
        long Version { get; }
        void LoadChangesFromHistory(IEnumerable<IDomainEvent> history, long currentVersion);
    }
}