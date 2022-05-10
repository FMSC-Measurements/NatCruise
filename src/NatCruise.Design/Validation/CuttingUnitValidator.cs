using FluentValidation;
using NatCruise.Models;

namespace NatCruise.Design.Validation
{
    public class CuttingUnitValidator : AbstractValidator<CuttingUnit>
    {
        public CuttingUnitValidator()
        {
            RuleFor(x => x.CuttingUnitCode)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]+$")
                .WithSeverity(Severity.Error)
                .WithMessage("Cutting Unit Code Should Only Contain Numbers and Letters");

            RuleFor(x => x.Area)
                .Must(x => x > 0)
                .WithMessage("Cutting Unit Area Should Be Grater Than 0")
                .WithSeverity(Severity.Info);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithSeverity(Severity.Info)
                .WithMessage("Cutting Unit Description is Recommended");
        }
    }
}