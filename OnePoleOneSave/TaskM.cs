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
    //一杆一档 存储
    public class TaskM {

        #region 静态参数定义
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
            m_sGeoDbId = 9;                                                 //几何参数存储数据库Id
            m_sImgDbId = 10;                                                //图像二进制存储数据库Id
            m_sInfoDbIdx = 11;                                              //图像信息数据库Id
            m_sAIFaultDbId = 12;                                            //智能识别缺陷数据库Id.
            m_iSaveImgNumByOnce = Settings.Default.ISaveImgNumByOnce;       //从内存数据库中一次批量读取的的图像数量 默认99张-实际为100张
        }
        #endregion

        //图像存储节点
        struct ImgSaveNode {
            public string StationName;//线路信息
            public string sKM_Pole;//公里标_杆号            
            public Int64 minImgTimeStamp;//最小时间戳
            public Int64 maxImgTimeStamp;//最大时间戳
        }

        #region 属性定义
        private RedisHelper imgDB; //图像数据库
        private RedisHelper imgInfoDB; //图像信息数据库        
        private long _iImgInd, _iTotalSaveNum;//当前图像id，当前定位id,当前缺陷id，当前删除图像id



        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        private ManualResetEvent _resetEvent;
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
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            _resetEvent = new ManualResetEvent(true);
            //_mongodb = new MongodbAccessImpl(Settings.Default.mongoDbIp, Settings.Default.mongoDbPort);

        }

        bool isStop = false;

        //任务1，不断读取MongoDB，并识别杆号变动
        private void TaskGetPoleKM() {
            CallInfo?.Invoke($"--> 读取匹配数据");
            Int64 imgTimeStamp = 0;
            ImgSaveNode imgNode = new ImgSaveNode();
            while (_taskRunning) {
                //找到比当前时间戳大的定位信息132430780599149500 132430780617149500 132430780678989500 132430780696989500
                List<BsonDocument> lstImgInfos = new List<BsonDocument>();
                try {
                  lstImgInfos = _mongodb.FindDataByLimit(imgTimeStamp, 1000);//132429982497195159
                } catch {
                }
                if (lstImgInfos == null || lstImgInfos.Count <= 1) {
                    //检查是否停止  132433202798556000 132433202807556000 132433202891446000 132433203258796000 132433203267796000 132433203294806000
                    try {
                        CallInfo?.Invoke($"--> 检查任务是否停止1");
                        if (!_mongodb.Connect()) {
                            isStop = true;
                            break;
                        }
                    } catch {

                    }
                    //read Stop()
                }
                if (lstImgInfos == null) continue;
                for (int i = 0; i < lstImgInfos.Count; i++) {
                    var imgInfo = lstImgInfos[i];
                    string sPoleNum = imgInfo.GetValue("基础支柱号").AsString;
                    if (imgTimeStamp == 0) {//起步
                        imgTimeStamp = imgInfo.GetValue("检测时间").AsInt64;
                        imgNode.minImgTimeStamp = imgTimeStamp;
                        CurrPoleNum = sPoleNum;
                        continue;
                    }
                    if (CurrPoleNum != sPoleNum) { //如果不相等 改变当前支柱号和 获取目录    
                        imgNode.StationName = imgInfo.GetValue("StationName").AsString;
                        imgNode.sKM_Pole = $"{imgInfo.GetValue("公里标（米）").ToString()}_{sPoleNum}"; //公里标_杆号
                        imgTimeStamp = imgInfo.GetValue("检测时间").AsInt64;
                        imgNode.maxImgTimeStamp = imgTimeStamp;
                        CurrPoleNum = sPoleNum;
                        lock (obj) {
                            queImgInfo.Enqueue(imgNode);
                        }
                        imgNode = new ImgSaveNode();
                        imgNode.minImgTimeStamp = imgTimeStamp;

                        break;
                    }
                    imgTimeStamp = imgInfo.GetValue("检测时间").AsInt64;
                }
            }
        }

        //任务2，不断读取节点信息存储并匹配吊弦图像
        private void TaskMatchImg() {
            ImgSaveNode imgNode;
            CallInfo?.Invoke($"--> 读取吊弦数据");
            //直到有值为止
            while (queImgInfo.Count == 0) {
                Thread.Sleep(1000);//休眠10秒
            }

            lock (obj) {
                imgNode = queImgInfo.Dequeue();
                CallInfo?.Invoke($"--> 匹配杆号:{imgNode.sKM_Pole}");
            }
            while (_taskRunning) {

                //获取数据起始下标--防止redis数据库崩溃
                bool redisConn = false;
                while (!redisConn) {
                    try {
                        string lsImgInd = imgInfoDB.StringGet("LstImgInd");
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

                    } else if (imgTimeStamp < imgNode.maxImgTimeStamp) {
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
                            CallInfo?.Invoke($"--> 杆号：{imgNode.sKM_Pole} #共存储:{_iTotalSaveNum}条");
                        }
                    } else {
                        //当前图像信息时间戳>杆号记录点时间戳 垮杆了则重新获取
                        while (queImgInfo.Count == 0) {
                            //若队列里面没有数据 判断任务是否结束
                            CallInfo?.Invoke($"--> 检查任务是否停止2");
                            if (isStop) {
                                TaskStop();
                            }
                            Thread.Sleep(10000);//休眠10秒
                        }
                        lock (obj) {
                            imgNode = queImgInfo.Dequeue(); //直到有值为止
                            CallInfo?.Invoke($"--> 匹配杆号:{imgNode.sKM_Pole}");
                        }
                        --i;
                    }
                }
                if (imgKeys.Length == 0) {
                    Thread.Sleep(1000);//若没有图像获取到休眠1分钟 
                } else {
                    _iImgInd += imgKeys.Length;
                    LogRecord();//日志数据记录
                }
            }
        }


        //监听任务停止 --- 暂不知道 如何停止任务
        public void ListenTaskStop() {
            string tastInfo = imgInfoDB.StringGet("tastInfo");
            if (!string.IsNullOrEmpty(tastInfo) && tastInfo.Equals("stop")) {
                Task.Run(() => { TaskStop(); });
            }
        }

        //任务：监听任务是否开始。
        public void ListenTaskStart() {
            //查找数据库监听任务状态
            while (true) {
                try {
                    imgDB = new RedisHelper(m_sImgDbId);                     //图像数据库ID
                    imgInfoDB = new RedisHelper(m_sInfoDbIdx);               //图像信息数据库ID
                } catch {
                    Thread.Sleep(1000); //若redis 无法连接则3分钟监听一次
                    continue;
                }
                //int ind = 0;
                //while (true) {
                //    StringBuilder sss = new StringBuilder();
                //    string[] imgKeys = imgInfoDB.ListRange("list", _iImgInd, _iImgInd + 10000);
                //    for (int i = 0; i < imgKeys.Length; i++) {

                //        string imgKey = imgKeys[i];

                //        string sJson = imgInfoDB.StringGet(imgKey);
                //        if (string.IsNullOrEmpty(sJson)) {
                //            continue;
                //        }

                //        //考虑一种情况，redis数据库崩溃了 丢失了很多图像， 中间很多定位信息无用直接跳过

                //        //t: 132430543471152878  min 132430663964619500  max 
                //        PicInfo picInfo = JsonHelper.GetModel<PicInfo>(sJson);
                //        Int64 imgTimeStamp = picInfo.UTC;//ConvertUTC(imgKey);
                //        sss.AppendLine($"id:{++ind},imgKey:{imgKey},CID:{picInfo.CID},UTC:{imgTimeStamp}" );

                //    }

                //    if (imgKeys.Length == 0) {
                //        break;
                //    }

                //    _iImgInd += imgKeys.Length;
                //    FileHelper.SaveTextFile("d:\\ImgKey.txt", sss.ToString(),true);
                // }
                //return;

                CallInfo?.Invoke("--> 任务[开启信号]监听。。。。。。");
                if (Settings.Default.ListenTask) { //需要监听任务是否开启
                    string tastInfo = imgInfoDB.StringGet("tastInfo");

                    if (!string.IsNullOrEmpty(tastInfo) && tastInfo.Equals("start")) {
                        Task.Run(() => { TaskStart(); });
                        break;
                    }
                    Thread.Sleep(10000); //1分钟监听一次
                } else {
                    //不监听任务字段，任务开启以唐源数据库是否访问作为依据
                    Task.Run(() => { TaskStart(); });
                    break;
                }
            }
        }
        /// <summary>
        /// 开始任务 
        /// </summary>
        public void TaskStart() {


            CurrPoleNum = "";//任务开始 设置空杆号
                             //MessageBox.Show("monogoDBIP:" + Settings.Default.mongoDbIp + " port" + Settings.Default.mongoDbPort);

            _mongodb = new MongodbAccessImpl(Settings.Default.mongoDbIp, Settings.Default.mongoDbPort);
            //  _mongodb = new MongodbAccessImpl("192.168.1.101", "26017");
            //CallInfo?.Invoke("--> 任务[数据]监听（1分钟）。。。。。。");
            while (true) {
                try {
                    bool flag = _mongodb.Connect();
                    if (flag) {
                        CallInfo?.Invoke("--> 任务[数据]监听成功！");
                        break;
                    }
                    CallInfo?.Invoke("--> 任务[数据]监听（1分钟）。。。。。。");
                    Thread.Sleep(10000); //若没有读取则1分钟后重新尝试   
                } catch {
                    Thread.Sleep(3000); //若没有读取则1分钟后重新尝试
                }

            }

            CallInfo?.Invoke("--> 任务[数据]监听成功！");
            //while (true) {
            //    if (_mongodb.Connect()) {
            //        CallInfo?.Invoke("--> 任务[数据]监听成功！");
            //        break;
            //    }
            //    CallInfo?.Invoke("--> 任务[数据]监听（1分钟）。。。。。。");
            //    Thread.Sleep(1000); //若没有读取则1分钟后重新尝试               
            //}

            //确保 数据库连接成功！
            TaskPath = string.Empty;

            //确保 MongoDB获取路径成功！
            while (true) {
                CallInfo?.Invoke("--> 获取路径中。。。。。。");
                Thread.Sleep(3000); //若没有读取则1分钟后重新尝试
                TaskPath = _mongodb.GetFullDir();
                if (string.IsNullOrEmpty(TaskPath)) {
                    CallInfo?.Invoke("-->MongoDB路径获取失败！");
                    Thread.Sleep(3000); //若没有读取则1分钟后重新尝试
                    continue;
                }
                CallInfo?.Invoke("-->MongoDB路径获取成功！");
                break;
            }
            CallInfo?.Invoke($"--> 任务开启: {TaskPath}");
            // MessageBox.Show("MongoDB路径读取成功！"+ TaskPath);
            //创建图像存储节点队列
            lock (obj) {
                queImgInfo = new Queue<ImgSaveNode>();
            }
            if (Settings.Default.isDelAllDB) {
                RedisHelper.ClearAllDB();//清除所有数据
            }
            _iImgInd = 0;
            _iTotalSaveNum = 0;
            LogRecord();//日志数据记录
            isStop = false;//是否停止
            _taskRunning = true;//任务开始状态
            //开启读取MongoDB数据库任务--不断识别存储节点信息（杆号+公里标）
            Task TaskGetSaveNode = new Task(TaskGetPoleKM, _token);
            TaskGetSaveNode.Start();
            //开启读取Redis数据库中的图像任务--读取图像信息与存储节点进行匹配
            Task TaskSaveImg = new Task(TaskMatchImg, _token);
            TaskSaveImg.Start();


        }

        private void TaskStop() {
            if (false == _taskRunning) {
                return;
            }

            if (_mongodb != null) {
                _mongodb.DisConnect();
            }

            CallInfo?.Invoke($"--> 任务停止: {TaskPath}");
            _taskRunning = false;//任务结束状态
            Thread.Sleep(10000);
            ListenTaskStart();


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
