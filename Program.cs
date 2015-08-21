using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EncodingConverter
{
    class Program
    {

        static void Main(string[] args)
        {
            Converter converter = new Converter();
            var commandLineParser = new CommandLineParser();

            InitializeCommandLineArguments(commandLineParser, converter);

            args = commandLineParser.Parse(args);

            converter.Init();

            foreach (var path in args)
            {
                converter.ConvertFile(path);
            }

#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
        }

        static void InitializeCommandLineArguments(CommandLineParser commandLineParser, Converter converter)
        {
            var helpOption = new Option()
            {
                Switch = true,
                Command = x => PrintUsage(),
            };
            commandLineParser.Options["h"] = helpOption;
            commandLineParser.Options["help"] = helpOption;

            var encodingsOption = new Option()
            {
                Switch = true,
                Command = x => PrintEncodings(),
            };
            commandLineParser.Options["e"] = encodingsOption;
            commandLineParser.Options["encodings"] = encodingsOption;

            var targetEncodingOption = new Option()
            {
                Command = x => converter.TargetEncoding = Encoding.GetEncoding(x)
            };
            commandLineParser.Options["t"] = targetEncodingOption;
            commandLineParser.Options["to"] = targetEncodingOption;

            var sourceEncodingOption = new Option()
            {
                Command = x => converter.SourceEncoding = Encoding.GetEncoding(x)
            };
            commandLineParser.Options["f"] = sourceEncodingOption;
            commandLineParser.Options["from"] = sourceEncodingOption;

            var ignoreEncodingOption = new Option()
            {
                Command = x => converter.SkipEncodings.Add(Encoding.GetEncoding(x))
            };
            commandLineParser.Options["i"] = ignoreEncodingOption;
            commandLineParser.Options["ignore"] = ignoreEncodingOption;

            var logFileOption = new Option()
            {
                Command = x => converter.Logger.Writers.Add(new StreamWriter(File.OpenWrite(x)))
            };
            commandLineParser.Options["l"] = logFileOption;
            commandLineParser.Options["log"] = logFileOption;

            var verboseOption = new Option()
            {
                Command = x => converter.Logger.Level = LogLevel.Debug
            };
            commandLineParser.Options["l"] = verboseOption;
            commandLineParser.Options["log"] = verboseOption;
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: EncodingConverter [options] <file(s)>");
            Console.WriteLine("Options:");
            Console.WriteLine("\t-e, --encodings: List supported encodings.");
            Console.WriteLine("\t-t,--to <encoding>: Sets the target encoding.");
            Console.WriteLine("\t-f,--from <encoding>: Sets the source encoding. This will disable character set guessing.");
            Console.WriteLine("\t-i,--ignore <encoding>: Sets the encodings to ignore.  Can be set more than once.");
            Console.WriteLine("\t-l,--log <file>: Log to a file.");
            Console.WriteLine("\t-v,--verbose: Turn on debug logging.");
            Console.WriteLine("\t-h,--help: Prints this message.");
            Console.WriteLine("");
            Console.WriteLine("Default Ignored Encodings: " + Encoding.Default.WebName);
#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
            Environment.Exit(0);
        }

        static void PrintEncodings()
        {
            Console.WriteLine("Supported Encodings:");
            foreach (var encodingInfo in Encoding.GetEncodings())
            {
                Console.WriteLine(encodingInfo.Name);
            }
#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
            Environment.Exit(0);
        }

    }
}
