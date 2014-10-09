FlowMonitor
===========

流量监控

##简介
监控进程流量，并记录流量信息。有助于了解流量的整体去向。

##功能
- 外网流量监控，抛开内网，让流量看的更精准
- 流量使用提醒
- 流量使用记录，明白流量去向

##截图
![](https://github.com/ONEWateR/FlowMonitor/blob/master/FlowMonitor/screenshot/1.png)
![](https://github.com/ONEWateR/FlowMonitor/blob/master/FlowMonitor/screenshot/2.png)

##那啥
这个算是大一下学期的一个小玩意儿，自从学习了计网后对这些理解的更深了，算是咸丰年前的东西吧。但还是存在一些小的bug。然后现在想把界面、结构方面重写一遍，让他更快、更准。

##项目结构
 |- app
    |- AppConfig.cs (应用配置) 
 |- bin
    |- FlowMonitor.exe (程序主体)
    |- flow_history.db (SQLite数据库，存放历史流量信息)
    |- SharpPcap.dll (抓包)
    |- PacketDotNet.dll (抓包)
    |- RemindLibrary.dll (提醒窗口)
    |- GetPIDByPort.dll (端口PID映射)
    |- System.Data.SQLite.dll (SQLite数据库)
    
 |- common
     |- AutoRun.cs (开机启动)
     |- CAAnimation.cs (动画的再次组装)
     |- SQLiteDBHelper.cs (SQLite操作辅助类)
 |- core
     |- Converter.cs (转换类，WPF数据绑定的转换)
     |- Flow.cs (流量实体)
     |- FlowMonitor.cs (核心文件，负责抓包以及对包的分析，也就是流量监控)
     |- Histroy.cs (历史处理，实则是对db文件的处理)
     |- Remind.cs (提醒类，流量超过警告线发出提醒)
 |- res (资源文件夹，存放控件、样式和图片)
 |- windows (一系列window内容)


##开源协议
MIT
