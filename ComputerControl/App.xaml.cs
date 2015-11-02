using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CC.Logic;
using Application = System.Windows.Application;

namespace ComputerControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //var window = new MainWindow();
            //window.Show();

            var runner = new Runner();
            runner.Start();

            var trayIcon = new NotifyIcon();

            var menuItemExit = new MenuItem()
            {
                Index = 0,
                Text = "Exit"
            };

            menuItemExit.Click += (sender, args) =>
            {
                runner.Stop();
                trayIcon.Visible = false;
                Current.Shutdown();
            };

            trayIcon.ContextMenu = new ContextMenu()
            {
                MenuItems =
                {
                    menuItemExit
                }
            };

            trayIcon.Text = "Computer Control";
            trayIcon.Icon = new Icon("app.ico");
            trayIcon.Visible = true;

            
        }
    }
}
