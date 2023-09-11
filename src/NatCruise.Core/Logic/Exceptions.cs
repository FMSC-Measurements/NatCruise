using System;

namespace NatCruise.Logic
{
    public class FrequencyMismatchException : Exception
    {
        public FrequencyMismatchException(string message) : base(message)
        { }
    }
}