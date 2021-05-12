using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class SampleGroupListViewModel : ViewModelBase
    {
        private DelegateCommand<string> _addSampleGroupCommand;
        private DelegateCommand<SampleGroup> _removeSampleGroupCommand;
        private ObservableCollection<SampleGroup> _sampleGroups;
        private Stratum _stratum;
        private SampleGroup _selectedSampleGroup;

        public SampleGroupListViewModel(IDataserviceProvider datastoreProvider)
        {
            var sampleGroupDataservice = datastoreProvider.GetDataservice<ISampleGroupDataservice>();

            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
        }

        protected ISampleGroupDataservice SampleGroupDataservice { get; }

        public ICommand AddSampleGroupCommand => _addSampleGroupCommand ??= new DelegateCommand<string>(AddSampleGroup);
        public ICommand RemoveSampleGroupCommand => _removeSampleGroupCommand ??= new DelegateCommand<SampleGroup>(RemoveSampleGroup);

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                SetProperty(ref _stratum, value);
                LoadSampleGroups(value);
            }
        }

        public ObservableCollection<SampleGroup> SampleGroups
        {
            get => _sampleGroups;
            protected set => SetProperty(ref _sampleGroups, value);
        }

        public SampleGroup SelectedSampleGroup
        {
            get => _selectedSampleGroup;
            set => SetProperty(ref _selectedSampleGroup, value);
        }

        public void AddSampleGroup(string code)
        {
            var newSampleGroup = new SampleGroup()
            {
                SampleGroupCode = code,
                StratumCode = Stratum.StratumCode,
                CruiseMethod = Stratum.Method,
                DefaultLiveDead = "L",
            };

            SampleGroupDataservice.AddSampleGroup(newSampleGroup);
            SampleGroups.Add(newSampleGroup);
            SelectedSampleGroup = newSampleGroup;
        }

        public void RemoveSampleGroup(SampleGroup sampleGroup)
        {
            if (sampleGroup is null) { throw new ArgumentNullException(nameof(sampleGroup)); }
            var sampleGroups = SampleGroups;

            SampleGroupDataservice.DeleteSampleGroup(sampleGroup);
            var index = sampleGroups.IndexOf(sampleGroup);
            sampleGroups.Remove(sampleGroup);

            if (index < 0) { return; }
            if (index <= sampleGroups.Count - 1)
            {
                var newSelectedSg = sampleGroups[index];
                SelectedSampleGroup = newSelectedSg;
            }
            else
            {
                SelectedSampleGroup = sampleGroups.LastOrDefault();
            }
        }

        protected void LoadSampleGroups(Stratum stratum)
        {
            if (stratum != null)
            {
                SampleGroups = new ObservableCollection<SampleGroup>(SampleGroupDataservice.GetSampleGroups(stratum.StratumCode));
            }
        }

        public override void Load()
        {
            base.Load();

            LoadSampleGroups(Stratum);
        }
    }
}