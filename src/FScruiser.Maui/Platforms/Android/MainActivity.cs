using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using System.Collections.Concurrent;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace FScruiser.Maui;

[Activity(Theme = "@style/FScruiser.Base", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    private const string TASKID_EXTRA = "taskid";
    private const string REQUESTCODE_EXTRA = "request_code";

    private static readonly ConcurrentDictionary<int, IntentTask> _tasks = new ConcurrentDictionary<int, IntentTask>();

#if false
// WIP this is part of a proof of concept where we ignore all touch events that
 // aren't sent from the stylus

    public override bool DispatchTouchEvent(MotionEvent ev)
    {
        
       

        var device = ev.Device;

        if (ev.Source.HasFlag(InputSourceType.Stylus))
        {
            return base.DispatchTouchEvent(ev);
        }
        else
        {
            return true;
        }

        var x = (int)ev.GetX();
        var y = (int)ev.GetY();

        var rootview = FindViewById(Android.Resource.Id.Content) as ViewGroup;
        var touchedView = GetViewAtLocation(rootview, x, y);
        if (touchedView != null)
        {

        }


        return base.DispatchTouchEvent(ev);
    }

    Android.Views.View GetViewAtLocation(ViewGroup parent, int x, int y)
    {
        var childCount = parent.ChildCount;
        if (childCount == 0)
        { return parent; }

        foreach (var i in Enumerable.Range(0, childCount))
        {
            var v = parent.GetChildAt(i);
            var loc = new int[2];
            v.GetLocationOnScreen(loc);

            var rect = new System.Drawing.Rectangle(loc[0], loc[1], v.Width, v.Height);
            if (rect.Contains(x, y))
            {
                if (v is ViewGroup vg)
                {
                    var foundView = GetViewAtLocation(vg, x, y);
                    if (foundView != null && foundView.IsShown)
                    {
                        return foundView;
                    }
                }
                else
                {
                    return v;
                }
            }
        }
        return null;
    }

#endif

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        //var attr = global::Android.Resource.Attribute.ColorActivatedHighlight;
        //using var value = new TypedValue();
        //var theme = Theme;
        //if(theme.ResolveAttribute(attr, value, true))
        //{
        //    Log.WriteLine(LogPriority.Debug, "MainActivity", $"ColorActivatedHighlight: {value.Data}");
        //}
        //else
        //{
        //    Log.WriteLine(LogPriority.Debug, "MainActivity", $"ColorActivatedHighlight: not found");
        //}
    }

    // required for maui essentials
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
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

    public Task<Intent> StartAsync(Intent intent, int requestCode, Action<Intent>? onCreate = null, Action<Intent>? onResult = null)
    {
        var data = new IntentTask(requestCode, onCreate, onResult);
        _tasks[data.RequestCode] = data;

        intent.PutExtra(REQUESTCODE_EXTRA, requestCode);

        StartActivityForResult(intent, requestCode);
        return data.TCS.Task;
    }

    private static IntentTask? GetTask(int requestCode, bool remove = false)
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
    public IntentTask(int requestCode, Action<Intent>? onCreate, Action<Intent>? onResult)
    {
        RequestCode = requestCode;
        OnResult = onResult;
        OnCreate = onCreate;
        TCS = new TaskCompletionSource<Intent>();
    }

    public int RequestCode { get; }

    public TaskCompletionSource<Intent> TCS { get; }

    public Action<Intent>? OnResult { get; }

    public Action<Intent>? OnCreate { get; }
}

