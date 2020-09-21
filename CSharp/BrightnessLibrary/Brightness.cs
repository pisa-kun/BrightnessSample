using System;
using System.Management;

namespace BrightnessLibrary
{
    public class Brightness
    {
        ManagementClass WmiMonitorBrightnessMethods = new ManagementClass("root/wmi", "WmiMonitorBrightnessMethods", null);
        ManagementClass WmiMonitorBrightness = new ManagementClass("root/wmi", "WmiMonitorBrightness", null);

        // via http://home.a00.itscom.net/hatada/csharp/lcd01.html
        // https://stackoverflow.com/questions/18083982/how-to-query-getmonitorbrightness-from-c-sharp
        // https://stackoverflow.com/questions/8194006/c-sharp-setting-screen-brightness-windows-7
        // https://msdn.microsoft.com/ja-jp/library/system.management.managementscope(v=vs.110).aspx
        public void SetValue(int level)
        {
            foreach (ManagementObject mo in WmiMonitorBrightnessMethods.GetInstances())
            {
                ManagementBaseObject inParams = mo.GetMethodParameters("WmiSetBrightness");
                inParams["Brightness"] = level; // 輝度を level % に
                inParams["Timeout"] = 5;       // 操作のタイムアウトを 5 秒にセット
                mo.InvokeMethod("WmiSetBrightness", inParams, null);
            }
        }

        public int GetValue()
        {
            if (WmiMonitorBrightness.GetInstances() == null)
            {
                return 0;
            }

            foreach (ManagementObject mo in WmiMonitorBrightness.GetInstances())
            {
                return Convert.ToInt32(mo["CurrentBrightness"].ToString());
            }

            return 0;
        }
    }
}
