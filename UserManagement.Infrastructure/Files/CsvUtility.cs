using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UserManagement.Contract.Utility;

namespace UserManagement.Infrastructure.Files
{
    public class CsvUtility<T> : ICsvFileUtility<T>
    {
        public virtual IEnumerable<T> Read(Stream stream)
        {
            throw new NotImplementedException();
        }

        public virtual Stream Write(IEnumerable<T> data)
        {
           var stream = new MemoryStream();
           using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
                foreach (var record in data)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }
            return stream;
        }
    }

}
