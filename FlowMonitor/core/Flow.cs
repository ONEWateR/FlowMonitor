using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace onewater.flowmonitor.core
{
    class Flow : INotifyPropertyChanged
    {
        public Flow() { 
            this.name = "UnKnow"; 
        }

        public Flow(int i) { 
            this.id = i; 
        }

        public int id;                                  // ID标识
        public string name { get; set; }                // 名字
        public string path;                             // 路径
        public string describe { get; set; }            // 描述
        public Icon icon { get; set; }                  // 图标
        public UInt32 up { get; set; }                  // [外] 上传流量
        public UInt32 down { get; set; }                // [外] 下载流量
        public UInt32 up_in;                            // [内] 上传流量
        public UInt32 down_in;                          // [内] 下载流量
        public bool active = true;                      // 程序是否退出
        public List<int> pid = new List<int> { };       // PID列表，为了应对部分程序多开的情况下，将其全部进程归为同一个程序

        // 外网总流量
        public UInt32 total_out() { 
            return up + down; 
        }

        // 内网总流量
        public UInt32 total_in() { 
            return up_in + down_in; 
        }

        // 总流量
        public UInt32 total_all() {
            return total_out() + total_in(); 
        }

        // 记录上次的上下流量
        public UInt32 lastUp = 0;
        public UInt32 lastDown = 0;

        // 上下传送速度
        private UInt32 upSpeedValue = 0;
        private UInt32 downSpeedValue = 0;

        /// <summary>
        /// 上传速度
        /// </summary>
        public UInt32 UpSpeed
        {
            get
            {
                return this.upSpeedValue;
            }

            set
            {
                if (value != this.upSpeedValue)
                {
                    this.upSpeedValue = value;
                    NotifyPropertyChanged("UpSpeed");
                }
            }
        }

        /// <summary>
        /// 下载速度
        /// </summary>
        public UInt32 DownSpeed
        {
            get
            {
                return this.downSpeedValue;
            }

            set
            {
                if (value != this.downSpeedValue)
                {
                    this.downSpeedValue = value;
                    NotifyPropertyChanged("DownSpeed");
                }
            }
        }

        /// <summary>
        /// 上传流量
        /// </summary>
        public UInt32 UpFlow
        {
            get
            {
                return this.up;
            }

            set
            {
                this.UpSpeed = value - this.up;
                if (value != this.up)
                {
                    this.up = value;
                    NotifyPropertyChanged("UpFlow");
                    NotifyPropertyChanged("TotalFlow");
                }
            }
        }

        /// <summary>
        /// 下载流量
        /// </summary>
        public UInt32 DownFlow
        {
            get
            {
                return this.down;
            }

            set
            {
                this.DownSpeed = value - this.down;
                if (value != this.down)
                {
                    this.down = value;
                    NotifyPropertyChanged("DownFlow");
                    NotifyPropertyChanged("TotalFlow");
                }
            }
        }

        /// <summary>
        /// 获取外网流量
        /// </summary>
        public UInt32 TotalFlow
        {
            get
            {
                return this.down + this.up;
            }
        }

        /// <summary>
        /// 重写判断Flow相等的方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(Flow)))
            {
                return (this.name.Equals(((Flow)obj).name) && this.path.Equals(((Flow)obj).path));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 数据变动监听
        /// </summary>
        /// <param name="info"></param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// 将流量大小转换成最高单位
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public static string ChangeFlow(double flow)
        {
            string[] unit = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            for (; i < unit.Length; i++)
            {
                if (flow < 1000)
                {
                    break;
                }
                else
                {
                    flow = flow / 1000;
                }
            }
            return Math.Round(flow, 2) + " " + unit[i];
        }
    }
}
