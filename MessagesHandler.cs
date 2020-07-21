using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using midiavox_chat.Utils;

namespace midiavox_chat
{
    /// <summary>
    /// Class responsible to send and receive messages through the websocket
    /// </summary>
    public class MessagesHandler
    {
        /// <summary>
        /// Delegate type that will be called when a text message is received
        /// </summary>
        /// <param name="message">Received message</param>
        public delegate void ReceivedMessageDelegate(string message);

        /// <summary>
        /// Delegate will be called everytime the websocket receives a message
        /// </summary>
        public ReceivedMessageDelegate ReceivedMessage { get; set; }
        private WebSocket _webSocket;
        private string ClassName = "MessagesHandler";
        public MessagesHandler (WebSocket webSocket)
        {
            _webSocket = webSocket;
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
            if (_webSocket.State == WebSocketState.Open)
            {
                try {
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
            var buffer = new Byte[4096];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text){
                    Logger.Log($"{ClassName}: {functionName} -- Received Text Message");
                    ReceivedMessage(Encoding.UTF8.GetString(buffer).Trim('\0'));
                }
            }
            Logger.Log($"{ClassName}: {functionName} -- Connection closed");
        }
    }
}
