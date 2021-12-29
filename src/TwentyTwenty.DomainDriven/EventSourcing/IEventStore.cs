using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventStore<TId>
    {
        void AppendEvents(TId streamId, IEnumerable<IDomainEvent> events, int? expectedVersion = default);
        void ArchiveStream(TId streamId);
        List<IEventDescriptor> GetEventsForStream(TId streamId);
        Task<List<IEventDescriptor>> GetEventsForStreamAsync(TId streamId);
        Task CommitEventsAsync();
        void CommitEvents();
    }
}
