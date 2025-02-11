using System.ComponentModel;

namespace MvvmToolkit.Core.Components
{
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        private TResult _defaultValue;
        public event PropertyChangedEventHandler? PropertyChanged;
        public Task<TResult> Task { get; private set; }
        public TResult Result
        {
            get
            {
                if (Task.Status == TaskStatus.RanToCompletion)
                {
                    return Task.Result;
                }
                else if (_defaultValue != null)
                {
                    return _defaultValue;
                }
                else
                {
                    return default(TResult);
                }
            }
        }
        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task.IsCanceled; 
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException? Exception => Task.Exception;
        public Exception InnerException => Exception?.InnerException;
        public string ErrorMessage => InnerException.Message??"";
        public NotifyTaskCompletion(Task<TResult> task, TResult defaultValue)
        {
            _defaultValue = defaultValue;
            Task = task;
            if(!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }
        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch { }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;

            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                  new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this,
                  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }
    }
}
