using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Registry keys handle
    /// </summary>
    public class Reg
    {
        /// <summary>
        /// Registry key check
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool regKey_Check(string keyPath, string keyName, string subKeyName)
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(keyName, true);
            if ((Registry.GetValue(keyPath, keyName, null) == null) && (!string.IsNullOrEmpty(rkApp.GetValue(subKeyName).ToString())))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// Registry key wirte
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>

        public static void regKey_WriteSubkey(string keyName, string subKeyName, string subKeyValue)
        {

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            (keyName, true);

            rk.SetValue(subKeyName, subKeyValue);
        }
        /// <summary>
        /// Delete registry subkey
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="subKeyName"></param>
        /// <param name="subKeyValue"></param>
        public static void regKey_DeleteSubkey(string keyName, string subKeyName)
        {
            try
            {

                RegistryKey rk = Registry.CurrentUser.OpenSubKey
                (keyName, true);

                rk.DeleteValue(subKeyName);
            }
            catch
            {

            }
        }



        /// <summary>
        /// Registry key reader from HKEY_CURRENT_USER
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string regKey_Read(string keyName, string subKeyName)
        {
            string key = string.Empty;

            string InstallPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\"+keyName, subKeyName, null);
            if (InstallPath != null)
            {
                key = InstallPath;
            }
            return key;
        }

        /// <summary>
        /// Registry key reader from HKEY_LOCAL_MACHINE
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string regKey_ReadMachine(string keyName, string subKeyName)
        {
            string key = string.Empty;

            string InstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\" + keyName, subKeyName, null);
            if (InstallPath != null)
            {
                key = InstallPath;
            }
            return key;
        }

        /// <summary>
        ///  Registry key create
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="subKeyName"></param>
        /// <param name="subKeyValue"></param>

        public static void regKey_CreateKey(string keyName, string subKeyName, string subKeyValue)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey
            (keyName);

            key.SetValue(subKeyName, subKeyValue);
            key.Close();
        }

    }
}
