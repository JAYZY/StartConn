using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StartConn {
    #region 需要的结构体

    public enum xinTo134_sRecvMsgType {
        xinTo134_sRecvMsgType_Start = 1,  // 开始检测  //对应结构体=xinTo134_StartInfo
        xinTo134_sRecvMsgType_Stop = 2,   // 结束检测
                                          //xinTo134_sRecvMsgType_curPos = 3,    // 当前位置 //对应结构体=xinTo134_PosMsg
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct xinTo134_StartInfo {
        //路径规则 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string szLineName; //线路名称  
        public int nDirection; // 0：未知 1：上行 2：下行
        public int nLineDirectionBackRun;      //上下行是否逆行 bool true逆行，false不逆行
        public int nTireDirection;             //轮的前进方向，用于计算定位设备，和接收系统之间的距离， 1车为1,8车位0。车头为1，车尾为0
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string sStation; //起始站
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string sPole; //起始杆号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string eStation; //结束站区
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string ePole; //结束杆号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string mongodbDataBaseName;
    };

    

    //typedef void (* xinTo134_pRecvFun) (xinTo134_sRecvMsgType nType, IntPtr pData, int nDataSize, void* pUserData);

    #endregion


    public class DllCall {


        //定义一个委托，其返回类型和形参与方法体的返回类型形参一致
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]//一定要加上这句，要不然C#中的回调函数只要被调用一次，程序就异常退出了！！！
        public delegate void xinTo134_pRecvFun(xinTo134_sRecvMsgType machStatus,  IntPtr pData, int nDataSize, IntPtr pUserData);



        [DllImport("dll/xinto134_client.dll", EntryPoint = "xinTo134_ConnectSever", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int xinTo134_ConnectSever(string szIp,int nPort);

        [DllImport("dll/xinto134_client.dll", EntryPoint = "xinTo134_OnRecvMsg", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int xinTo134_OnRecvMsg(xinTo134_pRecvFun pFun, IntPtr pUserData);
    }
}
