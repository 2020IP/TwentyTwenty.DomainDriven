using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventSourcingRepository<TId>
    {
        Task<T> GetById<T>(TId id)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task Save<T>(T aggregate, int? expectedVersion = null)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task Save<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task SaveAndArchive<T>(T aggregate, int? expectedVersion = null)
            where T : class, IEventSourcingAggregateRoot<TId>, new();

        Task SaveAndArchive<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<TId>, new();
    }
}