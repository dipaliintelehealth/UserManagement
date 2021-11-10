using CsvHelper.Configuration;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Mapper.CSV
{
    public sealed class MemberMenuModelForCsvMap : ClassMap<MemberMenuModelForCsv>
    {
        public MemberMenuModelForCsvMap()
        {
            Map(m => m.RoleId).Index(1);
            Map(m => m.MemberId).Index(2);
            Map(m => m.MenuMappingId).Index(3);
            Map(m => m.IsActive).Index(4);
            Map(m => m.InstitutionId).Index(5);
            Map(m => m.SourceId).Index(6);
        }
    }
}
