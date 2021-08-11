using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UserManagement.Contract.Utility;
using UserManagement.Domain;
using UserManagement.Infrastructure.Mapper.CSV;

namespace UserManagement.Infrastructure.Files
{
    public class MembersModelForCsvUtility : ICsvFileUtility<MembersModelForCsv>
    {
        public IEnumerable<MembersModelForCsv> Read(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Stream Write(IEnumerable<MembersModelForCsv> data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MembersModelForCsvMap>();
            csv.WriteRecords(data);
            writer.Flush();
            stream.Position = 0;
            var fs = new FileStream($"c:\\Dipali\\MembersModelForCsv_{DateTime.Now.ToString("ddMMyyyHHmmss")}.csv", FileMode.Create);
            fs.Write(stream.ToArray());
            fs.Close();
            return new MemoryStream(stream.ToArray());
        }
    }
}
