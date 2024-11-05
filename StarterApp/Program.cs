using System;
using System.Diagnostics;

namespace BlazorAppLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:5000";

            StartBlazorApp();
            OpenInBrowser(url);
        }

        static void StartBlazorApp()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.Combine(currentDirectory, "publish/GUI.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                CreateNoWindow = true
            };

            try
            {
                Process.Start(startInfo);
                Console.WriteLine("Blazor app started...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting Blazor app: {ex.Message}");
            }
        }

        static void OpenInBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                Console.WriteLine($"Opening Blazor app at {url}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening browser: {ex.Message}");
            }
        }
    }
}
