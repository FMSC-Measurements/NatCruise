using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace FScruiser.XF
{
    public class TestDialogService : INatCruiseDialogService
    {
        public TestDialogService(ITestOutputHelper output)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
        }

        ITestOutputHelper Output { get; set; }

        bool AskCancelResult { get; set; }
        string AskCruiserResult { get; set; }
        public int? AskKPIResult { get; set; }
        public object AskValueResult { get; set; }
        public bool AskYesNoResult { get; set; }

        public Task<bool> AskCancelAsync(string message, string caption, bool defaultCancel)
        {
            Output.WriteLine($"AskCancelAsync::msg={message}::caption={caption}::defaultCancel={defaultCancel}::result={AskCancelResult}");
            return Task.FromResult(AskCancelResult);
        }

        public Task<string> AskCruiserAsync()
        {
            Output.WriteLine($"AskCruiserAsync::result={AskCruiserResult}");
            return Task.FromResult(AskCruiserResult);
        }

        public Task<int?> AskKPIAsync(int max, int min = 1)
        {
            Output.WriteLine($"AskKPIAsync::max={max}::min={min}");
            return Task.FromResult(AskKPIResult);
        }

        public Task<string> AskValueAsync(string prompt, params string[] values)
        {
            Output.WriteLine($"AskValueAsync::prompt={prompt}::values={string.Join(',', values)}");
            return Task.FromResult(AskValueResult as string);
        }

        public Task<TValue> AskValueAsync<TValue>(string prompt, params object[] values) where TValue : class
        {
            Output.WriteLine($"AskValueAsync::prompt={prompt}::values={string.Join(',', values)}");
            return Task.FromResult(AskValueResult as TValue);
        }

        public Task<bool> AskYesNoAsync(string message, string caption, bool defaultNo = false)
        {
            Output.WriteLine($"AskYesNoAsync::msg={message}::caption={caption}::defaultNo={defaultNo}");
            return Task.FromResult(AskYesNoResult);
        }

        public Task ShowMessageAsync(string message, string caption = null)
        {
            Output.WriteLine($"ShowMessageAsync::msg={message}::caption={caption}");
            return Task.CompletedTask;
        }

        public Task<AskTreeCountResult> AskTreeCount(int? defaultTreeCount)
        {
            Output.WriteLine($"AskTreeCount::defaultTreeCount={defaultTreeCount}");
            return Task.FromResult(new AskTreeCountResult { TreeCount = defaultTreeCount });
        }

        public void ShowNotification(string message, string title = null)
        {
            Output.WriteLine($"ShowNotification::msg={message}::title={title}");
        }

        public Task ShowNotificationAsync(string message, string title = null)
        {
            Output.WriteLine($"ShowNotification::msg={message}::title={title}");
            return Task.CompletedTask;
        }
    }
}
