using System;
using System.Text;

namespace EncodingConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Converter converter = new Converter();
            converter.Logger.Writers.Add(Console.Out);

            converter.TargetEncoding = Encoding.UTF8;
            converter.SkipEncodings.Add(Encoding.Default);
            converter.SkipEncodings.Add(Encoding.UTF8);

            converter.Info();

            foreach (var path in args)
            {
                converter.ConvertFile(path);
            }

#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
        }
    }
}
