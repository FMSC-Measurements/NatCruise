using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("Species")]
    public class Species : BindableBase
    {
        private string _speciesCode;
        private string _contractSpecies;
        private string _fiaCode;

        public string SpeciesCode
        {
            get => _speciesCode;
            set => SetProperty(ref _speciesCode, value);
        }

        public string ContractSpecies
        {
            get => _contractSpecies;
            set => SetProperty(ref _contractSpecies, value);
        }

        public string FIACode
        {
            get => _fiaCode;
            set => SetProperty(ref _fiaCode, value);
        }
    }
}