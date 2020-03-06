namespace FScruiser.Validation
{
    public class NotNullRule<TargetType> : PropertyRule<TargetType, object>
    {
        public NotNullRule(string property, ValidationLevel level, string message) : base(property, level, message)
        {
        }

        public override ValidationError Validate(TargetType target, object value)
        {
            if (value == null)
            { return new ValidationError(this); }
            else { return null; }
        }
    }
}