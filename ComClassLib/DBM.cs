using ComClassLib.core;
using System;
using System.IO;

namespace ComClassLib.DB {
    public enum DbName {
        LoginDb,    //登录数据库
        IndexDb,    //索引数据库
        CurrDb,     //当前分库图像数据库
        TmpDbA,     //临时数据库A
        TmpDbB      //临时数据库A
    }
    public class DBM {

        #region 登录表管理 -- 停用

        private static string dbPwd = "jsXnl";
        private static string security = "Js028xNl";


        /// <summary>
        /// 创建登录数据库 加密 
        /// </summary>
        private static void CreateLoginDB(string dbDbFullName) {
            SqliteHelper loginDB = null;
            try {
                if (!System.IO.File.Exists(dbDbFullName)) {
                    FileOp.FileHelper.CreateDir(Path.GetDirectoryName(dbDbFullName));
                    //不存在文件 创建加密数据库
                    SqliteHelper.CreateDbWithPwd(dbDbFullName, dbPwd);
                }
                SqliteHelper.GenerateSqlite(DbName.IndexDb.ToString(), dbDbFullName, dbPwd);
                loginDB = SqliteHelper.GetSqlite(DbName.IndexDb.ToString());
                if (loginDB.IsTableExist("login")) {
                    //CreateLoginTB();
                    CreateFaultTB();
                }
            } catch {

            } finally {
                if (loginDB != null) {
                    loginDB.CloseDb();
                }
            }

        }
        /// <summary>
        /// 创建登录表
        /// </summary>
        private static void CreateLoginTB(SqliteHelper db) {
            try {

                string sTable = "create table login(uId INTEGER primary key AUTOINCREMENT, uName varchar(50) not null," +
                        "uPwd  varchar(100) not null,loginDatetime  datetime NOT NULL DEFAULT(datetime('now', 'localtime')))";
                db.ExecuteNonQuery(sTable, null);
                string pwd = Crypto.DesEncrypt("123");
                string sql = $"insert into login (uName,uPwd) values( 'admin','{pwd}')";
                db.ExecuteNonQuery(sql, null);

            } catch (Exception ex) {

                MsgBox.Error("创建登录表错误！\n详情信息：\n" + ex.ToString());
            }
        }
        /// <summary>
        /// 创建缺陷表
        /// </summary>
        private static void CreateFaultTB() {
            try {
                string strCreateFaultTB = "create table FaultInfo( pId INT64 primary key,imgGUID INT64,unitId int,fault varchar(255),mark varchar(100),faultLevel varchar(5),"
                 + "isAI int DEFAULT 1, analyzeDate datetime NOT NULL DEFAULT(datetime('now', 'localtime')), " +
                 "confirmDate datetime, confirmUser varchar(50), confirmResult int DEFAULT -1, memo varchar(100) )";

                SqliteHelper loginDB = SqliteHelper.GetSqlite(DbName.IndexDb.ToString());
                loginDB.ExecuteNonQuery(strCreateFaultTB, null);
            } catch (Exception ex) {
                MsgBox.Error("创建缺陷表错误！\n详情信息：\n" + ex.ToString());
            }
        }
        #endregion 

