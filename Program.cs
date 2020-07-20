using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace midiavox_chat
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrentStack<string> messages = new ConcurrentStack<string>();

            Console.WriteLine("Hello");
            Console.WriteLine("Waiting for other person to connect on the ip " + Utils.GetLocalIpAddress() + " to start the chat.");
            Console.WriteLine("You can also enter here an ip address to make a connection to another instance");
            var socket = GetSocket();
        }

        static WebSocket GetSocket(){
            var connectionHandler = new ConnectionHandler();
            return connectionHandler.GetWebSocketConnected();
        }

    }
}
