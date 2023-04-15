using Avalonia.Controls;
using UkmmFlash.Models;
using UkmmFlash.Models.RuleActions;

namespace UkmmFlash.Views;
public partial class RuleEditor : UserControl
{
    public RuleEditor(Rule? rule = null)
    {
        InitializeComponent();
        DataContext = new Rule(rule?.Pattern ?? string.Empty, rule?.Action ?? Rule.Library.ElementAt(0));
    }
}
