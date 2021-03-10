using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.Services
{
    public interface IDesignNavigationService : ICoreNavigationService
    {
        Task ShowSale();

        Task ShowStrata();

        Task ShowCuttingUnitList();

        Task ShowCruiseLandingLayout();
    }
}
