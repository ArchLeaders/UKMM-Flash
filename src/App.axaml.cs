using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using UkmmFlash.ViewModels;
using UkmmFlash.Views;

namespace UkmmFlash;
public partial class App : Application
{
    public static AppConfig Config { get; } = AppConfig.Load();
    public static TopLevel? VisualRoot { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new ShellView {
                DataContext = ShellViewModel.Shared,
            };

            VisualRoot = desktop.MainWindow.GetVisualRoot() as TopLevel;
        }

        base.OnFrameworkInitializationCompleted();
    }
}