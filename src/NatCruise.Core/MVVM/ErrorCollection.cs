using FluentValidation.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.MVVM
{
    public class ErrorCollection
    {
        public ErrorCollection()
        {
        }

        public ErrorCollection(EventHandler<DataErrorsChangedEventArgs> errorsChanged, EventHandler hasErrorsChanged)
        {
            ErrorsChanged += errorsChanged;
            HasErrorsChanged += hasErrorsChanged;
        }

        protected Dictionary<string, ValidationFailure[]> ValidationErrors { get; private set; } = new Dictionary<string, ValidationFailure[]>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public event EventHandler HasErrorsChanged;

        public string this[string key]
        {
            get
            {
                if(ValidationErrors.TryGetValue(key, out ValidationFailure[] errors ) )
                {
                    return string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage).ToArray());
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public bool HasErrors => ValidationErrors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            if(ValidationErrors.TryGetValue(propertyName, out ValidationFailure[] errors))
            { return errors.Select(x => x.ErrorMessage).ToArray(); }
            else
            { return Enumerable.Empty<string>(); }
        }

        public void Update(ValidationResult result)
        {
            var newErrorDict = result.Errors
                .Where(x => x.Severity == FluentValidation.Severity.Error)
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToArray());

            // compare old set of errors to new set of errors
            // to see if there are any changes
            var oldErrorDict = ValidationErrors;
            var allProps = oldErrorDict.Keys.Union(newErrorDict.Keys).ToArray();
            var changes = new List<string>();
            foreach (var p in allProps)
            {
                // if only error set contains property
                if(oldErrorDict.ContainsKey(p) ^ newErrorDict.ContainsKey(p))
                {
                    changes.Add(p);
                    continue;
                }

                if (oldErrorDict.ContainsKey(p) && newErrorDict.ContainsKey(p))
                {
                    var oldErrors = oldErrorDict[p];
                    var newErrors = newErrorDict[p];
                    if(oldErrors.Length != newErrors.Length)
                    {
                        changes.Add(p);
                        continue;
                    }

                    var newErrorMsgs = newErrors.Select(x => x.ErrorMessage);
                    var oldErrorMsgs = oldErrors.Select(x => x.ErrorMessage);

                    if(newErrorMsgs.Except(oldErrorMsgs, StringComparer.Ordinal).Any())
                    {
                        changes.Add(p);
                        continue;
                    }

                    if (oldErrorMsgs.Except(newErrorMsgs, StringComparer.Ordinal).Any())
                    {
                        changes.Add(p);
                        continue;
                    }
                }
            }

            ValidationErrors = newErrorDict;



            foreach (var p in changes)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(p));
            }

            if (oldErrorDict.Any() ^ newErrorDict.Any())
            {
                HasErrorsChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public void Clear()
        {
            var props = ValidationErrors.Keys.ToArray();

            ValidationErrors = new Dictionary<string, ValidationFailure[]>();



            foreach (var p in props)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(p));
            }

            if (props.Any())
            {
                HasErrorsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
