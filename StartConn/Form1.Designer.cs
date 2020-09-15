namespace StartConn {
    partial class Form1 {
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
            this.button1 = new System.Windows.Forms.Button();
            this.ipAddressInput1 = new DevComponents.Editors.IpAddressInput();
            this.button2 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblSaveImgNum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ipAddressInput1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(72, 149);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 91);
            this.button1.TabIndex = 0;
            this.button1.Text = "测试-开启任务";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ipAddressInput1
            // 
            this.ipAddressInput1.AutoOverwrite = true;
            // 
            // 
            // 
            this.ipAddressInput1.BackgroundStyle.Class = "DateTimeInputBackground";
            this.ipAddressInput1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ipAddressInput1.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.ipAddressInput1.ButtonFreeText.Visible = true;
            this.ipAddressInput1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ipAddressInput1.Location = new System.Drawing.Point(151, 44);
            this.ipAddressInput1.Name = "ipAddressInput1";
            this.ipAddressInput1.Size = new System.Drawing.Size(239, 26);
            this.ipAddressInput1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ipAddressInput1.TabIndex = 1;
            this.ipAddressInput1.Value = "192.168.1.199";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(72, 268);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(129, 91);
            this.button2.TabIndex = 0;
            this.button2.Text = "测试-停止任务";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblSaveImgNum
            // 
            this.lblSaveImgNum.AutoSize = true;
            this.lblSaveImgNum.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSaveImgNum.ForeColor = System.Drawing.Color.Red;
            this.lblSaveImgNum.Location = new System.Drawing.Point(300, 167);
            this.lblSaveImgNum.Name = "lblSaveImgNum";
            this.lblSaveImgNum.Size = new System.Drawing.Size(168, 21);
            this.lblSaveImgNum.TabIndex = 2;
            this.lblSaveImgNum.Text = "存储吊弦图像数：0条 ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblSaveImgNum);
            this.Controls.Add(this.ipAddressInput1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ipAddressInput1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private DevComponents.Editors.IpAddressInput ipAddressInput1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblSaveImgNum;
    }
}

