using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace onewater.flowmonitor.app
{
    class AppConfig
    {
        // 导航栏数据 - [正常状态图标, 选择后图标, 选项名称]
        public static string[] NavData = new string[] { 
            "fw_icon_n,fw_icon_s,流量监控",
            "tb_icon_n,tb_icon_s,历史流量",
            "rs_icon_n,rs_icon_s,设置",
            "about_icon_n,about_icon_s,关于"
        };

        public static string IconPath = "../res/img/icon/";
    }
}
