﻿using CruiseDAL.Schema;
using FluentValidation;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Validation
{
    public class StratumValidator : AbstractValidator<Stratum>
    {
        public StratumValidator()
        {
            RuleFor(x => x.StratumCode)
                .NotEmpty()
                .WithSeverity(Severity.Error);

            RuleFor(x => x.Method)
                .NotEmpty()
                .WithSeverity(Severity.Error);

            RuleFor(x => x.FixCNTField)
                .NotEmpty()
                .When(x => x.Method == CruiseMethods.FIXCNT)
                .WithMessage("FixCNT Field Should Not Be Empty")
                .WithSeverity(Severity.Error);

            RuleFor(x => x.FixedPlotSize)
                .Must(x => x > 0)
                .When(x => CruiseMethods.FIXED_SIZE_PLOT_METHODS.Contains(x.Method))
                .WithMessage("Fixed Plot Size Should Not Be Zero")
                .WithSeverity(Severity.Error);

            RuleFor(x => x.BasalAreaFactor)
                .Must(x => x > 0)
                .When(x => CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(x.Method))
                .WithMessage("Basal Area Factor Should Not Be Zero")
                .WithSeverity(Severity.Error);

            RuleFor(x => x.KZ3PPNT)
                .Must(x => x > 0)
                .When(x => CruiseMethods.THREEPPNT == x.Method)
                .WithMessage("3PPNT KZ Should Not Be Zero")
                .WithSeverity(Severity.Error);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithSeverity(Severity.Info);
        }
    }
}
