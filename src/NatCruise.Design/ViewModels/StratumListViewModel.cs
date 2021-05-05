using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class StratumListViewModel : ViewModelBase
    {
        private ICommand _addStratumCommand;
        private ICommand _removeStratumCommand;
        private ObservableCollection<Stratum> _strata;
        private Stratum _selectedStratum;

        public StratumListViewModel(IDataserviceProvider dataserviceProvider)
        {
            if (dataserviceProvider is null) { throw new System.ArgumentNullException(nameof(dataserviceProvider)); }

            StratumDataservice = dataserviceProvider.GetDataservice<IStratumDataservice>();
        }

        protected IStratumDataservice StratumDataservice { get; }

        public ObservableCollection<Stratum> Strata
        {
            get => _strata;
            protected set => SetProperty(ref _strata, value);
        }

        public Stratum SelectedStratum
        {
            get => _selectedStratum;
            set => SetProperty(ref _selectedStratum, value);
        }

        public override void Load()
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
            SelectedStratum = newStratum;
        }

        public void RemoveStratum(Stratum stratum)
        {
            if (stratum is null) { throw new System.ArgumentNullException(nameof(stratum)); }
            var strata = Strata;

            StratumDataservice.DeleteStratum(stratum);
            var index = strata.IndexOf(stratum);
            if (index < 0) { return; }
            strata.RemoveAt(index);

            if (index <= strata.Count - 1)
            {
                var newSelectedStratum = strata[index];
                SelectedStratum = newSelectedStratum;
            }
            else
            {
                SelectedStratum = strata.LastOrDefault();
            }
        }
    }
}