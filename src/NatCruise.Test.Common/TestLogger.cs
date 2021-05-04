using NatCruise.Services;
using System;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace NatCruise.Test
{
    internal class TestLogger : LoggingService
    {
        public TestLogger(ITestOutputHelper testOutput)
        {
            TestOutput = testOutput ?? throw new ArgumentNullException(nameof(testOutput));
        }

        public Xunit.Abstractions.ITestOutputHelper TestOutput { get; set; }

        public override void LogException(string catigory, string message, Exception ex, IDictionary<string, string> data = null)
        {
            TestOutput.WriteLine($"Error:::{catigory}::::{message}::::{ex.Message}::::");
            TestOutput.WriteLine(ex.StackTrace);
        }

        public override void LogEvent(string name, IDictionary<string, string> data = null)
        {
            TestOutput.WriteLine($"Event:::{name}::::");
        }
    }
}