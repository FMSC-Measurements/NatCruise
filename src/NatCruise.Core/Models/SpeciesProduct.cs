using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Models
{
    [Table("Species_Product")]
    public class SpeciesProduct : BindableBase
    {
        private string _contractSpecies;

        public string SpeciesCode { get; set; }
        public string PrimaryProduct { get; set; }

        public string ContractSpecies
        {
            get => _contractSpecies;
            set => SetProperty(ref _contractSpecies, value);
        }
    }
}
