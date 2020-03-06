using FScruiser.Services;
using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Services
{
    public class CruisersDataservice : ICruisersDataservice
    {

        const string CRUISERS_PROP_KEY = "cruisers";
        const string PROMPT_CRUISERS_ON_SAMPLE_KEY = "prompt_cruisers_on_sample";
        private string[] _cruisers;

        public CruisersDataservice(Application application)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
        }



        public Xamarin.Forms.Application Application { get; set; }

        public bool PromptCruiserOnSample
        {
            get
            {
                return Application.Properties.GetValueOrDefault(PROMPT_CRUISERS_ON_SAMPLE_KEY, false);
            }
            set
            {
                Application.Properties.SetValue(PROMPT_CRUISERS_ON_SAMPLE_KEY, value);
                Application.SavePropertiesAsync();
            }
        }

        protected void RefreshCruisers()
        {
            var app = Application;
            var props = Application.Properties;

            if(props.ContainsKey(CRUISERS_PROP_KEY))
            {
                var cruisersPropValue = (string)props[CRUISERS_PROP_KEY] ;
                _cruisers = cruisersPropValue.Split(',');
            }
            else
            {
                _cruisers = new string[0];
            }
        }

        protected void SaveCruisers()
        {
            var cruisers = _cruisers ?? new string[0];

            var props = Application.Properties;
            props.SetValue(CRUISERS_PROP_KEY, string.Join(",", cruisers));
            Application.SavePropertiesAsync(); 

        }

        public void AddCruiser(string cruiser)
        {
            RefreshCruisers();
            _cruisers = _cruisers.Append(cruiser).Distinct().ToArray();
            SaveCruisers();
        }

        public IEnumerable<string> GetCruisers()
        {
            RefreshCruisers();
            return _cruisers;
        }

        public void RemoveCruiser(string cruiser)
        {
            RefreshCruisers();
            _cruisers = _cruisers.Where(x => x != cruiser).ToArray();
            SaveCruisers();
        }

        //public void UpdateCruiser(string oldValue, string newValue)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
