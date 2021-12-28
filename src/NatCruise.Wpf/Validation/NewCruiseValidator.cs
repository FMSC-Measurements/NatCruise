using FluentValidation;
using NatCruise.Wpf.ViewModels;

namespace NatCruise.Wpf.Validation
{
    public class NewCruiseValidator : AbstractValidator<NewCruiseViewModel>
    {
        public NewCruiseValidator()
        {
            RuleFor(x => x.SaleNumber)
                .NotEmpty()
                .Matches("^[0-9]+$")
                .WithSeverity(Severity.Error)
                .WithMessage("Sale Number Should Only Contain Numbers and Not Be Blank");

            RuleFor(x => x.SaleName)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Sale Name Should Not Be Blank");

            RuleFor(x => x.UOM)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("UOM Should Not Be Blank");

            RuleFor(x => x.Region)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Region Should Not Be Blank");

            RuleFor(x => x.Forest)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Forest Should Not Be Blank");

            RuleFor(x => x.District)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("District Should Not Be Blank");

            RuleFor(x => x.Purpose)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Purpose Should Not Be Blank");
        }
    }
}