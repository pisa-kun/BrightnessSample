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
                    // 起動済みプロセスをkill
                    // A, A' の順で起動した場合、Process.GetProcessByName(A)->Array[]には
                    // Array[0]にA、Array[1]にA'の情報が入っているので末尾からプロセスキルする
                    for (var i = Process.GetProcessesByName(mutexName).Length - 1; i >= 0; i--)
                    {
                        Console.WriteLine(Process.GetProcessesByName(mutexName)[i].Id);
                        Process.GetProcessesByName(mutexName)[i].Kill();
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
