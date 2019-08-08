using NatCruise.Wpf.Data;
using NatCruise.Wpf.Models;
using NatCruise.Wpf.Navigation;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class StratumListViewModel : ViewModelBase
    {
        private ICommand _addStratumCommand;
        private ICommand _removeStratumCommand;
        private ObservableCollection<Stratum> _strata;

        public StratumListViewModel(IDataserviceProvider dataserviceProvider)
        {
            StratumDataservice = dataserviceProvider.GetDataservice<IStratumDataservice>();
        }

        protected IStratumDataservice StratumDataservice { get; }

        public ObservableCollection<Stratum> Strata
        {
            get => _strata;
            protected set => SetProperty(ref _strata, value);
        }

        protected override void Load()
        {
            var strata = StratumDataservice.GetStrata();
            Strata = new ObservableCollection<Stratum>(strata);
        }

        public ICommand AddStratumCommand => _addStratumCommand ?? (_addStratumCommand = new DelegateCommand<string>(AddStratum));

        public ICommand RemoveStratumCommand => _removeStratumCommand ?? (_removeStratumCommand = new DelegateCommand<Stratum>(RemoveStratum));


        public void AddStratum(string code)
        {
            var newStratum = new Stratum
            {
                StratumCode = code,
            };

            StratumDataservice.AddStratum(newStratum);
            Strata.Add(newStratum);
        }

        public void RemoveStratum(Stratum stratum)
        {
            StratumDataservice.DeleteStratum(stratum);

            Strata.Remove(stratum);
        }
    }
}
