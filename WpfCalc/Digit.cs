namespace WpfCalc;

using System;
using System.Windows.Markup;

[MarkupExtensionReturnType(typeof(long))]
sealed class Digit : MarkupExtension
{
	private readonly string _digit;

	public Digit(string digit)
	{
		_digit = digit;
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		if(_digit is null) return 0;
		if(_digit.Length == 0) return 0;
		var c = _digit[0];
		if(char.IsNumber(c)) return (long)(c - '0');
		return char.ToUpperInvariant(c) switch
		{
			'A' => (long)10,
			'B' => (long)11,
			'C' => (long)12,
			'D' => (long)13,
			'E' => (long)14,
			'F' => (long)15,
			_   => (object)0,
		};
	}
}
