using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using IslamicDesktop.ViewModels;
using IslamicDesktop.Views;

namespace IslamicDesktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;

#if MACOS
            // On macOS, when the app is activated (dock clicked), show the window if hidden
            desktop.Startup += (_, _) =>
            {
                AppDomain.CurrentDomain.ProcessExit += (_, _) => { /* optional cleanup */ };
            };

            desktop.Exit += (_, _) => { /* optional cleanup */ };

            // Avalonia macOS: handle activated events
            desktop.ApplicationLifetimeActivated += (_, _) =>
            {
                if (!mainWindow.IsVisible)
                    mainWindow.Show();
            };
#endif
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
