
using MView.Core;
using MView.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the splash screen and set it as the application main window.
            var splashScreen = new SplashWindow();
            this.MainWindow = splashScreen;
            splashScreen.Show();

            // In order to ensure the UI stays responsive, we need to do the work on a different thread.
            Task.Factory.StartNew(() =>
            {
                // Load settings
                string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Settings.SettingsPath);

                if (File.Exists(settingsPath))
                {
                    string settingsJson = FileManager.ReadTextFile(settingsPath, Encoding.UTF8);
                    Settings.Instance = JsonSerializer.Deserialize<Settings>(settingsJson);
                }

                // Load history
                string historyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Settings.HistoryPath);

                if (File.Exists(settingsPath))
                {
                    string historyJson = FileManager.ReadTextFile(historyPath, Encoding.UTF8);
                    Settings.Instance = JsonSerializer.Deserialize<Settings>(historyJson);
                }

                // Since we're not on the UI thread once we're done we need to use the Dispatcher to create and show the main window.
                this.Dispatcher.Invoke(() =>
                {
                    // Initialize the main window, set it as the application main window and close the splash screen.
                    var mainWindow = new MainWindow();
                    this.MainWindow = mainWindow;
                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // In order to ensure the UI stays responsive, we need to do the work on a different thread.
            Task.Factory.StartNew(() =>
            {
                // Save settings
                string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Settings.SettingsPath);

                if (!Directory.Exists(Path.GetDirectoryName(settingsPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
                }

                string settingsJson = JsonSerializer.Serialize(Settings.Instance);
                FileManager.WriteTextFile(settingsPath, settingsJson, Encoding.UTF8);

                // Save history
                string historyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Settings.HistoryPath);

                if (!Directory.Exists(Path.GetDirectoryName(historyPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(historyPath));
                }

                string historyJson = JsonSerializer.Serialize(Settings.Instance);
                FileManager.WriteTextFile(historyPath, historyJson, Encoding.UTF8);
            });
        }
    }
}
