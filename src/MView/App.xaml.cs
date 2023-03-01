using Caliburn.Micro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MView.Utilities;
using MView.Utilities.Text;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeLogger();
            InitializeLocalization();

            ReadSettings(); // The theme is applied by the deep copy function.

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();

            WriteSettings();

            base.OnExit(e);
        }

        private void InitializeLogger()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, @"data\logs\log-.log");
            string outputTemplateString = "{Timestamp:HH:mm:ss.ms} [{Level}] {Message}{NewLine}{Exception}";

            var log = new LoggerConfiguration()
                .WriteTo.Async(a => a.File(fileName, restrictedToMinimumLevel: LogEventLevel.Verbose, outputTemplate: outputTemplateString, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100000))
                .CreateLogger();

            Log.Logger = log;

            Log.Information("The logger has been initialized.");
        }

        public static void InitializeLocalization()
        {
            ResourceDictionary dict = new ResourceDictionary();

            switch (Thread.CurrentThread.CurrentCulture.Name)
            {
                case "en-US":
                    dict.Source = new Uri(@"..\Assets\Localizations\Localization.en-US.xaml", UriKind.Relative);
                    break;
                case "ko-KR":
                    dict.Source = new Uri(@"..\Assets\Localizations\Localization.ko-KR.xaml", UriKind.Relative);
                    break;
                case "ja-JP":
                    dict.Source = new Uri(@"..\Assets\Localizations\Localization.ja-JP.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri(@"..\Assets\Localizations\Localization.en-US.xaml", UriKind.Relative);
                    break;
            }

            App.Current.Resources.MergedDictionaries.Add(dict);
        }

        public static void ReadSettings()
        {
            if (File.Exists(VariableBuilder.GetSettingsPath()))
            {
                string json = TextManager.ReadTextFile(VariableBuilder.GetSettingsPath(), Encoding.UTF8);

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var settings = JsonSerializer.Deserialize<Settings>(stream);
                    IoC.Get<Settings>().DeepCopyFrom(settings ?? new Settings());
                }
            }
        }

        public static void WriteSettings()
        {
            var settings = IoC.Get<Settings>();

            using (MemoryStream stream = new MemoryStream())
            {
                // Serialize the settings.
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.IgnoreReadOnlyFields = true;
                options.IgnoreReadOnlyProperties = true;

                JsonSerializer.Serialize(stream, settings, options);

                // Write a JSON string into the settings file.
                string json = Encoding.UTF8.GetString(stream.ToArray());
                TextManager.WriteTextFile(VariableBuilder.GetSettingsPath(), json, Encoding.UTF8);
            }
        }

        public static void ApplyTheme(bool useDarkTheme = false)
        {
            var paletteHelper = new PaletteHelper();

            ResourceDictionary brushes = new ResourceDictionary();

            if (useDarkTheme)
            {
                ITheme theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(Theme.Dark);
                theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.Blue]);
                theme.SetSecondaryColor(SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.Blue]);
                paletteHelper.SetTheme(theme);

                brushes.Source = new Uri(@"../Assets/Resources/Brushes.Dark.xaml", UriKind.Relative);
            }
            else
            {
                ITheme theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(Theme.Light);
                theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.Blue]);
                theme.SetSecondaryColor(SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.Blue]);
                paletteHelper.SetTheme(theme);

                brushes.Source = new Uri(@"../Assets/Resources/Brushes.Light.xaml", UriKind.Relative);
            }

            App.Current.Resources.MergedDictionaries.Add(brushes);

            // Re-register the icons dictionary to change icons' color.
            ResourceDictionary icons = new ResourceDictionary();
            icons.Source = new Uri(@"../Assets/Resources/Icons.xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(icons);
        }
    }
}
