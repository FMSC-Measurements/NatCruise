using Android.Content;
using FScruiser.Maui.Services;
using NatCruise.Services;
using System;
using System.Threading.Tasks;

namespace FScruiser.Maui.Platforms.Android.Services;

public class AndroidFileDialogService : FileDialogServiceBase, IFileDialogService
{
    private TaskCompletionSource<string> _selectCruiseFileDestTcs;
    protected int CREATE_FILE_REQUESTCODE = 100;

    protected MainActivity Activity { get; }

    public AndroidFileDialogService(MainActivity activity)
    {
        Activity = activity ?? throw new ArgumentNullException(nameof(activity));
    }

    public override async Task<string> SelectCruiseFileDestinationAsync(string defaultDir = null, string defaultFileName = null, string defaultSaleFolder = null)
    {
        var action = Intent.ActionCreateDocument;
        var intent = new Intent(action);
        intent.SetType("application/x.crz3");

        if (string.IsNullOrWhiteSpace(defaultFileName) == false)
        {
            intent.PutExtra(Intent.ExtraTitle, defaultFileName);
        }

        var requestCode = CREATE_FILE_REQUESTCODE;
        intent.PutExtra("request_code", requestCode);

        var createIntent = Intent.CreateChooser(intent, "Select File Destination");

        try
        {
            string resultPath = null;
            void OnResult(Intent intent)
            {
                resultPath = intent.Data.ToString();
            }

            await Activity.StartAsync(createIntent, requestCode, onResult: OnResult);

            return resultPath;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    public override async Task<string> SelectBackupFileDestinationAsync(string defaultDir = null, string defaultFileName = null)
    {
        var action = Intent.ActionCreateDocument;
        var intent = new Intent(action);
        intent.SetType("application/x.crz3db");

        if (string.IsNullOrWhiteSpace(defaultFileName) == false)
        {
            intent.PutExtra(Intent.ExtraTitle, defaultFileName);
        }

        var requestCode = CREATE_FILE_REQUESTCODE;
        intent.PutExtra("request_code", requestCode);

        var createIntent = Intent.CreateChooser(intent, "Select Backup File Destination");

        try
        {
            string resultPath = null;
            void OnResult(Intent intent)
            {
                resultPath = intent.Data.ToString();
            }

            await Activity.StartAsync(createIntent, requestCode, onResult: OnResult);

            return resultPath;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
}