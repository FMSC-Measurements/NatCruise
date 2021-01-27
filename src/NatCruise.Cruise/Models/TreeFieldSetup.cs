using FMSC.ORM.EntityModel.Attributes;
using System;

namespace NatCruise.Cruise.Models
{
    [Table("TreeFieldSetup")]
    public class TreeFieldSetup
    {
        public string Field { get; set; }

        public string Heading { get; set; }

        public int FieldOrder { get; set; }

        // used to display default value to user
        public string DefaultValueAsString { get; set; } 

        public bool IsHidden { get; set; }

        public bool IsLocked { get; set; }
    }
}