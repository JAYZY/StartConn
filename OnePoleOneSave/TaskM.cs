using ComClassLib;
using ComClassLib.core;
using ComClassLib.DB;
using ComClassLib.FileOp;
using MongoDB.Bson;
using MongodbAccess;
using OnePoleOneSave.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OnePoleOneSave {

    class ImgSaveNode {
        public string StationName;//线路信息
        public string sKM_Pole;//公里标_杆号            
        public Int64 minImgTimeStamp;//最小时间戳
        public Int64 maxImgTimeStamp;//最大时间戳
        public List<string> lstImgId;//对应的图像id
    }
    //一杆一档 存储
    public class TaskM {

        #region 静态参数定义
        private static readonly int m_sTYDataId;                        //与唐源数据库匹配的信息存储
        private static readonly int m_sInfoDbIdx;                       //图像信息数据库Id:
        private static readonly int m_sImgDbId;                         //图像二进制存储数据库Id
        private static readonly int m_sAIFaultDbId;                     //智能识别缺陷数据库Id
        private static readonly int m_sGeoDbId;                         //几何参数数据库
        private static readonly int m_iMemLimit;                        //MemLimit:内存限制 （单位M)
        private static readonly int m_iSaveImgNumByOnce;                //一次事务处理的的图像数量
        private static readonly int m_iDelDataByOnce;                   //一次删除的图像数据
        private static readonly int m_iSaveGeoDataNumByOnce;            //一次事务处理的几何参数数据大小
        private static readonly int m_iSubDbSize;                       //分库大小 -1---只有一个分库 默认值；3000


        //静态构造函数
        static TaskM() {
            m_sTYDataId = 8;                                                //唐源数据匹配信息存储库Id
            m_sGeoDbId = 9;                                                 //几何参数存储数据库Id
            m_sImgDbId = 10;                                                //图像二进制存储数据库Id
            m_sInfoDbIdx = 11;                                              //图像信息数据库Id
            m_sAIFaultDbId = 12;                                            //智能识别缺陷数据库Id.
            m_iSaveImgNumByOnce = Settings.Default.ISaveImgNumByOnce;       //从内存数据库中一次批量读取的的图像数量 默认99张-实际为100张
        }
        #endregion

        //图像存储节点


        #region 属性定义
        private RedisHelper imgDB; //图像数据库
        private RedisHelper imgInfoDB, TYData; //图像信息数据库        
        private long _iImgInd, _iTotalSaveNum;//当前图像id，当前定位id,当前缺陷id，当前删除图像id



        private CancellationTokenSource _tokenSource, _networkTokenSource;
        private CancellationToken _token, _networkToken;
        private ManualResetEvent _resetEvent, _networkResetEvent;



        private string _taskName, _taskDir, _taskBackDir; //任务存储目录 -- 一个任务一个单独的目录                
        private bool _SaveImg, _taskRunning;
        private MongodbAccessImpl _mongodb;
        //完整的任务文件名称（路径+文件名）
        public string TaskIndFileFullName { get; set; }
        //完整的备份任务文件名称（路径+文件名）
        public string TaskIndBackFileFullName { get; set; }
        //当前的分库完整路径+文件名
        public SqliteHelper CurrSubDb { get; set; }
        public SqliteHelper IndexDb { get; set; }
        private string TaskPath { get; set; }

        private string CurrPoleNum { get; set; }
        private string CurrStationName { get; set; }//当前站点名称
        //节点队列
        private Queue<ImgSaveNode> queImgInfo = null;
        private object obj = new object();
        #endregion
        public static Action<string> CallInfo { get; set; }


        /// <summary>
        /// 任务管理构造函数
        /// </summary>
        public TaskM() {
            TaskIni();
        }
        /// <summary>
        /// 任务初始化
        /// </summary>
        private void TaskIni() {

            _networkTokenSource = new CancellationTokenSource();

            _networkToken = _networkTokenSource.Token;

            _networkResetEvent = new ManualResetEvent(true);

            _mongodb = new MongodbAccessImpl(Settings.Default.mongoDbIp, Settings.Default.mongoDbPort);
            _taskRunning = false;
        }

        bool isStop = false;

        //任务1，不断读取MongoDB，并识别杆号变动
        private void TaskGetPoleKM() {
            CallInfo?.Invoke($"T#读取匹配数据!\n");
            Int64 imgTimeStamp = 0;
            ImgSaveNode imgNode = new ImgSaveNode();
            imgNode.lstImgId = new List<string>();
            while (_taskRunning) {
                //找到比当前时间戳大的定位信息132430780599149500 132430780617149500 132430780678989500 132430780696989500
                List<BsonDocument> lstImgInfos = null;
                bool IsConnMongoDb = true;
                try {
                    lstImgInfos = _mongodb.FindDataByLimit(imgTimeStamp, 100);//132429982497195159
                } catch {
                    //MongoDB数据库出错！
                    IsConnMongoDb = false;
                }
                if (!IsConnMongoDb) {
                    try { // MongoDB数据库连接不上，重新连接数据库
                        IsConnMongoDb = _mongodb.Connect();
                    } catch {
                        IsConnMongoDb = false;
                    }
                }
                if (!IsConnMongoDb) {
                    CallInfo?.Invoke("T#重新连接【任务】服务器(数据库)---[失败]。3秒后重试！\n");
                    Thread.Sleep(3000);
                    continue;
                }
                if (lstImgInfos == null & lstImgInfos.Count < 1) {
                    try {
                        CallInfo?.Invoke($"T#等待任务数据,5秒后重试!\n");
                        Thread.Sleep(5000);
                    } catch {
                        //MongoDB数据库出错。不管！
                    }
                    continue;
                }
                #region 读取杆号
                string sPoleNum, sStationName;
                for (int i = 0; i < lstImgInfos.Count; i++) {
                    var imgInfo = lstImgInfos[i];
                    sPoleNum = imgInfo.GetValue("基础支柱号").AsString;
                    sStationName = imgInfo.GetValue("StationName").AsString;

                    if (imgTimeStamp == 0) {//起步
                        imgTimeStamp = imgInfo.GetValue("检测时间").AsInt64;
                        imgNode.minImgTimeStamp = imgTimeStamp;
                        imgNode.StationName = CurrStationName;
                        imgNode.sKM_Pole = $"{imgInfo.GetValue("公里标（米）").ToString()}_{sPoleNum}"; //公里标_杆号

                        CurrPoleNum = sPoleNum;
                        CurrStationName = sStationName;
                        //写入当前站名称
                        WriteTaskInfo("stationname", CurrStationName);
                        continue;
                    } else if (CurrPoleNum != sPoleNum) { //如果不相等 改变当前支柱号和 获取目录   
                        imgTimeStamp = imgInfo.GetValue("检测时间").AsInt64;
                        imgNode.maxImgTimeStamp = imgTimeStamp;
                        CurrPoleNum = sPoleNum;

                        lock (obj) {
                            queImgInfo.Enqueue(imgNode);
                        }

                        imgNode = new ImgSaveNode();
                        imgNode.minImgTimeStamp = imgTimeStamp; //最小和最大有个重叠。
                        imgNode.StationName = CurrStationName;
                        imgNode.sKM_Pole = $"{imgInfo.GetValue("公里标（米）").ToString()}_{sPoleNum}"; //公里标_杆号
                        imgNode.lstImgId = new List<string>();
                        break;
                    } else {
                        imgTimeStamp = imgInfo.GetValue("检测时间").AsInt64;//更新检测时间
                    }
                    if (!CurrStationName.Equals(sStationName)) {
                        CurrStationName = sStationName;
                        //写入当前站点名称
                        WriteTaskInfo("stationname", CurrStationName);
                    }
                }
                #endregion
            }
        }

        //任务2，不断读取Redis节点信息存储并匹配吊弦图像
        private void TaskMatchImg() {
            ImgSaveNode imgNode;
            CallInfo?.Invoke($"T#读取吊弦数据!\n");
            //直到有值为止
            while (queImgInfo.Count == 0) {
                Thread.Sleep(3000);//休眠3秒
            }

            lock (obj) {
                imgNode = queImgInfo.Dequeue();
                CallInfo?.Invoke($"T#匹配杆号:{imgNode.sKM_Pole}\n");
            }
            while (_taskRunning) {

                //获取数据起始下标--防止redis数据库崩溃
                bool redisConn = false;
                while (!redisConn) {
                    try {
                        if (RedisHelper.IsConnect) {
                            if (!RedisHelper.ReConnect()) {
                                Thread.Sleep(1000);//休息1秒重试
                                continue;
                            }
                        }
                        string lsImgInd = TYData.StringGet("LstImgInd");

                        if (string.IsNullOrEmpty(lsImgInd)) {
                            _iImgInd = 0;
                        }
                        _iImgInd = long.Parse(lsImgInd);
                        redisConn = true;
                    } catch (Exception) {
                        _iImgInd = 0;
                        redisConn = false;

                    }
                }

                if (queImgInfo.Count < 3) {
                    Thread.Sleep(1000);//防止redis数据写的太慢来不及匹配                    
                }
                string[] imgKeys = imgInfoDB.ListRange("list", _iImgInd, _iImgInd + m_iSaveImgNumByOnce);

                for (int i = 0; i < imgKeys.Length; ++i) {
                    string imgKey = imgKeys[i];
                    string sJson = imgInfoDB.StringGet(imgKey);
                    if (string.IsNullOrEmpty(sJson)) {
                        continue;
                    }

                    //考虑一种情况，redis数据库崩溃了 丢失了很多图像， 中间很多定位信息无用直接跳过

                    //t: 132430543471152878  min 132430663964619500  max 
                    PicInfo picInfo = JsonHelper.GetModel<PicInfo>(sJson);
                    Int64 imgTimeStamp = picInfo.UTC;//ConvertUTC(imgKey);

                    //说明redis中的图像 比 当前MongoDB数据早，扔掉
                    if (imgTimeStamp < imgNode.minImgTimeStamp) {
                        continue;

                    }//min<imgTimeStamp<max
                    else if (imgTimeStamp < imgNode.maxImgTimeStamp) {

                        imgNode.lstImgId.Add(imgKey);//添加到列表中

                        //获取图像数据 并存储               
                        byte[] imgData = imgDB.GetByte(imgKey);

                        //PATH+ 战区  \ 公里标_杆号\  时间_k公里标_杆号_区域编号_相机编号 115600414_K5778_Z2-25_6_13 
                        string imgFullPath = Path.Combine(TaskPath, imgNode.StationName);

                        // \ 公里标_杆号\ 
                        imgFullPath = Path.Combine(imgFullPath, "K" + imgNode.sKM_Pole);
                        FileHelper.CreateDir(imgFullPath);//如果不存在则创建当前文件夹
                        string cid = FileHelper.iTos(picInfo.CID);
                        string imgName = GetImgName(imgKey, cid, imgNode.sKM_Pole);
                        //MessageBox.Show(imgFullPath);
                        bool flag = FileHelper.ImgToFile(Path.Combine(imgFullPath, imgName), imgData);
                        if (flag) {
                            //成功存储 进行计数
                            ++_iTotalSaveNum;
                            CallInfo?.Invoke($"T#杆号：{imgNode.sKM_Pole} #共存储:{_iTotalSaveNum}条\n");
                        }
                    } else {
                        //当前图像信息时间戳>杆号记录点时间戳 垮杆了则重新获取imgTimeStamp>=imgNode.maxImgTimeStamp
                        while (queImgInfo.Count == 0) {
                            //若队列里面没有数据//休眠5秒                              
                            Thread.Sleep(5000);
                        }
                        string sjsonSaveNode = JsonHelper.GetJson(imgNode);
                        WriteTaskInfo(imgNode.sKM_Pole, sjsonSaveNode);//存储
                        lock (obj) {
                            imgNode = queImgInfo.Dequeue(); //直到有值为止                           
                            CallInfo?.Invoke($"T#匹配杆号:{imgNode.sKM_Pole}\n");
                        }
                        --i;
                    }
                }
                if (imgKeys.Length == 0) {
                    Thread.Sleep(10000);//若没有图像获取到休眠10秒 
                } else {
                    _iImgInd += imgKeys.Length;
                    // LogRecord();//日志数据记录
                    
                    //记录读取图像列表的下标
                    WriteTaskInfo("LstImgInd", _iImgInd.ToString());
                    //记录存储的图像总数量
                    WriteTaskInfo("saveImgNum", _iTotalSaveNum.ToString());
                }
            }
        }


        //监听任务停止 --- 监听网络停止信号
        public void ListenTaskStop(DataType.NetTaskCmd cmd) {
            if (cmd == DataType.NetTaskCmd.TaskEnd) {//任务结束
                if (false == _taskRunning) {
                    return;
                }
                _taskRunning = false;//任务结束状态
                if (_mongodb != null) {
                    _mongodb.DisConnect();
                }
                //取消所有的Task
                _tokenSource.Cancel();
                CallInfo?.Invoke($"T#======任务【{_taskName}】结束======\n");
                //写入任务状态
                WriteTaskInfo("taskInfo", "over");

                Thread.Sleep(60000);//任务结束后 1分钟以后再重新监听任务开始
                ListenTask();
            }
        }

        /// <summary>
        /// 监听任务开始
        /// </summary>
        private void ListenTaskStart() {

            #region Redis服务器和数据库状态
            CallInfo?.Invoke("T#连接【吊弦】服务器(数据库)");
            int tryNum = 0;
            while (true) {
                try {
                    imgDB = new RedisHelper(m_sImgDbId);                     //图像数据库ID
                    imgInfoDB = new RedisHelper(m_sInfoDbIdx);               //图像信息数据库ID
                    TYData = new RedisHelper(m_sTYDataId);                    //唐源数据匹配数据库  
                } catch (Exception) {
                    if (++tryNum % 6 == 0) {
                        CallInfo?.Invoke("#·\n");
                        CallInfo?.Invoke("T#连接【吊弦】服务器(数据库)");
                    } else {
                        CallInfo?.Invoke("# · ");
                    }
                    // CallInfo?.Invoke("#[失败!]，2秒后重试！\n");
                    Thread.Sleep(2000); //若redis 无法连接则10秒钟监听一次
                    continue;
                }
                break;
            }
            CallInfo?.Invoke("#[成功!]\n");
            #endregion
            CurrPoleNum = "";//任务开始 设置空杆号

            #region 唐源 服务器MongoDB数据库状态
            //唐源数据 任务监听中。
            tryNum = 0;
            CallInfo?.Invoke("T#连接【任务】服务器(数据库)");
            while (true) {
                try {

                    bool flag = _mongodb.Connect();
                    if (flag) {
                        CallInfo?.Invoke("#[成功!]\n");
                        break;
                    }
                    if (++tryNum % 6 == 0) {
                        CallInfo?.Invoke("#·\n");
                        CallInfo?.Invoke("T#连接【任务】服务器(数据库)");
                    } else {
                        CallInfo?.Invoke("# · ");
                    }
                    Thread.Sleep(2000); //若没有读取则2秒后重新尝试
                } catch {
                    if (++tryNum % 6 == 0) {
                        CallInfo?.Invoke("#·\n");
                        CallInfo?.Invoke("T#连接【任务】服务器(数据库)");
                    } else {
                        CallInfo?.Invoke("# · ");
                    }
                    Thread.Sleep(2000); //若没有读取则2秒后重新尝试
                }
            }
            #endregion

            //确保 数据库连接成功！

            #region 监听任务开启 -- MongoDB获取路径成功！
            while (true) {
                TaskPath = string.Empty;
                CallInfo?.Invoke("T#等待任务开启......");

                TaskPath = _mongodb.GetFullDir();
                if (string.IsNullOrEmpty(TaskPath)) {
                    CallInfo?.Invoke("#[失败！]5秒后重试");
                    Thread.Sleep(5000);//若没有任务则5秒钟后重新尝试
                    continue;
                }
                _taskName = ParseLineName(TaskPath);
                CallInfo?.Invoke($"#[成功]\n\t[任务名称]：{_taskName}\n\t[任务路径]：{TaskPath}\n");
                break;
            }
            #endregion

            //清除redis所有数据
            if (Settings.Default.isDelAllDB) {
                RedisHelper.ClearAllDB();
            }

            //写入redis  任务信息            
            if (String.IsNullOrEmpty(_taskName)) {
                WriteTaskInfo("taskInfo", _taskName);
            }

            //创建图像存储节点队列【一个新任务一个新队列】 
            lock (obj) {
                queImgInfo = new Queue<ImgSaveNode>();
            }

            _iImgInd = 0;
            _iTotalSaveNum = 0;

            //日志数据记录
            WriteTaskInfo("LstImgInd", "0");
            //记录存储的图像总数量
            WriteTaskInfo("saveImgNum", "0");

            isStop = false;//是否停止
            //任务开始状态
            _taskRunning = true;


            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            _resetEvent = new ManualResetEvent(true);
            //开启读取MongoDB数据库任务--不断识别存储节点信息（杆号+公里标）
            Task TaskGetSaveNode = new Task(TaskGetPoleKM, _token);
            TaskGetSaveNode.Start();
            //开启读取Redis数据库中的图像任务--读取图像信息与存储节点进行匹配
            Task TaskSaveImg = new Task(TaskMatchImg, _token);
            TaskSaveImg.Start();
        }



        //任务：任务监听 
        public void ListenTask() {
            //开始任务监听 
            ListenTaskStart();
            //同步的--任务开始监听后，再开始网络指令监听
            NetworkHelper.CallFunc = ListenTaskStop;
            NetworkHelper nw = new NetworkHelper();
            new Task(nw.Receive, _networkToken).Start(); //开启网络任务
        }
        public void StopAllTask() {
            _tokenSource.Cancel();
            _networkTokenSource.Cancel();
        }
        //获取线路名称
        private string ParseLineName(string path) {
            path = path.Trim();
            if (path.EndsWith("\\") || path.EndsWith("/")) {
                path = path.Substring(0, path.Length - 1);
            }
            string strRtn = path.Substring(path.LastIndexOf("\\"));
            return strRtn;
        }


        /// <summary>
        /// 写入任务信息到 redis
        /// </summary>
        private void WriteTaskInfo(string key, string value) {
            try {
                TYData.StringSet(key, value);
            } catch (Exception) {
                //写入失败 不做任何操作
            }
        }

        /// <summary>
        /// 开始任务 
        /// </summary>


        private void TaskStop() {



        }

        public Int64 ConvertUTC(string time) {
            //MessageBox.Show("T:" + time);
            int year = int.Parse($"20{time.Substring(0, 2)}");
            int month = int.Parse(time.Substring(2, 2));
            int day = int.Parse(time.Substring(4, 2));
            int hour = int.Parse(time.Substring(6, 2));
            int minute = int.Parse(time.Substring(8, 2));
            int second = int.Parse(time.Substring(10, 2));
            int millisecond = int.Parse(time.Substring(12, 3));
            DateTime dt = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local);

            return dt.ToFileTimeUtc();
        }

        private DateTime ConvertDateTime(string sTimeKey) {
            int year = int.Parse($"20{sTimeKey.Substring(0, 2)}");
            int month = int.Parse(sTimeKey.Substring(2, 2));
            int day = int.Parse(sTimeKey.Substring(4, 2));
            int hour = int.Parse(sTimeKey.Substring(6, 2));
            int minute = int.Parse(sTimeKey.Substring(8, 2));
            int second = int.Parse(sTimeKey.Substring(10, 2));
            int millisecond = int.Parse(sTimeKey.Substring(12, 3));
            DateTime dt = new DateTime(year, month, day, hour, minute, second, millisecond);
            return dt;
        }

        //获取图像名称 命名规则：  //时间_k公里标_杆号_区域编号_相机编号   
        private string GetImgName(string sTimeKey, string cid, string sKMPole) {
            string imgName = $"{sTimeKey.Substring(6, 2)}{sTimeKey.Substring(8, 2)}{sTimeKey.Substring(10, 2)}{sTimeKey.Substring(12, 3)}" +
                $"_K{sKMPole}_5_{cid}.jpg";
            return imgName;
        }

        private void LogRecord() {
            try {
                //记录读取图像列表的下标
                imgInfoDB.StringSet("LstImgInd", _iImgInd.ToString());
                //记录存储的图像总数量
                imgInfoDB.StringSet("saveImgNum", _iTotalSaveNum.ToString());
            } catch {


            }

        }

    }
}
