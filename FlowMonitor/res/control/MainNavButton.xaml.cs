using onewater.flowmonitor.app;

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
    /// MainNavButton.xaml 的交互逻辑
    /// </summary>
    public partial class MainNavButton : UserControl
    {
        public int id = 0;                                              // ID
        static MainNavButton checked_button = null;                     // 选中的按钮
        BitmapImage n_icon = new BitmapImage();                         // 正常状态的图标
        BitmapImage s_icon = new BitmapImage();                         // 选中状态的图标
        bool check;                                                     // 选中布尔值

        public MainNavButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void init()
        {
            // 默认第一个为选择对象
            if (id == 0)
            {
                checked_button = this;
                check = true;
            }

            string[] content = this.Tag.ToString().Split(',');

            n_icon.BeginInit();
            n_icon.UriSource = new Uri(AppConfig.IconPath + content[0] + ".png", UriKind.Relative);
            n_icon.EndInit();

            s_icon.BeginInit();
            s_icon.UriSource = new Uri(AppConfig.IconPath + content[1] + ".png", UriKind.Relative);
            s_icon.EndInit();

            this.content.Content = content[2];
            change_state();
        }

        /// <summary>
        /// 选择/未选择的状态改变
        /// </summary>
        public void change_state()
        {
            if (check)
            {
                this.icon.Source = s_icon;
                this.content.Foreground = Brushes.Black;
                this.bg.Fill = Brushes.White;
            }
            else
            {
                this.icon.Source = n_icon;
                this.content.Foreground = Brushes.White;
                this.bg.Fill = new SolidColorBrush(Color.FromRgb(4, 144, 226));
            }
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (!check)
            {
                check = true;
                change_state();
                checked_button.check = false;
                checked_button.change_state();
                checked_button = this;
            }
        }
    }
}
