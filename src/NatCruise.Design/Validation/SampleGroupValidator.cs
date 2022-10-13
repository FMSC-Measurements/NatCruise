using CruiseDAL.Schema;
using FluentValidation;
using NatCruise.Models;
using System;
using System.Linq;

namespace NatCruise.Design.Validation
{
    public class SampleGroupValidator : AbstractValidator<SampleGroup>
    {
        public SampleGroupValidator()
        {
            RuleFor(x => x.SampleGroupCode)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]+$")
                .WithSeverity(Severity.Error)
                .WithMessage("Sample Group Code Should Only Contain Numbers and Letters");

            RuleFor(x => x.PrimaryProduct)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Primary Product Can Not Be Blank");

            // allow Frequency to be 0 if BigBAF is set
            RuleFor(x => x.SamplingFrequency)
                .Must(x => x > 0)
                .When(x => CruiseMethods.FREQUENCY_SAMPLED_METHODS.Contains(x.CruiseMethod) && x.BigBAF == 0)
                .WithMessage("Sampling Frequency Should Not Be Zero")
                .WithSeverity(Severity.Error);

            RuleFor(x => x.KZ)
                .GreaterThan(0)
                .When(x => CruiseMethods.THREE_P_METHODS.Contains(x.CruiseMethod))
                .WithMessage("KZ Should Not Be Zero")
                .WithSeverity(Severity.Error);

            // allow MaxKPI to be 0 regardless, because we treat 0 as unbounded
            RuleFor(x => x.MaxKPI)
                .Must((sg, x) => x > sg.MinKPI)
                .When(x =>  x.MaxKPI > 0 && x.MinKPI > 0 && CruiseMethods.THREE_P_METHODS.Contains(x.CruiseMethod))
                .WithMessage("Max KPI Should Be Greater Than Min KPI")
                .WithSeverity(Severity.Error);
        }
    }
}