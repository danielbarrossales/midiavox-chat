using System;
using System.Collections.Concurrent;
using midiavox_chat.Utils;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace midiavox_chat
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConcurrentStack<string> messages = new ConcurrentStack<string>();

            Console.WriteLine("Hello");
            Console.WriteLine("Waiting for other person to connect on the ip " + NetworkUtils.GetLocalIpAddress() + " to start the chat.");
            Console.WriteLine("Enter 1 to connect to another client or enter 2 to wait for a client to connect to you");
            int opt = Int32.Parse(Console.ReadLine());
            switch (opt)
            {
                case 1:
                    await TryToConnect();
                    break;
                default:
                    await ListenForConnection();
                    break;
            }
        }

        static async Task TryToConnect()
        {
            WebSocket webSocket = null;
            while(webSocket == null)
            {
                Console.WriteLine("Enter with the ip you wish to connect");
                var ip = Console.ReadLine();
                Console.WriteLine("Enter with the target port");
                var port = Console.ReadLine();
                webSocket = await WebSocketConnectionHandler.WaitIpToConnectAsync(ip, port);
            }

            var messagesHandler = new MessagesHandler(webSocket);
            messagesHandler.ReceivedMessage = new MessagesHandler.ReceivedMessageDelegate(GotMessage);

            while(true) 
            {
                var message = Console.ReadLine();
                await messagesHandler.SendMessage(message);
            }
        }

        static void GotMessage(string message){
            Console.WriteLine("Message received: " + message);
        }
        static async Task ListenForConnection()
        {
            WebSocket webSocket = null;
            while(webSocket == null)
            {
                Console.WriteLine("Enter with the port in which the app will listen");
                var port = Console.ReadLine();
                webSocket = await WebSocketConnectionHandler.ListenToWSConnectionAsync(port);
            }

            var messagesHandler = new MessagesHandler(webSocket);
            messagesHandler.ReceivedMessage = new MessagesHandler.ReceivedMessageDelegate(GotMessage);

            while(true) 
            {
                var message = Console.ReadLine();
                await messagesHandler.SendMessage(message);
            }
        }
    }
}
