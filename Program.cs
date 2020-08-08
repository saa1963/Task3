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
            long i = x.index;
            long j = y.index;
            UInt32 codeX, codeY;
            byte[] tempBuf = new byte[4];
            int byteInSymbolX, byteInSymbolY;
            while (true)
            {
                byteInSymbolX = getByteInSymbol(i);
                byteInSymbolY = getByteInSymbol(i);
                
                
                switch (byteInSymbolX)
                {
                    case 1:
                        tempBuf[0] = 0;
                        tempBuf[1] = 0;
                        tempBuf[2] = 0;
                        tempBuf[3] = buffer[i];
                        break;
                    case 2:
                        tempBuf[0] = 0;
                        tempBuf[1] = 0;
                        tempBuf[2] = buffer[i + 1];
                        tempBuf[3] = buffer[i];
                        break;
                    case 3:
                        tempBuf[0] = 0;
                        tempBuf[1] = buffer[i + 2];
                        tempBuf[2] = buffer[i + 1];
                        tempBuf[3] = buffer[i];
                        break;
                    case 4:
                        tempBuf[0] = buffer[i + 3];
                        tempBuf[1] = buffer[i + 2];
                        tempBuf[2] = buffer[i + 1];
                        tempBuf[3] = buffer[i];
                        break;
                    default:
                        throw new Exception();
                }
                codeX = BitConverter.ToUInt32(tempBuf, 0);
                i += byteInSymbolX;

                switch (byteInSymbolY)
                {
                    case 1:
                        tempBuf[0] = 0;
                        tempBuf[1] = 0;
                        tempBuf[2] = 0;
                        tempBuf[3] = buffer[j];
                        break;
                    case 2:
                        tempBuf[0] = 0;
                        tempBuf[1] = 0;
                        tempBuf[2] = buffer[j + 1];
                        tempBuf[3] = buffer[j];
                        break;
                    case 3:
                        tempBuf[0] = 0;
                        tempBuf[1] = buffer[j + 2];
                        tempBuf[2] = buffer[j + 1];
                        tempBuf[3] = buffer[j];
                        break;
                    case 4:
                        tempBuf[0] = buffer[j + 3];
                        tempBuf[1] = buffer[j + 2];
                        tempBuf[2] = buffer[j + 1];
                        tempBuf[3] = buffer[j];
                        break;
                    default:
                        throw new Exception();
                }
                codeY = BitConverter.ToUInt32(tempBuf, 0);
                j += byteInSymbolY;

                if (codeX < codeY) return -1;
                if (codeX > codeY) return 1;
                if ((i >= x.index + x.length) && (j >= y.index + y.length))
                {
                    return 0;
                }
                if (i >= x.index + x.length)
                {
                    return -1;
                }
                if (j >= y.index + y.length)
                {
                    return 1;
                }
            }
        }

        private static int getByteInSymbol(long i)
        {
            if ((buffer[i] & 0b10000000) == 0)
            {
                return 1;
            }
            else if ((buffer[i] & 0b11100000) == 0b11000000)
            {
                return 2;

            }
            else if ((buffer[i] & 0b11110000) == 0b11100000)
            {
                return 3;
            }
            else if ((buffer[i] & 0b11111000) == 0b11110000)
            {
                return 4;
            }
            else
                throw new Exception();
        }
    }
}
