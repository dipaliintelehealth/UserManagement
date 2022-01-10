using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UserManagement.Domain;
namespace UserManagement.Contract.Utility
{
    public interface ICsvFileUtility<T> : IFileUtility<T>
    {
        ICsvFileUtility<T> Configure(CsvConfiguration csvConfiguration);
        bool Log(string fileName,byte[] data);
    }
}
