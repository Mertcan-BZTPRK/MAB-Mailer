using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;

namespace MAB_Mailer
{
    public static class ThemeService
    {
        public static void ApplySystemTheme()
        {
            bool isDark = IsSystemInDarkMode();
            SetTheme(isDark);
        }

        private static bool IsSystemInDarkMode()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object registryValueObject = key.GetValue("AppsUseLightTheme");
                        if (registryValueObject != null)
                        {
                            int registryValue = (int)registryValueObject;
                            return registryValue == 0;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private static void SetTheme(bool isDark)
        {
            var dict = Application.Current.Resources;

            if (isDark)
            {
                // --- KOYU MOD (MODERN & YUMUŞAK) ---
                dict["AppBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC181818")); 
                dict["CardBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#252526")); // VS Code tarzı kartlar

                // GİRİŞ KUTULARI 
                dict["InputBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")); // Yumuşak Antrasit
                dict["InputForeground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")); // Kirli Beyaz Yazı
                dict["InputBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555555"));     // İnce Gri Çerçeve

                dict["TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                dict["TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B0BEC5"));
                dict["BorderBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3E3E42"));
                dict["ShadowColor"] = Colors.Black;
            }
            else
            {
                // --- AÇIK MOD ---
                dict["AppBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                dict["CardBackground"] = new SolidColorBrush(Colors.White);

                // GİRİŞ KUTULARI
                dict["InputBackground"] = new SolidColorBrush(Colors.White);
                dict["InputForeground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
                dict["InputBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CFD8DC"));

                dict["TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#263238"));
                dict["TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#607D8B"));
                dict["BorderBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CFD8DC"));
                dict["ShadowColor"] = (Color)ColorConverter.ConvertFromString("#90A4AE");
            }
        }
    }
}