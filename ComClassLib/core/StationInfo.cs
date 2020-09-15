using ComClassLib.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComClassLib.core {
    /// <summary>
    /// 线路信息类
    /// </summary>
    public class StationInfo {
        private int sId;        //线路Id
        private string sLineName;   //
        private string sStartStation;//
        private string sEndStation;//
        private Int16 iType;
        private DateTime taskDate;
        #region  static 属性
        /// <summary>
        /// 登录用户编号
        /// </summary>
        public static int UId { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public static string User { get; set; }
        #endregion
        //#region 创建单实例对象
        //private static StationInfo _stationInfo;
        //private static readonly object _obj = new object();
        //public static StationInfo GetInstance() {
        //    if (_stationInfo == null) {
        //        lock (_obj) {
        //            if (_stationInfo == null) {
        //                _stationInfo = new StationInfo();
        //            }
        //        }
        //    }
        //    return _stationInfo;
        //}
        //private StationInfo() {
        //}
        //#endregion
        public StationInfo() { }

        public StationInfo(string sLineName, string sStartStation, string sEndStation, short iType, DateTime taskDate) {

            this.sLineName = sLineName;
            this.sStartStation = sStartStation;
            this.sEndStation = sEndStation;
            this.iType = iType;
            this.taskDate = taskDate;
        }
        public StationInfo(StationInfo stationInfo) {
            this.sLineName = stationInfo.sLineName;
            this.sStartStation = stationInfo.sStartStation;
            this.sEndStation = stationInfo.sEndStation;
            this.iType = stationInfo.iType;
            this.taskDate = stationInfo.taskDate;
        }

        //检测时间
        public DateTime TaskDate { get => taskDate; set => taskDate = value; }

        /// <summary>
        /// 0 - 上行 1--下行
        /// </summary>
        public int IType { set { iType = (short)value; } get { return iType; } }
        //站区类型上行
        public string SType {
            get { return (iType == 0) ? @"上行" : @"下行"; }
            set { iType = (Int16)(value.Equals("上行") ? 0 : 1); }
        }

        //线路名称
        public string LineName { get => sLineName; set => sLineName = value; }

        public string TaskName { get => $"{StartStation}-{EndStation}_{SType}"; }
        //起始站名称
        public string StartStation { get => sStartStation; set => sStartStation = value; }

        //结束站名称
        public string EndStation { get => sEndStation; set => sEndStation = value; }

        //站区编号
        public int SId { get => sId; set => sId = value; }
        
        public string GetDateStr() {
            return taskDate.ToString("yyyy年MM月dd日");
        }
        //输出 线路12.5
        public string GetShortInfo() {
            return LineName + taskDate.ToString("MM.dd");
        }
        /// <summary>
        /// 从数据库中读取 线路信息
        /// </summary>
        /// <returns></returns>
        public static StationInfo FromDb() {
            SqliteHelper db = SqliteHelper.GetSqlite(DbName.IndexDb.ToString());
            if (db == null)
                return null;
            string sqlStr = "select * from stationInfo";
            DataRow dr = db.ExecuteDataRow(sqlStr, null);
            if (dr == null) {
                return null;
            }
            StationInfo station = new StationInfo();
            station.sId = Int32.Parse(dr[0].ToString());
            station.sLineName = dr[1].ToString();
            station.sStartStation = dr[2].ToString();
            station.sEndStation = dr[3].ToString();
            station.iType = Convert.ToInt16(dr[4]);
            station.taskDate = Convert.ToDateTime(dr[5]);
            return station;
        }
        //public void SaveToDb() {
        //    string sSql = string.Format("insert into stationInfo (sLineName,sStartStation,sEndStation ,sType,taskDate)values ( '{0}','{1}','{2}',{3},'{4}' )"
        //        , this.LineName, this.sStartStation, this.sEndStation, this.iType, this.taskDate.ToString("s"));
        //    try {
        //        SqliteHelper.GetSqlite(Settings.Default.MDB).ExecuteNonQuery(sSql, null);
        //    } catch (Exception) {
        //        MessageBox.Show("写入任务信息失败！");
        //    }
        //}

        //public void UpdateDb() {
        //    string sSql = string.Format("update stationInfo set sLineName='{0}',sStartStation='{1}',sEndStation='{2}', sType={3},taskDate='{4}' where sId={5}"
        //        , this.LineName, this.sStartStation, this.sEndStation, this.iType, this.taskDate.ToString("s"), this.SId);
        //    try {
        //        SqliteHelper.GetSqlite(Settings.Default.MDB).ExecuteNonQuery(sSql, null);
        //    } catch (Exception) {
        //        MessageBox.Show(@"更新任务信息失败！");
        //    }

        //}


    }
}
