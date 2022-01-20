using MahApps.Metro.Controls;
using NatCruise.Design.Data;
using NatCruise.Services;
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

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            try
            {
                var designChecksds = Container.Resolve<IDesignCheckDataservice>();

                var designChecks = designChecksds.GetDesignChecks();
                if (designChecks.Any(x => x.Level == "Error"))
                {
                    var dialogService = Container.Resolve<IDialogService>();
                    if (!dialogService.AskYesNoAsync(
@"Cruise Has Design Errors.
See Design Checks Page For Details.", "Do You Want To Exit?", defaultNo: true).Result)
                    {
                        e.Cancel = true;
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
    }

    
}
