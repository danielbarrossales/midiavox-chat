using MidiavoxChat.Core;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MidiavoxChat.Pages
{
    /// <summary>
    /// Interação lógica para ConectPage.xam
    /// </summary>
    public partial class ConnectToPage : Page
    {
        public ConnectToPage()
        {
            InitializeComponent();
        }

        private void BackToStartingPage(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new StartingPage());
        }

        private void TryToConnectWithIp()
        {
            string ip = IpTextBox.Text;
            var task = Task.Run(async () =>
            {
                var webSocket = await WebSocketConnectionHandler.WaitIpToConnectAsync(ip, "8080");
                this.Dispatcher.Invoke(() =>
                {
                    this.ConnectionCallback(webSocket);
                });
            });
            
            var a = task.Status;
            
        }

        private void ConnectionCallback(WebSocket webSocket)
        {
            if (webSocket == null)
            {
                MessageBox.Show("Impossível se conectar com esse ip, tente outro");
                return;
            }

            this.NavigationService.Navigate(new ChatPage(webSocket));
        }
        private void OnConnectButtonClick(object sender, RoutedEventArgs e)
        {
            TryToConnectWithIp();
        }

        private void OnIpTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) TryToConnectWithIp();
        }
    }
}
