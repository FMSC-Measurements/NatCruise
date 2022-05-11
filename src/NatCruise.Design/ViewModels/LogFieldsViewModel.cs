using NatCruise.Design.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ITemplateDataservice TemplateDataservice { get; }

        public LogField SelectedLogField
        {
            get => _selectedLogField;
            set
            {
                if(_selectedLogField != null) { _selectedLogField.PropertyChanged -= SelectedLogField_PropertyChanged; }
                SetProperty(ref _selectedLogField, value);
                if(value != null) { value.PropertyChanged += SelectedLogField_PropertyChanged; }
            }
        }

        private void SelectedLogField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(sender is LogField lf && lf != null)
            {
                TemplateDataservice.UpdateLogField(lf);
            }
        }

        public LogFieldsViewModel(ITemplateDataservice templateDataservice)
        {
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
        }

        public override void Load()
        {
            base.Load();
            LogFields = TemplateDataservice.GetLogFields();
        }
    }
}
