﻿using NatCruise.Design.Models;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.ViewModels
{
    public class StratumFieldSetupViewModel : ViewModelBase
    {
        private Stratum _stratum;

        public StratumFieldSetupViewModel(StratumTreeFieldSetupViewModel treeFieldsViewModel, StratumLogFieldSetupViewModel logFieldsViewModel)
        {
            TreeFieldsViewModel = treeFieldsViewModel ?? throw new ArgumentNullException(nameof(treeFieldsViewModel));
            LogFieldsViewModel = logFieldsViewModel ?? throw new ArgumentNullException(nameof(logFieldsViewModel));
        }

        public StratumTreeFieldSetupViewModel TreeFieldsViewModel { get; }

        public StratumLogFieldSetupViewModel LogFieldsViewModel { get; }

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                _stratum = value;
                OnPropertyChanged(nameof(Stratum));
                LogFieldsViewModel.Stratum = value;
                TreeFieldsViewModel.Stratum = value;
            }
        }

        public override void Load()
        {
            base.Load();

            TreeFieldsViewModel.Load();
            LogFieldsViewModel.Load();
        }
    }
}
