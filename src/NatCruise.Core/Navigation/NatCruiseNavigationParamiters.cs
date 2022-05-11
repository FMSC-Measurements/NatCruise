using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Navigation
{
    public class NatCruiseNavigationParamiters : Dictionary<string, object>
    {
        public string SaleID
        {
            get => GetValueInternal<string>();
            set => SetValue(value);
        }

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


        protected TResult GetValueInternal<TResult>([CallerMemberName] string name = null) where TResult : class
        {
            if (base.TryGetValue(name, out var value)) { return (TResult)value; }
            else { return (TResult)null; }

            //return base.GetValue<TResult>(name);
        }


        protected void SetValue(object value, [CallerMemberName] string name = null)
        {
            if (ContainsKey(name))
            { base[name] = value; }
            else
            { base.Add(name, value); }

            //base.Add(name, value);
        }
    }
}
