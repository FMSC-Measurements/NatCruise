namespace NatCruise.Data
{
    public interface IMessageLogDataservice : IDataservice
    {
        void LogMessage(string message, string level);
    }
}