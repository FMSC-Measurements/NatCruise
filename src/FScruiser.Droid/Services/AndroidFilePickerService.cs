using Android.Content;
using Com.Obsez.Android.Lib.Filechooser;
using FScruiser.XF.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FScruiser.Droid.Services
{
    public class AndroidFilePickerService : IFilePickerService
    {
        private TaskCompletionSource<string> _tcs;

        private Context Context { get; }

        public AndroidFilePickerService(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<string> PickCruiseFileAsync()
        {
            var newTcs = new TaskCompletionSource<string>();
            var existingTcs = Interlocked.Exchange(ref _tcs, newTcs);
            if (existingTcs != null)
            {
                existingTcs.TrySetResult(null);
            }

            var chooserDialog = new ChooserDialog(Context)
                .WithFilter(false, false, "cruise", "CRUISE", "crz3", "CRZ3")
                .WithStringResources("Choose a file",
                    "Choose", "Cancel")
                .WithOptionStringResources("New folder",
                    "Delete", "Cancel", "Ok");

            chooserDialog.WithChosenListener((path, file) =>
            {
                var tcs = Interlocked.Exchange(ref _tcs, null);

                tcs.SetResult(file.AbsolutePath);
            });

            chooserDialog.WithOnCancelListener(x =>
            {
                var tcs = Interlocked.Exchange(ref _tcs, null);

                tcs.SetResult(null);
            });

            //chooserDialog.WithOnBackPressedListener(d =>
            //{
            //    chooserDialog.GoBack();
            //});

            chooserDialog.Build().Show();

            return _tcs.Task;
        }
    }
}