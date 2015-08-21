using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncodingConverter
{
    class Converter
    {

        public Logger Logger { get; set; }

        public Encoding SourceEncoding { get; set; }

        public Encoding TargetEncoding { get; set; }

        public List<Encoding> SkipEncodings { get; set; } 

        public int BufferSize { get; set; }

        public Converter()
        {
            BufferSize = 4096;
            Logger = new Logger();
            TargetEncoding = Encoding.UTF8;
            SkipEncodings = new List<Encoding>();
        }

        public void Init()
        {
            if (Logger.Writers.Count == 0)
            {
                Logger.Writers.Add(Console.Out);
            }

            if (SkipEncodings.Count == 0)
            {
                SkipEncodings.Add(Encoding.Default);
            }

            SkipEncodings.Add(TargetEncoding);

            Logger.Info("System Character Encoding: {0}", Encoding.Default.WebName);
            Logger.Info("Target Encoding: {0}", TargetEncoding.WebName);
        }

        public void ConvertFile(string path)
        {
            if (String.IsNullOrWhiteSpace(path)) return;
            if (Directory.Exists(path))
            {
                Logger.Info("Scanning Directory: {0}", path);
                foreach (var subPath in Directory.EnumerateFileSystemEntries(path))
                {
                    ConvertFile(subPath);
                }
            }
            else if (File.Exists(path))
            {
                var encoding = SourceEncoding ?? DetectEncoding(path);
                if (SkipEncodings.Contains(encoding))
                {
                    Logger.Debug("Skipping File With Accepted Encoding. Encoding: {0}, Path: {1}", 
                        encoding.WebName,
                        path);
                }
                else
                {
                    ConvertFile(path, encoding, TargetEncoding);
                }
                
            }
            else
            {
                Logger.Warning("File Doesn't Exist: {0}", path);
            }
        }

        public void ConvertFile(string path, Encoding fromEncoding, Encoding toEncoding)
        {
            if (String.IsNullOrWhiteSpace(path)) return;
            if (!File.Exists(path))
            {
                Logger.Warning("File Doesn't Exist: {0}", path);
            }
            else
            {
                Logger.Info("Converting From Encoding: {0}, To Encoding {1}, File: {2}", fromEncoding.WebName,
                    toEncoding.WebName, path);
                try
                {
                    var tempFileName = Path.GetTempFileName();
                    using (var inStream = File.OpenRead(path))
                    using (var outStream = File.OpenWrite(tempFileName))
                    {
                        char[] buffer = new char[BufferSize];
                        using (var inReader = new StreamReader(inStream, fromEncoding))
                        using (var outWriter = new StreamWriter(outStream, toEncoding))
                        {
                            var charactersRead = inReader.Read(buffer, 0, BufferSize);
                            while (charactersRead > 0)
                            {
                                outWriter.Write(buffer, 0, charactersRead);
                                charactersRead = inReader.Read(buffer, 0, BufferSize);
                            }
                        }
                    }
                    File.Delete(path);
                    File.Move(tempFileName, path);
                }
                catch (Exception e)
                {
                    Logger.Error("Couldn't Convert File. Path: {0}, Error: {1}", path, e.Message);
                }
            }
        }

        public Encoding DetectEncoding(string path)
        {
            Encoding encoding = Encoding.Default;
            if (!String.IsNullOrWhiteSpace(path))
            { 
                if (!File.Exists(path))
                {
                    Logger.Warning("File Doesn't Exist: {0}", path);
                }
                else
                {
                    var buffer = new byte[4];
                    using (var file = new FileStream(path, FileMode.Open))
                    {
                        file.Read(buffer, 0, 4);
                    }
                    if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                    {
                        encoding = Encoding.UTF8;
                    }
                    else if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0xFE && buffer[3] == 0xFF)
                    {
                        encoding = Encoding.UTF32;
                    }
                    else if (buffer[0] == 0xFF && buffer[1] == 0xFE && buffer[2] == 0x00 && buffer[3] == 0x00)
                    {
                        encoding = Encoding.UTF32;
                    }
                    else if (buffer[0] == 0x2B && buffer[1] == 0x2F && buffer[2] == 0x76)
                    {
                        encoding = Encoding.UTF7;
                    }
                    else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                    {
                        encoding = Encoding.BigEndianUnicode;
                    }
                    else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                    {
                        encoding = Encoding.Unicode;
                    }
                }
}
            return encoding;
        }
 
    }
}
