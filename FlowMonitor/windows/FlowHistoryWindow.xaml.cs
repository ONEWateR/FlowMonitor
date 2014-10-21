using onewater.flowmonitor.core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// FlowHistoryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FlowHistoryWindow : UserControl
    {

        List<ViewType> viewlist = new List<ViewType> { ViewType.TheMonth, ViewType.Yesterday, ViewType.TheDay };
        public enum ViewType { TheDay, Yesterday, TheMonth };
        ObservableCollection<Flow> BindData = new ObservableCollection<Flow> { };

        public FlowHistoryWindow()
        {
            InitializeComponent();
            this.ViewCombox.SelectedIndex = 2;
            this.ViewList.ItemsSource = BindData;
        }


        private void GetFlowList(ViewType vt)
        {
            // 数据处理
            BindData.Clear();

            DataTable dt = new DataTable();
            if (vt == ViewType.TheDay)
            {
                dt = Histroy.GetDataTale(DateTime.Now);
            }
            else if (vt == ViewType.Yesterday)
            {
                dt = Histroy.GetDataTale(DateTime.Now.AddDays(-1));
            }
            else if (vt == ViewType.TheMonth)
            {
                dt = Histroy.GetDataTale(DateTime.Now.ToString("MM"));
            }

            foreach (DataRow data in dt.Rows)
            {
                Flow f = new Flow();

                int id = Convert.ToInt32(data[0]);
                string name = (string)data[1];
                string path = (string)data[2];
                UInt32 up = Convert.ToUInt32(data[3]);
                UInt32 down = Convert.ToUInt32(data[4]);
                UInt32 total = up + down;

                System.Drawing.Icon icon = null;

                try
                {
                    icon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                }
                catch (Exception e)
                {
                    icon = new System.Drawing.Icon("program_icon.ico");
                }
                
                f.id = id;
                f.name = name;
                f.path = path;
                f.up = up;
                f.down = down;
                f.icon = icon;
                //f.iconPath = icon;

                BindData.Add(f);
            }
        }

        private void ViewCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ((ComboBox)sender).SelectedIndex;


            // 修改数据

            Thread t = null;
            t = new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    GetFlowList(viewlist[index]);

                    // 动态改变窗口宽度
                    if (index == 0)
                    {

                        // Uri uri = new Uri(@"pack://siteoforigin:,,,/chart/history.htm");
                        //web.Navigate(AppDomain.CurrentDomain.BaseDirectory + @"chart\history.htm");

                        //CAAnimation.PlayAnimation(this.Width, DetailedWidth, Window.WidthProperty, this, 1);
                    }
                    else
                    {
                        //if (this.Width != NormalWidth)
                        //CAAnimation.PlayAnimation(this.Width, NormalWidth, Window.WidthProperty, this, 1);
                    }

                }));
                t.Abort();
            });
            t.Start();
        }


        private void ViewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO:
            if (this.ViewCombox.SelectedIndex == 0)
            {
            }
        }


    }
}
