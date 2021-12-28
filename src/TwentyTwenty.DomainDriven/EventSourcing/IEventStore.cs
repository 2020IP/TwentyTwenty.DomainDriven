using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventStore<TId>
    {
        void AppendEvents(TId aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = default);
        void ArchiveAggregate(TId aggregateId);
        List<IEventDescriptor> GetEventsForAggregate(TId aggregateId);
        Task<List<IEventDescriptor>> GetEventsForAggregateAsync(TId aggregateId);
        Task CommitEventsAsync();
        void CommitEvents();
    }
}
