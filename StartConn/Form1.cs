using ComClassLib.DB;
using System;
using System.Windows.Forms;

namespace StartConn {


    public partial class Form1 : Form {
        int m_sInfoDbIdx = 11;
        DllCall.xinTo134_pRecvFun pFun;
        public Form1() {
            InitializeComponent();
            imgInfoDB = new RedisHelper(m_sInfoDbIdx);               //图像信息数据库ID
        }
       
        RedisHelper imgInfoDB = null;
        private void button1_Click(object sender, EventArgs e) {

            string str = imgInfoDB.StringGet("tastInfo");
            imgInfoDB.StringSet("tastInfo", "start");

        }
        public void fnResCallBack(xinTo134_sRecvMsgType machStatus, IntPtr pData, int nDataSize, IntPtr pUserData) {
            //xinTo134_StartInfo info=(xinTo134_StartInfo)pData;
            MessageBox.Show(machStatus.ToString());

            return;
        }


        private void timer1_Tick(object sender, EventArgs e) {
            lblSaveImgNum.Text = $"存储吊弦图像数：{imgInfoDB.StringGet("saveImgNum")}条";
        }
    }
}
