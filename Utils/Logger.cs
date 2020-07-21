using System;
using System.IO;
using System.Threading.Tasks;

namespace midiavox_chat.Utils
{
    public static class Logger
    {
        static Logger()
        {
            var result = System.IO.Directory.CreateDirectory("logs");
        }
        public static async Task Log(string message)
        {
            using (var log = File.AppendText($"logs/log_{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt"))
            {
                string logLine = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} - {message}";
                await log.WriteLineAsync(logLine);
            }
        }
    }
}
