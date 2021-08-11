using CsvHelper.Configuration;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Mapper.CSV
{
    public sealed class InstitutionModelForCsvMap : ClassMap<InstitutionModelForCsv>
    {
        public InstitutionModelForCsvMap()
        {
            Map(m => m.Name).Index(1);
            Map(m => m.AddressLine1).Index(2);
            Map(m => m.AddressLine2).Index(3);
            Map(m => m.ReferenceNumber).Index(4);
            Map(m => m.CountryId).Index(5);
            Map(m => m.StateId).Index(6);
            Map(m => m.DistrictId).Index(7);
            Map(m => m.CityId).Index(8);
            Map(m => m.PinCode).Index(9);
            Map(m => m.Mobile).Index(10);
            Map(m => m.Email).Index(11);
            Map(m => m.ImagePath).Index(12);
            Map(m => m.InstitutionTypeId).Index(13);
            Map(m => m.IsActive).Index(14);
            Map(m => m.Fax).Index(15);
            Map(m => m.CreatedDate).Index(16);
            Map(m => m.SourceId).Index(17);
            Map(m => m.StatusId).Index(18);

        }
    }

}
