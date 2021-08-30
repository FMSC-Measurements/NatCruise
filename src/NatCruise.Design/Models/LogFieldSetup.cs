using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;

namespace NatCruise.Design.Models
{
    [Table("LogFieldSetup")]
    public partial class LogFieldSetup : BindableBase
    {
        private string _heading;

        public String StratumCode { get; set; }

        public String Field { get; set; }

        public Int32? FieldOrder { get; set; }

        //public String Heading
        //{
        //    get => _heading;
        //    set => SetProperty(ref _heading, value);
        //}

        //public Double? Width { get; set; }
    }
}