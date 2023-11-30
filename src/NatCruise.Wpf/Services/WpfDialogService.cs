using NatCruise.Navigation;
using NatCruise.Services;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using NatCruise.Wpf.Views;
using System.Windows.Navigation;
using NatCruise.Wpf.DialogViews;
using System.IO;

namespace NatCruise.Wpf.Services
{
    public class WpfDialogService : INatCruiseDialogService
    {
        public IAppService AppService { get; }
        public MainWindow MainWindow => (MainWindow)AppService.MainWindow;

        public WpfDialogService(IAppService appService)
        {
            AppService = appService;
        }

        public Task<string> AskCruiserAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<int?> AskKPIAsync(int max, int min = 1)
        {
            var window = MainWindow;
            var settings = window.MetroDialogOptions;
            var dialog = new KpiDialog(window, settings)
            {
                Title = "Enter KPI",
                MaxKPI = max,
                MinKPI = min,
            };

            await window.ShowMetroDialogAsync(dialog);

            var result = await dialog.WaitForResult();
            await window.HideMetroDialogAsync(dialog, settings);
            return result;
        }

        public async Task<AskTreeCountResult> AskTreeCount(int? defaultTreeCount)
        {
            var result = await MainWindow.ShowInputAsync("Enter Tree Count", "");

            if (int.TryParse(result, out var treeCount))
            {
                return new AskTreeCountResult() { TreeCount = treeCount };
            }
            else if (defaultTreeCount != null)
            {
                return new AskTreeCountResult { TreeCount = defaultTreeCount };
            }
            return null;
        }

        public async Task<string> AskValueAsync(string prompt, params string[] values)
        {


            var window = MainWindow;
            var settings = window.MetroDialogOptions;
            var dialog = new SelectValueDialog(window, settings)
            {
                Title = prompt,
                Values = values,
            };

            await window.ShowMetroDialogAsync(dialog, settings);

            var result = await dialog.WaitForResult();

            await window.HideMetroDialogAsync(dialog, settings);


            return result;
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            return Task.Run(() =>
                MessageBox.Show(message, caption,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    (defaultNo) ? MessageBoxResult.No : MessageBoxResult.Yes) == MessageBoxResult.Yes
                );
        }

        public Task ShowMessageAsync(string message, string caption = null)
        {
            return MainWindow.ShowMessageAsync(message, caption);
        }

        public void ShowNotification(string message, string title = null)
        {
            MessageBox.Show(message, title);
        }

        public Task ShowNotificationAsync(string message, string title = null)
        {
            return Task.Run(() => MessageBox.Show(message, title));
        }
    }
}