using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UserManagement.Contract.Utility;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Files
{
    public abstract class CsvUtilityBase<T> : ICsvFileUtility<T>
    {
        protected CsvConfiguration _configuration = new CsvConfiguration();
        public ICsvFileUtility<T> Configure(CsvConfiguration csvConfiguration)
        {
            _configuration = csvConfiguration;
            return this;
        }

        public Task<bool> Log(string fileName,byte[] data)
        {
            var directorypath = _configuration.CsvLogPath;
            foreach(var file in Directory.GetFiles(directorypath))
            {
                var fileAge = File.GetLastWriteTime(file) - DateTime.Now;
                if (fileAge.Days > 15)
                {
                    File.Delete(file);
                }
            }
            var fs = new FileStream(fileName, FileMode.Create);
            fs.Write(data);
            fs.Close();
            return Task.FromResult(true);
        }
        
        public abstract IEnumerable<T> Read(Stream stream);

        public abstract Stream Write(IEnumerable<T> data);


    }
}
