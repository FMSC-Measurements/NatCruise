using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace NatCruise.Async
{
    // https://msdn.microsoft.com/en-us/magazine/dn605875.aspx

    public sealed class NotifyTaskCompletion : INotifyPropertyChanged
    {
        public static NotifyTaskCompletion<TResult> Create<TResult>(Task<TResult> task, TResult defaultResult = default, EventHandler success = null)
            => new NotifyTaskCompletion<TResult>(task, defaultResult, success);


        public NotifyTaskCompletion(Task task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                _ = WatchTaskAsync(task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            { /* allow exceptions to be ignored for now. if task has exception it will raise property changed on IsFaulted */ }

            var propertyChanged = PropertyChanged;

            if (propertyChanged == null)
                return;

            propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
                propertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
                propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            }
            else
            {
                propertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
            }
        }

        public Task Task { get; private set; }

        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;
        public Exception InnerException => Exception?.InnerException;
        public string ErrorMessage => InnerException?.Message;

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        //public NotifyTaskCompletion(Task<TResult> task)
        //{
        //    Task = task;
        //    if (!task.IsCompleted)
        //    {
        //        _ = WatchTaskAsync(task);
        //    }
        //}

        

        public NotifyTaskCompletion(Task<TResult> task, TResult defaultResult = default, EventHandler success = null)
        {
            Success = success;
            DefaultResult = defaultResult;
            Task = task;
            if (!task.IsCompleted)
            {
                _ = WatchTaskAsync(task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            { /* allow exceptions to be ignored for now. if task has exception it will raise property changed on IsFaulted */ }

            if(task.Status == TaskStatus.RanToCompletion)
            { Success?.Invoke(this, EventArgs.Empty); }

            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
                propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
                propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
                if (task.IsCanceled)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
                }
                else if (task.IsFaulted)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                    propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
                    propertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
                    propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
                }
                else
                {
                    propertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
                    propertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
                }
            }
        }

        public TResult DefaultResult { get; }

        public Task<TResult> Task { get; private set; }

        public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);

        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;
        public Exception InnerException => Exception?.InnerException;
        public string ErrorMessage => InnerException?.Message;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Success;
    }
}