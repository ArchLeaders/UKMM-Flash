using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UkmmFlash.Helpers;
using UkmmFlash.Models;
using UkmmFlash.Views;

namespace UkmmFlash.ViewModels;
public class ShellViewModel : ReactiveObject
{
    public static ShellViewModel Shared { get; } = new();

    private string _name = Path.GetFileName(Directory.GetCurrentDirectory());
    public string Name {
        get => _name;
        set {
            ModPath = Path.Combine(App.Config.ModsPath, value.Replace(' ', '-'));
            this.RaiseAndSetIfChanged(ref _name, value);
        }
    }

    private string _modPath = Directory.GetCurrentDirectory();
    public string ModPath {
        get => _modPath;
        set => this.RaiseAndSetIfChanged(ref _modPath, value);
    }

    private ObservableCollection<Rule> _rules = new();
    public ObservableCollection<Rule> Rules {
        get => _rules;
        set => this.RaiseAndSetIfChanged(ref _rules, value);
    }

    private Rule? _selectedRule;
    public Rule? SelectedRule {
        get => _selectedRule;
        set => this.RaiseAndSetIfChanged(ref _selectedRule, value);
    }

    public async Task CreateMod()
    {
        ContentDialog dlg = new() {
            Content = new TextBox {
                Watermark = "Mod Name"
            },
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "OK",
            SecondaryButtonText = "Cancel",
            Title = "Create Mod"
        };

        ContentDialogResult result = await dlg.ShowAsync();
        if (result == ContentDialogResult.Primary && dlg.Content is TextBox tb && !string.IsNullOrEmpty(tb.Text)) {
            Name = tb.Text;

            Directory.CreateDirectory(Path.Combine(ModPath, "assets"));
            Directory.CreateDirectory(Path.Combine(ModPath, "build"));
            Directory.CreateDirectory(Path.Combine(ModPath, "scripts"));

            Meta.GetMetaFile(ModPath, Name, Platform.WiiU);
        }
    }

