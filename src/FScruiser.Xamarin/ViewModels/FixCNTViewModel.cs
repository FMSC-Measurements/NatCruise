using FScruiser.Data;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class FixCNTViewModel : ViewModelBase
    {
        private Command<FixCNTTallyBucket> _processTallyCommand;
        private bool _isUntallyEnabled;

        public IEnumerable<FixCntTallyPopulation> TallyPopulations { get; set; }

        public IFixCNTDataservice FixCNTDataservice { get; }

        public ICommand ProcessTallyCommand => _processTallyCommand ?? (_processTallyCommand = new Command<FixCNTTallyBucket>(ProcessTally));

        public bool IsUntallyEnabled
        {
            get { return _isUntallyEnabled; }
            set { SetValue(ref _isUntallyEnabled, value); }
        }

        public string Unit { get; private set; }
        public int PlotNumber { get; private set; }

        protected FixCNTViewModel() { }

        public FixCNTViewModel(INavigationService navigationService, IDataserviceProvider datastoreProvider) : base(navigationService)
        {
            FixCNTDataservice = datastoreProvider.Get<IFixCNTDataservice>();
        }

        public void Tally(FixCNTTallyBucket tallyBucket)
        {

            var tallyPop = tallyBucket.TallyPopulation;

            FixCNTDataservice.IncrementFixCNTTreeCount(
                Unit,
                PlotNumber,
                tallyPop.StratumCode,
                tallyPop.SampleGroupCode,
                tallyPop.Species,
                tallyPop.LiveDead,
                tallyPop.FieldName,
                tallyBucket.Value);

            tallyBucket.TreeCount += 1;
        }

        //public void Tally(string species, Double midValue)
        //{
        //    var tallyPopulation = TallyPopulations.Where(x => x.Species == species).First();

        //    var bucket = tallyPopulation.Buckets.Where(x => x.Value == midValue).Single();

        //    Tally(bucket);
        //}

        public void UnTally(FixCNTTallyBucket tallyBucket)
        {
            var tallyPop = tallyBucket.TallyPopulation;

            FixCNTDataservice.DecrementFixCNTTreeCount(
                Unit,
                PlotNumber,
                tallyPop.StratumCode,
                tallyPop.SampleGroupCode,
                tallyPop.Species,
                tallyPop.LiveDead,
                tallyPop.FieldName,
                tallyBucket.Value);

            tallyBucket.TreeCount = Math.Max(0, tallyBucket.TreeCount - 1);
        }

        public void ProcessTally(FixCNTTallyBucket tallyBucket)
        {
            if (IsUntallyEnabled)
            {
                UnTally(tallyBucket);
            }
            else
            {
                Tally(tallyBucket);
            }
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var unit = Unit = parameters.GetValue<string>(NavParams.UNIT);
            var plotNumber = PlotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);
            var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);


            //read fixcount tally populations
            var tallyPopulations = FixCNTDataservice.GetFixCNTTallyPopulations(stratumCode).ToArray();

            //foreach tally population calculate and itterate through interval values
            foreach (var tp in tallyPopulations)
            {
                var buckets = new List<FixCNTTallyBucket>();
                var interval = tp.Min + tp.IntervalSize / 2;

                //foreach interval value try to read a tree
                do
                {
                    var treeCount = FixCNTDataservice.GetTreeCount(unit, plotNumber, stratumCode, tp.SampleGroupCode, tp.Species, tp.LiveDead, tp.FieldName, interval);

                    buckets.Add(new FixCNTTallyBucket(tp, interval, treeCount));

                    interval += tp.IntervalSize;
                } while (interval <= tp.Max);

                tp.Buckets = buckets;
            }

            TallyPopulations = tallyPopulations;
            RaisePropertyChanged(nameof(TallyPopulations));
        }
    }
}