using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace midiavox_chat
{
    /// <summary>
    /// Class responsible for handling websocket connection
    /// </summary>
    public class ConnectionHandler
    {
        public WebSocket[] sockets = new WebSocket[2];

        /// <summary>
        /// Waits for a client to connect with this program or for the user to enter an ip to create a connection with
        /// </summary>
        /// <returns>Returns a websocket with a open connection</returns>
        public WebSocket GetWebSocketConnected()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var connectionTasks = new Task[] { Task.Run(() => ListenToWSConnectionAsync()), Task.Run(() => WaitIpToConnectAsync()) };
            var connectionIndex = Task.WaitAny(connectionTasks, token);
            tokenSource.Cancel();
            return sockets[connectionIndex];
        }
        private async Task<bool> WaitIpToConnectAsync()
        {
            var ip = Console.ReadLine();
            ClientWebSocket webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri("ws://" + ip + ":8080/"), CancellationToken.None);
            if (webSocket.State == WebSocketState.Open)
            {
                sockets[1] = webSocket;
                return true;
            }
            else {
                Console.WriteLine("Connection Handler: Unable to connect with that ip");
                return await WaitIpToConnectAsync();
            }
            
        }
        private async Task<bool> ListenToWSConnectionAsync()
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8080/");
            httpListener.Start();

            while (true)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                    var webSocket = webSocketContext.WebSocket;
                    if (webSocket.State == WebSocketState.Open)
                    {
                        sockets[1] = webSocketContext.WebSocket;
                        return true;
                    }
                    continue;
                }
            }
        }
    }
}
