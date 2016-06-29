using System;

namespace TwentyTwenty.DomainDriven
{
    public interface IAggregateRoot<TId> where TId : IEquatable<TId>
    {
        
    }
}