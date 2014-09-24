using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

/* 引入SharpPcap */
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.AirPcap;
using SharpPcap.WinPcap;
using PacketDotNet;

using System.Net.Sockets;
using System.Diagnostics;
using System.Timers;
using System.ComponentModel;
using System.Drawing;

namespace onewater.flowmonitor.core
{
    class FlowMonitor
    {
           
        #region Declare Variable 声明变量

        private static FlowMonitor backstageMonitor = null;

        // 通过端口获取PID
        [DllImport("GetPIDByPort.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern uint GetProcessIdByPort(uint type, uint port);

        // 获取程序的图标
        [DllImport("shell32.dll")]
        static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);


        // 变量声明
        enum ProtocolType { TCP, UDP };                             // 协议类型 ： TCP 和 UDP
        IPAddress hostIP;                                           // 本机IP地址
        int psid = -1;                                              // 程序内部有流量程序的唯一标识ID
        long downFlow = 0;                                          // 下载流量（程序关闭则归0，下同）
        long upFlow = 0;                                            // 上传流量
        Hashtable PID_port_tcp = new Hashtable();                   // 端口 PID 临时映射表 TCP
        Hashtable PID_port_udp = new Hashtable();                   // 端口 PID 临时映射表 UDP
        Hashtable processFlow = new Hashtable();                    // 进程流量
        ArrayList packet_array = new ArrayList();                   // 临时存储包
        bool firstHistory = true;                                   // 打开历史是否读取数据库的布尔值
        BackgroundWorker backgroundWorker1;                         // 监控专用线程
        Timer MainTimer = new Timer();                              // 每秒对抓包数据进行分析的 Timer
        UInt32[] TheDayFlow = { 0, 0 };                             // 当天 上传 下载 的流量 （外网）
        ObsCollection<Flow> ViewData = new ObsCollection<Flow>();   // 视图数据
        ArrayList SpecialPID = new ArrayList () { 0, 4, 32, 128 };  // 特殊进程 PID 不显示
        string[] SpecialPath = { "" };                              // TODO：特殊程序不显示

        #endregion

        /// <summary>
        /// 私有构建函数。
        /// </summary>
        private FlowMonitor()
        {
            Initialise();
        }

        /// <summary>
        /// 返回后台监控。
        /// </summary>
        /// <returns></returns>
        public static FlowMonitor GetMonitor()
        {
            if (backstageMonitor == null)
                backstageMonitor = new FlowMonitor();
            return backstageMonitor;
        }

        #region Function 函数


        /* ------------------ Function ~ ------------------ */
        /// <summary>
        /// 初始化。
        /// </summary>
        private void Initialise() {
            // 获取本机IP地址
            hostIP = getHostIPAddress();

            // 新建线程监控。
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker_DoWork;
            backgroundWorker1.RunWorkerAsync();

            // 创建数据库记录流量使用情况
            try
            {
                //SQLite.Create();
                //TheDayFlow = SQLite.GetTheDayFlow(DateTime.Now);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }

            // 每秒处理数据
            MainTimer.Elapsed += ManageDataTimerEvent;
            MainTimer.Interval = 1000;
            MainTimer.Enabled = true;

            // 每分钟写入流量信息。
            Timer FlowWriteTimer = new Timer();
            FlowWriteTimer.Elapsed += (a, b) =>
            {
                //SQLite.Write();
            };
            FlowWriteTimer.Interval = 60 * 1000;
            FlowWriteTimer.Start();

        }

        /// <summary>
        /// 通过判断IP地址是否在3类IP地址区间内进而得出是否为内网。
        /// 
        /// A类：10.0.0.0 ~ 10.255.255.255
        /// B类：172.16.0.0 ~ 172.31.255.255
        /// C类：192.168.0.0 ~ 192.168.255.255
        /// 
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns></returns>
        bool IsInnerNet(IPAddress ip)
        {
            byte[] address_byte = ip.GetAddressBytes();

            bool a = IsInArea(10, 10, address_byte[0]) && IsInArea(0, 255, address_byte[1]) && IsInArea(0, 255, address_byte[2]) && IsInArea(0, 255, address_byte[3]);
            bool b = IsInArea(172, 172, address_byte[0]) && IsInArea(16, 31, address_byte[1]) && IsInArea(0, 255, address_byte[2]) && IsInArea(0, 255, address_byte[3]);
            bool c = IsInArea(192, 192, address_byte[0]) && IsInArea(168, 168, address_byte[1]) && IsInArea(0, 255, address_byte[2]) && IsInArea(0, 255, address_byte[3]);

            return (a || b || c);
        }

        /// <summary>
        /// 判断某数字是否在一个区间内。
        /// 即 i 属于 [a,b]
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        bool IsInArea(byte a, byte b, byte i)
        {

            return (i >= a && i <= b);
        }

        /// <summary>
        /// 获取主机IP。
        /// </summary>
        /// <returns></returns>
        private IPAddress getHostIPAddress()
        {
            IPHostEntry host;
            IPAddress localIP = null;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip;
                    break;
                }
            }
            return localIP;
        }

