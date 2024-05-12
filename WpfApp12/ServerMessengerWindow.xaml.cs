using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LocalNetworkMessenger
{
    public partial class ServerMessengerWindow : Window
    {
        private Messenger messenger;

        public ServerMessengerWindow()
        {
            InitializeComponent();
            messenger = new Messenger();
            messenger.MessageReceived += OnMessageReceived;
            messenger.ClientConnected += OnClientConnected;
            messenger.ClientDisconnected += OnClientDisconnected;

            // Запускаем сервер на заданном порту
            int port = 8888; // Пример порта
            Task.Run(() => messenger.StartServerAsync(port));
        }

        public ServerMessengerWindow(Messenger messenger) : this()
        {
            this.messenger = messenger;
        }
        private void OnMessageReceived(string userName, string message)
        {
            Dispatcher.Invoke(() =>
            {
                AddMessageToChat($"{userName}: {message}");
            });
        }

        private void OnClientConnected(string userName)
        {
            Dispatcher.Invoke(() =>
            {
                AddMessageToChat($"Пользователь {userName} подключился.");
                UpdateUserList();
            });
        }

        private void OnClientDisconnected(string userName)
        {
            Dispatcher.Invoke(() =>
            {
                AddMessageToChat($"Пользователь {userName} отключился.");
                UpdateUserList();
            });
        }

        private void AddMessageToChat(string message)
        {
            ChatPanel.Children.Add(new TextBlock { Text = message });
        }

        private void UpdateUserList()
        {
            UserList.ItemsSource = messenger.Users;
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                messenger.BroadcastMessage(message);
                AddMessageToChat($"Вы: {message}");
                MessageTextBox.Clear();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            messenger.StopServer();
            // Возвращаемся к окну выбора
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
