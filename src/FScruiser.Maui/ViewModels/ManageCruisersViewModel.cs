using CommunityToolkit.Mvvm.Input;
using NatCruise.Data;
using NatCruise.MVVM;
using System.Windows.Input;

namespace FScruiser.Maui.ViewModels;

public class ManageCruisersViewModel : ViewModelBase
{
    private ICommand? _addCruiserCommand;
    private ICommand? _removeCruiserCommand;

    public ICruisersDataservice CruisersDataservice { get; }

    public ICommand AddCruiserCommand => _addCruiserCommand ??= new RelayCommand<string>(AddCruiser);

    public ICommand RemoveCruiserCommand => _removeCruiserCommand ??= new RelayCommand<string>(RemoveCruiser);

    public IEnumerable<string> Cruisers => CruisersDataservice.GetCruisers();

    public bool PromptCruiserOnSample
    {
        get { return CruisersDataservice.PromptCruiserOnSample; }
        set { CruisersDataservice.PromptCruiserOnSample = value; }
    }

    public ManageCruisersViewModel(ICruisersDataservice cruisersDataservice)
    {
        CruisersDataservice = cruisersDataservice ?? throw new ArgumentNullException(nameof(cruisersDataservice));
    }

    public void AddCruiser(string? cruiser)
    {
        CruisersDataservice.AddCruiser(cruiser);
        OnPropertyChanged(nameof(Cruisers));
    }

    public void RemoveCruiser(string? cruiser)
    {
        CruisersDataservice.RemoveCruiser(cruiser);
        OnPropertyChanged(nameof(Cruisers));
    }
}