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
    /// Interação lógica para ChatPage.xam
    /// </summary>
    public partial class ChatPage : Page
    {
        private MessageHandler _messageHandler;
        public ChatPage(WebSocket webSocket)
        {
            InitializeComponent();
            _messageHandler = new MessageHandler(webSocket, new MessageHandler.ReceivedMessageDelegate(ReceivedMessage));   
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentMessageText.Text.Length > 100)
            {
                CurrentMessageText.Text = CurrentMessageText.Text.Substring(0, 100);
                CurrentMessageText.CaretIndex = 100;
            }
        }

        private void SendCurrentMessage()
        {
            string message = CurrentMessageText.Text;
            CurrentMessageText.Text = "";
            Task.Run(async () => 
            {
                var sendMessageTask = await _messageHandler.SendMessage(message);
                if (sendMessageTask)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.UpdateMessageHistory(message, false);
                    });
                }
                else 
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.WsConnectionClosed();
                    });
                }
            });
        }
        
        private void WsConnectionClosed()
        {
            MessageBox.Show("Connection was closed");
            this.NavigationService.Navigate(new StartingPage());
        }

        private void UpdateMessageHistory(string message, bool received)
        {
            var labelMessage = new Label();
            labelMessage.Content = message;
            if (received)
            {
                labelMessage.HorizontalContentAlignment = HorizontalAlignment.Left;
            }
            else
            {
                labelMessage.HorizontalContentAlignment = HorizontalAlignment.Right;
            }
            MessagesPanel.Children.Add(labelMessage);
            MessagesScrollBar.ScrollToEnd();
        }

        private void ReceivedMessage(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                UpdateMessageHistory(message, true);
            });
        }

        private void SendMessageBtn(object sender, RoutedEventArgs e)
        {
            SendCurrentMessage();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendCurrentMessage();
            }
        }
    }
}
