namespace WpfCalc;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

/// <summary>Базовый класс модели представления.</summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
	/// <summary>Значение свойства изменено.</summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>Сообщить об изменении значения свойства.</summary>
	/// <param name="propertyName">Имя свойства.</param>
	protected void OnPropertyChanged(string? propertyName)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	/// <summary>Сообщить об изменении значения свойства.</summary>
	/// <param name="args">Аргументы события <see cref="M:PropertyChanged"/>.</param>
	protected void OnPropertyChanged(PropertyChangedEventArgs args)
		=> PropertyChanged?.Invoke(this, args);

	/// <summary>Сообщить об изменении значения свойства.</summary>
	/// <typeparam name="T">Тип свойства.</typeparam>
	/// <param name="exp">Функция, возвращающая свойство.</param>
	protected void OnPropertyChanged<T>(Expression<Func<T>> exp)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(((MemberExpression)exp.Body).Member.Name));

	private static bool AreEqual<T>(T? value1, T? value2)
		=> EqualityComparer<T>.Default.Equals(value1, value2);

	/// <summary>Обновить значение свойства.</summary>
	/// <typeparam name="T">Тип свойства.</typeparam>
	/// <param name="oldValue">Ссылка на поле, хранящее значение свойства.</param>
	/// <param name="newValue">Новое значение параметра.</param>
	/// <param name="property">Функция, возвращающая свойство.</param>
	/// <returns><c>true</c>, если свойство было изменено, иначе <c>false</c>.</returns>
	/// <example>
	/// <code>
	/// private bool _field;
	/// 
	/// public bool Property
	/// {
	///		get { return _field; }
	///		set { UpdatePropertyValue(ref _field, value, () => Property); }
	/// }
	/// </code>
	/// </example>
	protected bool UpdatePropertyValue<T>(ref T oldValue, T newValue, Expression<Func<T>> property)
	{
		if(!AreEqual(oldValue, newValue))
		{
			oldValue = newValue;
			var handler = PropertyChanged;
			if(handler is not null)
			{
				var propertyName = ((MemberExpression)property.Body).Member.Name;
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
			return true;
		}
		return false;
	}

	/// <summary>Обновить значение свойства.</summary>
	/// <typeparam name="T">Тип свойства.</typeparam>
	/// <param name="oldValue">Ссылка на поле, хранящее значение свойства.</param>
	/// <param name="newValue">Новое значение параметра.</param>
	/// <param name="propertyName">Имя свойства.</param>
	/// <returns><c>true</c>, если свойство было изменено, иначе <c>false</c>.</returns>
	/// <example>
	/// <code>
	/// private bool _field;
	/// 
	/// public bool Property
	/// {
	///		get { return _field; }
	///		set { UpdatePropertyValue(ref _field, value, "Property"); }
	/// }
	/// </code>
	/// </example>
	protected bool UpdatePropertyValue<T>(ref T oldValue, T newValue, string propertyName)
	{
		if(!AreEqual(oldValue, newValue))
		{
			oldValue = newValue;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
		return false;
	}
}
