using FMSC.ORM.EntityModel.Attributes;
using System;

namespace NatCruise.Cruise.Models
{
    [Table("TreeFieldSetup_V3")]
    public class TreeFieldSetup
    {
        [Field("Field")]
        public string Field { get; set; }

        [Field("Heading")]
        public string Heading { get; set; }

        [Field("FieldOrder")]
        public int FieldOrder { get; set; }

        [Obsolete]
        [IgnoreField]
        public string ColumnType { get; set; }
    }
}