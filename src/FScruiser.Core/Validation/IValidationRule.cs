namespace FScruiser.Validation
{
    public interface IValidationRule<in TargetType> : IValidationRule
    {
        ValidationError Validate(TargetType target);
    }

    public interface IValidationRule
    {
        string Property { get; }

        ValidationLevel Level { get; }

        string Message { get; }
    }
}