using System.Collections.Generic;
using System.Linq;

namespace FScruiser.Validation
{
    public class Validator_Base<TargetType>
    {
        public List<IValidationRule<TargetType>> Rules { get; set; } = new List<IValidationRule<TargetType>>();

        public ValidationResult Validate(TargetType target, string property = null)
        {
            ValidationResult result = new ValidationResult();

            foreach (var rule in Rules.Where(r => property == null || r.Property == property))
            {
                var error = rule.Validate(target);
                if (error != null)
                {
                    result.Errors.Add(error);
                }
            }

            return result;
        }

        public ValidationResult Validate(TargetType target, IEnumerable<string> properties)
        {
            var result = new ValidationResult();

            foreach (var property in properties)
            {
                foreach (var rule in Rules.Where(r => property == null || r.Property == property))
                {
                    var error = rule.Validate(target);
                    if (error != null)
                    {
                        result.Errors.Add(error);
                    }
                }
            }

            return result;
        }
    }
}