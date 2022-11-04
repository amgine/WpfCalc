namespace WpfCalc;

using System;
using System.Globalization;
using System.Windows.Data;

class EqualityToBooleanConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		=> Equals(value, parameter);

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		=> throw new NotSupportedException();
}
