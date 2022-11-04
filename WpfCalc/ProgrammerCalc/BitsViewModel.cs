namespace WpfCalc;

using System;

public sealed class BitsViewModel
{
	private long _number;

	public event EventHandler? BitChanged;

	public void Update(long number)
	{
		if(_number != number)
		{
			_number = number;
		}
	}

	public long Number
	{
		get => _number;
		set => _number = value;
	}

	public bool this[int bit]
	{
		get => ((_number >> bit) & 1) != 0;
		set
		{
			unchecked
			{
				long newNumber = value
					? _number |  ((long)1 << bit)
					: _number & ~((long)1 << bit);
				if(newNumber != _number)
				{
					_number = newNumber;
					BitChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}
	}
}
