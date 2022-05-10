using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Models
{
    [Table("LogFieldSetup")]
    public partial class LogFieldSetup : BindableBase
    {
        public string StratumCode { get; set; }

        public string Field { get; set; }

        public int? FieldOrder { get; set; }

        public string Heading { get; set; }
    }
}
