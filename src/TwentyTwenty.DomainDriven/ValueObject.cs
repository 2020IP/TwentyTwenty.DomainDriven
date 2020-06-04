using System;
using System.Collections.Generic;
using System.Reflection;

namespace TwentyTwenty.DomainDriven
{
    public abstract class ValueObject<T> : IEquatable<T> where T : ValueObject<T>
    {        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            
            return Equals(obj as T);
        }
        
        public override int GetHashCode()
        {
            var fields = GetFields();
            
            var startValue = 17;
            var multiplier = 59;
            
            var hashCode = startValue;
            
            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                
                if (value != null)
                {
                    hashCode = hashCode * multiplier + value.GetHashCode();
                }
            }
            
            return hashCode;
        }
        
        public virtual bool Equals(T other)
        {
            if (other == null)
            {
                return false;
            }
            
            var t = GetType();
            var otherType = other.GetType();
            
            if (t != otherType)
            {
                return false;
            }
            
            var fields = GetFields();
            
            foreach (var field in fields)
            {
                var value1 = field.GetValue(other);
                var value2 = field.GetValue(this);
                
                if (value1 == null)
                {
                    if (value2 != null)
                    {
                        return false;
                    }
                }
                else if (!value1.Equals(value2))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private IEnumerable<FieldInfo> GetFields()
        {
            var t = GetType();
            var fields = new List<FieldInfo>();
            
            while (t != typeof(object))
            {
                fields.AddRange(t.GetTypeInfo().DeclaredFields);
                t = t.GetTypeInfo().BaseType;
            }
            
            return fields;
        }
        
        public static bool operator == (ValueObject<T> x, ValueObject<T> y)
        {
            
            if (Equals(null, x))
            {
                return true;
            }
            
            return x.Equals(y);
        }
        
        public static bool operator != (ValueObject<T> x, ValueObject<T> y)
        {
            return !(x == y);
        }
    }
}