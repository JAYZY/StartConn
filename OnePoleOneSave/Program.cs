using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnePoleOneSave {
    static class Program {
        public static MainFrm mainForm;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Run();// Application.Run(new MainFrm());
            //      //TaskM taskM = new TaskM();
            //      //taskM.ListenTaskStart();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try {
                Run();
            } catch (Exception ex) {
                MessageBox.Show("程序发生未知原因异常,将终止运行!异常是\n" + ex.ToString());
            }



        }
        static void Run() {

            bool createNew;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew)) {
                if (createNew) {
                    Application.Run(mainForm = MainFrm.GetInstance());
                    //Application.Run(mainForm = FrmMain.GetInstance());
                } else {
                    MessageBox.Show(@"客户端程序已经在运行中...");
                    System.Threading.Thread.Sleep(100);
                    System.Environment.Exit(1);
                }
            }
        }
    }
}
