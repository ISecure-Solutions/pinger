using System.Net.Sockets;

namespace Pinger;

public class Pinger
{
    private string Protocol { get; set; }
    private string Host { get; set; }
    private int Port { get; set; }

    public Pinger(string protocol, string host, int port)
    {
        Protocol = protocol;
        Host = host;
        Port = port;
    }

    private NetworkError InternalConnect(string protocol, string host, int port)
    {
        try
        {
            HttpClient client = new HttpClient();

            client.GetAsync($"{protocol}://{host}:{port}").Wait();

            return NetworkError.Success;
        }
        catch (AggregateException e)
        {
            SocketException ex;

            try
            {
                ex = (SocketException)e.InnerException!.InnerException!;
            }
            catch
            {
                return NetworkError.Unspecified;
            }

            Console.WriteLine($"Exceção da conexão: {ex.Message}\nStatus: {ex.ErrorCode}");

            return ex.ErrorCode == (int)SocketError.AccessDenied ?
                NetworkError.AccessDenied : NetworkError.Unspecified;
        }
    }

    public NetworkError TestConnection()
    {
        return InternalConnect(Protocol, Host, Port);
    }

    public NetworkError TestConnection(string protocol, string host, int port)
    {
        return InternalConnect(protocol, host, port);
    }
}