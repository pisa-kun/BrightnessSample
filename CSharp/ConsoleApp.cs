using BrightnessLibrary;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace BrightnessControll
{
    class ConsoleApp
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Mutex名を決める（必ずアプリケーション固有の文字列に変更すること！）
            string mutexName = Process.GetCurrentProcess().ProcessName;
            //Mutexオブジェクトを作成する
            System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName);

            bool hasHandle = false;

            try
            {
                //ミューテックスの所有権を要求する
                hasHandle = mutex.WaitOne(0, false);

                //ミューテックスを得られたか調べる
                if (hasHandle == false)
                {
                    Console.WriteLine("多重起動できませんでした");
                    var now_id = Process.GetCurrentProcess().Id;
                    if(now_id == Process.GetProcessesByName(mutexName)[0].Id)
                    {
                        // first other kill,
                        Process.GetProcessesByName(mutexName)[1].Kill();
                        Process.GetProcessesByName(mutexName)[0].Kill();
                    }
                    else
                    {
                        Process.GetProcessesByName(mutexName)[0].Kill();
                        Process.GetProcessesByName(mutexName)[1].Kill();
                    }
                }

                // Main
                Console.WriteLine("\"Escape\"で終了");

                var current = Environment.CurrentDirectory;
                var log = Path.Combine(current, "brightness.log");

                if (File.Exists(log))
                {
                    File.Delete(log);
                }

                Console.WriteLine($"output folder : {log}");

                var n = new Brightness();
                var nowBrightness = n.GetValue();

                using (var f = File.Create(log))
                {
                    while (true)
                    {
                        if (nowBrightness != n.GetValue())
                        {
                            AddText(f, "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + "Brightness " + n.GetValue() + "\n");
                            Console.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + "Brightness " + n.GetValue());
                            nowBrightness = n.GetValue();
                        }

                        // ブロックせずにキー入力受付
                        if (Console.KeyAvailable)
                        {
                            var k = Console.ReadKey();
                            if (k.Key.ToString() == "Escape")
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (hasHandle)
                {
                    //ミューテックスを解放する
                    mutex.ReleaseMutex();
                }
                mutex.Close();
            }

        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
    }
}
