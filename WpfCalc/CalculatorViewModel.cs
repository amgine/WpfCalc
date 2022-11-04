using System.Media;
using System;
using System.Windows.Input;

namespace WpfCalc;

public abstract class CalculatorViewModel : ViewModelBase
{
	protected static ICommand DisabledCommand { get; } = new DelegateCommand(
		() => { }, () => false, isAutomaticRequeryDisabled: true);

	protected static readonly char[] Alphabet =
		new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

	protected static void Beep()
	{
		if(OperatingSystem.IsWindows())
		{
			SystemSounds.Beep.Play();
		}
	}
}
