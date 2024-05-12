using System;
using System.Windows;
using System.Windows.Controls;
using LocalNetworkMessenger;

namespace LocalNetworkMessenger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartChatButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UserNameTextBox.Text))
            {
                string userName = UserNameTextBox.Text;
                Messenger messenger = new Messenger(userName);
                ServerMessengerWindow messengerWindow = new ServerMessengerWindow(messenger);
                messengerWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Please enter your name.");
            }
        }
       

    }
}
