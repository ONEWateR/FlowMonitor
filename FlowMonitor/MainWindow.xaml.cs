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

        public MainWindow()
        {
            InitializeComponent();



            this.Closed += (a, b) =>
            {
                FlowMonitor.GetMonitor().Close();
            };

        }

        /// <summary>
        /// 程序启动相关操作
        /// </summary>
        private void Startup() 
        {
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
                }
            }
            CAAnimation.Show(this.contentControl);
            this.contentControl.Content = windows[id];
        }

    }
}
