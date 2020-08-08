using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    class PString
    {
        public PString(long _index, long _length)
        {
            index = _index;
            length = _length;
        }
        public long index;
        public long length;
    }
    class Program
    {
        static private Encoding enc = new UTF8Encoding();
        static private byte[] buffer;
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2) throw new Exception();
                string inputFile = args[0];
                string outputFile = args[1];
                if (!File.Exists(args[0])) throw new Exception();
                if (File.Exists(outputFile)) File.Delete(outputFile);

                // Читаем файл в байтовый массив
                buffer = File.ReadAllBytes(inputFile);
                if (buffer.LongLength == 0) throw new Exception();

                // Разбиваем на строки (конец CR LF)
                List<PString> lst = new List<PString>();
                long pos = 0; long length = 0;
                for (long i = 0; i < buffer.LongLength; ++i)
                {
                    if (buffer[i] == 0xd && buffer[i + 1] == 0xa)
                    {
                        lst.Add(new PString(pos, length));
                        pos = i + 2;
                        i++;
                        length = 0;
                    }
                    else
                        length++;
                }
                if (length > 0)
                {
                    lst.Add(new PString(pos, length));
                }

                // Сортируем строки сравнивая побайтово
                lst.Sort(comparison);

                // Пишем в выходной файл
                using (var fs = File.OpenWrite(outputFile))
                {
                    foreach (var s in lst)
                    {
                        fs.Write(buffer, (int)s.index, (int)s.length);
                        fs.WriteByte(0xd);
                        fs.WriteByte(0xa);
                    }
                }
                Environment.ExitCode = 0;
                Console.WriteLine("ExiteCode = 0");
            }
            catch
            {
                Environment.ExitCode = 1;
                Console.WriteLine("ExiteCode = 1");
            }
        }

        private static int comparison(PString x, PString y)
        {
            if (x.length == 0 && y.length == 0) return 1;
            if (x.length == 0) return -1;
            if (y.length == 0) return 1;
            long i = 0;
            long j = 0;
            int comp;
            char[] chX = enc.GetChars(buffer, (int)x.index, (int)x.length);
            char[] chY = enc.GetChars(buffer, (int)y.index, (int)y.length);
            while ((comp = chX[i].CompareTo(chY[j])) == 0)
            {
                i++;
                j++;
                if ((i == x.index + x.length) && (j == y.index + y.length))
                    return 0;
                if (i == x.index + x.length)
                    return -1;
                if (j == y.index + y.length)
                    return 1;
            }
            return comp;
        }
    }
}
