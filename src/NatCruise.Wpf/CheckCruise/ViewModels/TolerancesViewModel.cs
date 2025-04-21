using NatCruise.Wpf.CheckCruise;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using NatCruise.Wpf.CheckCruise.Models;
using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NatCruise.Wpf.CheckCruise.ViewModels
{
    public class TolerancesViewModel : ObservableObject
    {
        public event EventHandler DoneConfig; 

        private ToleranceConfig _toleranceConfig;
        private Sale _sale;

        public ToleranceConfig ToleranceConfig
        {
            get => _toleranceConfig;
            protected set
            {
                SetProperty(ref _toleranceConfig, value);
            }
        }

        public ToleranceConfig PersistedToleranceConfig { get; protected set; }

        public Sale Sale
        {
            get => _sale;
            protected set => SetProperty(ref _sale, value);
        }

        protected string ToleranceFilePath { get; }

        protected IDataContextService DataContext { get; }
        protected ISaleDataservice SaleDataservice { get; }
        protected DefaultTolerances DefaultTolerancesDataService { get; }
        protected ILogger Log { get; }

        public TolerancesViewModel(IDataContextService dataContext, ISaleDataservice saleDataservice, ILogger<TolerancesViewModel> log)
        {
            DataContext = dataContext;
            SaleDataservice = saleDataservice;
            DefaultTolerancesDataService = new DefaultTolerances();

            var filePath = DataContext.DatabasePath;
            var fileDir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            ToleranceFilePath = Path.Combine(fileDir, fileName + ".ccTolerance");
        }

        protected void OnInitialize(IDictionary<string, object> parameters)
        {
            var sale = SaleDataservice.GetSale();

            var tolerancePath = ToleranceFilePath;

            if (File.Exists(tolerancePath))
            {
                var toleranceJson = File.ReadAllText(tolerancePath);

                try
                {
                    var config = JsonSerializer.Deserialize<ToleranceConfig>(toleranceJson);
                    PersistedToleranceConfig = config;
                    ToleranceConfig = (ToleranceConfig)config.Clone();
                }
                catch(JsonException ex)
                {
                    Log.LogError(ex, "Error Deserializing Tolerance File {FilePath}", tolerancePath);
                }
            }

            if(ToleranceConfig == null)
            {
                var region = int.Parse(sale.Region);

                ToleranceConfig = DefaultTolerancesDataService.GetDefaultConfig(region);
            }
        }

        public void Finish()
        {
            var tolerancePath = ToleranceFilePath;
            var toleranceFile = new FileInfo(tolerancePath);

            string toleranceJson = null;
            try
            {
                toleranceJson = JsonSerializer.Serialize(ToleranceConfig);
            }
            catch (ArgumentException ex)
            {
                Log?.LogError(ex, "Error Serializing Tolerances");
            }

            if(toleranceJson != null)
            {
                try
                {
                    File.WriteAllText(toleranceFile.FullName, toleranceJson);
                }
                catch (Exception fileEx)
                {
                    Log?.LogError(fileEx, $"Error Writing Tolerance File");
                }
            }

            DoneConfig?.Invoke(this, EventArgs.Empty);
            



        }
    }
}
