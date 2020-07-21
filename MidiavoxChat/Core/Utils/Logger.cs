using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MidiavoxChat.Core.Utils
{
    public static class Logger
    {
        private static readonly Object _appendLock = new Object();
        static Logger()
        {
            System.IO.Directory.CreateDirectory("logs");
        }
        public static void Log(string message)
        {
            Task.Run(() =>
            {
                lock (_appendLock)
                {
                    using (var log = File.AppendText($"logs/log_{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt"))
                    {
                        string logLine = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - {message}";
                        log.WriteLineAsync(logLine).Wait();
                    }
                }
            });
        }
    }
}
