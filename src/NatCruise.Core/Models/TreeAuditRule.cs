using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("TreeAuditRule")]
    public class TreeAuditRule : BindableBase
    {
        private string _field;
        private double? _min;
        private double? _max;
        private string _description;

        public string TreeAuditRuleID { get; set; }

        public string Field
        {
            get => _field;
            set => SetProperty(ref _field, value);
        }

        public double? Min
        {
            get => _min;
            set => SetProperty(ref _min, value);
        }

        public double? Max
        {
            get => _max;
            set => SetProperty(ref _max, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}