    public async Task OpenMod()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFolder, "Open Mod Root Folder", instanceBrowserKey: "OpenModDialog");
        if (await dialog.ShowDialog() is string path && Directory.Exists(path)) {
            Name = Path.GetFileName(path);
        }
    }

    private bool _isDecompiled = false;
    public async Task Decompile()
    {
        if (!_isDecompiled) {
            SetStatus("Decompiling");
            _isDecompiled = true;

            foreach (var rule in Rules) {
                await Task.Run(() => rule.Decompile(ModPath));
            }

            SetStatus("Ready");
        }
    }

    public async Task Compile()
    {
        if (_isDecompiled) {
            SetStatus("Compiling");
            _isDecompiled = false;

            foreach (var rule in Rules.Reverse()) {
                await Task.Run(() => rule.Compile(ModPath));
            }

            SetStatus("Ready");
        }
    }

    public async Task Deploy()
    {
        SetStatus("Setting Game Mode");
        Meta.SetNextVersion(Meta.GetInfoFile(ModPath, Name, Platform.WiiU));

        await Process.Start(App.Config.UkmmPath, $"""
            mode "Wii U"
            """).WaitForExitAsync();

        SetStatus("Uninstalling Old Version");
        await Process.Start(App.Config.UkmmPath, $"""
            uninstall 0
            """).WaitForExitAsync();

        SetStatus("Installing & Deploying");
        await Process.Start(App.Config.UkmmPath, $"""
            install "{Path.Combine(ModPath, "build")}" "{Name}" --deploy
            """).WaitForExitAsync();

        SetStatus("Ready");
    }

    public async Task Package()
    {
        SetStatus("Updating Version");
        string meta = Meta.GetMetaFile(ModPath, Name, Platform.WiiU);
        string verion = Meta.SetNextVersion(meta);

        ContentDialog dlg = new() {
            Content = new TextBox {
                Watermark = "Version"
            },
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "OK",
            SecondaryButtonText = "Cancel",
            Title = "Package Version"
        };

        if (await dlg.ShowAsync() == ContentDialogResult.Primary) {
            SetStatus("Packaging");
            await Process.Start(App.Config.UkmmPath, $"""
                package "{Path.Combine(ModPath, "build")}" "{Path.Combine(ModPath, $"{Name}-{verion}.zip")}" "{meta}"
                """).WaitForExitAsync();
        }


        SetStatus("Ready");
    }

    public async Task FetchFile()
    {
        ContentDialog dlg = new() {
            Content = new TextBox {
                Watermark = "Search Pattern or Path"
            },
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "Fetch",
            SecondaryButtonText = "Cancel",
            Title = "Fetch Files"
        };

        ContentDialogResult result = await dlg.ShowAsync();
        if (result == ContentDialogResult.Primary && dlg.Content is TextBox tb && !string.IsNullOrEmpty(tb.Text)) {
            var fetchResult = GameFiles.Fetch(tb.Text);
            ContentDialog verifyDlg = new() {
                Content = new ScrollViewer {
                    MaxHeight = 150,
                    Content = new TextBlock {
                        Text = "Found Files:\n" + string.Join('\n', fetchResult.Select(x => string.Join('\n', x.Value))),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    }
                },
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = "Continue",
                SecondaryButtonText = "Cancel",
                Title = "Fetch Files Result"
            };

            if (await verifyDlg.ShowAsync() == ContentDialogResult.Primary) {
                await fetchResult.CopyInto(Path.Combine(ModPath, "build"));
            }
        }
    }

    public static async Task OpenSettings()
    {
        ContentDialog dlg = new() {
            Content = new ScrollViewer {
                MaxHeight = 250,
                Content = new StackPanel {
                    Margin = new(0, 0, 15, 0),
                    Spacing = 10,
                    Children = {
                        new TextBox {
                            Text = App.Config.UkmmPath,
                            Watermark = "UKMM Path",
                            UseFloatingWatermark = true,
                        },
                        new TextBox {
                            Text = App.Config.ModsPath,
                            Watermark = "Mods Path",
                            UseFloatingWatermark = true,
                        },
                        new TextBox {
                            Text = App.Config.Game.GamePath,
                            Watermark = "Game Path",
                            UseFloatingWatermark = true,
                        },
                        new TextBox {
                            Text = App.Config.Game.UpdatePath,
                            Watermark = "Update Path",
                            UseFloatingWatermark = true,
                        },
                        new TextBox {
                            Text = App.Config.Game.DlcPath,
                            Watermark = "DLC Path (optioanl)",
                            UseFloatingWatermark = true,
                        },
                        new TextBox {
                            Text = App.Config.Game.GamePathNx,
                            Watermark = "Game Path NX",
                            UseFloatingWatermark = true,
                        },
                        new TextBox {
                            Text = App.Config.Game.DlcPathNx,
                            Watermark = "DLC Path NX (optional)",
                            UseFloatingWatermark = true,
                        },
                    }
                }
            },
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Cancel",
            Title = "Settings"
        };

        if (await dlg.ShowAsync() == ContentDialogResult.Primary) {
            var stack = dlg.Content as StackPanel;
            App.Config.UkmmPath = (stack!.Children[0] as TextBox)!.Text!;
            App.Config.ModsPath = (stack!.Children[1] as TextBox)!.Text!;

            App.Config.Game.GamePath = (stack!.Children[2] as TextBox)!.Text!;
            App.Config.Game.UpdatePath = (stack!.Children[3] as TextBox)!.Text!;
            App.Config.Game.DlcPath = (stack!.Children[4] as TextBox)!.Text!;

            App.Config.Game.GamePathNx = (stack!.Children[5] as TextBox)!.Text!;
            App.Config.Game.DlcPathNx = (stack!.Children[6] as TextBox)!.Text!;

            Shared.Name = Shared.Name; // reprocess Path
            App.Config.Save();
        }
    }

    public async Task NewRule()
    {
        ContentDialog dlg = new() {
            Content = new RuleEditor(),
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Cancel",
            Title = "New Rules"
        };

        if (await dlg.ShowAsync() == ContentDialogResult.Primary) {
            Rules.Add((Rule)((UserControl)dlg.Content).DataContext!);
        }
    }

    public async Task EditRule()
    {
        ContentDialog dlg = new() {
            Content = new RuleEditor(SelectedRule!),
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Cancel",
            Title = "Edit Rule"
        };

        if (SelectedRule != null && await dlg.ShowAsync() == ContentDialogResult.Primary) {
            Rules[Rules.IndexOf(SelectedRule)] = (Rule)((UserControl)dlg.Content).DataContext!;
        }
    }

    public async Task RemoveRule()
    {
        ContentDialog dlg = new() {
            Content = $"Are you sure you want to remove {SelectedRule?.Pattern}?",
            DefaultButton = ContentDialogButton.Secondary,
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            Title = "Remove Rule"
        };

        if (SelectedRule != null && await dlg.ShowAsync() == ContentDialogResult.Primary) {
            Rules.Remove(SelectedRule);
        }
    }

    public async Task ClearRules()
    {
        ContentDialog dlg = new() {
            Content = $"Are you sure you want to clear all rules?",
            DefaultButton = ContentDialogButton.Secondary,
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            Title = "Clear all Rules"
        };

        if (await dlg.ShowAsync() == ContentDialogResult.Primary) {
            Rules.Clear();
        }
    }

    public void MoveRuleUp()
    {
        if (SelectedRule != null) {
            int index = Rules.IndexOf(SelectedRule);
            if (index > 0) {
                (Rules[index - 1], Rules[index]) = (Rules[index], Rules[index - 1]);
                SelectedRule = Rules[index - 1];
            }
        }
    }

    public void MoveRuleDown()
    {
        if (SelectedRule != null) {
            int index = Rules.IndexOf(SelectedRule);
            if (Rules.Count - 1 > index) {
                (Rules[index + 1], Rules[index]) = (Rules[index], Rules[index + 1]);
                SelectedRule = Rules[index + 1];
            }
        }
    }

    private bool _isLoading = false;
    public bool IsLoading {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private string _status = "Ready";
    public string Status {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public void SetStatus(string status = "Ready", bool? isLoading = null)
    {
        IsLoading = isLoading ?? status != "Ready";
        Status = status;
    }
}
