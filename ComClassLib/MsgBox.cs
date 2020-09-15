using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComClassLib {
    public class MsgBox {
        public static DialogResult Show(String p) {
            return MessageBox.Show(p, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Show(String p, String title) {
            return MessageBox.Show(p, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static DialogResult Warning(String p, String title) {
            return MessageBox.Show(p, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static DialogResult WarningYesNo(String p, String title) {
            return MessageBox.Show(p, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }
        public static DialogResult Error(String p) {
            return MessageBox.Show(p, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult Error(String p, String title) {
            return MessageBox.Show(p, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult YesNo(String p) {
            return MessageBox.Show(p, "", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
        }

        public static DialogResult YesNo(String p, String title) {
            return MessageBox.Show(p, title, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
        }

        public static DialogResult NoYes(String p) {
            return MessageBox.Show(p, "", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                , MessageBoxDefaultButton.Button2);
        }

        public static DialogResult NoYes(String p, String title) {
            return MessageBox.Show(p, title, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                , MessageBoxDefaultButton.Button2);
        }
    }


}
