using MidiavoxChat.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidiavoxChat.Core
{
    /// <summary>
    /// Class responsible to send and receive messages through the websocket
    /// </summary>
    public class MessageHandler
    {
        /// <summary>
        /// Delegate type that will be called when a text message is received
        /// </summary>
        /// <param name="message">Received message</param>
        public delegate void ReceivedMessageDelegate(string message);
        public delegate void ReceivedImageDelegate(byte[] image);

        private ReceivedMessageDelegate ReceivedMessage { get; set; }
        private ReceivedImageDelegate ReceivedImage { get; set; }
        private WebSocket _webSocket;
        private readonly string ClassName = "MessagesHandler";
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSocket">WebSocket with open connection that will be used by the object</param>
        /// <param name="receivedMessageDelegate">Will be called everytime the websocket receives a message of text type</param>        
        public MessageHandler(WebSocket webSocket, ReceivedMessageDelegate receivedMessageDelegate, ReceivedImageDelegate receivedImageDelegate)
        {
            _webSocket = webSocket;
            ReceivedMessage = receivedMessageDelegate;
            ReceivedImage = receivedImageDelegate;
            ReceiveMessage();
        }

        /// <summary>
        /// Send a message through the websocket
        /// </summary>
        /// <param name="message"></param>
        /// <returns>true if the message is send successfuly, otherwhise returns false</returns>
        public async Task<bool> SendMessage(string message)
        {
            var functionName = "SendMessage";
            if (message.Length > 100)
            {
                message = message.Substring(0, 100);
            }
            Logger.Log($"{ClassName}: {functionName} -- Trying to send Message");
            if (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    Logger.Log($"{ClassName}: {functionName} -- Message Sent");
                    await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log($"{ClassName}: {functionName} -- Failed attempt to send Message: {e.Message}");
                    return false;
                }
            }
            Logger.Log($"{ClassName}: {functionName} -- Connection closed");
            return false;
        }

        public async Task<bool> SendImage(byte[] image)
        {
            var functionName = "SendImage";
            
            Logger.Log($"{ClassName}: {functionName} -- Trying to send Image");
            if (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var byteArraySegment = new ArraySegment<byte>(image, 0, image.Length);
                    await _webSocket.SendAsync(byteArraySegment, WebSocketMessageType.Binary, true, CancellationToken.None);
                    Logger.Log($"{ClassName}: {functionName} -- Image Sent with length of {image.Length}");
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log($"{ClassName}: {functionName} -- Failed attempt to send Image: {e.Message}");
                    return false;
                }
            }
            Logger.Log($"{ClassName}: {functionName} -- Connection closed");
            return false;
        }

        /// <summary>
        /// Will run for as long as the Web Socket connection is up
        /// Calls a delegate function everytime it receives a text message.
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveMessage()
        {
            var functionName = "ReceiveMessage";
            while (_webSocket.State == WebSocketState.Open)
            {
                var buffer = new Byte[5000000];
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    Logger.Log($"{ClassName}: {functionName} -- Received Text Message");
                    ReceivedMessage(Encoding.UTF8.GetString(buffer).Trim('\0'));
                }
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    int i = buffer.Length - 1;
                    for (; buffer[i] == 0; i--) { }

                    i += 1;
                    var image = new Byte[i];

                    for(int j = 0; j < i; j++)
                    {
                        image[j] = buffer[j];
                    }

                    Logger.Log($"{ClassName}: {functionName} -- Received Image with length = {image.Length}");

                    ReceivedImage(buffer);
                }
            }
            Logger.Log($"{ClassName}: {functionName} -- Connection closed");
        }
    }
}
