using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("StratumTemplateLogFieldSetup")]
    public class StratumTemplateLogFieldSetup : BindableBase
    {
        private string _field;

        public string StratumTemplateName { get; set; }

        public string Field
        {
            get => _field;
            set => SetProperty(ref _field, value);
        }

        public int? FieldOrder { get; set; }
    }
}