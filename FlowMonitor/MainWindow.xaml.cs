using onewater.flowmonitor.app;
using onewater.flowmonitor.common;
using onewater.flowmonitor.res;
using onewater.flowmonitor.core;
using onewater.flowmonitor.windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using onewater.flowmonitor.res.control;
using RemindLibrary;


namespace onewater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private UserControl[] windows = new UserControl[]{
            null, null, null, null
        };
        System.Windows.Forms.NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();



            this.Closed += (a, b) =>
            {
                this.notifyIcon.Visible = false;
                FlowMonitor.GetMonitor().Close();
            };

            MakeIcon();


            if (!AutoRun.isHaveKey())
            {
                AutoRun.Set(System.Windows.Forms.Application.ExecutablePath);
            }



        }

        private void MakeIcon()
        {
       
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.Icon = new System.Drawing.Icon("program_icon.ico");
            this.notifyIcon.Text = "流量监控 - CA";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(notifyIcon_DoubleClick);

            System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem close = new System.Windows.Forms.MenuItem();
            close.Text = "退出";
            close.Click += new EventHandler(delegate { this.Close(); });
            menu.MenuItems.Add(close);

            this.notifyIcon.ContextMenu = menu;
            
            
        }



        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = WindowState.Normal;

        }

        /// <summary>
        /// 程序启动相关操作
        /// </summary>
        private void Startup() 
        {
        }


        /// <summary>
        /// 开机启动
        /// </summary>
        public static void ShowByAuto()
        {
            MessageBox.Show("aa");
            
        }
        

        /// <summary>
        /// 窗口载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainNav.init(AppConfig.NavData);
            foreach (var item in MainNav.mnbs)
            {
                item.MouseDown += (a, b) => {
                    int id = ((MainNavButton)a).id;
                    ShowWindow(id);
                };
            }

            
            ShowWindow(0);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="id"></param>
        private void ShowWindow(int id)
        {
            if (windows[id] == null)
            {
                switch (id)
                {
                    case 0:
                        windows[id] = new FlowMonitorWindow();
                        break;
                    case 1:
                        windows[id] = new FlowHistoryWindow();
                        break;
                    case 2:
                        windows[id] = new SettingWindow();
                        break;
                    case 3:
                        windows[id] = new AboutWindow();
                        break;
                }
            }
            CAAnimation.Show(this.contentControl);
            this.contentControl.Content = windows[id];
        }

        private void Window_StateChanged_1(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
        }

    }
}
