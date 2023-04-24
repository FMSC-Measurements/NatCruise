using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("LogFieldSetup")]
    public partial class LogFieldSetup : BindableBase
    {
        public string StratumCode { get; set; }

        public string Field { get; set; }

        public int? FieldOrder { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public string Heading { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public string DbType { get; set; }
    }
}
