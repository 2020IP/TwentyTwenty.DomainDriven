using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public class InMemoryEventStore<TId> : IEventStore<TId>
    {
        private readonly IEventPublisher _publisher;

        private class EventDescriptor
        {
            public EventDescriptor(TId id, IDomainEvent eventData, int version)
            {
                Data = eventData;
                Version = version;
                Id = id;
            }

            public IDomainEvent Data { get; }

            public DateTime TimeStamp { get; set; }

            public TId Id { get; }

            public int Version { get; }
        }
        
        public InMemoryEventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        private readonly Dictionary<TId, List<EventDescriptor>> _current = new Dictionary<TId, List<EventDescriptor>>();

        public void SaveEvents(TId aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = null)
        {
            List<EventDescriptor> eventDescriptors;

            // try to get event descriptors list for given aggregate id
            // otherwise -> create empty dictionary
            if (!_current.TryGetValue(aggregateId, out eventDescriptors))
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

                // publish current event to the bus for further processing by subscribers
                _publisher.Publish(@event);
            }
        }

        // collect all processed events for given aggregate and return them as a list
        // used to build up an aggregate from its history (Domain.LoadsFromHistory)
        public  List<IEventDescriptor> GetEventsForAggregate(TId aggregateId)
        {
            List<EventDescriptor> eventDescriptors;

            if (!_current.TryGetValue(aggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }

            return eventDescriptors
                .Cast<IEventDescriptor>()
                .ToList();
        }

        public Task SaveEventsAsync(TId aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = default(int?))
        {
            SaveEvents(aggregateId, events, expectedVersion);
            return Task.FromResult(false);
        }

        public Task<List<IEventDescriptor>> GetEventsForAggregateAsync(TId aggregateId)
        {
            var events = GetEventsForAggregate(aggregateId);
            return Task.FromResult(events);
        }
    }
}
