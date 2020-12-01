using ComClassLib.Properties;
using System;
using System.Net;
using System.Net.Sockets;
//网络连接操作
namespace ComClassLib.core {
    public class NetworkHelper {
        //定义主机的IP及端口 
        private IPAddress Ipaddress;
        private IPEndPoint IpEnd;
        private Socket ConnSocket;
        public static Action<DataType.NetTaskCmd> CallFunc { get; set; } //回调函数
        // bool IsConnect;
        public NetworkHelper() {
            //定义主机的IP 
            Ipaddress = IPAddress.Parse(Settings.Default.NetTarkServIP);
            //将IP地址和端口号绑定到网络节点endpoint上
            IpEnd = new IPEndPoint(Ipaddress, Settings.Default.NetTarkPort); //端口号
        }

        public object _lock = new object();

        public bool ConnectSvr() {
            //尝试连接--采用同步方法
            try {
                lock (_lock) {
                    //重新链接
                    if (ConnSocket != null && !ConnSocket.Connected) {
                        ConnSocket.Close();
                    } //定义套接字类型
                    ConnSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    ConnSocket.Connect(IpEnd);
                }
            } catch (SocketException) {
                //Console.Write("Fail to connect server");
                // Console.Write(e.ToString());
                return false;
            }
            return true;
        }
        /// <summary>
        /// 任务监听一直打开
        /// </summary>
        public void Receive() {
            while (true) {
                while (ConnSocket == null || !ConnSocket.Connected) {
                    ConnectSvr();//链接失败 反复尝试
                    CallFunc?.Invoke(DataType.NetTaskCmd.Disconn);
                    //Console.Write("Fail to connect server");
                }
                try {
                    //定义接收数据的长度
                    byte[] data = new byte[1024];
                    int recv = ConnSocket.Receive(data);
                    CallFunc?.Invoke(DataType.NetTaskCmd.Conn);
                    if (recv == 0) {//服务器断开 重新连接
                        CallFunc?.Invoke(DataType.NetTaskCmd.Reconn);
                        continue;
                    } else {
                        int cmdState = BitConverter.ToInt32(data, 6);
                        if (1 == cmdState) {
                            CallFunc?.Invoke(DataType.NetTaskCmd.TaskStart);
                        } else
                        if (2 == cmdState) {
                            CallFunc?.Invoke(DataType.NetTaskCmd.TaskEnd);
                        }
                    }
                } catch { }
            }
        }
    }
}
