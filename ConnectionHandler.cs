using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using midiavox_chat.Utils;

namespace midiavox_chat
{
    /// <summary>
    /// Class responsible for handling websocket connection
    /// </summary>
    public class ConnectionHandler
    {
        static string ClassName = "ConnectionHandler";
        /// <summary>
        /// Try to connect to websocket
        /// </summary>
        /// <param name="ip">Ip address of the target client</param>
        /// <param name="port">Port on which the client is listening</param>
        /// <returns>Return a websocket with open connection, if it couldn't connect returns null</returns>
        public async Task<WebSocket> WaitIpToConnectAsync(string ip, string port)
        {
            string functionName = "WaitIpConnectAsync";
            ClientWebSocket webSocket = new ClientWebSocket();
            string uri = "ws://" + ip + ":" + port + "/";
            while (webSocket.State != WebSocketState.Open)
            {
                try
                {
                    Logger.Log($"{ClassName}: {functionName} -- Trying to connect on {uri}");
                    await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                }
                catch (Exception e)
                {
                    Logger.Log($"{ClassName}: {functionName} -- Couldn't connect on {uri} with error: {e.Message}");
                    webSocket.Abort();
                    return null;
                }
            }
            Logger.Log($"{ClassName}: {functionName} -- Connected on {uri}");
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
            string functionName = "WaitIpConnectAsync";
            Logger.Log($"{ClassName}: {functionName} -- Trying to listen on port {port}");
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
                            Logger.Log($"{ClassName}: {functionName} -- Connection made on port {port}");
                            return webSocket;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"{ClassName}: {functionName} -- Error while listening on port {port}: {e.Message}");
                httpListener.Abort();
                return null;
            }
        }
    }
}
