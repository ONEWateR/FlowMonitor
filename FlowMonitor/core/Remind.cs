using RemindLibrary;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace onewater.flowmonitor.core
{
    /// <summary>
    /// 超限提醒类
    /// 
    /// 当用户使用流量超过警告线的一系列操作由该类负责。
    /// </summary>
    public class Remind
    {

        private UInt32[] theDayFlow = new UInt32[] { };                 // 当天流量 [总，上传]
        private short[] remindTime = new short[] { };                   // 提醒次数 [总，上传]
        private UInt32 warningALL = 0;                                  // 总流量警告线
        private UInt32 warningUP = 0;                                   // 上传流量警告线
        private enum FlowType { UP, ALL }                               // 流量类型

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            // 获取本地存储的数据
            GetDataFromLocal();

            if (Properties.Settings.Default.LastRemindTime.Date != DateTime.Now.Date)
            {
                SetRemindTime();
            }
            SaveRemindDate();

            // 刷新警告上限的数据
            RefreshWarningData(remindTime[0], FlowType.ALL);
            RefreshWarningData(remindTime[1], FlowType.UP);

        }




        /// <summary>
        /// 获取本地数据，包括当天使用的流量大小，提醒次数
        /// </summary>
        private void GetDataFromLocal()
        {
            theDayFlow = Histroy.GetTheDayFlow(DateTime.Now);
            remindTime = new short[] { 
                Properties.Settings.Default.RemindALL,
                Properties.Settings.Default.RemindUP
            };
        }

        /// <summary>
        /// 每秒进行的数据处理
        /// </summary>
        public void Refresh()
        {
            // 获取数据
            theDayFlow = FlowMonitor.GetMonitor().GetTheDayFlow();

            // 判断现在是否为新的一天
            if (DateTime.Now.ToLongTimeString() == "00:00:00")
            {
                theDayFlow[0] = theDayFlow[1] = 0;
                remindTime = new short[] { 0, 0 };
                SetRemindTime(remindTime[0], remindTime[1]);
                SaveRemindDate();
            }

            // 判断是否超过警告线流量
            if (theDayFlow[0] >= warningALL)
            {
                // 提醒超过流量
                ShowRemindWindow();
                // 设置新的警告上限
                RefreshWarningData(++remindTime[0], FlowType.ALL);
                SetRemindTime(remindTime[0], remindTime[1]);
            }
            if (theDayFlow[1] >= warningUP)
            {
                // 提醒超过流量
                ShowRemindWindow();
                RefreshWarningData(++remindTime[1], FlowType.UP);
                SetRemindTime(remindTime[0], remindTime[1]);
            }

        }

        /// <summary>
        /// 显示提醒窗口
        /// </summary>
        private void ShowRemindWindow()
        {
            /*
             * 先判断程序是否禁止了提醒，再判断今天是否禁止了提醒，然后再进行推送
             */
            if (!Properties.Settings.Default.UnableRemind)
            {
                if (!Properties.Settings.Default.UnableRemindTheDay)
                {

                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        // 提醒窗口
                        string content = string.Format("你的流量已超过警告线！！");
                        RemindWindow rw = new RemindWindow("警告", content, RemindIcon.Warning);
                        rw.Show();
                    }));
                    
                }
            }
        }

        /// <summary>
        /// 刷新警告线上限的数据
        /// </summary>
        /// <param name="i">提醒的次数</param>
        /// <param name="dt">流量类型</param>
        private void RefreshWarningData(int i, FlowType dt)
        {
            if (dt == FlowType.ALL)
            {
                warningALL = Properties.Settings.Default.WarningALL + Convert.ToUInt32(450 * Math.Log(i + 1));
                //warningALL *= 1048576;
                warningALL *= 1000000;
            }
            if (dt == FlowType.UP)
            {
                warningUP = Properties.Settings.Default.WarningUP + Convert.ToUInt32(450 * Math.Log(i + 1));
                warningUP *= 1000000;
            }
        }

        /// <summary>
        /// 设置提醒次数
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        private void SetRemindTime(short s1 = 0, short s2 = 0)
        {
            Properties.Settings.Default.RemindALL = s1;
            Properties.Settings.Default.RemindUP = s2;
            Properties.Settings.Default.Save();
        }

        private void SaveRemindDate()
        {
            Properties.Settings.Default.LastRemindTime = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 获取总流量警告线
        /// </summary>
        /// <returns></returns>
        public UInt32 GetWarningALL()
        {
            return warningALL;
        }
    }
}
