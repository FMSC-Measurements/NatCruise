using System;
using System.Reflection;

namespace FScruiser.Validation
{
    public abstract class PropertyRule<TargetType, ValueType> : IValidationRule<TargetType>
    {
        public string Property
        {
            get { return _property; }
            set
            {
                _property = value;
                _propertyInfo = typeof(TargetType).GetProperty(value) ?? throw new ArgumentException("invalid property value :" + value);
            }
        }

        public ValidationLevel Level { get; set; }

        public string Message { get; set; }

        private PropertyInfo _propertyInfo;
        private string _property;

        protected PropertyRule()
        {
        }

        protected PropertyRule(string property, ValidationLevel level, string message)
        {
            Property = property;
            Level = level;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public ValidationError Validate(TargetType target)
        {
            var value = (ValueType)_propertyInfo.GetValue(target);
            return Validate(target, value);
        }

        public abstract ValidationError Validate(TargetType target, ValueType value);
    }
}