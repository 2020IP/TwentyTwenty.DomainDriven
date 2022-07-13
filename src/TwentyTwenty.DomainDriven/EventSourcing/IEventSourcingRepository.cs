using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventSourcingRepository<TId> : IDisposable, IAsyncDisposable
    {
        Task<T> GetById<T>(TId id, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task Save<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task Save<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task SaveOptimistic<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task SaveOptimistic<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task Archive<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();
    }
}