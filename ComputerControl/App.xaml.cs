using System.Drawing;
using System.Windows.Forms;
using CC.Logic;
using IWshRuntimeLibrary;

namespace ComputerControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            //var window = new MainWindow();
            //window.Show();

            var runner = new Runner();
            runner.Start();

            var shell = new WshShellClass();

            var trayIcon = new NotifyIcon();

            var menuItemExit = new MenuItem
            {
                Index = 1,
                Text = "Exit"
            };

            var menuItemStartup = new MenuItem()
            {
                Index = 0,
                Text = "Start at boot"
            };

            menuItemStartup.Click += (sender, args) =>
            {

            };

            menuItemExit.Click += (sender, args) =>
            {
                runner.Stop();
                trayIcon.Visible = false;
                Current.Shutdown();
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
