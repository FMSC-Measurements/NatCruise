using NatCruise.Async;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Commands;
using System.Windows.Input;
using NatCruise.Util;

namespace FScruiser.Maui.ViewModels;

public class SampleGroupListViewModel : ViewModelBase
{
    private IEnumerable<SampleGroup>? _sampleGroups;
    private Stratum? _stratum;

    public SampleGroupListViewModel(ISampleGroupDataservice sampleGroupDataservice, IStratumDataservice stratumDataservice, INatCruiseNavigationService navigationService)
    {
        SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
        StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    public ISampleGroupDataservice SampleGroupDataservice { get; }
    public IStratumDataservice StratumDataservice { get; }
    public INatCruiseNavigationService NavigationService { get; }

    public Stratum? Stratum
    {
        get => _stratum;
        set
        {
            SetProperty(ref _stratum, value);
            if (value != null)
            {
                SampleGroups = SampleGroupDataservice.GetSampleGroups(value.StratumCode).ToArray();
            }
            else
            {
                SampleGroups = Enumerable.Empty<SampleGroup>();
            }
        }
    }

    public IEnumerable<SampleGroup>? SampleGroups
    {
        get => _sampleGroups;
        private set => SetProperty(ref _sampleGroups, value);
    }

    public ICommand ShowSubpopulationsCommand => new DelegateCommand<SampleGroup>(sg => NavigationService.ShowSubpopulations(sg.StratumCode, sg.SampleGroupCode).FireAndForget());

    public override void Load()
    {
        base.Load();

        var stratumCode = Parameters.GetValue<string>(NavParams.STRATUM);
        Stratum = StratumDataservice.GetStratum(stratumCode);
    }
}