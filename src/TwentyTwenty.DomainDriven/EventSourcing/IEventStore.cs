using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventStore<TId>
    {
        void SaveEvents(TId aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = null);

        Task SaveEventsAsync(TId aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = default(int?));

        List<IEventDescriptor> GetEventsForAggregate(TId aggregateId);

        Task<List<IEventDescriptor>> GetEventsForAggregateAsync(TId aggregateId);
    }
}
