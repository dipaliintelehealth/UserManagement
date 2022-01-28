using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.ViewModel
{
   public class KeyValue<KeyType, ValueType> : IEquatable<KeyValue<KeyType, ValueType>>
    {
        public KeyType Id { get; set; }
        public ValueType Value { get; set; }

        public bool Equals(KeyValue<KeyType, ValueType> other)
        {
            return Id.Equals(other.Id);
        }
    }
    public class KeyValueStringComparer : IEqualityComparer<KeyValue<string, string>> 
    {
        public bool Equals(KeyValue<string, string> x, KeyValue<string, string> y)
        {
            if (x!= null && !string.IsNullOrEmpty(x.Id))
            {
                return x.Id.Equals(y?.Id);
            }
            return false;
        }

        public int GetHashCode(KeyValue<string, string> obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
