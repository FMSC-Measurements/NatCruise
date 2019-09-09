using NatCruise.Wpf.Data;
using NatCruise.Wpf.Models;
using NatCruise.Wpf.Navigation;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NatCruise.Wpf.ViewModels
{
    public class SampleGroupListPageViewModel : ViewModelBase
    {
        private DelegateCommand<string> _addSampleGroupCommand;
        private DelegateCommand<SampleGroup> _removeSampleGroupCommand;
        private ObservableCollection<SampleGroup> _sampleGroups;
        private Stratum _stratum;

        public SampleGroupListPageViewModel(IDataserviceProvider datastoreProvider)
        {
            var sampleGroupDataservice = datastoreProvider.GetDataservice<ISampleGroupDataservice>();

            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
        }

        public ICommand AddSampleGroupCommand => _addSampleGroupCommand ?? (_addSampleGroupCommand = new DelegateCommand<string>(AddSampleGroup));
        public ICommand RemoveSampleGroupCommand => _removeSampleGroupCommand ?? (_removeSampleGroupCommand = new DelegateCommand<SampleGroup>(RemoveSampleGroup));

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                SetProperty(ref _stratum, value);
                OnStratumChanged(value);
            }
        }

        

        public ObservableCollection<SampleGroup> SampleGroups
        {
            get => _sampleGroups;
            protected set => SetProperty(ref _sampleGroups, value);
        }

        protected ISampleGroupDataservice SampleGroupDataservice { get; }

        #region overrides

        //protected override void Refresh(CruiseManagerNavigationParamiters navParams)
        //{
        //    var stratum = StratumCode = navParams.StratumCode;

        //    SampleGroups = new ObservableCollection<SampleGroup>(SampleGroupDataservice.GetSampleGroups(stratum));
        //}

        protected override void Load()
        {
            
        }

        private void OnStratumChanged(Stratum stratum)
        {
            if(stratum != null)
            {
                SampleGroups = new ObservableCollection<SampleGroup>( SampleGroupDataservice.GetSampleGroups(stratum.StratumCode));
            }
        }

        #endregion overrides

        #region public

        public void AddSampleGroup(string code)
        {
            var newSampleGroup = new SampleGroup()
            {
                SampleGroupCode = code,
                StratumCode = Stratum.StratumCode,
                DefaultLiveDead = "L",
            };

            SampleGroupDataservice.AddSampleGroup(newSampleGroup);
            SampleGroups.Add(newSampleGroup);
        }

        public void RemoveSampleGroup(SampleGroup sampleGroup)
        {
            SampleGroups.Remove(sampleGroup);
            SampleGroupDataservice.DeleteSampleGroup(sampleGroup);
        }

        #endregion public
    }
}