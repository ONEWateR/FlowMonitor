using onewater.flowmonitor.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace onewater.flowmonitor.core
{
    /// <summary>
    /// 历史模块。
    /// 
    /// 记录程序的历史流量信息。
    /// </summary>
    public class Histroy
    {
        static SQLiteDBHelper db = null;

        /// <summary>
        /// 初始化
        /// </summary>
        public static bool Init()
        {
            try
            {
                string dbPath = Environment.CurrentDirectory + "/" + "flow_history.db";

                //如果不存在改数据库文件，则创建该数据库文件 
                if (!System.IO.File.Exists(dbPath))
                {
                    SQLiteDBHelper.CreateDB(dbPath);
                }

                db = new SQLiteDBHelper(dbPath);

                string sql;
                sql = "CREATE TABLE IF NOT EXISTS flow(fid integer PRIMARY KEY autoincrement DEFAULT (1),pid integer, flow_up long, flow_down long, note_date date);";
                db.ExecuteNonQuery(sql, null);  // 创建 flow 表

                sql = "CREATE TABLE IF NOT EXISTS program(pid integer PRIMARY KEY autoincrement DEFAULT (1), name varchar(30), path text, describe text);";
                db.ExecuteNonQuery(sql, null);  // 创建 program 表

            }
            catch (Exception e)
            {
                //FM_Info.showError(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 写入数据库
        /// </summary>
        /// <returns></returns>
        public static void Write()
        {
            foreach (Flow f in FlowMonitor.GetMonitor().GetViewData())
            {
                WriteDataByFlow(f);
            }
        }

        /// <summary>
        /// 将Flow数据写入至数据库
        /// </summary>
        /// <param name="f">Flow对象</param>
        public static void WriteDataByFlow(Flow f)
        {
            
            // 1 : 判断该程序是否存在数据库中  Y：下一步 N：新建
            // 2 ：查看今天是否存在该PID。     Y：更新  N：新建
            //Flow f = (Flow)de.Value;
            string sql;
            string now = DateTime.Now.ToString("yyyy-MM-dd");
            long up, down;
            sql = string.Format("select pid from program where name='{0}' AND path='{1}';", f.name, f.path);
            object id = db.ExecuteScalar(sql, null);
            if (id == null)
            {

                sql = string.Format("insert into program (name,path,describe) values ('{0}','{1}','{2}');", f.name, f.path, f.describe);
                db.ExecuteNonQuery(sql, null);
                id = db.ExecuteScalar("select MAX(pid) from program;", null);
            }

            sql = string.Format("select COUNT(*) from flow where pid={0} AND note_date='{1}';", id, now);
            // 如果不存在今天的程序流量
            if (Convert.ToInt32(db.ExecuteScalar(sql, null)) == 0)
            {

                // 则插入
                sql = string.Format("insert into flow (pid,flow_up,flow_down,note_date) values ({0},{1},{2},'{3}');", id, f.up, f.down, now);
                db.ExecuteNonQuery(sql, null);

            }
            else
            {
                sql = string.Format("select * from flow where pid={0} AND note_date='{1}';", id, now);
                DataRow dr = db.ExecuteDataTable(sql, null).Rows[0];
                up = (long)dr["flow_up"];
                down = (long)dr["flow_down"];
                up += f.up - f.note_up;
                down += f.down - f.note_down;
                f.note_up = f.up;
                f.note_down = f.down;
                sql = string.Format("update flow set flow_up={0}, flow_down={1} where pid={2} AND note_date='{3}';", up, down, id, now);
                db.ExecuteNonQuery(sql, null);
            }
        }

        /// <summary>
        /// [JS]
        /// 获取某年某月的流量使用情况。
        /// 即那月份的每天流量使用情况。
        /// 返回一个数组字符串。
        /// eg: 100,256,123,....,115
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public static string getMonthFlows(int year, int month)
        {

            StringBuilder result = new StringBuilder();

            int days = DateTime.DaysInMonth(year, month);
            DateTime dt = new DateTime(year, month, 1);
            double flow;

            while (days-- != 0)
            {
                string date = dt.ToString("yyyy-MM-dd");
                string sql = "select SUM(flow_up)+SUM(flow_down) from flow where note_date='" + dt.ToString("yyyy-MM-dd") + "';";
                object sum = db.ExecuteScalar(sql, null);

                if (System.DBNull.Value.Equals(sum))
                {
                    flow = 0;
                }
                else
                {
                    flow = Math.Round(((long)sum) / 1024 / 1024.0, 0);
                }

                //flow = Convert.ToInt32(db.ExecuteScalar("select SUM(flow_up)+SUM(flow_down) from flow where note_date='{" + dt.ToString("yyyy-MM-dd") + "}';", null)) / 1024 / 1024;
                result.Append(flow.ToString() + (days == 0 ? "" : ","));
                dt = dt.AddDays(1);
            }

            return result.ToString();
        }

        /// <summary>
        /// [JS]
        /// 获取某天的流量使用情况
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string[] getCertainDay(int year, int month, int day)
        {
            string[] result = new string[2] { string.Empty, string.Empty };

            /*
             * select name,(flow_up+flow_down) from flow,program 
               where note_date='2014-01-16' AND flow.pid = program.pid 
             * order by (flow_up+flow_down) desc
             * limit 10;
             */

            string sql = "select name,(flow_up+flow_down) from flow,program where note_date='" + new DateTime(year, month, day).ToString("yyyy-MM-dd") + "' AND flow.pid = program.pid order by (flow_up+flow_down) desc limit 10;";
            DataTable dt = db.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                result[0] += dt.Rows[i][0] + (dt.Rows.Count == i + 1 ? "" : ","); // 程序名称
                result[1] += dt.Rows[i][1] + (dt.Rows.Count == i + 1 ? "" : ","); // 程序流量
            }


            return result;
        }

        /// <summary>
        /// 获取某天的流量使用情况
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns></returns>
        public static UInt32[] GetTheDayFlow(DateTime dt)
        {
            UInt32[] result = new UInt32[2] { 0, 0 };
            string sql;
            object value;

            sql = "select SUM(flow_up) from flow where note_date='" + dt.ToString("yyyy-MM-dd") + "';";
            value = db.ExecuteScalar(sql, null);
            if (System.DBNull.Value.Equals(value)) result[0] = 0;
            else result[0] = Convert.ToUInt32(value);


            sql = "select SUM(flow_down) from flow where note_date='" + dt.ToString("yyyy-MM-dd") + "';";
            value = db.ExecuteScalar(sql, null);
            if (System.DBNull.Value.Equals(value)) result[1] = 0;
            else result[1] = Convert.ToUInt32(value);

            return result;
        }



        public static DataTable GetDataTale(DateTime dt)
        {
            /*
             * select flow.pid,name,path,flow_up,flow_down from flow,program 
             * where note_date='2014-01-17' AND flow.pid = program.pid 
             * order by (flow_up+flow_down) desc;
             */
            string sql = "select flow.pid,name,path,flow_up,flow_down from flow,program where note_date='" + dt.ToString("yyyy-MM-dd") + "' AND flow.pid = program.pid order by (flow_up+flow_down) desc;";
            DataTable result = db.ExecuteDataTable(sql, null);
            return result;
        }

        public static DataTable GetDataTale(string month)
        {
            /*
             * select flow.pid,name,path,SUM(flow_up),SUM(flow_down) from flow,program 
             * where strftime("%m",note_date) = "01" AND flow.pid = program.pid 
             * group by name 
             * order by (SUM(flow_up)+SUM(flow_down)) desc;
             */
            string sql = "select flow.pid,name,path,SUM(flow_up),SUM(flow_down) from flow,program where strftime('%m',note_date) = '" + month + "' AND flow.pid = program.pid group by name order by (SUM(flow_up)+SUM(flow_down)) desc;";
            DataTable result = db.ExecuteDataTable(sql, null);
            return result;
        }

        public static DataTable GetUpFlowDataTable()
        {
            string sql = "select name,flow_up from flow,program where note_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND flow.pid = program.pid order by flow_up desc limit 2;";
            DataTable result = db.ExecuteDataTable(sql, null);
            return result;
        }

    }
}
