using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using NatCruise.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class TallySettingsViewModel : ViewModelBase
    {
        private readonly string[] HOTKEY_OPTIONS = new[] {  "A","B","C","D","E","F",
                                                    "G","H","I","J","K","L",
                                                    "M","N","O","P","Q","R",
                                                    "S","T","U","V","W","X","Y","Z",
                                                    "1","2","3","4","5","6","7","8","9","0"};

        private SampleGroup _sampleGroup;
        private IEnumerable<TallyPopulation> _tallyPopulations;

        protected ISampleGroupDataservice SampleGroupDataservice { get; }

        protected ISubpopulationDataservice SubpopulationDataservice { get; set; }

        protected ITallyPopulationDataservice TallyPopulationDataservice { get; set; }

        protected ITallySettingsDataservice TallySettingsDataservice { get; }

        public IEnumerable<string> HotKeyOptions { get; protected set; }

        public IEnumerable<TallyPopulation> TallyPopulations
        {
            get => _tallyPopulations;
            set
            {
                OnTallyPopulationsChanging(_tallyPopulations);
                SetProperty(ref _tallyPopulations, value);
                OnTallyPopulationsChanged(value);
            }
        }

        private void OnTallyPopulationsChanged(IEnumerable<TallyPopulation> newTallyPopulations)
        {
            if (newTallyPopulations == null) { return; }
            foreach (var tp in newTallyPopulations)
            {
                tp.PropertyChanged += TallyPopulation_propertyChanged;
            }
        }

        private void OnTallyPopulationsChanging(IEnumerable<TallyPopulation> oldTallyPopulations)
        {
            if (oldTallyPopulations == null) { return; }
            foreach (var tp in oldTallyPopulations)
            {
                tp.PropertyChanged -= TallyPopulation_propertyChanged;
            }
        }

        private void TallyPopulation_propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tallyPopulation = (TallyPopulation)sender;
            var propertyName = e.PropertyName;
            if (propertyName == nameof(TallyPopulation.HotKey))
            {
                TallySettingsDataservice.SetHotKey(tallyPopulation.HotKey,
                    tallyPopulation.StratumCode,
                    tallyPopulation.SampleGroupCode,
                    tallyPopulation.Species,
                    tallyPopulation.LiveDead);
            }
            else if (propertyName == nameof(TallyPopulation.Description))
            {
                TallySettingsDataservice.SetDescription(tallyPopulation.HotKey,
                    tallyPopulation.StratumCode,
                    tallyPopulation.SampleGroupCode,
                    tallyPopulation.Species,
                    tallyPopulation.LiveDead);
            }
        }

        public SampleGroup SampleGroup
        {
            get => _sampleGroup;
            set
            {
                OnSampleGroupChanging(_sampleGroup);
                SetProperty(ref _sampleGroup, value);
                OnSampleGroupChanged(value);
            }
        }

        private void OnSampleGroupChanging(SampleGroup oldSampleGroup)
        {
            if (oldSampleGroup == null) { return; }
            oldSampleGroup.PropertyChanged -= SampleGroup_PropertyChanged;
        }

        private void OnSampleGroupChanged(SampleGroup newSampleGroup)
        {
            if (newSampleGroup == null) { return; }

            RefreshTallyPopulations(newSampleGroup);

            var usedHotKeys = TallySettingsDataservice.GetUsedHotKeys(newSampleGroup.StratumCode);
            HotKeyOptions = HOTKEY_OPTIONS.Except(usedHotKeys)
                .ToArray();
        }

        private void SampleGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propName = e.PropertyName;
            var sampleGroup = (SampleGroup)sender;
            if (propName == nameof(SampleGroup.TallyBySubPop))
            {
                SampleGroupDataservice.SetTallyBySubPop(sampleGroup.TallyBySubPop,
                    sampleGroup.StratumCode,
                    sampleGroup.SampleGroupCode);
            }
        }

        private void RefreshTallyPopulations(SampleGroup sampleGroup)
        {
            TallyPopulations = TallyPopulationDataservice.GetTallyPopulations(sampleGroup.StratumCode, sampleGroup.SampleGroupCode);
        }

        //protected override void Load()
        //{
        //    //var stratumCode = navParams.StratumCode;
        //    //var sampleGroupCode = navParams.SampleGroupCode;

        //    //var sampleGroup = SampleGroup = SampleGroupDataservice.GetSampleGroup(stratumCode, sampleGroupCode);
        //}
    }
}