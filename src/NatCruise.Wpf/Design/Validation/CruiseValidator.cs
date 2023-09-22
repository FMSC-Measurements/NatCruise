using FluentValidation;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Validation
{
    public class CruiseValidator : AbstractValidator<Cruise>
    {
        public CruiseValidator()
        {
            RuleFor(x => x.CruiseNumber)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]+$")
                .WithSeverity(Severity.Error)
                .WithMessage("Cruise Number Should Only Contain Numbers and Letters");

            RuleFor(x => x.Purpose)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Cruise Purpose Should Not Be Blank");

            RuleFor(x => x.DefaultUOM)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Default UOM Should Not Be Blank");
        }
    }
}
