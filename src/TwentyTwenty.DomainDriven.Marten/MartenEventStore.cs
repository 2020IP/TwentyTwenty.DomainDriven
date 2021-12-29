using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEventStore : IEventStore<Guid>, IDisposable
    {
        private readonly IDocumentSession _session;

        public MartenEventStore(IDocumentSession session)
        {
            _session = session;
        }
        
        public async Task<List<IEventDescriptor>> GetEventsForStream(Guid streamId, CancellationToken token = default)
        {
            var stream = await _session.Events.FetchStreamAsync(streamId, token: token);
            return stream?.Select(e => (IEventDescriptor)new MartenEvent(e.Id, e.Version, e.Timestamp.UtcDateTime, e.Data as IDomainEvent)).ToList();
        }

        public void AppendEvents(Guid streamId, IEnumerable<IDomainEvent> events, int? expectedVersion = default)
        {
            if (expectedVersion.HasValue)
            {
                _session.Events.Append(streamId, expectedVersion, events);
            }
            else
            {
                _session.Events.Append(streamId, events);
            }
        }

        public void ArchiveStream(Guid streamId)
        {
            _session.Events.ArchiveStream(streamId);
        }

        public Task CommitEvents(CancellationToken token = default)
           =>  _session.SaveChangesAsync(token);

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}