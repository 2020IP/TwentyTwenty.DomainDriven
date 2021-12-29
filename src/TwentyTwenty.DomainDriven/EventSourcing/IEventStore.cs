using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventStore<TId>
    {
        Task<StreamEvents> GetEventsForStream(TId streamId, CancellationToken token = default);
        void AppendEvents(TId streamId, IEnumerable<IDomainEvent> events, long? expectedVersion = default);
        Task ArchiveStream(TId streamId, CancellationToken token = default);
        Task CommitEvents(CancellationToken token = default);
    }
}
