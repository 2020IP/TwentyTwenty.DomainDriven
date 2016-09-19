using System;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventDescriptor
    {
        Guid Id { get; }

        int Version { get; }

        DateTime TimeStamp { get; }

        IDomainEvent Data { get; }
    }
}