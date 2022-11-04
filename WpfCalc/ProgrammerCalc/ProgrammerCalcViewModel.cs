namespace WpfCalc;

using System;
using System.Collections.Generic;
using System.Windows.Input;

public sealed class ProgrammerCalcViewModel : CalculatorViewModel
{
	private readonly BitsViewModel _bits;
	private long _operand;
	private long _number;
	private bool _newSequence;
	private NumberNotation _numberNotation;
	private Operation? _operation;

	public ProgrammerCalcViewModel()
	{
		_bits = new BitsViewModel();
		_bits.BitChanged += OnBitChanged;
		_numberNotation = NumberNotation.Dec;

		AppendDigitCommand = new DelegateCommand<long>(
			digit => AppendDigitInCurrentNotation(digit),
			digit => digit < GetRadix(_numberNotation));
		EraseLastDigitCommand = new DelegateCommand(EraseLastDigit);
		ChangeSignCommand     = new DelegateCommand(ChangeSign);
		ResetCommand          = new DelegateCommand(Reset);
		EqualsCommand         = new DelegateCommand(() => ExecuteQueuedOperation());
		AddCommand            = new DelegateCommand(() => QueueBinaryOperation(Operation.Add));
		SubtractCommand       = new DelegateCommand(() => QueueBinaryOperation(Operation.Subtract));
		MultiplyCommand       = new DelegateCommand(() => QueueBinaryOperation(Operation.Multiply));
		DivideCommand         = new DelegateCommand(() => QueueBinaryOperation(Operation.Divide));
		AndCommand            = new DelegateCommand(() => QueueBinaryOperation(Operation.Add));
		OrCommand             = new DelegateCommand(() => QueueBinaryOperation(Operation.Or));
		XorCommand            = new DelegateCommand(() => QueueBinaryOperation(Operation.Xor));
		LshCommand            = new DelegateCommand(() => QueueBinaryOperation(Operation.Lsh));
		RshCommand            = new DelegateCommand(() => QueueBinaryOperation(Operation.Rsh));
		ModCommand            = new DelegateCommand(() => QueueBinaryOperation(Operation.Modulo));
	}

	public ICommand AppendDigitCommand { get; }

	public ICommand EraseLastDigitCommand { get; }

	public ICommand ChangeSignCommand { get; }

	public ICommand ResetCommand { get; }

	public ICommand ResetCurrentNumberCommand => ResetCommand;

	public ICommand EqualsCommand { get; }

	public ICommand AddCommand { get; }

	public ICommand SubtractCommand { get; }

	public ICommand MultiplyCommand { get; }

	public ICommand DivideCommand { get; }

	public ICommand AndCommand { get; }

	public ICommand OrCommand { get; }

	public ICommand XorCommand { get; }

	public ICommand RshCommand { get; }

	public ICommand LshCommand { get; }

	public ICommand ModCommand { get; }

	#region Unsupported Commands

	public ICommand AppendCommaCommand => DisabledCommand;

	public ICommand ReciprocCommand => DisabledCommand;

	public ICommand SqrtCommand => DisabledCommand;

	public ICommand PercentCommand => DisabledCommand;

	#endregion

	private void OnBitChanged(object? sender, EventArgs e)
		=> UpdateCurrentNumber(_bits.Number);

	public bool IsBinNumberNotation
	{
		get => _numberNotation == NumberNotation.Bin;
		set { if(value) NumberNotation = NumberNotation.Bin; }
	}

	public bool IsOctNumberNotation
	{
		get => _numberNotation == NumberNotation.Oct;
		set { if(value) NumberNotation = NumberNotation.Oct; }
	}

	public bool IsDecNumberNotation
	{
		get => _numberNotation == NumberNotation.Dec;
		set { if(value) NumberNotation = NumberNotation.Dec; }
	}

	public bool IsHexNumberNotation
	{
		get => _numberNotation == NumberNotation.Hex;
		set { if(value) NumberNotation = NumberNotation.Hex; }
	}

	public bool Is8BytesMode
	{
		get => true;
		set { }
	}

	public bool Is4BytesMode
	{
		get => false;
		set { }
	}

	public bool Is2BytesMode
	{
		get => false;
		set { }
	}

	public bool Is1ByteMode
	{
		get => false;
		set { }
	}

	public BitsViewModel Bits => _bits;

	public long CurrentNumber
	{
		get => _number;
		set => UpdateCurrentNumber(value);
	}

	public NumberNotation NumberNotation
	{
		get => _numberNotation;
		set
		{
			if(UpdatePropertyValue(ref _numberNotation, value, () => NumberNotation))
			{
				OnPropertyChanged(nameof(CurrentNumberAsString));
			}
		}
	}

	private void Reset()
	{
		_operand   = default;
		_operation = default;
		UpdateCurrentNumber(0);
	}

