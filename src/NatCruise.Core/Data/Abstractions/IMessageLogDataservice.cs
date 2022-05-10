namespace NatCruise.Data
{
    public interface IMessageLogDataservice
    {
        void LogMessage(string message, string level);
    }
}