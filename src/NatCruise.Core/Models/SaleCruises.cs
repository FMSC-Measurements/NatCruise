using System.Collections.Generic;

namespace NatCruise.Models
{
    public class SaleCruises
    {
        public Sale Sale { get; set; }

        public IEnumerable<Cruise> Cruises { get; set; }
    }
}