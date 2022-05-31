using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Models
{
   public static class DOBFormats
    {
        public static string[] Formats { get; set; } =
                      {         "dd/MM/yyyy","dd-MM-yyyy","dd.MM.yyyy",
                                "MM/dd/yyyy","MM-dd-yyyy","MM.dd.yyyy",
                                "d/M/yyyy","d-M-yyyy","d.M.yyyy",
                                "M/dd/yyyy","M-dd-yyyy","M.dd.yyyy",
                                "MM/d/yyyy","MM-d-yyyy","MM.d.yyyy",
                                "dd/M/yyyy","dd-M-yyyy","dd.M.yyyy",
                                "yyyy/dd/MM", "yyyy-dd-MM","yyyy.dd.MM",
                                "yyyy/MM/dd","yyyy-MM-dd","yyyy.MM.dd",
                                "yyyy/d/MM", "yyyy-d-MM","yyyy.d.MM",
                                "yyyy/M/dd","yyyy-M-dd","yyyy.M.dd",
                                "yyyy/d/M","yyyy-M-d","yyyy.M.d"
                        };
    }
}
