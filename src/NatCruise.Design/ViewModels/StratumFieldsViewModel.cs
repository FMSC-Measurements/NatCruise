﻿using NatCruise.Design.Models;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.ViewModels
{
    public class StratumFieldsViewModel : ViewModelBase
    {
        private Stratum _stratum;

        public StratumFieldsViewModel(StratumTreeFieldSetupViewModel treeFieldsViewModel, StratumLogFieldSetupViewModel logFieldsViewModel)
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