using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace onewater.flowmonitor.common
{
    public class AutoRun
    {

        public const string defaultKeyName = "FlowMonitor";

        public static bool Set(string path, string keyname = defaultKeyName)
        {
            RegistryKey run = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            try
            {
                
                //SetValue:存储值的名称
                run.SetValue(keyname, path);
                return true;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return false;
            }
        }

        public static bool Delete(string keyname = defaultKeyName)
        {
            try
            {
                RegistryKey run = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                run.DeleteValue(keyname);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool isHaveKey(string keyname = defaultKeyName)
        {
            RegistryKey run = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (run.GetValue(keyname) != null)
            {
                return true;
            }
            return false;
        }
    }
}
