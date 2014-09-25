using onewater.flowmonitor.core;

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
        }

        /// <summary>
        /// 刷新进度条
        /// </summary>
        private void RefreshProgressBar()
        {

        }
    }
}
