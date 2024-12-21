using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class UdpClientApp
{
    public static async Task Start(string address, int port, CancellationToken token)
    {
        Console.WriteLine("UDP Client started. Type messages to send.");

        using var udpClient = new UdpClient();
        var serverEndPoint = new IPEndPoint(IPAddress.Parse(address), port);

        try
        {
            while (!token.IsCancellationRequested)
            {
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await udpClient.SendAsync(messageBytes, messageBytes.Length, serverEndPoint);
                }
            }
        }
        catch (Exception ex)
        {
            LogError("UDP Client error: " + ex.Message);
        }
    }

    private static void LogError(string message)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        File.AppendAllText("log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
    }
}