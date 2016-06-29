using System;
using System.Collections.Generic;

namespace TwentyTwenty.DomainDriven
{
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    {
        public virtual TId Id { get; protected set; }

        public bool Equals(Entity<TId> other)
        {
            if (other == null)
            {
                return false;
            }
            
            return Id.Equals(other.Id);
        }

        public virtual bool IsTransient()
        {
            return EqualityComparer<TId>.Default.Equals(Id, default(TId));
        }
        
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity<TId>;
            
            if (ReferenceEquals(compareTo, null))
            {
                return false;                
            }
            if (ReferenceEquals(this, compareTo))
            {
                return true;
            }
            if (!IsTransient() && !compareTo.IsTransient() && EqualityComparer<TId>.Default.Equals(Id, compareTo.Id))
            {
                return true;
            }
            
            return false;
        }
        
        public static bool operator ==(Entity<TId> a, Entity<TId> b)
        {
            // TODO: Verify this... do we want to treat two null entities as equal?
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }
            
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }
            
            return a.Equals(b);
        } 
        
        public static bool operator !=(Entity<TId> a, Entity<TId> b)
        {
            return !(a == b);
        }
    }
}