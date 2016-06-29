using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventStore
    {
        void SaveEvents(Guid aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = null);

        Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = default(int?));

        List<IEventDescriptor> GetEventsForAggregate(Guid aggregateId);
    }
}
