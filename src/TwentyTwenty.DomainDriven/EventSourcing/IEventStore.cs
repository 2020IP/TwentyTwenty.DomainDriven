using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventStore<TId> : IDisposable, IAsyncDisposable
    {
        Task<StreamEvents> GetEventsForStream(TId streamId, CancellationToken token = default);
        void AppendEvents(TId streamId, IEnumerable<IDomainEvent> events, long? expectedVersion = default);
        void ArchiveStream(TId streamId);
        Task SaveChanges(CancellationToken token = default);
    }
}
