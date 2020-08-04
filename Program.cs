using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2) throw new Exception();
                string inputFile = args[0];
                string outputFile = args[1];
                if (!File.Exists(args[0])) throw new Exception();
                var lines = File.ReadLines(inputFile).OrderBy(x => x).ToArray();
                File.WriteAllLines(outputFile, lines);

                Environment.ExitCode = 0;
                Console.WriteLine("ExiteCode = 0");
            }
            catch
            {
                Environment.ExitCode = 1;
                Console.WriteLine("ExiteCode = 1");
            }
        }
    }
}
