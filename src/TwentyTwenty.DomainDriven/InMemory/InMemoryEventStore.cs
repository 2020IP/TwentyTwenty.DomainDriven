using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.InMemory
{
    public class InMemoryEventStore<TId> : IEventStore<TId>
    {
        private class EventDescriptor
        {
            public IDomainEvent Data { get; }
            public DateTime TimeStamp { get; set; }
            public TId Id { get; }
            public long Version { get; }

            public EventDescriptor(TId id, IDomainEvent eventData, long version)
            {
                Data = eventData;
                Version = version;
                Id = id;
            }
        }

        private readonly Dictionary<TId, List<EventDescriptor>> _current = new Dictionary<TId, List<EventDescriptor>>();

        // collect all processed events for given aggregate and return them as a list
        // used to build up an aggregate from its history (Domain.LoadsFromHistory)
        public Task<StreamEvents> GetEventsForStream(TId aggregateId, CancellationToken token = default)
        {
            if (!_current.TryGetValue(aggregateId, out List<EventDescriptor> eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }

            var events = eventDescriptors
                .Cast<IEventDescriptor>()
                .ToList();

            return Task.FromResult(new StreamEvents
            {
                Events = events,
                CurrentVersion = events.Count,
            });
        }

        public void AppendEvents(TId aggregateId, IEnumerable<IDomainEvent> events, long? expectedVersion = null)
        {
            // try to get event descriptors list for given aggregate id
            // otherwise -> create empty dictionary
            if (!_current.TryGetValue(aggregateId, out List<EventDescriptor> eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                _current.Add(aggregateId, eventDescriptors);
            }
            // check whether latest event version matches current aggregate version
            // otherwise -> throw exception
            else if (expectedVersion.HasValue &&
                eventDescriptors[eventDescriptors.Count - 1].Version != expectedVersion &&
                expectedVersion != -1)
            {
                throw new ConcurrencyException();
            }

            var i = expectedVersion.GetValueOrDefault();

            // iterate through current aggregate events increasing version with each processed event
            foreach (var @event in events)
            {
                i++;

                // push event to the event descriptors list for current aggregate
                eventDescriptors.Add(new EventDescriptor(aggregateId, @event, i));
            }
        }

        public void ArchiveStream(TId aggregateIdt)
        {
        }

        public Task SaveChanges(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync() => default;
    }
}
