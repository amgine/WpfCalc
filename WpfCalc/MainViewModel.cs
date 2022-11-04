namespace WpfCalc;

using System;
using System.Windows.Input;

sealed class MainViewModel : ViewModelBase
{
	private CalculatorViewModel? _currentView;
	private CalculatorType _calculatorType;

	public MainViewModel()
	{
		CalculatorType = CalculatorType.Programmer;
		SetCalculatorTypeCommand = new DelegateCommand<CalculatorType>(
			type => CalculatorType = type);
	}

	public CalculatorViewModel? CurrentView
		=> _currentView ??= CreateCalcViewModel(_calculatorType);

	public ICommand SetCalculatorTypeCommand { get; }

	public CalculatorType CalculatorType
	{
		get => _calculatorType;
		set
		{
			if(UpdatePropertyValue(ref _calculatorType, value, nameof(CalculatorType)))
			{
				_currentView = CreateCalcViewModel(value);
				OnPropertyChanged(nameof(CurrentView));
			}
		}
	}

	private static CalculatorViewModel? CreateCalcViewModel(CalculatorType calculatorType)
		=> calculatorType switch
		{
			CalculatorType.Normal     => new NormalCalcViewModel(),
			CalculatorType.Programmer => new ProgrammerCalcViewModel(),
			CalculatorType.Engineer   => new EngineerCalcViewModel(),
			CalculatorType.Statistics => new StatisticsCalcViewModel(),
			_ => default,
		};
}
