using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using NatCruise.Navigation;
using Prism.Common;

namespace FScruiser.Maui.ViewModels;

public class StratumFieldSetupViewModel : ViewModelBase
{
    private Stratum? _stratum;

    public StratumFieldSetupViewModel(IStratumDataservice stratumDataservice, StratumTreeFieldSetupViewModel treeFieldSetupViewModel, StratumLogFieldSetupViewModel logFieldSetupViewModel)
    {
        StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));

        TreeFieldSetupViewModel = treeFieldSetupViewModel;
        LogFieldSetupViewModel = logFieldSetupViewModel;
    }

    public IStratumDataservice StratumDataservice { get; }
    public StratumTreeFieldSetupViewModel TreeFieldSetupViewModel { get; }
    public StratumLogFieldSetupViewModel LogFieldSetupViewModel { get; }

    public Stratum? Stratum
    {
        get => _stratum;
        set
        {
            _stratum = value;
            OnPropertyChanged(nameof(Stratum));
            TreeFieldSetupViewModel.Stratum = value;
            LogFieldSetupViewModel.Stratum = value;
        }
    }

    protected override void OnInitialize(IDictionary<string, object> parameters)
    {

        TreeFieldSetupViewModel.Initialize(parameters);
        LogFieldSetupViewModel.Initialize(parameters);

        if (parameters != null)
        {
            var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
            Stratum = StratumDataservice.GetStratum(stratumCode);
        }
    }

    public override void Load()
    {
        base.Load();

        TreeFieldSetupViewModel.Load();
        LogFieldSetupViewModel.Load();
    }
}