using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FScruiser.Util
{
    public class INPC_Base : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propName));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void SetValue<tTarget>(ref tTarget target, tTarget value, [CallerMemberName] string propName = null)
        {
            if (object.Equals(target, value)) { return; }
            target = value;
            if (propName != null) { RaisePropertyChanged(propName); }
        }
    }
}