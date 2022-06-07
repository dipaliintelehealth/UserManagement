using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Models
{
   public static class DOBFormats
    {
        public static string[] Formats { get; set; } =
                      {         "dd/MM/yyyy","dd-MM-yyyy","dd.MM.yyyy",
                                "dd/MM/y","dd-MM-y","dd.MM.y",
                                "MM/dd/yyyy","MM-dd-yyyy","MM.dd.yyyy",
                                "MM/dd/y","MM-dd-yyyy","MM.dd.y",
                                "d/M/yyyy","d-M-yyyy","d.M.yyyy",
                                "d/M/y","d-M-y","d.M.y",
                                "M/dd/yyyy","M-dd-yyyy","M.dd.yyyy",
                                "M/dd/y","M-dd-y","M.dd.y",
                                "MM/d/yyyy","MM-d-yyyy","MM.d.yyyy",
                                "MM/d/y","MM-d-y","MM.d.y",
                                "dd/M/yyyy","dd-M-yyyy","dd.M.yyyy",
                                "dd/M/y","dd-M-y","dd.M.y",
                                "yyyy/dd/MM", "yyyy-dd-MM","yyyy.dd.MM",
                                "y/dd/MM", "y-dd-MM","y.dd.MM",
                                "yyyy/MM/dd","yyyy-MM-dd","yyyy.MM.dd",
                                "y/MM/dd","y-MM-dd","y.MM.dd",
                                "yyyy/d/MM", "yyyy-d-MM","yyyy.d.MM",
                                "y/d/MM", "y-d-MM","y.d.MM",
                                "yyyy/M/dd","yyyy-M-dd","yyyy.M.dd",
                                "y/M/dd","y-M-dd","y.M.dd",
                                "yyyy/d/M","yyyy-M-d","yyyy.M.d",
                                "y/d/M","y-M-d","y.M.d",
                                "dd/MMM/yyyy","dd-MMM-yyyy","dd.MMM.yyyy",
                                "dd/MMM/y","dd-MMM-y","dd.MMM.y",
                                "dd/MMMM/yyyy","dd-MMMM-yyyy","dd.MMMM.yyyy",
                                "dd/MMMM/y","dd-MMMM-y","dd.MMMM.y",
                                "MMM dd,yyyy","MMM dd.yyyy",
                                "MMM d,yyyy","MMM d.yyyy",
                                "MMM dd,y", "MMM dd.y",
                                "MMM d,y", "MMM d.y",
                                "MMMM dd,yyyy","MMMM dd.yyyy",
                                "MMMM d,yyyy","MMMM d.yyyy",
                                "MMMM dd,y", "MMMM dd.y",
                                "MMMM d,y", "MMMM d.y"
                        };
    }
}
