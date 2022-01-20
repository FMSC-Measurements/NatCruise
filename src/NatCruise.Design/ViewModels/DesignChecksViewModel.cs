using NatCruise.Design.Data;
using NatCruise.Design.Models;
using System;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class DesignChecksViewModel : ViewModelBase
    {
        private DesignCheck[] _designChecks;

        public DesignChecksViewModel(IDesignCheckDataservice designCheckDataservice)
        {
            DesignCheckDataservice = designCheckDataservice ?? throw new ArgumentNullException(nameof(designCheckDataservice));
        }

        public IDesignCheckDataservice DesignCheckDataservice { get; }

        public DesignCheck[] DesignChecks
        {
            get => _designChecks;
            protected set => SetProperty(ref _designChecks, value);
        }

        public override void Load()
        {
            base.Load();

            DesignChecks = DesignCheckDataservice.GetDesignChecks().ToArray();
        }
    }
}