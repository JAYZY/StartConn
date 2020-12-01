using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnePoleOneSave {
    public partial class ExitWarning : Form {
        public ExitWarning() {
            InitializeComponent();
            
        }

        private void btnExit_Click(object sender, EventArgs e) {
            if (tbPwd.Text.Equals("wytc")) {
                this.DialogResult = DialogResult.OK;
                this.Close();
            } else {
                ComClassLib.MsgBox.Error("密码错误，没有退出权限！");
                tbPwd.Text = "";
            }           
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
