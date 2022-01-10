using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using UserManagement.Domain.ViewModel;
using UserManagement.Infrastructure.Mapper.CSV;
using UserManagement.Models;

namespace UserManagement.Infrastructure.Files
{
    public class MemberBulkImportVmCsvUtility : CsvUtilityBase<MemberBulkImportVM>
    {
        private readonly string _sessionId;

        public MemberBulkImportVmCsvUtility(string sessionId)
        {
            _sessionId = sessionId;
        }
        public override IEnumerable<MemberBulkImportVM> Read(Stream stream)
        {
            //need to refactor and move file path to upper level and transfer only stream to read
            var fileName = $"{_configuration.CsvLogPath}/MemberBulkImportVmCsv_{_sessionId}.csv";
            var reader = new StreamReader(fileName);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MemberBulkImportVmCsvMap>();
            var records = csv.GetRecords<MemberBulkImportVM>();
            //reader.Close();
            return records;
        }

        public override Stream Write(IEnumerable<MemberBulkImportVM> data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MemberBulkImportVmCsvMap>();
            csv.WriteRecords(data);
            writer.Flush();
            stream.Position = 0;
            var fileName = $"{_configuration.CsvLogPath}/MemberBulkImportVmCsv_{_sessionId}.csv";
            Log(fileName, stream.ToArray());
            return new MemoryStream(stream.ToArray());
        }
    }
    public class MemberBulkValidCsvUtility : CsvUtilityBase<MemberBulkValid>
    {
        private readonly string _sessionId;

        public MemberBulkValidCsvUtility(string sessionId)
        {
            _sessionId = sessionId;
        }
        public override IEnumerable<MemberBulkValid> Read(Stream stream)
        {
            //need to refactor and move file path to upper level and transfer only stream to read
            var fileName = $"{_configuration.CsvLogPath}/MemberBulkValidCsv_{_sessionId}.csv";
            var records = Enumerable.Empty<MemberBulkValid>();
            if (!File.Exists(fileName))
            {
                return records;
            }
            using (var reader = new StreamReader(fileName))
            {
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<MemberBulkValidCsvMap>();
                records = csv.GetRecords<MemberBulkValid>()?.ToList();
            }
            return records;
        }

        public override Stream Write(IEnumerable<MemberBulkValid> data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MemberBulkValidCsvMap>();
            csv.WriteRecords(data);
            writer.Flush();
            stream.Position = 0;
            var fileName = $"{_configuration.CsvLogPath}/MemberBulkValidCsv_{_sessionId}.csv";
            Log(fileName, stream.ToArray());
            return new MemoryStream(stream.ToArray());
        }
        public bool CompleteTask()
        {
            var fileName = $"{_configuration.CsvLogPath}/MemberBulkValidCsv_{_sessionId}";
            System.IO.FileInfo fi = new System.IO.FileInfo($"{fileName}.csv");
            // Check if file is there  
            if (fi.Exists)
            {
                // Move file with a new name. Hence renamed.  
                fi.MoveTo($"{fileName}_processed.csv");
            }

            return true;
        }
    }
    public class MemberBulkInvalidCsvUtility : CsvUtilityBase<MemberBulkInvalid>
    {
        private readonly string _sessionId;

        public MemberBulkInvalidCsvUtility(string sessionId)
        {
            _sessionId = sessionId;
        }
        public override IEnumerable<MemberBulkInvalid> Read(Stream stream)
        {
            //need to refactor and move file path to upper level and transfer only stream to read
            var fileName = $"{_configuration.CsvLogPath}/MemberBulkInvalidCsv_{_sessionId}.csv";
            var records = Enumerable.Empty<MemberBulkInvalid>();
            if (!File.Exists(fileName))
            {
                return records;
            }
            using (var reader = new StreamReader(fileName))
            {
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<MemberBulkInvalidCsvMap>();
                records = csv.GetRecords<MemberBulkInvalid>()?.ToList();
            }
            return records;
        }

        public override Stream Write(IEnumerable<MemberBulkInvalid> data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MemberBulkInvalidCsvMap>();
            csv.WriteRecords(data);
            writer.Flush();
            stream.Position = 0;
            var fileName = $"{_configuration.CsvLogPath}/MemberBulkInvalidCsv_{_sessionId}.csv";
            Log(fileName, stream.ToArray());
            return new MemoryStream(stream.ToArray());
        }
        public bool CompleteTask()
        {
            var fileName = $"{_configuration.CsvLogPath}/MemberBulkInvalidCsv_{_sessionId}";
            System.IO.FileInfo fi = new System.IO.FileInfo($"{fileName}.csv");
            // Check if file is there  
            if (fi.Exists)
            {
                // Move file with a new name. Hence renamed.  
                fi.MoveTo($"{fileName}_processed.csv");
            }

            return true;
        }
    }
}