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
                .WithSeverity(Severity.Error);

            RuleFor(x => x.Purpose)
                .NotEmpty()
                .WithSeverity(Severity.Error);

            RuleFor(x => x.DefaultUOM)
                .NotEmpty()
                .WithSeverity(Severity.Error);
        }
    }
}
