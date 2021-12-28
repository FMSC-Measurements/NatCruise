using FluentValidation;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Validation
{
    public class TreeFieldSetupValidator : AbstractValidator<TreeFieldSetup>
    {
        public TreeFieldSetupValidator()
        {
            RuleFor(x => x.DefaultValueInt)
                .GreaterThanOrEqualTo(0).When(x => x.DefaultValueInt != null)
                .WithSeverity(Severity.Error)
                .WithName("Default Value");

            RuleFor(x => x.DefaultValueReal)
                .GreaterThanOrEqualTo(0.0).When(x => x.DefaultValueReal != null)
                .WithSeverity(Severity.Error)
                .WithName("Default Value");

        }
    }
}
