namespace CameraUpdateTool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cbProject = new System.Windows.Forms.ComboBox();
            this.cbParking = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUpdateUrl = new System.Windows.Forms.TextBox();
            this.txtUpdateMd5 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSendUpdate = new System.Windows.Forms.Button();
            this.listCamera = new System.Windows.Forms.ListView();
            this.hdIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdDriveway = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdMode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdMacAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "项目：";
            // 
            // cbProject
            // 
            this.cbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProject.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbProject.FormattingEnabled = true;
            this.cbProject.Location = new System.Drawing.Point(72, 11);
            this.cbProject.Name = "cbProject";
            this.cbProject.Size = new System.Drawing.Size(250, 21);
            this.cbProject.TabIndex = 1;
            // 
            // cbParking
            // 
            this.cbParking.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbParking.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbParking.FormattingEnabled = true;
            this.cbParking.Location = new System.Drawing.Point(383, 11);
            this.cbParking.Name = "cbParking";
            this.cbParking.Size = new System.Drawing.Size(250, 21);
            this.cbParking.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(336, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "车场：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 431);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "更新地址：";
            // 
            // txtUpdateUrl
            // 
            this.txtUpdateUrl.Location = new System.Drawing.Point(96, 428);
            this.txtUpdateUrl.Name = "txtUpdateUrl";
            this.txtUpdateUrl.Size = new System.Drawing.Size(403, 21);
            this.txtUpdateUrl.TabIndex = 5;
            // 
            // txtUpdateMd5
            // 
            this.txtUpdateMd5.Location = new System.Drawing.Point(96, 455);
            this.txtUpdateMd5.Name = "txtUpdateMd5";
            this.txtUpdateMd5.Size = new System.Drawing.Size(403, 21);
            this.txtUpdateMd5.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 458);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "MD5值：";
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(96, 482);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(403, 21);
            this.txtVersion.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 485);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "版本号：";
            // 
            // btnSendUpdate
            // 
            this.btnSendUpdate.Location = new System.Drawing.Point(513, 428);
            this.btnSendUpdate.Name = "btnSendUpdate";
            this.btnSendUpdate.Size = new System.Drawing.Size(120, 75);
            this.btnSendUpdate.TabIndex = 10;
            this.btnSendUpdate.Text = "发送更新指令";
            this.btnSendUpdate.UseVisualStyleBackColor = true;
            this.btnSendUpdate.Click += new System.EventHandler(this.btnSendUpdate_Click);
            // 
            // listCamera
            // 
            this.listCamera.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hdIndex,
            this.hdDriveway,
            this.hdMode,
            this.hdMacAddress,
            this.hdVersion});
            this.listCamera.Location = new System.Drawing.Point(27, 50);
            this.listCamera.Name = "listCamera";
            this.listCamera.Size = new System.Drawing.Size(606, 358);
            this.listCamera.TabIndex = 11;
            this.listCamera.TileSize = new System.Drawing.Size(128, 28);
            this.listCamera.UseCompatibleStateImageBehavior = false;
            this.listCamera.View = System.Windows.Forms.View.Details;
            // 
            // hdIndex
            // 
            this.hdIndex.Text = "序号";
            // 
            // hdDriveway
            // 
            this.hdDriveway.Text = "车道";
            this.hdDriveway.Width = 150;
            // 
            // hdMode
            // 
            this.hdMode.Text = "车道模式";
            this.hdMode.Width = 120;
            // 
            // hdMacAddress
            // 
            this.hdMacAddress.Text = "物理地址";
            this.hdMacAddress.Width = 200;
            // 
            // hdVersion
            // 
            this.hdVersion.Text = "当前版本";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 532);
            this.Controls.Add(this.listCamera);
            this.Controls.Add(this.btnSendUpdate);
            this.Controls.Add(this.txtVersion);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtUpdateMd5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtUpdateUrl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbParking);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbProject);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "相机版本管理工具";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbProject;
        private System.Windows.Forms.ComboBox cbParking;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUpdateUrl;
        private System.Windows.Forms.TextBox txtUpdateMd5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSendUpdate;
        private System.Windows.Forms.ListView listCamera;
        private System.Windows.Forms.ColumnHeader hdIndex;
        private System.Windows.Forms.ColumnHeader hdDriveway;
        private System.Windows.Forms.ColumnHeader hdMode;
        private System.Windows.Forms.ColumnHeader hdMacAddress;
        private System.Windows.Forms.ColumnHeader hdVersion;
    }
}