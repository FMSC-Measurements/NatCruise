using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class StratumListPageViewModel : ViewModelBase
    {
        private ICommand _addStratumCommand;
        private ICommand _removeStratumCommand;
        private ObservableCollection<Stratum> _strata;

        public StratumListPageViewModel(IDataserviceProvider dataserviceProvider)
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