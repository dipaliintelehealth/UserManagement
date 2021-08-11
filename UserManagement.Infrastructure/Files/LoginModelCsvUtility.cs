﻿using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UserManagement.Contract.Utility;
using UserManagement.Domain;
using UserManagement.Infrastructure.Mapper.CSV;

namespace UserManagement.Infrastructure.Files
{
    public class LoginModelCsvUtility : ICsvFileUtility<LoginModelForCsv>
    {
        public IEnumerable<LoginModelForCsv> Read(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Stream Write(IEnumerable<LoginModelForCsv> data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<LoginModelForCsvMap>();
            csv.WriteRecords(data);
            writer.Flush();
            stream.Position = 0;
            var fs = new FileStream($"c:\\Dipali\\LoginModelForCsv_{DateTime.Now.ToString("ddMMyyyHHmmss")}.csv", FileMode.Create);
            fs.Write(stream.ToArray());
            fs.Close();
            return new MemoryStream(stream.ToArray());
        }
    }
}
