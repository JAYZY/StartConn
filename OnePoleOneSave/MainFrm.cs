using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnePoleOneSave {
    public partial class MainFrm : Form {

        #region 创建单实例对象
        private static MainFrm _frmParent;
        private static object _obj = new object();
        public static MainFrm GetInstance() {
            if (_frmParent == null) {
                lock (_obj) {
                    if (_frmParent == null) {
                        _frmParent = new MainFrm();
                    }
                }
            }
            return _frmParent;
        }
        
        private MainFrm() {
            InitializeComponent();
            
            TaskM.CallInfo = ShowTaskMsg;
            TaskM taskM = new TaskM();
            Task.Run(() => { taskM.ListenTaskStart(); });
            //taskM.ListenTaskStart();
           

        }
        #endregion
        private void ShowTaskMsg(string sMsg) {
            if (rTxtBoxTip.InvokeRequired) {
                Action<string> a = ShowTaskMsg;
                rTxtBoxTip.Invoke(a, sMsg);
            } else {
                try {
                    if (rTxtBoxTip.Lines.Length > 30) {
                        rTxtBoxTip.Clear();
                    }
                    string sTip = $"# {DateTime.Now.ToString()} : {sMsg}";
                    rTxtBoxTip.AppendText(sTip + "\n");
                    rTxtBoxTip.ScrollToCaret();
                } catch { }
            }
        }

        private void btnMinium_Click(object sender, EventArgs e) {
            this.Visible = false;
            notifyIcon1.ShowBalloonTip(5000, "提示", "双击一杆一档吊弦图像存储服务器", ToolTipIcon.Info);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e) {
            this.Visible = !this.Visible;
        }

        private void MainFrm_Shown(object sender, EventArgs e) {
            this.Visible = false;
        }
    }
}
