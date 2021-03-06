using System;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEvent : IEventDescriptor
    {
        public MartenEvent(Guid id, int version, DateTime timeStamp, IDomainEvent data)
        {
            Id = id;
            Version = version;
            Data = data;
            TimeStamp = timeStamp;
        }

        public IDomainEvent Data { get; }

        public Guid Id { get; }

        public DateTime TimeStamp { get; }

        public int Version { get; }
    }
}