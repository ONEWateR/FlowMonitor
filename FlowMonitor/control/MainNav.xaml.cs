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

namespace onewater.flowmonitor.control
{
    /// <summary>
    /// MainNav.xaml 的交互逻辑
    /// </summary>
    public partial class MainNav : UserControl
    {
        public static List<MainNavButton> mnbs = new List<MainNavButton> { };

        public MainNav()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void init(string[] func_list)
        {
            for (int i = 0; i < func_list.Length; i++)
            {
                // 分割线
                Image img = new Image();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(AppConfig.IconPath, UriKind.Relative);
                bi.EndInit();
                img.Margin = new Thickness(5, (i + 1) * 72 - 20, 0, 0);
                img.Width = 150;
                img.Height = 10;
                img.HorizontalAlignment = HorizontalAlignment.Left;
                img.VerticalAlignment = VerticalAlignment.Top;
                img.Source = bi;

                // 按钮
                MainNavButton mnb = new MainNavButton();
                mnb.Tag = func_list[i];
                mnb.id = i;
                mnb.init();
                mnb.HorizontalAlignment = HorizontalAlignment.Left;
                mnb.VerticalAlignment = VerticalAlignment.Top;
                mnb.Margin = new Thickness(0, (i + 1) * 72 - 20 + 3, 0, 0);
                if (i == 0)
                {
                    mnb.change_state();
                }
                mnbs.Add(mnb);
                this.main.Children.Add(img);
                this.main.Children.Add(mnb);
            }

        }
    }
}
