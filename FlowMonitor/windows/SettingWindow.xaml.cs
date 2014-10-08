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
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : UserControl
    {
        public SettingWindow()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            x0201.Text = Properties.Settings.Default.WarningALL.ToString();
            x0202.Text = Properties.Settings.Default.WarningUP.ToString();
        }

        private void Apply()
        {
            try
            {
                Properties.Settings.Default.WarningALL = uint.Parse(x0201.Text);
                Properties.Settings.Default.WarningUP = uint.Parse(x0202.Text);
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Apply();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WarningALL = 500;
            Properties.Settings.Default.WarningUP = 50;
            Properties.Settings.Default.Save();
            x0201.Text = Properties.Settings.Default.WarningALL.ToString();
            x0202.Text = Properties.Settings.Default.WarningUP.ToString();
        }

    }
}
