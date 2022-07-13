using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public interface IEventPublishingRepository<TId> : IDisposable, IAsyncDisposable
    {
        Task<T> GetById<T>(TId id, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        Task Save<T>(T entity, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        Task Save<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        Task Delete<T>(T entity, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<TId>, new();
    }
}