using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class TcpClientApp
{
    public static async Task Start(string address, int port, CancellationToken token)
    {
        Console.WriteLine("TCP Client started. Type messages to send.");

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(address, port);

        try
        {
            using var stream = tcpClient.GetStream();
            var readMessagesTask = Task.Run(async () =>
            {
                byte[] buffer = new byte[1024];
                while (!token.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead > 0)
                    {
                        string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"[SERVER] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {serverResponse}");
                    }
                }
            }, token);

            while (!token.IsCancellationRequested)
            {
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(messageBytes, 0, messageBytes.Length, token);
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Client error: " + ex.Message);
        }
    }

    private static void LogError(string message)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        File.AppendAllText("log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
    }
}