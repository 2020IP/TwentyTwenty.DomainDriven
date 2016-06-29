using System;

namespace TwentyTwenty.DomainDriven
{
    public interface IUnitOfWork<TId> where TId : IEquatable<TId>
    {
        void Add<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : IAggregateRoot<TId>;
        
        TAggreateRoot Get<TAggreateRoot>(TId id, int? expectedVersion = null) where TAggreateRoot : IAggregateRoot<TId>;
        
        void Commit();        
    }
}