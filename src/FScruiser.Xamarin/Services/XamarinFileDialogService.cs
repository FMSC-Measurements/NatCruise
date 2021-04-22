using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace FScruiser.XF.Services
{
    public class XamarinFileDialogService : IFileDialogService
    {
        public async Task<string> SelectCruiseFileAsync()
        {
            var fileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>()
                {
                    { DevicePlatform.Android, new string[] {"application/cruise", "application/crz3"}},
                };

            var result = await FilePicker.PickAsync();

            return result?.FullPath;
        }

        public virtual Task<string> SelectCruiseFileDestinationAsync(string defaultDir = null, string defaultFileName = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> SelectTemplateFileAsync()
        {
            throw new NotImplementedException();
        }
    }
}