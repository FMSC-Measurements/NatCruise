using FluentValidation;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Validation
{
    public class SaleValidator : AbstractValidator<Sale>
    {
        public SaleValidator()
        {
            RuleFor(x => x.SaleNumber)
                .NotEmpty()
                .WithSeverity(Severity.Error);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithSeverity(Severity.Error);

            RuleFor(x => x.DefaultUOM)
                .NotEmpty()
                .WithSeverity(Severity.Warning);
        }
    }
}
