using MidiavoxChat.Core.Utils;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Delegate will be called everytime the websocket receives a message
        /// </summary>
        private ReceivedMessageDelegate ReceivedMessage { get; set; }
        private WebSocket _webSocket;
        private readonly string ClassName = "MessagesHandler";
        public MessageHandler(WebSocket webSocket, ReceivedMessageDelegate receivedMessageDelegate)
        {
            _webSocket = webSocket;
            ReceivedMessage = receivedMessageDelegate;
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

        public async Task ReceiveMessage()
        {
            var functionName = "ReceiveMessage";
            while (_webSocket.State == WebSocketState.Open)
            {
                var buffer = new Byte[4096];
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    Logger.Log($"{ClassName}: {functionName} -- Received Text Message");
                    ReceivedMessage(Encoding.UTF8.GetString(buffer).Trim('\0'));
                }
            }
            Logger.Log($"{ClassName}: {functionName} -- Connection closed");
        }
    }
}
