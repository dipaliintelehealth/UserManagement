using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UserManagement.Domain;
using UserManagement.Infrastructure.Mapper.CSV;

namespace UserManagement.Infrastructure.Files
{
    public class AuditTrailModelCsvUtility : CsvUtilityBase<AuditTrailModel>
    {
        public override IEnumerable<AuditTrailModel> Read(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override Stream Write(IEnumerable<AuditTrailModel> data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<AuditTrailModelForCsvMap>();
            csv.WriteRecords(data);
            writer.Flush();
            stream.Position = 0;
            var id = Guid.NewGuid();
            var fileName = $"{_configuration.CsvLogPath}/AuditTrailModelForCsv_{id}.csv";
            Log(fileName, stream.ToArray());
            return new MemoryStream(stream.ToArray());
        }
    }
}
