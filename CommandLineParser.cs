using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.String;

namespace EncodingConverter
{

    class Option
    {
        public delegate void CommandDelegate(string argument);
        /// <summary>
        /// This is executed when the option is found in the list of command line arguments.
        /// </summary>
        public CommandDelegate Command { get; set; }
        /// <summary>
        /// Set this to true if the option doesn't take an argument.
        /// </summary>
        public bool Switch { get; set; } = false;
    }

    class CommandLineParser
    {
        /// <summary>
        /// The list of possible options.
        /// option names must be lowercased.
        /// </summary>
        public Dictionary<string, Option> Options { get; } = new Dictionary<string, Option>();

        public string[] Parse(string[] args)
        {
            var optionPrefixes = @"(-|--|/)";
            switch (System.Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    optionPrefixes = @"(-|--)";
                    break;
                default:
                    break;
            }

            var optionRegex = new Regex(@"^\s*" + optionPrefixes + @"(\w+)\s*$");

            var lastOption = 0;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (IsNullOrWhiteSpace(arg)) continue;

                var match = optionRegex.Match(arg);
                if (!match.Success) continue;

                var optionName = match.Groups[2].Value.ToLower();
                if (!Options.ContainsKey(optionName)) continue;

                var option = Options[optionName];
                var nextArg = "";
                if (!option.Switch)
                {
                    i++;
                    nextArg = args[i];
                }
                option.Command(nextArg);
                lastOption = i + 1;
            }

            // Remove options from the argument list
            var returnValues = new string[args.Length - lastOption];
            Array.Copy(args, lastOption, returnValues, 0, (args.Length - lastOption));
            return returnValues;

        }
    }
}
