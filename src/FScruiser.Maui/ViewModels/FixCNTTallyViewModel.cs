using FScruiser.Maui.Services;
using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Common;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public class FixCNTTallyViewModel : ViewModelBase
{
    private Command<FixCNTTallyBucket>? _processTallyCommand;
    private bool _isUntallyEnabled;
    private string? _unitCode;
    private int _plotNumber;

    public string Title => $"FixCNT Tally Unit {Unit} Plot {PlotNumber}";

    public IEnumerable<FixCntTallyPopulation>? TallyPopulations { get; set; }

    public IFixCNTDataservice FixCNTDataservice { get; }
    public ISoundService SoundService { get; }

    public ICommand ProcessTallyCommand => _processTallyCommand ??= new Command<FixCNTTallyBucket>(ProcessTally);

    public bool OneTreePerTally { get; protected set; } = true;

    public bool IsUntallyEnabled
    {
        get { return _isUntallyEnabled; }
        set { SetProperty(ref _isUntallyEnabled, value); }
    }

    public string? Unit
    {
        get => _unitCode;
        private set => SetProperty(ref _unitCode, value);
    }

    public int PlotNumber
    {
        get => _plotNumber;
        private set => SetProperty(ref _plotNumber, value);
    }

    public FixCNTTallyViewModel(IFixCNTDataservice fixCNTDataservice, ISoundService soundService)
    {
        FixCNTDataservice = fixCNTDataservice ?? throw new ArgumentNullException(nameof(fixCNTDataservice));
        SoundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
        OneTreePerTally = fixCNTDataservice.GetOneTreePerTallyOption();
    }

    public void Tally(FixCNTTallyBucket tallyBucket)
    {
        var tallyPop = tallyBucket.TallyPopulation;

        if (OneTreePerTally)
        {
            FixCNTDataservice.AddFixCNTTree(
                Unit,
                PlotNumber,
                tallyPop.StratumCode,
                tallyPop.SampleGroupCode,
                tallyPop.SpeciesCode,
                tallyPop.LiveDead,
                tallyPop.FieldName,
                tallyBucket.Value);
        }
        else
        {
            FixCNTDataservice.IncrementFixCNTTreeCount(
                Unit,
                PlotNumber,
                tallyPop.StratumCode,
                tallyPop.SampleGroupCode,
                tallyPop.SpeciesCode,
                tallyPop.LiveDead,
                tallyPop.FieldName,
                tallyBucket.Value);
        }

        tallyBucket.TreeCount += 1;
        SoundService.SignalTallyAsync().FireAndForget();
    }

    public void UnTally(FixCNTTallyBucket tallyBucket)
    {
        var tallyPop = tallyBucket.TallyPopulation;

        if (OneTreePerTally)
        {
            FixCNTDataservice.RemoveFixCNTTree(
                Unit,
                PlotNumber,
                tallyPop.StratumCode,
                tallyPop.SampleGroupCode,
                tallyPop.SpeciesCode,
                tallyPop.LiveDead,
                tallyPop.FieldName,
                tallyBucket.Value);
        }
        else
        {
            FixCNTDataservice.DecrementFixCNTTreeCount(
                Unit,
                PlotNumber,
                tallyPop.StratumCode,
                tallyPop.SampleGroupCode,
                tallyPop.SpeciesCode,
                tallyPop.LiveDead,
                tallyPop.FieldName,
                tallyBucket.Value);
        }

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

    protected override void Load(IDictionary<string, object> parameters)
    {
        if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

        var unit = Unit = parameters.GetValue<string>(NavParams.UNIT);
        var plotNumber = PlotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);
        var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
        OnPropertyChanged(nameof(Title));

        //read fixcount tally populations
        var tallyPopulations = FixCNTDataservice.GetFixCNTTallyPopulations(stratumCode).ToArray();

        //foreach tally population calculate and iterate through interval values
        foreach (var tp in tallyPopulations)
        {
            var buckets = new List<FixCNTTallyBucket>();
            foreach (var interval in NatCruise.Data.FixCNTDataservice.GetIntervalValues(tp.Min, tp.Max, tp.IntervalSize))
            {
                var treeCount = FixCNTDataservice.GetTreeCount(unit, plotNumber, stratumCode, tp.SampleGroupCode, tp.SpeciesCode, tp.LiveDead, tp.FieldName, interval);

                buckets.Add(new FixCNTTallyBucket(tp, interval, treeCount));
            }

            tp.Buckets = buckets;
        }

        TallyPopulations = tallyPopulations;
        OnPropertyChanged(nameof(TallyPopulations));
    }


}