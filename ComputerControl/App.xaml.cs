using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CC.Logic;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace ComputerControl
{
    public partial class App
    {
        public App()
        {
            var runner = new Runner();
            runner.Start();

            var shell = new WshShellClass();
            var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\ComputerControl.lnk";

            var trayIcon = new NotifyIcon();

            var menuItemExit = new MenuItem
            {
                Index = 1,
                Text = "Exit"
            };
            
            var menuItemStartup = new MenuItem()
            {
                Index = 0,
                Text = "Start on Boot",
                Checked = File.Exists(shortcutPath)
            };

            menuItemStartup.Click += (sender, args) =>
            {
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                    menuItemStartup.Checked = false;
                }
                else
                {
                    var shortcut = (IWshShortcut) shell.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = Assembly.GetEntryAssembly().Location;
                    shortcut.Description = "Startup for Computer Control";
                    shortcut.WorkingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    shortcut.Save();
                    menuItemStartup.Checked = true;
                }
                
            };

            menuItemExit.Click += (sender, args) =>
            {
                runner.Stop();
                trayIcon.Visible = false;
                Environment.Exit(0);
            };

            trayIcon.ContextMenu = new ContextMenu
            {
                MenuItems =
                {
                    menuItemStartup,
                    menuItemExit
                }
            };

            trayIcon.Text = "Computer Control";
            trayIcon.Icon = new Icon("app.ico");
            trayIcon.Visible = true;
        }
    }
}
