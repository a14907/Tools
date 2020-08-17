using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SendFileByEmail
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string from = string.Empty, to = string.Empty, pwd = string.Empty, attachFileName = string.Empty;
            if (args.Length == 1 && args[0] == "-h")
            {
                Console.WriteLine(@" 
send email: -f from@xx.com -pwd password -t to@xx.com -a filename
    just support single file，the params order should the same. support 163 and gamil.
");
                return;
            }
            else if (args.Length == 8 && args[0] == "-f" && args[2] == "-pwd" && args[4] == "-t" && args[6] == "-a")
            {
                from = args[1].Trim();
                pwd = args[3].Trim();
                to = args[5].Trim();
                attachFileName = args[7].Trim();
                Console.WriteLine($"params: from-{from},to-{to},pwd-{pwd},attachFileName:{attachFileName}");
            }
            else
            {
                Console.WriteLine(@"
get help: -h
send email: -f from@xx.com -pwd password -t to@xx.com -a filename
    just support single file，the params order should the same. support 163 and gamil.
");
                return;
            }

            using var client = new SmtpClient();

            if (from.Contains("@163.com"))
            {
                client.Host = "smtp.163.com";
            }
            else if (from.Contains("@gmail.com"))
            {
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Port = 465;
            }
            else
            {
                Console.WriteLine("we just support 163 and gmail!");
                return;
            }

            client.Credentials = new NetworkCredential(from, pwd);

            var stopwatch = new Stopwatch();
            try
            {
                client.Timeout = 1000000;
                Console.WriteLine("timeout val: " + client.Timeout + "ms");
                stopwatch.Start();
                client.Send(new System.Net.Mail.MailMessage(from, to)
                {
                    Subject = "这是我们这边的最新附件，请注意查收",
                    IsBodyHtml = false,
                    Body = "发送之后请勿直接回复",
                    Attachments = {
                    new System.Net.Mail.Attachment(attachFileName)
                    }
                });
                Console.WriteLine("ok");
            }
            catch (Exception ex)
            {
                throw new Exception("send err", ex);
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine("total seconds:" + stopwatch.Elapsed.TotalSeconds);
            }

        }
    }
}
