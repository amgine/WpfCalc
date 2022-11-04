namespace WpfCalc;

using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

/// <summary>
///     This class allows delegating the commanding logic to methods passed as parameters,
///     and enables a View to bind commands to objects that are not part of the element tree.
/// </summary>
public class DelegateCommand : ICommand
{
	#region Constructors

	/// <summary>Constructor</summary>
	public DelegateCommand(Action executeMethod, Func<bool>? canExecuteMethod = null, bool isAutomaticRequeryDisabled = false)
	{
		if(executeMethod is null) throw new ArgumentNullException(nameof(executeMethod));

		_executeMethod = executeMethod;
		_canExecuteMethod = canExecuteMethod;
		_isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
	}

	#endregion

	#region Public Methods

	/// <summary>
	///     Method to determine if the command can be executed
	/// </summary>
	public bool CanExecute()
	{
		if(_canExecuteMethod is not null)
		{
			return _canExecuteMethod();
		}
		return true;
	}

	/// <summary>
	///     Execution of the command
	/// </summary>
	public void Execute()
	{
		if(_executeMethod is not null)
		{
			_executeMethod();
		}
	}

	/// <summary>
	///     Property to enable or disable CommandManager's automatic requery on this command
	/// </summary>
	public bool IsAutomaticRequeryDisabled
	{
		get => _isAutomaticRequeryDisabled;
		set
		{
			if(_isAutomaticRequeryDisabled != value)
			{
				if(value)
				{
					CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
				}
				else
				{
					CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
				}
				_isAutomaticRequeryDisabled = value;
			}
		}
	}

	/// <summary>
	///     Raises the CanExecuteChaged event
	/// </summary>
	public void RaiseCanExecuteChanged()
	{
		OnCanExecuteChanged();
	}

	/// <summary>
	///     Protected virtual method to raise CanExecuteChanged event
	/// </summary>
	protected virtual void OnCanExecuteChanged()
	{
		CommandManagerHelper.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
	}

	#endregion

	#region ICommand Members

	/// <summary>ICommand.CanExecuteChanged implementation.</summary>
	public event EventHandler? CanExecuteChanged
	{
		add
		{
			if(!_isAutomaticRequeryDisabled)
			{
				CommandManager.RequerySuggested += value;
			}
			CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
		}
		remove
		{
			if(!_isAutomaticRequeryDisabled)
			{
				CommandManager.RequerySuggested -= value;
			}
			CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
		}
	}

	bool ICommand.CanExecute(object? parameter)
	{
		return CanExecute();
	}

	void ICommand.Execute(object? parameter)
	{
		Execute();
	}

	#endregion

	#region Data

	private readonly Action _executeMethod;
	private readonly Func<bool>? _canExecuteMethod;
	private bool _isAutomaticRequeryDisabled;
	private List<WeakReference>? _canExecuteChangedHandlers;

	#endregion
}

/// <summary>
///     This class allows delegating the commanding logic to methods passed as parameters,
///     and enables a View to bind commands to objects that are not part of the element tree.
/// </summary>
/// <typeparam name="T">Type of the parameter passed to the delegates</typeparam>
public class DelegateCommand<T> : ICommand
{
	#region Constructors

