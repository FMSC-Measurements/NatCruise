using FluentValidation;
using FluentValidation.Results;
using NatCruise.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NatCruise.MVVM
{
    public class ValidationViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { ErrorCollection.ErrorsChanged += value; }
            remove { ErrorCollection.ErrorsChanged -= value; }
        }

        public ErrorCollection ErrorCollection { get; } = new ErrorCollection();

        protected IValidator Validator { get; }

        public bool HasErrors => ErrorCollection.HasErrors;

        public ValidationViewModelBase(IValidator validator)
        {
            Validator = validator;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return ErrorCollection.GetErrors(propertyName);
        }

        //protected void SetPropertyAndValidate<TModel, T>(TModel model, T value, Action<TModel, T> setter, Action<TModel> validated, [CallerMemberName] string propName = null)
        //{
        //    if (setter is null) { throw new ArgumentNullException(nameof(setter)); }
        //    if (string.IsNullOrEmpty(propName)) { throw new ArgumentException($"'{nameof(propName)}' cannot be null or empty.", nameof(propName)); }

        //    if(model == null)
        //    { return; }

        //    setter.Invoke(model, value);

        //    // we need to validate all properties just incase there are any validation rules
        //    // that are dependant on the set property, also we don't want to invoke the validated
        //    // action if another validation rule fails
        //    var results = Validator.Validate(new ValidationContext<TModel>(model));
        //    var errorDict = results.Errors.ToCollectionDictionary(x => x.PropertyName);
        //    Errors = errorDict;

        //    RaiseErrorsChanged(propName);

        //    if(false == results.Errors.Any(x => x.Severity == Severity.Error))
        //    { validated?.Invoke(model); }
        //}

        protected void SetPropertyAndValidate<TModel, T>(TModel model, T value, Action<TModel, T> setter, [CallerMemberName] string propName = null)
        {
            if (setter is null) { throw new ArgumentNullException(nameof(setter)); }
            if (string.IsNullOrEmpty(propName)) { throw new ArgumentException($"'{nameof(propName)}' cannot be null or empty.", nameof(propName)); }

            if (model == null)
            { return; }

            setter.Invoke(model, value);

            // we need to validate all properties just in case there are any validation rules
            // that are dependent on the set property, also we don't want to invoke the validated
            // action if another validation rule fails
            var results = Validator.Validate(new ValidationContext<TModel>(model));
            ErrorCollection.Update(results);
        }

        protected void ValidateAll<TModel>(TModel model)
        {
            if (model != null)
            {
                var results = Validator.Validate(new ValidationContext<TModel>(model));
                ErrorCollection.Update(results);
            }
            else
            {
                ErrorCollection.Clear();
            }
        }
    }
}