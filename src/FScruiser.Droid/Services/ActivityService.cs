using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Droid.Services
{
    public class ActivityService : IActivityService
    {
        public ActivityService(Activity activity)
        {
            Activity = activity;
        }

        public Activity Activity { get; }
    }
}
