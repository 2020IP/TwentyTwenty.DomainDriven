using System;

namespace TwentyTwenty.DomainDriven
{
    public interface IDomainEvent : IMessage
    {
        Guid Id { get; }
    }
}
