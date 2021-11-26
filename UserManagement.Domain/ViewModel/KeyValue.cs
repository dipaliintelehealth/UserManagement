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
}
