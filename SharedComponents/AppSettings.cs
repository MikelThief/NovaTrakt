using Microsoft.Win32;

namespace NovaTrakt.SharedComponents
{
    public static class AppSettings
    {
        public static void set(string keyName, object value)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\NovaTrakt");
            key.SetValue(keyName, value);
            key.Close();
        }

        public static string getString(string keyName, string defaultValue)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NovaTrakt");
            string value = defaultValue;

            if (key != null && key.GetValue(keyName) != null)
            {
                value = key.GetValue(keyName).ToString();
                key.Close();
            }
            return value;
        }

        public static bool getBool(string keyName, bool defaultValue)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NovaTrakt");
            bool value = defaultValue;

            if (key != null && key.GetValue(keyName) != null)
            {
                if (key.GetValue(keyName).ToString() == "True")
                    value = true;
                else if (key.GetValue(keyName).ToString() == "False")
                    value = false;

                key.Close();
            }
            return value;
        }
    }
}
