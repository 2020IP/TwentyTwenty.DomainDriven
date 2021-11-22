using System;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEvent : IEventDescriptor
    {
        public Guid Id { get; }
        public IDomainEvent Data { get; }
        public DateTime TimeStamp { get; }
        public long Version { get; }

        public MartenEvent(Guid id, long version, DateTime timeStamp, IDomainEvent data)
        {
            Id = id;
            Version = version;
            Data = data;
            TimeStamp = timeStamp;
        }
    }
}