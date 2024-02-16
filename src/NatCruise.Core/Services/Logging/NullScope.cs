using System;

namespace NatCruise.Services.Logging
{
    public class NullScope : IDisposable
    {
        public static IDisposable Instance { get; } = new NullScope();

        public void Dispose()
        { }
    }
}