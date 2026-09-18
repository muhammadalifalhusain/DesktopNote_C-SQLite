using DesktopNote.Views;
using Microsoft.UI.Xaml;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace DesktopNote
{
    public partial class App : Application
    {
        private const string StartupRegistryValueName = "DesktopNote";

        private const string StartupRegistryKeyPath =
            @"Software\Microsoft\Windows\CurrentVersion\Run";

        private Window? _window;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new DeckWindow();
            _window.Activate();

            EnsureStartupRegistryEntry();
        }

        private static void EnsureStartupRegistryEntry()
        {
            try
            {
                string? exePath =
                    Process.GetCurrentProcess().MainModule?.FileName;

                if (string.IsNullOrEmpty(exePath))
                    return;

                using var key =
                    Registry.CurrentUser.OpenSubKey(
                        StartupRegistryKeyPath,
                        writable: true);

                if (key is null)
                    return;

                object? existingValue =
                    key.GetValue(StartupRegistryValueName);

                string desiredValue =
                    $"\"{exePath}\"";

                if (existingValue is not string existingPath ||
                    !string.Equals(
                        existingPath,
                        desiredValue,
                        StringComparison.OrdinalIgnoreCase))
                {
                    key.SetValue(
                        StartupRegistryValueName,
                        desiredValue);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}