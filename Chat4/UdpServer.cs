using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class UdpServer
{
    public static async Task Start(string address, int port, CancellationToken token)
    {
        var udpListener = new UdpClient(port);
        Console.WriteLine($"UDP Server is running on {address}:{port}");

        try
        {
            while (!token.IsCancellationRequested)
            {
                var receivedData = await udpListener.ReceiveAsync(token);
                string message = Encoding.UTF8.GetString(receivedData.Buffer);
                Console.WriteLine($"[UDP] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }
        catch (Exception ex)
        {
            LogError("UDP Server error: " + ex.Message);
        }
        finally
        {
            udpListener.Close();
        }
    }

    private static void LogError(string message)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        File.AppendAllText("log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
    }
}