	private void UpdateCurrentNumber(long newNumber)
	{
		if(_number == newNumber) return;

		_number = newNumber;
		_bits.Number = _number;
		OnPropertyChanged(nameof(Bits));
		OnPropertyChanged(nameof(CurrentNumber));
		OnPropertyChanged(nameof(CurrentNumberAsString));
	}

	private static long GetRadix(NumberNotation notation)
		=> notation switch
		{
			NumberNotation.Bin => 2,
			NumberNotation.Oct => 8,
			NumberNotation.Dec => 10,
			NumberNotation.Hex => (long)16,
			_ => throw new ApplicationException("Unknown number notation."),
		};

	private static ulong GetDigitMask(NumberNotation notation)
		=> notation switch
		{
			NumberNotation.Bin => 1,
			NumberNotation.Oct => 0x07,
			NumberNotation.Dec => throw new NotSupportedException(),
			NumberNotation.Hex => 0x0f,
			_ => throw new ApplicationException("Unknown number notation."),
		};

	private static int GetDigitOffset(NumberNotation notation)
		=> notation switch
		{
			NumberNotation.Bin => 1,
			NumberNotation.Oct => 3,
			NumberNotation.Dec => throw new NotSupportedException(),
			NumberNotation.Hex => 4,
			_ => throw new ApplicationException("Unknown number notation."),
		};

	private void QueueBinaryOperation(Operation operation)
	{
		if(_operation.HasValue)
		{
			if(!ExecuteQueuedOperation()) return;
		}
		_operand     = _number;
		_operation   = operation;
		_newSequence = true;
	}

	private void AppendDigitInCurrentNotation(long digit)
	{
		var number = _number;
		if(_newSequence)
		{
			number = 0;
			_newSequence = false;
		}
		long newNumber = 0;
		bool overflow = false;
		try
		{
			checked
			{
				newNumber = number * GetRadix(_numberNotation) + digit;
			}
		}
		catch(OverflowException)
		{
			overflow = true;
			Beep();
		}
		if(!overflow)
		{
			UpdateCurrentNumber(newNumber);
		}
	}

	private void EraseLastDigit()
	{
		if(_newSequence)
		{
			UpdateCurrentNumber(0);
			_newSequence = false;
			return;
		}
		UpdateCurrentNumber(_number / GetRadix(_numberNotation));
	}

	private void ChangeSign()
	{
		if(_number == long.MinValue) return;
		UpdateCurrentNumber(-_number);
	}

	private bool ExecuteBinary(long a, long b, Operation operation)
	{
		checked
		{
			long result;
			try
			{
				result = operation switch
					{
						Operation.Add      => a + b,
						Operation.Subtract => a - b,
						Operation.Multiply => a * b,
						Operation.Divide   => a / b,
						Operation.Modulo   => a % b,
						Operation.And      => a & b,
						Operation.Or       => a | b,
						Operation.Xor      => a ^ b,
						Operation.Lsh      => a << (int)b,
						Operation.Rsh      => a >> (int)b,
						_ => throw new ArgumentException($"Unknown binary operation: {operation}", nameof(operation)),
					};
			}
			catch(OverflowException)
			{
				Beep();
				return false;
			}
			UpdateCurrentNumber(result);
		}
		return true;
	}

	private bool ExecuteQueuedOperation()
	{
		if(!_operation.HasValue) return false;
		switch(_operation.Value)
		{
			case Operation.Add:
			case Operation.Subtract:
			case Operation.Multiply:
			case Operation.Divide:
			case Operation.Modulo:
			case Operation.And:
			case Operation.Or:
			case Operation.Xor:
			case Operation.Lsh:
			case Operation.Rsh:
				if(_newSequence) return false;
				if(!ExecuteBinary(_operand, _number, _operation.Value)) return false;
				break;
			default:
				break;
		}
		_operand     = default;
		_operation   = default;
		_newSequence = true;
		return true;
	}

	public string CurrentNumberAsString
	{
		get
		{
			var stack = new Stack<char>(64);
			if(_numberNotation == NumberNotation.Dec)
			{
				var sign = Math.Sign(_number);
				long number = _number;
				while(number != 0)
				{
					stack.Push(Alphabet[Math.Abs(number % 10)]);
					number /= 10;
				}
				if(sign == -1)
				{
					stack.Push('-');
				}
			}
			else
			{
				var mask = GetDigitMask(_numberNotation);
				var offset = GetDigitOffset(_numberNotation);
				ulong number = (ulong)_number;
				while(number != 0)
				{
					stack.Push(Alphabet[number & mask]);
					number >>= offset;
				}
			}
			if(stack.Count == 0)
			{
				return "0";
			}
			else
			{
				var chars = new char[stack.Count];
				for(int i = 0; i < chars.Length; ++i)
				{
					chars[i] = stack.Pop();
				}
				return new(chars);
			}
		}
	}
}
