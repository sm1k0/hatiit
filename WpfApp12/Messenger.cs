using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LocalNetworkMessenger
{
    public class Messenger
    {
        private TcpServer tcpServer;
        private ObservableCollection<string> users;
        public event Action<string> ClientConnected;
        public event Action<string> ClientDisconnected;
        public event Action<string, string> MessageReceived;
        private string userName;

        public Messenger(string userName)
        {
            this.userName = userName;
        }
        public ObservableCollection<string> Users
        {
            get { return users; }
            private set { users = value; }
        }

        public Messenger()
        {
            Users = new ObservableCollection<string>();
        }

        public async Task StartServerAsync(int port)
        {
            tcpServer = new TcpServer();
            tcpServer.ClientConnected += OnClientConnected;
            tcpServer.ClientDisconnected += OnClientDisconnected;
            tcpServer.MessageReceived += OnMessageReceived;

            await tcpServer.StartListeningAsync(port);
        }

        private void OnClientConnected(string userName)
        {
            Users.Add(userName);
            BroadcastMessage($"JOIN:{userName}");
        }

        private void OnClientDisconnected(string userName)
        {
            Users.Remove(userName);
            BroadcastMessage($"LEAVE:{userName}");
        }

        private void OnMessageReceived(string userName, string message)
        {
            BroadcastMessage(message);
        }

        public void BroadcastMessage(string message)
        {
            tcpServer.BroadcastMessage(message);
        }

        public void StopServer()
        {
            tcpServer?.StopListening();
        }
    }
}
