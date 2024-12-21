using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static CancellationTokenSource cancellationSource = new CancellationTokenSource();

    static void Main(string[] args)
    {
        var (serverAddress, serverPort) = Config.LoadConfiguration();

        Console.WriteLine("Starting application. Press Ctrl+C to exit.");


        Console.WriteLine("Select protocol: (1) TCP (2) UDP");
        string? protocolChoice = Console.ReadLine();

        if (protocolChoice == "1")
        {

            Task.Run(() => TcpServer.Start(serverAddress, serverPort, cancellationSource.Token));
            Task.Run(() => TcpClientApp.Start(serverAddress, serverPort, cancellationSource.Token));
        }
        else if (protocolChoice == "2")
        {
            // Запускаем UDP сервер и клиент
            Task.Run(() => UdpServer.Start(serverAddress, serverPort, cancellationSource.Token));
            Task.Run(() => UdpClientApp.Start(serverAddress, serverPort, cancellationSource.Token));
        }
        else
        {
            Console.WriteLine("Invalid choice, defaulting to TCP.");
            Task.Run(() => TcpServer.Start(serverAddress, serverPort, cancellationSource.Token));
            Task.Run(() => TcpClientApp.Start(serverAddress, serverPort, cancellationSource.Token));
        }


        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cancellationSource.Cancel();
        };

  
        cancellationSource.Token.WaitHandle.WaitOne();
        Console.WriteLine("Application terminated.");
    }
}