using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using FScruiser.XF;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;

namespace FScruiser.Droid
{
    [Activity(Label = "FScruiser", Icon = "@drawable/fscruiser_32dp", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    //[IntentFilter(new[] { Intent.ActionView },
    //    DataScheme = "file",
    //    DataMimeType = "*/*",
    //    // for the DataPathPatterns we need to specify many addition patters because of an unfortunate limitation in androids pattern matching. See https://stackoverflow.com/questions/3400072/pathpattern-to-match-file-extension-does-not-work-if-a-period-exists-elsewhere-i/8599921
    //    DataPathPatterns = new[] { ".*\\\\.cruise", ".*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\..*\\\\.cruise" },
    //    DataHost = "*",
    //    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //    Icon = "@drawable/fscruiser_32dp")]

    //[IntentFilter(new[] { Intent.ActionView },
    //    DataScheme = "file",
    //    // for the DataPathPatterns we need to specify many addition patters because of an unfortunate limitation in androids pattern matching. See https://stackoverflow.com/questions/3400072/pathpattern-to-match-file-extension-does-not-work-if-a-period-exists-elsewhere-i/8599921
    //    DataPathPatterns = new[] { ".*\\\\.cruise", ".*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\..*\\\\.cruise" },
    //    DataHost = "*",
    //    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //    Icon = "@drawable/fscruiser_32dp")]

    //[IntentFilter(new[] { Intent.ActionView },
    //    DataScheme = "file",
    //    DataMimeType = "application/vnd.ni.custom",
    //    // for the DataPathPatterns we need to specify many addition patters because of an unfortunate limitation in androids pattern matching. See https://stackoverflow.com/questions/3400072/pathpattern-to-match-file-extension-does-not-work-if-a-period-exists-elsewhere-i/8599921
    //    DataPathPatterns = new[] { ".*\\\\.cruise", ".*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\.cruise", ".*\\\\..*\\\\..*\\\\..*\\\\.cruise" },
    //    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //    Icon = "@drawable/fscruiser_32dp")]

    public class MainActivity : FormsAppCompatActivity
    {
        private const string TASKID_EXTRA = "taskid";
        private const string REQUESTCODE_EXTRA = "request_code";

        private static readonly ConcurrentDictionary<int, IntentTask> _tasks = new ConcurrentDictionary<int, IntentTask>();

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            //var device = ev.Device;

            //if (ev.Source.HasFlag(InputSourceType.Stylus))
            //{
            //    return base.DispatchTouchEvent(ev);
            //}
            //else
            //{
            //    return true;
            //}

            return base.DispatchTouchEvent(ev);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.MyTheme);//set theme to main theme, because it should be set at launch to the splash theme

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags(new[] { "CollectionView_Experimental", "Shell_Experimental" });
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            var app = new App(new AndroidPlatformInitializer(this));

            LoadApplication(app);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (GetTask(requestCode, true) is IntentTask task)
            {
                var tcs = task.TCS;

                if (resultCode == Result.Canceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    try
                    {
                        data ??= new Intent();
                        task.OnResult?.Invoke(data);
                        tcs.TrySetResult(data);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }
        }

        public Task<Intent> StartAsync(Intent intent, int requestCode, Action<Intent> onCreate = null, Action<Intent> onResult = null)
        {
            var data = new IntentTask(requestCode, onCreate, onResult);
            _tasks[data.RequestCode] = data;

            intent.PutExtra(REQUESTCODE_EXTRA, requestCode);

            StartActivityForResult(intent, requestCode);
            return data.TCS.Task;
        }

        private static IntentTask GetTask(int requestCode, bool remove = false)
        {
            if (remove)
            {
                _tasks.TryRemove(requestCode, out var task);
                return task;
            }
            else
            {
                _tasks.TryGetValue(requestCode, out var task);
                return task;
            }
        }
    }

    internal class IntentTask
    {
        public IntentTask(int requestCode, Action<Intent> onCreate, Action<Intent> onResult)
        {
            RequestCode = requestCode;
            OnResult = onResult;
            OnCreate = onCreate;
            TCS = new TaskCompletionSource<Intent>();
        }

        public int RequestCode { get; }

        public TaskCompletionSource<Intent> TCS { get; }

        public Action<Intent> OnResult { get; }

        public Action<Intent> OnCreate { get; }
    }
}