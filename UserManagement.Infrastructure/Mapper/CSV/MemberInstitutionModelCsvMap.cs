using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Mapper.CSV
{
    public sealed class MemberInstitutionModelCsvMap : ClassMap<MemberInstitutionModel>
    {
        public MemberInstitutionModelCsvMap()
        {
            Map(m => m.MemberId).Index(1);
            Map(m => m.InstitutionId).Index(2);
            Map(m => m.IsActive).Index(3);
            Map(m => m.SourceId).Index(4);

        }
    }
}
