﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvvmToolkit.Core.Components
{
    public abstract class NotifyPropertyChangeObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SetProperty<T>(ref T reference, T value, [CallerMemberName] string propertyName = "")
        {
            if (reference == null)
            {
                reference = value;
                OnPropertyChanged(propertyName);
                return;
            }
            if (!reference.Equals(value))
            {
                reference = value;
                OnPropertyChanged(propertyName);
                return;
            }
        }
        public virtual void OnActive()
        {

        }
    }
}
