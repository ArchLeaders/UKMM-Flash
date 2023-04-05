using Avalonia.Controls;
using Avalonia.Input;
using UkmmFlash.ViewModels;

namespace UkmmFlash.Views;
public partial class ShellView : Window
{
    public ShellView()
    {
        InitializeComponent();
    }

    public async void RuleList_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) {
            await ShellViewModel.Shared.EditRule();
        }
        else if (e.Key == Key.Delete) {
            await ShellViewModel.Shared.RemoveRule();
        }
    }

    public async void RuleItem_DoubleTapped(object sender, TappedEventArgs e)
    {
        await ShellViewModel.Shared.EditRule();
    }
}
