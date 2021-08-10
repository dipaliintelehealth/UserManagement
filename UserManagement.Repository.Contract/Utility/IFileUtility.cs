using System.Collections.Generic;
using System.IO;

namespace UserManagement.Contract.Utility
{
    public interface IFileUtility<T>
    {
        IEnumerable<T> Read(Stream stream);
        Stream Write(IEnumerable<T> data);
    }
}
