using CsvHelper.Configuration;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Mapper.CSV
{
    public sealed class AuditTrailModelForCsvMap : ClassMap<AuditTrailModelForCsv>
    {
        public AuditTrailModelForCsvMap()
        {
            Map(m => m.Message).Index(1);
            Map(m => m.CreatedDate).Index(2);
            Map(m => m.IconPath).Index(3);
            Map(m => m.MemberId).Index(4);
            Map(m => m.ModuleId).Index(5);
            Map(m => m.EventId).Index(6);
            Map(m => m.AccessType).Index(7);
            Map(m => m.LocationIPAddress).Index(8);
            Map(m => m.SourceId).Index(9);
            Map(m => m.UserTypeId).Index(10);
        }
    }
}
