using CsvHelper.Configuration;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Mapper.CSV
{
    public sealed class MemberSlotForCsvMap : ClassMap<MemberSlotModelForCsv>
    {
        public MemberSlotForCsvMap()
        {
            Map(m => m.MemberId).Index(1);
            Map(m => m.Day).Index(2);
            Map(m => m.SlotTo).Index(3);
            Map(m => m.SlotFrom).Index(4);
            Map(m => m.CreatedDate).Index(5);
            Map(m => m.IsActive).Index(4);
            Map(m => m.SourceId).Index(5);
        }
    }

}
