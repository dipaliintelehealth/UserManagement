using CsvHelper.Configuration;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Mapper.CSV
{
    public sealed class LoginModelForCsvMap : ClassMap<LoginModelForCsv>
    {
        public LoginModelForCsvMap()
        {
            Map(m => m.UserName).Index(1);
            Map(m => m.Password).Index(2);
            Map(m => m.ReferenceId).Index(3);
            Map(m => m.IsActive).Index(4);
            Map(m => m.SourceId).Index(5);
        }
    }

}