        /// <summary>
        /// 返回视图数据。
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Flow> GetViewData() {
            return ViewData;
        }


        /// <summary>
        /// 通过PID获取Flow类。
        /// 包括进程的名称、描述、路径和图标等信息。
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private Flow getFlowFromPID(int pid)
        {

            // 不存在映射，则新建进程流量类
            if (processFlow[pid] == null)
            {
                Flow f = new Flow(psid++);
                Process p = Process.GetProcessById(pid);
                f.name = p.ProcessName;
                f.pid.Add(pid);
                try
                {
                    // 获取进程路径和图标
                    string path = p.MainModule.FileName;
                    f.path = path;
                    // .FileVersionInfo.FileDescription
                    f.describe = p.MainModule.FileVersionInfo.FileDescription;
                    f.icon = Icon.FromHandle(ExtractIcon(IntPtr.Zero, path, 0));

                }
                catch (Exception e)
                {
                    f.path = "null";
                    // 默认图标
                    f.icon = new Icon("program_icon.ico");
                    //BitmapImage bt = new BitmapImage();

                }

                int i = -1;
                // 重复处理
                foreach (DictionaryEntry de in processFlow)
                {
                    //if (de.Value == null) MessageBox.Show("asd");
                    if (((Flow)de.Value).Equals(f))
                    {
                        i = (int)de.Key;
                        psid--;
                        break;
                    }
                }

                // 不存在重复进程的情况下
                if (i == -1)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        // 特殊程序不显示
                         if (SpecialPID.IndexOf(pid) == -1)
                            ViewData.Add(f);
                    }));
                    processFlow[pid] = f;
                }
                else
                {
                    processFlow[pid] = processFlow[i];
                    ((Flow)processFlow[pid]).pid.Add(pid);
                }
            }

            return (Flow)processFlow[pid];
        }

        /// <summary>
        /// 端口获取PID。
        /// </summary>
        /// <param name="port"></param>
        /// <param name="type"></param>
        /// <returns></returns> 
        private int getPIDformPort(int port, ProtocolType type = ProtocolType.TCP)
        {
            // TCP
            if (type == ProtocolType.TCP)
            {
                // 端口映射PID的关系存放在哈希表
                if (PID_port_tcp[port] == null)
                {
                    PID_port_tcp.Add(port, Convert.ToInt32(GetProcessIdByPort(1, Convert.ToUInt32(port))));
                }
                return (int)PID_port_tcp[port];
            }

            // UDP
            if (type == ProtocolType.UDP)
            {
                // 端口映射PID的关系存放在哈希表
                if (PID_port_udp[port] == null)
                {
                    PID_port_udp.Add(port, Convert.ToInt32(GetProcessIdByPort(2, Convert.ToUInt32(port))));
                }
                return (int)PID_port_udp[port];
            }
            return 0;
        }



        public UInt32[] GetTheDayFlow() {
            return TheDayFlow;
        }

        /* --------------------------------------------------- */


        #endregion

        /// <summary>
        /// 监听事件。每当捕获到包时触发该事件。
        /// </summary>
        private void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            lock (packet_array)
                packet_array.Add(e.Packet);

        }

        /// <summary>
        /// 多线程运行，启动监控。
        ///     Refer to SharpPcap Example
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 延迟 1s 开启监控
            System.Threading.Thread.Sleep(1000);

            try
            {
                if (CaptureDeviceList.Instance.Count < 1)
                {
                    //MessageBox.Show("No devices were found on this machine");
                    return;
                }

                var devices = CaptureDeviceList.Instance;

                int readTimeoutMilliseconds = 1000;

                for (int i = 0; i < CaptureDeviceList.Instance.Count; i++)
                {

                    var device = CaptureDeviceList.Instance[i];

                    /*
                     * 不开启混杂模式
                     */

                    device.Open(DeviceMode.Normal, readTimeoutMilliseconds);

                    device.OnPacketArrival +=
                        new PacketArrivalEventHandler(device_OnPacketArrival);

                    // Open the device for capturing
                    if (device is AirPcapDevice)
                    {
                        // NOTE: AirPcap devices cannot disable local capture
                        var airPcap = device as AirPcapDevice;
                        airPcap.Open(SharpPcap.WinPcap.OpenFlags.DataTransferUdp, readTimeoutMilliseconds);

                    }
                    else if (device is WinPcapDevice)
                    {
                        var winPcap = device as WinPcapDevice;
                        winPcap.Open(SharpPcap.WinPcap.OpenFlags.MaxResponsiveness | SharpPcap.WinPcap.OpenFlags.Promiscuous, readTimeoutMilliseconds);
                    }
                    else if (device is LibPcapLiveDevice)
                    {
                        var livePcapDevice = device as LibPcapLiveDevice;
                        livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

                    }
                    else
                    {
                        //MessageBox.Show("unknown device type of " + device.GetType().ToString());
                        throw new System.InvalidOperationException("unknown device type of " + device.GetType().ToString());
                    }

                    device.StartCapture();

                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        List<int> ActivePIDs;

        /// <summary>
        /// 关闭监控
        /// </summary>
        public void Close()
        {
            MainTimer.Dispose();
            //Remind.GetRemind().Stop();
            for (int i = 0; i < CaptureDeviceList.Instance.Count; i++)
            {
                var device = CaptureDeviceList.Instance[i];
                device.StopCapture();
            }
            //SQLite.Write();
        }

        int time = 0;

        /// <summary>
        /// 处理捕获的包。
        /// 1秒周期的Timer。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageDataTimerEvent(object sender, EventArgs e)
        {
            Flow flow;

            // 克隆后清空
            ArrayList _packet_array = new ArrayList();
            _packet_array = (ArrayList)packet_array.Clone();
            packet_array.Clear();

            // 声明变量
            RawCapture ep;
            Packet packet;
            int len;
            TcpPacket tcpPacket;
            UdpPacket udpPacket;

            // 遍历1秒内捕获的所有包
            for (int i = 0; i < _packet_array.Count; i++)
            {
                // 处理
                ep = ((RawCapture)_packet_array[i]);
                packet = Packet.ParsePacket(ep.LinkLayerType, ep.Data);
                len = ep.Data.Length;

                // 解析成tcp包和udp包
                tcpPacket = PacketDotNet.TcpPacket.GetEncapsulated(packet);
                udpPacket = PacketDotNet.UdpPacket.GetEncapsulated(packet);

                ProtocolType pt = tcpPacket != null ? ProtocolType.TCP : ProtocolType.UDP;

                if (tcpPacket != null || udpPacket != null)
                {
                    IpPacket ipPacket = (IpPacket)(tcpPacket != null ? tcpPacket.ParentPacket : udpPacket.ParentPacket);

                    // 目的、源的IP地址和端口
                    IPAddress srcIp = ipPacket.SourceAddress;
                    IPAddress dstIp = ipPacket.DestinationAddress;
                    int srcPort = tcpPacket != null ? tcpPacket.SourcePort : udpPacket.SourcePort;
                    int dstPort = tcpPacket != null ? tcpPacket.DestinationPort : udpPacket.DestinationPort;

                    int port;           // 本机端口
                    bool isUp, isIn;    // 是否上传，是否内网

                    // 判断是否为上传
                    if (srcIp.Equals(hostIP))
                    {
                        port = srcPort;
                        isUp = true;
                        isIn = IsInnerNet(dstIp);
                    }
                    else
                    {
                        port = dstPort;
                        isUp = false;
                        isIn = IsInnerNet(srcIp);
                    }

                    flow = getFlowFromPID(getPIDformPort(port, pt));

                    if (isIn && isUp) flow.up_in += Convert.ToUInt32(len);
                    if (isIn && !isUp) flow.down_in += Convert.ToUInt32(len);
                    if (!isIn && isUp) flow.lastUp += Convert.ToUInt32(len);
                    if (!isIn && !isUp) flow.lastDown += Convert.ToUInt32(len);
                }
            }


            // 获取所有处于运行状态的进程ID
            ActivePIDs = new List<int> { };
            foreach (Process p in Process.GetProcesses())
            {
                ActivePIDs.Add(p.Id);
            }

            // 
            foreach (DictionaryEntry de in processFlow)
            {
                Flow f = (Flow)de.Value;

                f.UpFlow += f.lastUp;
                f.DownFlow += f.lastDown;

                TheDayFlow[0] += f.lastUp;
                TheDayFlow[1] += f.lastDown;

                f.UpSpeed = f.lastUp;
                f.DownSpeed = f.lastDown;

                f.lastUp = 0;
                f.lastDown = 0;


                // 查看进程是否运行
                bool active = true;
                foreach (int pid in f.pid)
                {
                    if (ActivePIDs.IndexOf(pid) == -1)
                    {
                        active = false;
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
                        {
                            ViewData.Remove(f);
                        }));
                        break;
                    }
                }
                f.active = active;

            }

            // 清楚临时映射表
            PID_port_tcp.Clear();
            PID_port_udp.Clear();
        }

        public UInt32[] GetTest() {
            return TheDayFlow;
        }
    }

    public class ObsCollection<T> : ObservableCollection<T>
    {
        public void UpdateCollection()
        {
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(
                            System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
    }
}
