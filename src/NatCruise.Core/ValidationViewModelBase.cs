using FluentValidation;
using FluentValidation.Results;
using NatCruise.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NatCruise
{
    public class ValidationViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private Dictionary<string, IEnumerable<ValidationFailure>> _errors = new();

        protected Dictionary<string, IEnumerable<ValidationFailure>> Errors
        {
            get => _errors;
            set
            {
                _errors = value;
                if (value != null)
                {
                    RaiseErrorsChanged(null);
                }
            }
        }

        protected IValidator Validator { get; }

        public bool HasErrors => _errors?.Any() ?? false;

        public ValidationViewModelBase(IValidator validator)
        {
            Validator = validator;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            var errors = Errors;
            if (errors != null && errors.TryGetValue(propertyName, out var value))
            {
                return value;
            }
            else { return null; }
        }

        protected void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            RaisePropertyChanged(nameof(HasErrors));
        }

        protected void SetPropertyAndValidate<TModel, T>(TModel model, T value, Action<TModel, T> setter, Action<TModel> validated, [CallerMemberName] string propName = null)
        {
            if (setter is null) { throw new ArgumentNullException(nameof(setter)); }
            if (string.IsNullOrEmpty(propName)) { throw new ArgumentException($"'{nameof(propName)}' cannot be null or empty.", nameof(propName)); }

            if(model == null)
            { return; }

            setter.Invoke(model, value);

            var selector = ValidatorOptions.Global.ValidatorSelectors.MemberNameValidatorSelectorFactory.Invoke(new[] { propName, });
            var results = Validator.Validate(new ValidationContext<TModel>(model, new FluentValidation.Internal.PropertyChain(), selector));

            Errors[propName] = results.Errors;
            RaiseErrorsChanged(propName);

            if(results.IsValid)
            { validated?.Invoke(model); }
        }

        protected void ValidateAll<TModel>(TModel model)
        {
            var results = Validator.Validate(new ValidationContext<TModel>(model));
            var errorDict = results.Errors.ToCollectionDictionary(x => x.PropertyName);
            Errors = errorDict;
        }
    }
}