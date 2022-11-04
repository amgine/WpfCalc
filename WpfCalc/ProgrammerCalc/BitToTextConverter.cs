namespace WpfCalc;

using System;
using System.Globalization;
using System.Windows.Data;

[ValueConversion(typeof(bool), typeof(string))]
public sealed class BitToTextConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		=> ((bool)value) ? "1" : "0";

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		=> (string)value == "1";
}
