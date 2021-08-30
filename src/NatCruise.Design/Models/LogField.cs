using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Design.Models
{
    [Table("LogField")]
    public class LogField : BindableBase
    {
        private string _heading;

        public string Field { get; set; }

        public string Heading
        {
            get => _heading;
            set => SetProperty(ref _heading, value);
        }

        public string DefaultHeading { get; set; }

        public string DbType { get; set; }

        public override string ToString()
        {
            return Heading ?? DefaultHeading;

        }
    }
}