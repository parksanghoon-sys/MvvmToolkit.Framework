using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvvmToolkit.Core.Components
{
    public abstract class NotifyPropertyChangeObject : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
    {
        private bool _disposed;        
        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected NotifyPropertyChangeObject()
        {
            OnActive();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
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
     
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 관리되는 자원 해제 (예: 이벤트 핸들러 제거)
                    PropertyChanged = null;
                    PropertyChanging = null;
                    DisposeCore();
                }
                // 관리되지 않는 자원 해제
                _disposed = true;
            }
        }
        protected virtual void DisposeCore()
        {

        }
    }
}
