using onewater.flowmonitor.common;
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

namespace onewater.flowmonitor.res.control
{
    /// <summary>
    /// ProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBar : UserControl
    {
        public ProgressBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 动态改变进度条长度
        /// </summary>
        /// <param name="rate"></param>
        public void Goto(double rate)
        {
            CAAnimation.PlayAnimation(this.back.ActualWidth,
                                      this.Width * rate,
                                      UserControl.WidthProperty,
                                      this.back);
        }
    }
}
