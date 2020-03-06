using System;

namespace FScruiser.Validation
{
    public class PredicateRule<TargetType> : IValidationRule<TargetType>
    {
        public PredicateRule(string property, string message, ValidationLevel level, Predicate<TargetType> predicate)
        {
            Property = property;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Level = level;
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public string Property { get; set; }

        public string Message { get; set; }

        public ValidationLevel Level { get; set; }

        public Predicate<TargetType> Predicate { get; set; }

        public ValidationError Validate(TargetType target)
        {
            if (!Predicate(target))
            {
                return new ValidationError(this);
            }
            else
            { return null; }
        }
    }
}