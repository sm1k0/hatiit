using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalNetworkMessenger
{
    public class TcpServer
    {
        private TcpListener server;
        private CancellationTokenSource cancellationTokenSource;

        public event Action<string> ClientConnected;
        public event Action<string> ClientDisconnected;
        public event Action<string, string> MessageReceived;
        private List<TcpClient> clients = new List<TcpClient>();

        public TcpServer()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartListeningAsync(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    HandleClient(client);
                }
            }
            catch (OperationCanceledException)
            {
                // Task was canceled
            }
            finally
            {
                server.Stop();
            }
        }

        private async void HandleClient(TcpClient client)
        {
            string userName = null;

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;
                clients.Add(client);

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (message.StartsWith("JOIN:"))
                    {
                        userName = message.Substring(5);
                        ClientConnected?.Invoke(userName);
                    }
                    else if (message.StartsWith("LEAVE:"))
                    {
                        ClientDisconnected?.Invoke(userName);
                        break;
                    }
                    else
                    {
                        MessageReceived?.Invoke(userName, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
                if (userName != null)
                {
                    ClientDisconnected?.Invoke(userName);
                }
            }
        }

        public void StopListening()
        {
            cancellationTokenSource.Cancel();
        }
        public virtual void BroadcastMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            // Отправляем сообщение каждому клиенту
            foreach (TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
