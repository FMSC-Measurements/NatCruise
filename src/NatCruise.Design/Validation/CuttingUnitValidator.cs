using FluentValidation;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Validation
{
    public class CuttingUnitValidator : AbstractValidator<CuttingUnit>
    {
        public CuttingUnitValidator()
        {
            RuleFor(x => x.CuttingUnitCode)
                .NotEmpty()
                .WithSeverity(Severity.Error);

            RuleFor(x => x.Area)
                .Must(x => x > 0)
                .WithMessage("Cutting Unit Area is 0")
                .WithSeverity(Severity.Info);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithSeverity(Severity.Info);

        }
    }
}
