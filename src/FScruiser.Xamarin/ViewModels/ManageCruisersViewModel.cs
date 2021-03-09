using NatCruise.Cruise.Services;
using FScruiser.XF.Services;
using Prism.Navigation;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using NatCruise.Data;
using System;

namespace FScruiser.XF.ViewModels
{
    public class ManageCruisersViewModel : XamarinViewModelBase
    {
        private Command<string> _addCruiserCommand;
        private Command<string> _removeCruiserCommand;

        public ICruisersDataservice CruisersDataservice { get; }

        public ICommand AddCruiserCommand => _addCruiserCommand ?? (_addCruiserCommand = new Command<string>(AddCruiser));

        public ICommand RemoveCruiserCommand => _removeCruiserCommand ?? (_removeCruiserCommand = new Command<string>(RemoveCruiser));

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

        public void AddCruiser(string cruiser)
        {
            CruisersDataservice.AddCruiser(cruiser);
            RaisePropertyChanged(nameof(Cruisers));
        }

        public void RemoveCruiser(string cruiser)
        {
            CruisersDataservice.RemoveCruiser(cruiser);
            RaisePropertyChanged(nameof(Cruisers));
        }
    }
}