using System;
using System.Collections.Generic;
using System.IO;

namespace EncodingConverter
{
    enum LogLevel
    {
        Debug   = 1,
        Info    = 2,
        Warning = 3,
        Error   = 4
    }

    class Logger
    {
        public string LogFormat { get; set; }
        public LogLevel Level { get; set; }
        public List<TextWriter> Writers { get; set; } 


        public Logger()
        {
            Writers = new List<TextWriter>();
            Level = LogLevel.Info;
            LogFormat = "| {0:s} | {1} | {2}";
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            if (Level.CompareTo(level) <= 0)
            {
                foreach (var writer in Writers)
                {
                    writer.WriteLine(LogFormat, DateTime.Now, level.ToString(), String.Format(format, args));
                    writer.Flush();
                }
            }
        }

        public void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        public void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public void Warning(string format, params object[] args)
        {
            Log(LogLevel.Warning, format, args);
        }

        public void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

    }
}
