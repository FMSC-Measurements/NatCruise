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
                .Matches("^[0-9]+$")
                .WithSeverity(Severity.Error)
                .WithMessage("Sale Number Should Only Contain Numbers and Not Be Blank");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Sale Name Should Not Be Blank");

            RuleFor(x => x.DefaultUOM)
                .NotEmpty()
                .WithSeverity(Severity.Error)
                .WithMessage("Default UOM Should Not Be Blank");
        }
    }
}
