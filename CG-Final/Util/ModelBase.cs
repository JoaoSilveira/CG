using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CG_Final.Annotations;

namespace CG_Final.Util
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        [NotifyPropertyChangedInvocator]
        protected virtual void SetProperty<T>(ref T oldValue, T newValue, EventHandler<ValueChangedEventArgs<T>> changedHandler = null,
            EventHandler<ValueChangedEventArgs<T>> changingHandler = null, [CallerMemberName] string propertyName = "")
        {
            if (oldValue != null && oldValue.Equals(newValue)) return;

            changingHandler?.Invoke(this, new ValueChangedEventArgs<T>(oldValue, newValue));
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

            var old = oldValue;
            oldValue = newValue;

            changedHandler?.Invoke(this, new ValueChangedEventArgs<T>(old, newValue));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}