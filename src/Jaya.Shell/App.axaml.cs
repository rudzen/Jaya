using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using Jaya.Shell.ViewModels;
using Jaya.Shell.Views;

namespace Jaya.Shell
{
    public class App : Application
    {
        const string DSGN_THEME = "--designer-theme=";
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            string[] args = Environment.GetCommandLineArgs();
            string dsgnThemeArg = args.FirstOrDefault(x => x.StartsWith(DSGN_THEME));
            if (!(string.IsNullOrEmpty(dsgnThemeArg) || string.IsNullOrWhiteSpace(dsgnThemeArg)))
            {
                string themeName = dsgnThemeArg.Substring(DSGN_THEME.Length);
                if (themeName == "Light")
                {
                    (Styles[0] as FluentTheme).Mode = FluentThemeMode.Light;
                    var include = Styles[2] as StyleInclude;
                    Styles.Remove(include);
                    include = new StyleInclude(new Uri("avares://Jaya.Shell"))
                    {
                        Source = new Uri(
                                $"avares://Jaya.Shell/Styles/Fluent/ColorSchemes/Fluent{themeName}.axaml" //include.Source.AbsolutePath.Replace("Dark", themeName)
                                //, UriKind.RelativeOrAbsolute)
                            )
                    };
                    Styles.Insert(2, include);
                }
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindowView
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
