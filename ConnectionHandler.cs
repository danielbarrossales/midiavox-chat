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
        /// <summary>
        /// Try to connect to websocket
        /// </summary>
        /// <param name="ip">Ip address of the target client</param>
        /// <param name="port">Port on which the client is listening</param>
        /// <returns>Return a websocket with open connection, if it couldn't connect returns null</returns>
        public async Task<WebSocket> WaitIpToConnectAsync(string ip, string port)
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            while (webSocket.State != WebSocketState.Open)
            {
                try
                {
                    await webSocket.ConnectAsync(new Uri("ws://" + ip + ":" + port + "/"), CancellationToken.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    webSocket.Abort();
                    return null;
                }
            }
            Console.WriteLine(webSocket.State);
            return webSocket;
        }
        /// <summary>
        /// Listen for another clients connection
        /// </summary>
        /// <param name="port">The port on which the server will listen</param>
        /// <returns>Returns an websocket with open connection, returns null if it couldn't listen for a connection</returns>
        public async Task<WebSocket> ListenToWSConnectionAsync(string port)
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:" + port + "/");
            try
            {
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
                            return webSocket;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
