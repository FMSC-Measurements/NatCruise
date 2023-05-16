using FluentValidation;
using NatCruise.Models;

namespace NatCruise.Validation
{
    public class TreeFieldSetupValidator : AbstractValidator<TreeFieldSetup>
    {
        public TreeFieldSetupValidator()
        {
            RuleFor(x => x.DefaultValueInt)
                .GreaterThanOrEqualTo(0).When(x => x.DefaultValueInt != null)
                .WithSeverity(Severity.Error)
                .WithName("Default Value");

            RuleFor(x => x.DefaultValueReal)
                .GreaterThanOrEqualTo(0.0).When(x => x.DefaultValueReal != null)
                .WithSeverity(Severity.Error)
                .WithName("Default Value");
        }
    }
}