        #region 索引数据库管理 -- 加密
        public static SqliteHelper CreateIndDB(string dbDbFullName) {
            SqliteHelper indexDB = null;
            try {
                if (!System.IO.File.Exists(dbDbFullName)) {
                    FileOp.FileHelper.CreateDir(Path.GetDirectoryName(dbDbFullName));
                    //不存在文件 创建加密数据库
                    // SqliteHelper.CreateDbWithPwd(dbDbFullName, dbPwd);
                }
                indexDB = SqliteHelper.GenerateSqlite(DbName.IndexDb.ToString(), dbDbFullName);
                if (!indexDB.IsTableExist("picInfoInd")) {
                    //若库中表不存在，则创建
                    string strImgInofTb = "CREATE TABLE picInfoInd(imgGUID INT64 PRIMARY KEY ,cId INTEGER,shootTime INT64,poleNum TEXT,KMValue TEXT,STN TEXT,SubDBId int);";
                    indexDB.ExecuteNonQuery(strImgInofTb);
                    //创建定位缺陷表 
                    const string sCreateLocFaultTB = "CREATE TABLE locFaultInfo(imgGUID INT64 PRIMARY KEY ,ExistFault INTEGER,sJson TEXT);";
                    indexDB.ExecuteNonQuery(sCreateLocFaultTB);
                    // 创建 线路信息表 stationInfo
                    const string strCreateStationTB = "CREATE TABLE stationInfo(sId INTEGER PRIMARY KEY AUTOINCREMENT,sLineName varchar(50),sStartStation varchar(50),sEndStation varchar(50),iType tinyint,taskDate DATE );";
                    indexDB.ExecuteNonQuery(strCreateStationTB);
                    // 创建登录表
                    CreateLoginTB(indexDB);
                    // 创建缺陷表
                    CreateFaultTB();
                }
            } catch (Exception ex) {
                if (indexDB != null) {
                    indexDB.CloseDb();
                    indexDB = null;
                }
                MsgBox.Error("创建索引数据库错误！\n详情信息：\n" + ex.ToString());
            }

            return indexDB;
        }
        /// <summary>
        /// 写入站点信息
        /// </summary>
        public static bool WriteStationInfo(StationInfo station) {

            SqliteHelper indexDB = SqliteHelper.GetSqlite(DbName.IndexDb.ToString());
            string sSql = string.Format("insert into stationInfo (sLineName,sStartStation,sEndStation ,iType,taskDate)values ( '{0}','{1}','{2}',{3},'{4}' )"
                , station.LineName, station.StartStation, station.EndStation, station.IType, DateTime.Now.ToString("s"));
            try {
                SqliteHelper.GetSqlite(DbName.IndexDb.ToString()).ExecuteNonQuery(sSql, null);
            } catch (Exception) {
                MsgBox.Error("写入任务信息失败！");
                return false;
            }

            return true;
        }

        //创建点击信息表
        public static void CreateProcesedInfoTB() {
            try {
                SqliteHelper sqlite = SqliteHelper.GetSqlite(DbName.IndexDb.ToString());
                if (sqlite != null && !sqlite.IsTableExist("processedInfo")) {
                    string sSql = "create table  processedInfo(pInfoId INTEGER primary key AUTOINCREMENT,imgGUID int64 not null,clickUser varchar(50))";
                    sqlite.ExecuteNonQuery(sSql);
                }
            } catch (Exception) {
                MsgBox.Error("创建点击信息表失败！");
            }
        }

        #endregion

        #region 分表管理

        /// <summary>
        /// 创建分表 -- 只包含图像数据
        /// </summary>
        public static SqliteHelper CreateCurrentDB(string dbDbFullName) {
            SqliteHelper currImgDB = null;
            try {
                currImgDB = SqliteHelper.GenerateSqlite(DbName.CurrDb.ToString(), dbDbFullName);
                if (!currImgDB.IsTableExist("imgInfo")) {
                    //若图像表不存在 创建 图像表
                    string strCreateImgTb = "CREATE TABLE imgInfo(imgGUID INT64 PRIMARY KEY ,imgContent BLOB,sJson TEXT );";
                    currImgDB.ExecuteNonQuery(strCreateImgTb);
                }
            } catch (Exception e) {
                MsgBox.Error($"分库-{dbDbFullName}-创建失败\n详细信息\n" + e.ToString());
                if (currImgDB != null) {
                    currImgDB.CloseDb();
                    currImgDB = null;
                }
            }

            return currImgDB;
        }


        #endregion
        //创建点击信息表
        public static void CreateClickInfoTB(SqliteHelper IndexDB) {
            try {

                if (IndexDB == null) {
                    IndexDB = SqliteHelper.GetSqlite(DbName.IndexDb.ToString());
                }

                if (!IndexDB.IsTableExist("processedInfo")) {
                    string strCreateClickInfoTB = "create table  processedInfo(pInfoId INTEGER primary key AUTOINCREMENT,imgGUID int64 not null,clickUser varchar(50))";
                    IndexDB.ExecuteNonQuery(strCreateClickInfoTB, null);
                }
            } catch (Exception ex) {
                MsgBox.Error("创建点击信息表错误！\n详情信息：\n" + ex.ToString());
            }
        }






    }
}
