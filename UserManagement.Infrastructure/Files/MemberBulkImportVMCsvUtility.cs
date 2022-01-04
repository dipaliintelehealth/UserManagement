using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            var reader = new StreamReader(fileName);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MemberBulkValidCsvMap>();
            var records = csv.GetRecords<MemberBulkValid>();
            //reader.Close();
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
            var reader = new StreamReader(fileName);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<MemberBulkInvalidCsvMap>();
            var records = csv.GetRecords<MemberBulkInvalid>();
            //reader.Close();
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
    }
}