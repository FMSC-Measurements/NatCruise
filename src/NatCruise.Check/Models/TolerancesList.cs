using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NatCruise.Check.Models
{
    [Table("Tolerances")]
    public class TolerancesList : ICloneable
    {
        public const string ADD_PARAM_NONE = "None";
        public const string ADD_PARAM_7PCT = "7% whichever >";
        //public const string ADD_PARAM_3PCT = "3% whichever >"; // not used 
        public const string ADD_PARAM_PLUS1PER25FTOVER100 = "+1 per 25 feet > 100 feet";

        public const string UNIT_INCHES = "inches";
        public const string UNIT_FEET = "feet";
        public const string UNIT_PERCENT = "percent";

        public static readonly IReadOnlyCollection<string> ADD_PARAM_VALUES = new string[]
        {
            ADD_PARAM_NONE,
            ADD_PARAM_7PCT,
            //ADD_PARAM_3PCT,
            ADD_PARAM_PLUS1PER25FTOVER100,
        };

        public string T_Element { get; set; }
        public string T_Tolerance { get; set; }
        public string T_Units { get; set; }
        public string T_AddParam { get; set; }
        public float T_Weight { get; set; }

        public int T_IncludeVolume { get; set; }
        public int T_BySpecies { get; set; }
        public int T_ByProduct { get; set; }
        public float T_ElementAccuracy { get; set; }
        public float T_OverallAccuracy { get; set; }
        public string T_DateStamp { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
