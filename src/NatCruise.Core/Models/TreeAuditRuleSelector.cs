using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("TreeAuditRuleSelector")]
    public class TreeAuditRuleSelector : BindableBase
    {
        private string _speciesCode;
        private string _liveDead;
        private string _primaryProduct;

        public string SpeciesCode
        {
            get => _speciesCode;
            set => SetProperty(ref _speciesCode, value);
        }

        public string LiveDead
        {
            get => _liveDead;
            set => SetProperty(ref _liveDead, value);
        }

        public string PrimaryProduct
        {
            get => _primaryProduct;
            set => SetProperty(ref _primaryProduct, value);
        }

        public string TreeAuditRuleID { get; set; }
    }
}