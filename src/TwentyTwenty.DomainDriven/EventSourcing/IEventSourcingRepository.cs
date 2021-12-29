using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventSourcingRepository<TId>
    {
        Task<T> GetById<T>(TId id, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task Save<T>(T aggregate, int? expectedVersion = null, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task Save<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task SaveAndArchive<T>(T aggregate, int? expectedVersion = null, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task SaveAndArchive<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventSourcingAggregateRoot<TId>, new();
    }
}