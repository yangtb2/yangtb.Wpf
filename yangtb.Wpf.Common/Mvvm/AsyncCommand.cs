using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace yangtb.Wpf.Common;

public sealed class AsyncCommand : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public AsyncCommand(Func<object, Task> execute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    }

    public AsyncCommand(Func<object, Task> execute, Func<object, bool> canExecute) : this(execute)
    {
        _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    public bool CanExecute(object parameter)
    {
        var canExecute = _canExecute?.Invoke(parameter) ?? true;
        var isCompleted = _executeTask?.GetAwaiter().IsCompleted ?? true;
        return canExecute && isCompleted;
    }

    public void Execute(object parameter)
    {
        if (CanExecute(parameter))
        {
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            _executeTask = _execute.Invoke(parameter);
            _executeTask.GetAwaiter().OnCompleted(() =>
            {
                Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            });
        }
    }

    public async void ExecuteAsync(object parameter)
    {
        if (CanExecute(parameter))
        {
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            _executeTask = _execute.Invoke(parameter);
            await _executeTask;
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }
    }

    private Task _executeTask;
    private readonly Func<object, Task> _execute;
    private readonly Func<object, bool> _canExecute;
}

public sealed class AsyncCommand<T> : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public AsyncCommand(Func<object, Task> execute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    }

    public AsyncCommand(Func<object, Task> execute, Func<object, bool> canExecute) : this(execute)
    {
        _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    public bool CanExecute(object parameter)
    {
        var canExecute = _canExecute?.Invoke(parameter) ?? true;
        var isCompleted = _executeTask?.GetAwaiter().IsCompleted ?? true;
        return canExecute && isCompleted;
    }

    public void Execute(object parameter)
    {
        if (CanExecute(parameter))
        {
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            _executeTask = _execute.Invoke((T)parameter);
            _executeTask.GetAwaiter().OnCompleted(() =>
            {
                Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            });
        }
    }

    public async Task ExecuteAsync(object parameter)
    {
        if (CanExecute(parameter))
        {
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            _executeTask = _execute.Invoke((T)parameter);
            await _executeTask;
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }
    }

    private Task _executeTask;
    private readonly Func<object, Task> _execute;
    private readonly Func<object, bool> _canExecute;
}