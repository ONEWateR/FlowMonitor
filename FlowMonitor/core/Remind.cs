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

        private long[] theDayFlow = new long[] { };                 // 当天流量 [总，上传]
        private short[] remindTime = new short[] { };               // 提醒次数 [总，上传]
        private long allFlow = 0;
        private long upFlow = 0;
        private enum FlowType { UP, ALL }

        /// <summary>
        /// 获取本地数据，包括当天使用的流量大小，提醒次数
        /// </summary>
        private void GetDataFromLocal()
        {
        }

        /// <summary>
        /// 每秒进行的数据处理
        /// </summary>
        private void Refresh()
        {
            // 获取数据

            // 更新进度条

            // 判断现在是否为新的一天
            if (DateTime.Now.ToLongTimeString() == "00:00:00")
            {
                theDayFlow = new long[] { 0, 0 };
                remindTime = new short[] { 0, 0 };
            }

            // 判断是否超过警告线流量

            /*
             * 若超过，先判断程序是否禁止了提醒，再判断今天是否禁止了提醒，然后再进行推送
             */
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
                allFlow = 500 + Convert.ToInt64(450 * Math.Log(i));
            }
            if (dt == FlowType.UP)
            {
                upFlow = 50 + Convert.ToInt64(450 * Math.Log(i));
            }
        }
    }
}
