using System.Collections.Generic;

namespace FScruiser.Validation
{
    public class ValidationResult
    {
        public ValidationResult()
        {
        }

        public ValidationResult(IEnumerable<ValidationError> errors)
        {
            Errors.AddRange(errors);
        }

        public bool HasErrors => Errors.Count > 0;

        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();
    }
}