using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NatCruise.Design.ViewModels
{
    public class LogFieldsViewModel : ViewModelBase
    {
        private IEnumerable<LogField> _logFields;
        private LogField _selectedLogField;

        public IEnumerable<LogField> LogFields
        {
            get => _logFields;
            set => SetProperty(ref _logFields, value);
        }

        public ILogFieldDataservice LogFieldDataservice { get; }

        public LogField SelectedLogField
        {
            get => _selectedLogField;
            set
            {
                if (_selectedLogField != null) { _selectedLogField.PropertyChanged -= SelectedLogField_PropertyChanged; }
                SetProperty(ref _selectedLogField, value);
                if (value != null) { value.PropertyChanged += SelectedLogField_PropertyChanged; }
            }
        }

        private void SelectedLogField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is LogField lf && lf != null)
            {
                LogFieldDataservice.UpdateLogField(lf);
            }
        }

        public LogFieldsViewModel(ILogFieldDataservice logFieldDataservice)
        {
            LogFieldDataservice = logFieldDataservice ?? throw new ArgumentNullException(nameof(logFieldDataservice));
        }

        public override void Load()
        {
            base.Load();
            LogFields = LogFieldDataservice.GetLogFields();
        }
    }
}