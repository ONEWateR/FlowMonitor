using onewater.flowmonitor.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace onewater.flowmonitor.windows
{
    /// <summary>
    /// FlowMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class FlowMonitorWindow : UserControl
    {
        public FlowMonitorWindow()
        {
            InitializeComponent();

            // 数据源与后台数据连接。
            datagrid.ItemsSource = FlowMonitor.GetMonitor().GetViewData();

            // 新建提醒对象
            Remind remind = new Remind();
            remind.Init();
            
            // 新建定时器
            Timer FMTimer = new Timer((e) =>
            {
                remind.Refresh();
                
                // 刷新[今日流量使用情况]
                UInt32 a = FlowMonitor.GetMonitor().GetTheDayFlow()[0];
                UInt32 b = remind.GetWarningALL();
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    this.probar.Goto((b - a) / a);
                    this.max.Content = "流量警告线：" + Flow.ChangeFlow(b);
                    this.rest.Content = "还剩：" + Flow.ChangeFlow(a);
                }));
            }, null, 0, 1000);
            
            
        }

        /// <summary>
        /// 刷新进度条
        /// </summary>
        private void RefreshProgressBar()
        {
            this.probar.Goto(0.5);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            this.probar.Goto(r.NextDouble());
        }
    }
}
