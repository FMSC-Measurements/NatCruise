using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Logic
{
    public class FrequencyMismatchException : Exception
    {
        public FrequencyMismatchException(string message) : base(message)
        { }
    }
}
