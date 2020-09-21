using BrightnessLibrary;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.Text;

namespace WinApp.ViewModels
{
    public class ViewModels : BindableBase
    {
        private Brightness bright;

        private FileStream f;

        private System.Timers.Timer timersTimer;

        private int _BrightValue;
        /// <summary>
        /// two way
        /// </summary>
        public int BrightValue
        {
            get => _BrightValue;
            set
            {
                AddText(f, "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + "Brightness " + BrightValue + "\n");
                bright.SetValue(value);
                this.SetProperty(ref _BrightValue, value);
            }                  
        }

        public DelegateCommand OpenCommand => new DelegateCommand(() => 
        {
            var current = Environment.CurrentDirectory;

            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = current;
            dialog.Title = "ファイルを開く";
            dialog.DefaultExt = ".log";
            dialog.Filter = "|*.log";

            // fileは開けない
            dialog.ShowDialog();
        });

        /// <summary>
        /// 
        /// </summary>
        ~ViewModels()
        {
            f.Close();
        }

        public ViewModels()
        {
            var current = Environment.CurrentDirectory;
            var log = Path.Combine(current, "brightness.log");

            if (File.Exists(log))
            {
                File.Delete(log);
            }
            f = File.Create(log);

            bright = new Brightness();
            BrightValue = bright.GetValue();

            // 100msおきに明るさが外部から更新されていないか確認
            timersTimer = new System.Timers.Timer(100);
            timersTimer.Elapsed += (s, e) =>
            {
                if(BrightValue != bright.GetValue())
                {
                    BrightValue = bright.GetValue();
                    AddText(f, "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + "Brightness " + BrightValue + "\n");
                }
            };
            timersTimer.AutoReset = true;
            timersTimer.Enabled = true;
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
    }
}
