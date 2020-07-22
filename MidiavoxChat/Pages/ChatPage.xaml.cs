using Microsoft.Win32;
using MidiavoxChat.Core;
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
    /// Interação lógica para ChatPage.xam
    /// </summary>
    public partial class ChatPage : Page
    {
        private MessageHandler _messageHandler;
        private CancellationTokenSource cancelationTokenSource = new CancellationTokenSource();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="webSocket">Web Socket with connection status of type open, that will be used in the chat</param>
        public ChatPage(WebSocket webSocket)
        {
            InitializeComponent();
            _messageHandler = new MessageHandler(webSocket, new MessageHandler.ReceivedMessageDelegate(ReceivedMessage), new MessageHandler.ReceivedImageDelegate(ReceivedImage));
            DownTimeMessage();
        }

        /// <summary>
        /// Ensures that the text has no more than a 100 characteres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentMessageText.Text.Length > 100)
            {
                CurrentMessageText.Text = CurrentMessageText.Text.Substring(0, 100);
                CurrentMessageText.CaretIndex = 100;
            }
        }

        /// <summary>
        /// Tries to send the message through the websocket connection
        /// if it fails cleans up and send user back to the starting page
        /// Cancel any DownTimeMessage and starts another one whenever is called
        /// </summary>
        private void SendMessage(string messageToSend)
        {
            cancelationTokenSource.Cancel();
            string message = messageToSend;
            Task.Run(async () => 
            {
                var sendMessageTask = await _messageHandler.SendMessage(message);
                if (sendMessageTask)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.UpdateMessageHistory(message, false);
                        cancelationTokenSource = new CancellationTokenSource();
                        this.DownTimeMessage();
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
        
        /// <summary>
        /// Clean up before exiting page
        /// </summary>
        private void WsConnectionClosed()
        {
            MessageBox.Show("Connection was closed");
            WebSocketConnectionHandler.StopServer();
            this.NavigationService.Navigate(new StartingPage());
        }

        /// <summary>
        /// Method that create objects to display the received and send messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="received">If true assumes the message was received from the websocket and align the message to the left</param>
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

        private void UpdateMessageHistory(byte[] imageByteArray, bool received)
        {            
            var image = new Image();
            image.Source = (ImageSource) new ImageSourceConverter().ConvertFrom(imageByteArray);
            image.MaxWidth = 300;
            if (received)
            {
                image.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                image.HorizontalAlignment = HorizontalAlignment.Right;
            }
            MessagesPanel.Children.Add(image);
            MessagesScrollBar.ScrollToEnd();
        }

        /// <summary>
        /// Method that handles an incoming message
        /// </summary>
        /// <param name="message"></param>
        private void ReceivedMessage(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                UpdateMessageHistory(message, true);
            });
        }

        private void ReceivedImage(byte[] image)
        {
            this.Dispatcher.Invoke(() =>
            {
                UpdateMessageHistory(image, true);
            });
        }

        private void SendMessageBtn(object sender, RoutedEventArgs e)
        {
            SendMessage(CurrentMessageText.Text);
            CurrentMessageText.Text = "";
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage(CurrentMessageText.Text);
                CurrentMessageText.Text = "";
            }
        }


        /// <summary>
        /// Send the date and year after 60000 seconds
        /// </summary>
        private void DownTimeMessage()
        {
            var cancellationToken = cancelationTokenSource.Token;
            Task.Run(async () => 
            {
                await Task.Delay(60000);
                cancellationToken.ThrowIfCancellationRequested();
                this.Dispatcher.Invoke(()=>
                {
                    var time = DateTime.Now;
                    this.SendMessage($"{time.Day}/{time.Month}/{time.Year} - {time.Hour}:{time.Minute}:{time.Second}");
                });
            }, cancellationToken);
        }

        private void LoadFileBtn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de Imagens (*.jpg)|*.jpg";
            openFileDialog.ShowDialog();

            this.NavigationService.Navigate(new ConfirmImage(new Uri(openFileDialog.FileName), this));
        }

        public async void SendFile(byte[] image)
        {

            await _messageHandler.SendImage(image);
            UpdateMessageHistory(image, false);
        }
    }
}
