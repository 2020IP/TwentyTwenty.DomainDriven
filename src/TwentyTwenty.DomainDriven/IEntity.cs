using System;

namespace TwentyTwenty.DomainDriven
{
    public interface IEntity<TId> : IEquatable<IEntity<TId>>
    {
        TId Id { get; }
    }
}