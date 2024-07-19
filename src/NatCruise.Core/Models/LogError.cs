using Prism.Mvvm;

namespace NatCruise.Models
{
    public class LogError : BindableBase
    {
        public string LogID { get; set; }

        public int LogNumber { get; set; }

        public string Message { get; set; }
    }
}