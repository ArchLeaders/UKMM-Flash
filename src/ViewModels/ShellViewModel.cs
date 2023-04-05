﻿using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System.Collections.ObjectModel;
using UkmmFlash.Helpers;
using UkmmFlash.Models;
using UkmmFlash.Models.RuleActions;
using UkmmFlash.Views;

namespace UkmmFlash.ViewModels;
public class ShellViewModel : ReactiveObject
{
    public static ShellViewModel Shared { get; } = new();

    private string _name = "The Legend of John";
    public string Name {
        get => _name;
        set {
            Path = System.IO.Path.Combine(App.Config.ModsPath, value.Replace(' ', '-'));
            this.RaiseAndSetIfChanged(ref _name, value);
        }
    }

    private string _path = string.Empty;
    public string Path {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public ShellViewModel()
    {
        _path = System.IO.Path.Combine(App.Config.ModsPath, _name.Replace(' ', '-'));
    }


    private ObservableCollection<Rule> _rules = new() {
        Rule.Create<SarcAction>("**/*.pack"),
        Rule.Create<SarcAction>("**/*.sbactorpack"),
        Rule.Create<BymlAction>("/build/content/Actor/ActorInfo.product.sbyml"),
        Rule.Create<AampAction>("**/*.bxml"),
    };
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
        }
    }

    public async Task OpenMod()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFolder, "Open Mod Root Folder", instanceBrowserKey: "OpenModDialog");
        if (await dialog.ShowDialog() is string path) {
            Path = path;
        }
    }

    private bool _isDecompiled = false;
    public async Task Decompile()
    {
        if (!_isDecompiled) {
            SetStatus("Decompiling");
            _isDecompiled = true;

            foreach (var rule in Rules) {
                await rule.Decompile(Path);
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
                await rule.Compile(Path);
            }

            SetStatus("Ready");
        }
    }

    public async Task Deploy()
    {
        SetStatus("Deploying");

        await Task.Delay(3000);
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
            Title = "Fetch File"
        };

        ContentDialogResult result = await dlg.ShowAsync();
        if (result == ContentDialogResult.Primary && dlg.Content is TextBox tb && !string.IsNullOrEmpty(tb.Text)) {
            // fetch tb.Text
        }
    }

    public static async Task OpenSettings()
    {
        ContentDialog dlg = new() {
            Content = new StackPanel {
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
            Shared.Name = Shared.Name; // reprocess Path
            await App.Config.Save();
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
            SelectedRule.Pattern = "*";
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