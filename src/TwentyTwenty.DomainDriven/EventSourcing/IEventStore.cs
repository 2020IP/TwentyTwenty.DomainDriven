using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventStore<TId>
    {
        Task<List<IEventDescriptor>> GetEventsForStream(TId streamId, CancellationToken token = default);
        void AppendEvents(TId streamId, IEnumerable<IDomainEvent> events, int? expectedVersion = default);
        void ArchiveStream(TId streamId);
        Task CommitEvents(CancellationToken token = default);
    }
}
