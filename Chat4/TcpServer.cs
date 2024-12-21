using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class TcpServer
{
    private static readonly List<TcpClient> connectedClients = new List<TcpClient>();

    public static async Task Start(string address, int port, CancellationToken token)
    {
        var listener = new TcpListener(IPAddress.Parse(address), port);
        listener.Start();
        Console.WriteLine($"TCP Server is running on {address}:{port}");

        try
        {
            while (!token.IsCancellationRequested)
            {
                if (listener.Pending())
                {
                    var client = await listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClient(client, token));
                }
                await Task.Delay(100, token);
            }
        }
        catch (Exception ex)
        {
            LogError("TCP Server error: " + ex.Message);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task HandleClient(TcpClient client, CancellationToken token)
    {
        lock (connectedClients)
            connectedClients.Add(client);

        try
        {
            using var networkStream = client.GetStream();
            byte[] buffer = new byte[1024];
            while (!token.IsCancellationRequested && client.Connected)
            {
                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[TCP] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");

                    // Broadcast message to all clients
                    BroadcastMessage(message, token);
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Error handling TCP client: " + ex.Message);
        }
        finally
        {
            lock (connectedClients)
                connectedClients.Remove(client);
        }
    }

    private static void BroadcastMessage(string message, CancellationToken token)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        lock (connectedClients)
        {
            foreach (var client in connectedClients)
            {
                if (client.Connected)
                {
                    try
                    {
                        var stream = client.GetStream();
                        stream.WriteAsync(data, 0, data.Length, token);
                    }
                    catch
                    {
                        // Ignore errors when sending to client
                    }
                }
            }
        }
    }

    private static void LogError(string message)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        File.AppendAllText("log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
    }
}