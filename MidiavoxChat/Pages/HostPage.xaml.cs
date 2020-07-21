using MidiavoxChat.Core;
using MidiavoxChat.Core.Utils;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
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
    /// Interação lógica para HostPage.xam
    /// </summary>
    public partial class HostPage : Page
    {

        public delegate void ShowIpCallback(string message);
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public HostPage()
        {
            InitializeComponent();
            waitingForConnectionLabel.Content = "Waiting for connection on your local ip:";
            Task.Run(() =>
            {
                var ip = NetworkUtils.GetLocalIpAddress();
                this.Dispatcher.Invoke(() => {
                    waitingForConnectionLabel.Content = $"Waiting for connection on your local ip: {ip}";
                });
            });
            Task.Run( async () =>
            {
                var webSocket = await WebSocketConnectionHandler.ListenToWSConnectionAsync("8080");
                if (webSocket != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ConnectionStarted(webSocket);
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (!cancellationTokenSource.IsCancellationRequested)
                        {
                            MessageBox.Show("Erro ao tentar iniciar host, por favor tente abrir a aplicação como administrador.");
                        }
                        this.Cancel();
                        this.NavigationService.Navigate(new StartingPage());
                    });
                }
            }, cancellationTokenSource.Token);
        }

        private void ConnectionStarted(WebSocket webSocket)
        {
            this.NavigationService.Navigate(new ChatPage(webSocket));
        }

        private void BackToStartingPage(object sender, RoutedEventArgs e)
        {
            Cancel();
            this.NavigationService.Navigate(new StartingPage());
        }

        private void Cancel() 
        {
            cancellationTokenSource.Cancel();
            WebSocketConnectionHandler.StopServer();
        }
    }
}
