using System.Windows.Input;

namespace BeamCalculator.Components;

public class TitleViewButton
{
    public string ImageSource { get; set; }
    public ICommand Command { get; set; }
}
