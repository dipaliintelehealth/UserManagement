using System.Collections.Generic;

namespace UserManagement.Domain
{
    public class ResultModel<T> where T :class
    {
        public T Model { get; set; }
        public bool Success { get; set; } = true;
        public List<string> Messages { get; set; } = new List<string>();
    }
}
