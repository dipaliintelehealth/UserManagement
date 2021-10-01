using System.Collections.Generic;

namespace UserManagement.Domain
{
    public class ExcelConfiguration
    {
        public Dictionary<string, string> ColumnPropertyMapping { get; set; } = new Dictionary<string, string>();
        public string DateTimeFormat { get; set; } = "dd-MM-yyyy";
    }
}
