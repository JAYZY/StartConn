namespace OnePoleOneSave {
    partial class MainFrm {
     
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.btnMinium = new System.Windows.Forms.Button();
            this.rTxtBoxTip = new System.Windows.Forms.RichTextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 32F);
            this.label1.ForeColor = System.Drawing.Color.SlateBlue;
            this.label1.Location = new System.Drawing.Point(223, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(369, 57);
            this.label1.TabIndex = 0;
            this.label1.Text = "一杆一档后台服务";
            // 
            // btnMinium
            // 
            this.btnMinium.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinium.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMinium.Location = new System.Drawing.Point(754, 0);
            this.btnMinium.Name = "btnMinium";
            this.btnMinium.Size = new System.Drawing.Size(52, 49);
            this.btnMinium.TabIndex = 1;
            this.btnMinium.Text = "最小化";
            this.btnMinium.UseVisualStyleBackColor = true;
            this.btnMinium.Click += new System.EventHandler(this.btnMinium_Click);
            // 
            // rTxtBoxTip
            // 
            this.rTxtBoxTip.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rTxtBoxTip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rTxtBoxTip.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rTxtBoxTip.Location = new System.Drawing.Point(0, 102);
            this.rTxtBoxTip.Name = "rTxtBoxTip";
            this.rTxtBoxTip.Size = new System.Drawing.Size(806, 119);
            this.rTxtBoxTip.TabIndex = 3;
            this.rTxtBoxTip.Text = "";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "吊弦系统总控平台";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 221);
            this.ControlBox = false;
            this.Controls.Add(this.rTxtBoxTip);
            this.Controls.Add(this.btnMinium);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "# 一杆一档存储服务 #";
            this.Shown += new System.EventHandler(this.MainFrm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMinium;
        private System.Windows.Forms.RichTextBox rTxtBoxTip;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

