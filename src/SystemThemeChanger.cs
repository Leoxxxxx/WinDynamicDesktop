﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WinDynamicDesktop
{
    class SystemThemeChanger
    {
        private const string registryThemeLocation =
            @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private static ToolStripMenuItem menuItem;

        public static List<ToolStripItem> GetMenuItems()
        {
            RegistryKey themeKey = Registry.CurrentUser.OpenSubKey(registryThemeLocation);

            if (themeKey != null)
            {
                themeKey.Close();

                menuItem = new ToolStripMenuItem("&Change Windows 10 theme color", null,
                    OnThemeItemClick);
                menuItem.Checked = JsonConfig.settings.changeSystemTheme;

                return new List<ToolStripItem>() { menuItem };
            }
            else
            {
                JsonConfig.settings.changeSystemTheme = false;

                return new List<ToolStripItem>();
            }
        }

        public static void TryUpdateSystemTheme()
        {
            if (!JsonConfig.settings.changeSystemTheme)
            {
                return;
            }

            bool darkTheme = !WallpaperChangeScheduler.isDayNow || JsonConfig.settings.darkMode;
            RegistryKey themeKey = Registry.CurrentUser.OpenSubKey(registryThemeLocation, true);

            if (darkTheme)
            {
                themeKey.SetValue("AppsUseLightTheme", 0);      // Dark app theme
                themeKey.SetValue("SystemUsesLightTheme", 0);   // Dark system theme
            }
            else
            {
                themeKey.SetValue("AppsUseLightTheme", 1);      // Light app theme
                themeKey.SetValue("SystemUsesLightTheme", 1);   // Light system theme
            }

            themeKey.Close();
        }

        private static void OnThemeItemClick(object sender, EventArgs e)
        {
            bool isEnabled = JsonConfig.settings.changeSystemTheme ^ true;
            JsonConfig.settings.changeSystemTheme = isEnabled;
            menuItem.Checked = isEnabled;

            TryUpdateSystemTheme();
            JsonConfig.SaveConfig();
        }
    }
}
