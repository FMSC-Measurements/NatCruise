using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
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
