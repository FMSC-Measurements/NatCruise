﻿using MahApps.Metro.Controls;
using NatCruise.Data;
using NatCruise.Navigation;
using NatCruise.Wpf.Data;
using Prism.Ioc;
using System;
using System.ComponentModel;
using System.Linq;

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        protected MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IContainerProvider container) : this()
        {
            Container = container;
        }

        public IContainerProvider Container { get; }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            try
            {
                var dataContext = Container.Resolve<IDataContextService>();
                if (dataContext.Database == null) return;

                var designChecksds = Container.Resolve<IDesignCheckDataservice>();

                var designChecks = designChecksds.GetDesignChecks();
                if (designChecks.Any(x => x.Level == "Error"))
                {
                    var dialogService = Container.Resolve<INatCruiseDialogService>();
                    if (!dialogService.AskYesNoAsync(
@"Cruise Has Design Errors.
See Design Checks Page For Details.", "Do You Want To Exit?", defaultNo: true).Result)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception)
            {
                //do nothing
            }
        }
    }
}