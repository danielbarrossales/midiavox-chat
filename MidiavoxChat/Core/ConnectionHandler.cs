using MidiavoxChat.Core.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidiavoxChat.Core
{
    /// <summary>
    /// Class responsible for handling websocket connection
    /// </summary>
    public static class WebSocketConnectionHandler
    {
        static string ClassName = "ConnectionHandler";
        private static HttpListener httpListener;

        /// <summary>
        /// Try to connect to websocket
        /// </summary>
        /// <param name="ip">Ip address of the target client</param>
        /// <param name="port">Port on which the client is listening</param>
        /// <returns>Return a websocket with open connection, if it couldn't connect returns null</returns>
        public static async Task<WebSocket> WaitIpToConnectAsync(string ip, string port)
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
        public static async Task<WebSocket> ListenToWSConnectionAsync(string port)
        {
            string functionName = "WaitIpConnectAsync";
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:" + port + "/");
            httpListener.Prefixes.Add("http://127.0.0.1:" + port + "/");
            httpListener.Prefixes.Add("http://*:" + port + "/");
            try
            {
                httpListener.Start();
                Logger.Log($"{ClassName}: {functionName} -- Trying to listen on port {port}");
                HttpListenerContext context = await httpListener.GetContextAsync();
                Logger.Log($"{ClassName}: {functionName} -- Received request with context: {context.Request.ToString()}");
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
                StopServer();
                return null;
            }
            catch (Exception e)
            {
                Logger.Log($"{ClassName}: {functionName} -- Error while listening on port {port}: {e.GetType()} | {e.Message}");
                StopServer();
                return null;
            }
        }

        /// <summary>
        /// Stop the Http server responsible for responding to websocket connection attempts
        /// </summary>
        public static void StopServer()
        {
            if (httpListener != null)
            {
                httpListener.Stop();
            }
        }
    }
}
