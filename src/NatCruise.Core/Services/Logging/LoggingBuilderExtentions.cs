using Microsoft.Extensions.Logging;

namespace NatCruise.Services.Logging
{
    public static class LoggingBuilderExtentions
    {
        public static void AddAppCenterLogger(this ILoggingBuilder builder)
        {
            builder.AddProvider(new AppCenterLoggerProvider());
        }
    }
}