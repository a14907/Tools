using System;
using System.IO;
using System.Security.Cryptography;

namespace Sha1Base64
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("params should not be empty! and can be only one params.");
                return 1;
            }

            var param = args[0];
            using var sha1 = SHA1.Create();

            if (File.Exists(param))
            {
                HashFile(param);
                return 0;
            }
            else if (Directory.Exists(param))
            {
                var files = Directory.GetFiles(param, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    HashFile(file);
                }
            }
            else
            {
                Console.WriteLine("params should only the exists file or directory!");
                return 1;
            }

            return 0;

            void HashFile(string fileName)
            {
                using var fs=new FileStream(fileName,FileMode.Open);
                var res = sha1.ComputeHash(fs);
                var base64Str = Convert.ToBase64String(res);
                Console.WriteLine($"{fileName}\nbase64: {base64Str}\n");
            }
        }
    }
}