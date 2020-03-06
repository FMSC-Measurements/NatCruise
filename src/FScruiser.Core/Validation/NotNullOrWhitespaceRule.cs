namespace FScruiser.Validation
{
    public class NotNullOrWhitespaceRule<TargetType> : PropertyRule<TargetType, string>
    {
        public NotNullOrWhitespaceRule(string property, ValidationLevel level, string message) : base(property, level, message)
        {
        }

        public override ValidationError Validate(TargetType target, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            { return new ValidationError(this); }
            else
            { return null; }
        }
    }
}