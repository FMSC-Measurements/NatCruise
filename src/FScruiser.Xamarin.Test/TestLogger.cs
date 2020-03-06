using System;
using System.Collections.Generic;
using System.Text;
using Prism.Logging;
using Xunit.Abstractions;

namespace FScruiser.XF
{
    class TestLogger : Prism.Logging.ILoggerFacade
    {
        public TestLogger(ITestOutputHelper testOutput)
        {
            TestOutput = testOutput ?? throw new ArgumentNullException(nameof(testOutput));
        }

        public Xunit.Abstractions.ITestOutputHelper TestOutput { get; set; }

        public void Log(string message, Category category, Priority priority)
        {
            switch (priority)
            {
                case Priority.High:
                case Priority.Medium:
                case Priority.Low:
                case Priority.None:
                    {
                        TestOutput.WriteLine($"{category.ToString().ToUpper()}::::{message}");
                        break;
                    }
            }
        }
    }
}
