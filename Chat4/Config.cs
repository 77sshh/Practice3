using System;
using System.IO;

public static class Config
{
    private static readonly string configFileName = "config.txt";

    public static (string, int) LoadConfiguration()
    {
        try
        {
            if (File.Exists(configFileName))
            {
                var lines = File.ReadAllLines(configFileName);
                return (lines[0], int.Parse(lines[1]));
            }
        }
        catch (Exception ex)
        {
            LogError("Failed to load configuration: " + ex.Message);
        }

        return ("127.0.0.1", 5000); // Default values
    }

    private static void LogError(string message)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        File.AppendAllText("log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
    }
}
