using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventSourcingAggregateRoot<TId> : IEventPublishingAggregateRoot<TId>
    {
        int Version { get; }

        void LoadChangesFromHistory(IEnumerable<IDomainEvent> history);
    }
}