	/// <summary>Constructor</summary>
	public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod = null, bool isAutomaticRequeryDisabled = false)
	{
		if(executeMethod is null)
		{
			throw new ArgumentNullException(nameof(executeMethod));
		}

		_executeMethod = executeMethod;
		_canExecuteMethod = canExecuteMethod;
		_isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
	}

	#endregion

	#region Public Methods

	/// <summary>
	///     Method to determine if the command can be executed
	/// </summary>
	public bool CanExecute(T parameter)
	{
		if(_canExecuteMethod != null)
		{
			return _canExecuteMethod(parameter);
		}
		return true;
	}

	/// <summary>
	///     Execution of the command
	/// </summary>
	public void Execute(T parameter)
	{
		if(_executeMethod != null)
		{
			_executeMethod(parameter);
		}
	}

	/// <summary>
	///     Raises the CanExecuteChaged event
	/// </summary>
	public void RaiseCanExecuteChanged()
	{
		OnCanExecuteChanged();
	}

	/// <summary>
	///     Protected virtual method to raise CanExecuteChanged event
	/// </summary>
	protected virtual void OnCanExecuteChanged()
	{
		CommandManagerHelper.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
	}

	/// <summary>
	///     Property to enable or disable CommandManager's automatic requery on this command
	/// </summary>
	public bool IsAutomaticRequeryDisabled
	{
		get => _isAutomaticRequeryDisabled;
		set
		{
			if(_isAutomaticRequeryDisabled != value)
			{
				if(value)
				{
					CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
				}
				else
				{
					CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
				}
				_isAutomaticRequeryDisabled = value;
			}
		}
	}

	#endregion

	#region ICommand Members

	/// <summary>
	///     ICommand.CanExecuteChanged implementation
	/// </summary>
	public event EventHandler? CanExecuteChanged
	{
		add
		{
			if(!_isAutomaticRequeryDisabled)
			{
				CommandManager.RequerySuggested += value;
			}
			CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
		}
		remove
		{
			if(!_isAutomaticRequeryDisabled)
			{
				CommandManager.RequerySuggested -= value;
			}
			CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
		}
	}

	bool ICommand.CanExecute(object? parameter)
	{
		// if T is of value type and the parameter is not
		// set yet, then return false if CanExecute delegate
		// exists, else return true
		if(parameter is null)
		{
			if(typeof(T).IsValueType) return true;
		}
		return parameter is T typed
			? CanExecute(typed)
			: false;
	}

	void ICommand.Execute(object? parameter)
	{
		if(parameter is null)
		{
			Execute(default);
		}
		else if(parameter is T typed)
		{
			Execute(typed);
		}
	}

	#endregion

	#region Data

	private readonly Action<T> _executeMethod;
	private readonly Func<T, bool>? _canExecuteMethod;
	private bool _isAutomaticRequeryDisabled = false;
	private List<WeakReference>? _canExecuteChangedHandlers;

	#endregion
}

/// <summary>
///     This class contains methods for the CommandManager that help avoid memory leaks by
///     using weak references.
/// </summary>
static class CommandManagerHelper
{
	internal static void CallWeakReferenceHandlers(object sender, List<WeakReference>? handlers)
	{
		if(handlers != null)
		{
			// Take a snapshot of the handlers before we call out to them since the handlers
			// could cause the array to me modified while we are reading it.

			var callees = new EventHandler[handlers.Count];
			int count = 0;

			for(var i = handlers.Count - 1; i >= 0; i--)
			{
				var reference = handlers[i];
				var handler = reference.Target as EventHandler;
				if(handler == null)
				{
					// Clean up old handlers that have been collected
					handlers.RemoveAt(i);
				}
				else
				{
					callees[count] = handler;
					count++;
				}
			}

			// Call the handlers that we snapshotted
			for(var i = 0; i < count; i++)
			{
				var handler = callees[i];
				var dispObj = handler.Target as DispatcherObject;
				if(dispObj != null)
				{
					if(!dispObj.CheckAccess())
					{
						dispObj.Dispatcher.BeginInvoke(
							handler,
							DispatcherPriority.DataBind,
							new object[] { sender, EventArgs.Empty });
					}
					else
					{
						handler(sender, EventArgs.Empty);
					}
				}
				else
				{
					handler(sender, EventArgs.Empty);
				}
			}
		}
	}

	internal static void AddHandlersToRequerySuggested(IEnumerable<WeakReference>? handlers)
	{
		if(handlers != null)
		{
			foreach(var handlerRef in handlers)
			{
				if(handlerRef.Target is EventHandler handler)
				{
					CommandManager.RequerySuggested += handler;
				}
			}
		}
	}

	internal static void RemoveHandlersFromRequerySuggested(IEnumerable<WeakReference>? handlers)
	{
		if(handlers != null)
		{
			foreach(var handlerRef in handlers)
			{
				if(handlerRef.Target is EventHandler handler)
				{
					CommandManager.RequerySuggested -= handler;
				}
			}
		}
	}

	internal static void AddWeakReferenceHandler(ref List<WeakReference>? handlers, EventHandler handler)
	{
		AddWeakReferenceHandler(ref handlers, handler, -1);
	}

	internal static void AddWeakReferenceHandler(ref List<WeakReference>? handlers, EventHandler handler, int defaultListSize)
	{
		if(handlers == null)
		{
			handlers = (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
		}

		handlers.Add(new WeakReference(handler));
	}

	internal static void RemoveWeakReferenceHandler(List<WeakReference>? handlers, EventHandler handler)
	{
		if(handlers != null)
		{
			for(var i = handlers.Count - 1; i >= 0; i--)
			{
				var reference = handlers[i];
				var existingHandler = reference.Target as EventHandler;
				if((existingHandler == null) || (existingHandler == handler))
				{
					// Clean up old handlers that have been collected
					// in addition to the handler that is to be removed.
					handlers.RemoveAt(i);
				}
			}
		}
	}
}
