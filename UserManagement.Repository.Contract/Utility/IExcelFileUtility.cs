using System;
using System.Text;
using UserManagement.Domain;

namespace UserManagement.Contract.Utility
{
    public interface IExcelFileUtility<T> : IFileUtility<T>
    {
        IExcelFileUtility<T> Configure(ExcelConfiguration excelConfiguration);
    }
}
