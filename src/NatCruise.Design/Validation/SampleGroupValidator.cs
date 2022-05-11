﻿using CruiseDAL.Schema;
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
                .WithMessage("Primary Product Should Not Be Blank");

            RuleFor(x => x.SamplingFrequency)
                .Must(x => x > 0)
                .When(x => CruiseMethods.FREQUENCY_SAMPLED_METHODS.Contains(x.CruiseMethod))
                .WithMessage("Sampling Frequency Should Not Be Zero")
                .WithSeverity(Severity.Error);

            RuleFor(x => x.KZ)
                .Must(x => x > 0)
                .When(x => CruiseMethods.THREE_P_METHODS.Contains(x.CruiseMethod))
                .WithMessage("KZ Should Not Be Zero")
                .WithSeverity(Severity.Error);

            RuleFor(x => x.MinKPI)
                .Must(x => x > 0)
                .When(x => CruiseMethods.THREE_P_METHODS.Contains(x.CruiseMethod))
                .WithMessage("Min KPI Should Be Greater Than Zero")
                .WithSeverity(Severity.Warning);

            RuleFor(x => x.MaxKPI)
                .Must(x => x > 0)
                .Must((sg, x) => x > sg.MinKPI)
                .When(x => CruiseMethods.THREE_P_METHODS.Contains(x.CruiseMethod))
                .WithMessage("Max KPI Should Be Greater Than Zero");

            RuleFor(x => x.MaxKPI)
                .Must((sg, x) => x > sg.MinKPI)
                .When(x => CruiseMethods.THREE_P_METHODS.Contains(x.CruiseMethod) && x.MinKPI > 0)
                .WithMessage("Max KPI Should Be Greater Than Min KPI");
        }
    }
}