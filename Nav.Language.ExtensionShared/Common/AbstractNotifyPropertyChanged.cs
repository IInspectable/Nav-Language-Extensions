#region Using Directives

using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common; 

abstract class AbstractNotifyPropertyChanged : INotifyPropertyChanged {

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
        OnPropertyChanged(propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(string propertyName) {
            
    }

    /// <returns>True if the property was updated</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
        if (!EqualityComparer<T>.Default.Equals(field, value)) {
            field = value;
            // ReSharper disable once ExplicitCallerInfoArgument
            NotifyPropertyChanged(propertyName);
            return true;
        }

        return false;
    }
}