using BrightnessLibrary;
using System;
using System.IO;
using System.Text;

namespace BrightnessControll
{
    class ConsoleApp
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\"Escape\"で終了");

            var current = Environment.CurrentDirectory;
            var log = Path.Combine(current, "brightness.log");

            if(File.Exists(log))
            {
                File.Delete(log);
            }

            Console.WriteLine($"output folder : {log}");

            var n = new Brightness();
            var nowBrightness = n.GetValue();

            using (var f = File.Create(log))
            {
                while(true)
                {
                    if (nowBrightness != n.GetValue())
                    {
                        AddText(f, "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + "Brightness " + n.GetValue() + "\n");
                        Console.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + "Brightness " + n.GetValue());
                        nowBrightness = n.GetValue();
                    }

                    // ブロックせずにキー入力受付
                    if(Console.KeyAvailable)
                    {
                        var k = Console.ReadKey();
                        if(k.Key.ToString() == "Escape")
                        {
                            break;
                        }
                    }
                }
            }
                       
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
    }
}
