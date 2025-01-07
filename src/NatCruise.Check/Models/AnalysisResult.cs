using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Check.Models
{
    public class AnalysisResult
    {
        public IReadOnlyList<ResultsList> Results { get; set; }
    }
}
