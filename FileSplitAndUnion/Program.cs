﻿using System;
using System.IO;
using System.Linq;

namespace FileSplitAndUnion
{
    class Program
    {
        static void Main(string[] args)
        {
            // -t s(split)/u(union) -f filename -s sizemb(int)
            if (args.Length < 4 || args.Length > 6 || args[0] != "-t" || args[2] != "-f" || (args[1] == "s" && args[4] != "-s"))
            {
                Console.WriteLine("args: -t s(split)/u(union) -f filename -s sizemb(int)");
                return;
            }
            var type = args[1];
            var filename = args[3];
            var size = (type == "s" ? int.Parse(args[5]) : 0) * 1024 * 1024;

            switch (type)
            {
                case "s":
                    SplitFile();
                    return;
                case "u":
                    UnionFile();
                    return;
            }

            Console.WriteLine("unsupport operation type.");
            return;

            void SplitFile()
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine("split file not exist.");
                    return;
                }
                int index = 0;
                long sum = 0;

                var fileInfo = new FileInfo(filename);
                var fileTotalLen = fileInfo.Length;
                var bufSize = Math.Min(size, 20 * 1024 * 1024);
                var buf = new byte[bufSize];

                using (var rawFileStream = new FileStream(filename, FileMode.Open))
                {
                    while (true)
                    {
                        if (sum >= fileTotalLen)
                        {
                            break;
                        }
                        var newFileName = $"{filename}.{index}";
                        var chunkFileSize = (fileTotalLen - sum) > size ? size : fileTotalLen - sum;
                        using (var writeFileStream = new FileStream(newFileName, FileMode.OpenOrCreate))
                        {
                            long readSum = 0;
                            while (true)
                            {
                                if (readSum >= chunkFileSize)
                                {
                                    break;
                                }
                                var readSize = (chunkFileSize - readSum) > bufSize ? bufSize : (chunkFileSize - readSum);
                                EnsureRead();
                                sum += readSize;
                                readSum += readSize;
                                writeFileStream.Write(buf, 0, (int)readSize);

                                void EnsureRead()
                                {
                                    var s = 0;
                                    while (true)
                                    {
                                        if (s >= readSize)
                                        {
                                            break;
                                        }
                                        s += rawFileStream.Read(buf, s, (int)readSize - s);
                                    }
                                }
                            }
                        }
                        index++;
                    }
                }
                Console.WriteLine("ok");
            }

            void UnionFile()
            {
                var files = Directory.GetFiles(".").Where(m => m.Contains($"{filename}.")).OrderBy(m =>
                {
                    var arr = m.Split('.');
                    return int.Parse(arr[^1]);
                }).ToArray();
                if (files.Length == 0)
                {
                    Console.WriteLine("file ready to union not exist.");
                    return;
                }
                var buf = new byte[20 * 1024 * 1024];
                var targetFileName = $"{filename}.union";
                if (File.Exists(targetFileName))
                {
                    File.Delete(targetFileName);
                }
                using (var targetFile = new FileStream(targetFileName, FileMode.Create))
                {
                    foreach (var file in files)
                    {
                        using var f = new FileStream(file, FileMode.Open);
                        while (true)
                        {
                            var t = f.Read(buf, 0, buf.Length);
                            if (t == 0)
                            {
                                break;
                            }
                            targetFile.Write(buf, 0, t);
                        }
                    }
                }
                Console.WriteLine("ok");
            }
        }
    }
}
