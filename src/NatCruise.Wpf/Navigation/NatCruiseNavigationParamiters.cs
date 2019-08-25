using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Navigation
{
    public class NatCruiseNavigationParamiters : Prism.Regions.NavigationParameters
    {
        public string CuttingUnitCode
        {
            get => GetValueInternal<string>();
            set => SetValue(value);
        }

        public string StratumCode
        {
            get => GetValueInternal<string>();
            set => SetValue(value);
        }

        public string SampleGroupCode
        {
            get => GetValueInternal<string>();
            set => SetValue(value);
        }


        protected TResult GetValueInternal<TResult>([CallerMemberName] string name = null)
        {
            return base.GetValue<TResult>(name);
        }


        protected void SetValue(object value, [CallerMemberName] string name = null)
        {
            base.Add(name, value);
        }
    }
}
