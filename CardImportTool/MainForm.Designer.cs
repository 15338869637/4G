namespace CardImportTool
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
            this.txtThreadCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnChecking = new System.Windows.Forms.Button();
            this.cbProject = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblFailCount = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblSuccessCount = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbCarType = new System.Windows.Forms.ComboBox();
            this.cbParking = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lvDataSpeed = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDataCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.dgvMonthCard = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.label4 = new System.Windows.Forms.Label();
            this.btnFilePath = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cbSelectCarType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonthCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtThreadCount
            // 
            this.txtThreadCount.Location = new System.Drawing.Point(775, 397);
            this.txtThreadCount.Name = "txtThreadCount";
            this.txtThreadCount.Size = new System.Drawing.Size(55, 21);
            this.txtThreadCount.TabIndex = 48;
            this.txtThreadCount.Text = "5";
            this.txtThreadCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(716, 400);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 47;
            this.label7.Text = "线程数量：";
            // 
            // btnChecking
            // 
            this.btnChecking.Location = new System.Drawing.Point(727, 15);
            this.btnChecking.Name = "btnChecking";
            this.btnChecking.Size = new System.Drawing.Size(108, 52);
            this.btnChecking.TabIndex = 46;
            this.btnChecking.Text = "验证数据";
            this.btnChecking.UseVisualStyleBackColor = true;
            this.btnChecking.Click += new System.EventHandler(this.btnChecking_Click);
            // 
            // cbProject
            // 
            this.cbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProject.FormattingEnabled = true;
            this.cbProject.Location = new System.Drawing.Point(81, 17);
            this.cbProject.Name = "cbProject";
            this.cbProject.Size = new System.Drawing.Size(162, 20);
            this.cbProject.TabIndex = 45;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 44;
            this.label10.Text = "选择项目：";
            // 
            // lblFailCount
            // 
            this.lblFailCount.AutoSize = true;
            this.lblFailCount.Font = new System.Drawing.Font("宋体", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFailCount.ForeColor = System.Drawing.Color.Red;
            this.lblFailCount.Location = new System.Drawing.Point(317, 477);
            this.lblFailCount.Name = "lblFailCount";
            this.lblFailCount.Size = new System.Drawing.Size(25, 26);
            this.lblFailCount.TabIndex = 43;
            this.lblFailCount.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(251, 486);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 42;
            this.label9.Text = "失败条数：";
            // 
            // lblSuccessCount
            // 
            this.lblSuccessCount.AutoSize = true;
            this.lblSuccessCount.Font = new System.Drawing.Font("宋体", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSuccessCount.ForeColor = System.Drawing.Color.Red;
            this.lblSuccessCount.Location = new System.Drawing.Point(97, 477);
            this.lblSuccessCount.Name = "lblSuccessCount";
            this.lblSuccessCount.Size = new System.Drawing.Size(25, 26);
            this.lblSuccessCount.TabIndex = 41;
            this.lblSuccessCount.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 486);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 40;
            this.label6.Text = "导入成功条数：";
            // 
            // cbCarType
            // 
            this.cbCarType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCarType.FormattingEnabled = true;
            this.cbCarType.Location = new System.Drawing.Point(559, 17);
            this.cbCarType.Name = "cbCarType";
            this.cbCarType.Size = new System.Drawing.Size(162, 20);
            this.cbCarType.TabIndex = 39;
            // 
            // cbParking
            // 
            this.cbParking.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbParking.FormattingEnabled = true;
            this.cbParking.Location = new System.Drawing.Point(320, 17);
            this.cbParking.Name = "cbParking";
            this.cbParking.Size = new System.Drawing.Size(162, 20);
            this.cbParking.TabIndex = 38;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(488, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 37;
            this.label5.Text = "选择车类：";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 451);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(695, 23);
            this.progressBar1.TabIndex = 35;
            // 
            // lvDataSpeed
            // 
            this.lvDataSpeed.Location = new System.Drawing.Point(12, 528);
            this.lvDataSpeed.Name = "lvDataSpeed";
            this.lvDataSpeed.Size = new System.Drawing.Size(823, 189);
            this.lvDataSpeed.TabIndex = 34;
            this.lvDataSpeed.UseCompatibleStateImageBehavior = false;
            this.lvDataSpeed.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(10, 424);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(641, 12);
            this.label3.TabIndex = 33;
            this.label3.Text = "注意：请仔细核对数据后，再执行导入操作。尤其注意表头、表尾部的数据。执行后将无法执行回滚操作，请谨慎操作。";
            // 
            // lblDataCount
            // 
            this.lblDataCount.AutoSize = true;
            this.lblDataCount.Font = new System.Drawing.Font("宋体", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDataCount.ForeColor = System.Drawing.Color.Red;
            this.lblDataCount.Location = new System.Drawing.Point(95, 389);
            this.lblDataCount.Name = "lblDataCount";
            this.lblDataCount.Size = new System.Drawing.Size(25, 26);
            this.lblDataCount.TabIndex = 32;
            this.lblDataCount.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 399);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 31;
            this.label2.Text = "本次解析条数：";
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(711, 424);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(124, 79);
            this.btnImport.TabIndex = 30;
            this.btnImport.Text = "确认导入";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // dgvMonthCard
            // 
            this.dgvMonthCard.AllowUserToAddRows = false;
            this.dgvMonthCard.AllowUserToDeleteRows = false;
            this.dgvMonthCard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMonthCard.Location = new System.Drawing.Point(12, 100);
            this.dgvMonthCard.Name = "dgvMonthCard";
            this.dgvMonthCard.ReadOnly = true;
            this.dgvMonthCard.RowTemplate.Height = 23;
            this.dgvMonthCard.Size = new System.Drawing.Size(823, 280);
            this.dgvMonthCard.TabIndex = 29;
            this.dgvMonthCard.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvMonthCard_RowPostPaint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 28;
            this.label1.Text = "文件路径：";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(81, 46);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(553, 21);
            this.txtFilePath.TabIndex = 27;
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(249, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 36;
            this.label4.Text = "选择车场：";
            // 
            // btnFilePath
            // 
            this.btnFilePath.Location = new System.Drawing.Point(646, 44);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Size = new System.Drawing.Size(75, 23);
            this.btnFilePath.TabIndex = 26;
            this.btnFilePath.Text = "选择";
            this.btnFilePath.UseVisualStyleBackColor = true;
            this.btnFilePath.Click += new System.EventHandler(this.btnFilePath_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 49;
            this.label8.Text = "数据筛选：";
            // 
            // cbSelectCarType
            // 
            this.cbSelectCarType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelectCarType.FormattingEnabled = true;
            this.cbSelectCarType.Location = new System.Drawing.Point(81, 75);
            this.cbSelectCarType.Name = "cbSelectCarType";
            this.cbSelectCarType.Size = new System.Drawing.Size(162, 20);
            this.cbSelectCarType.TabIndex = 50;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 560);
            this.Controls.Add(this.cbSelectCarType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtThreadCount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnChecking);
            this.Controls.Add(this.cbProject);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblFailCount);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblSuccessCount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbCarType);
            this.Controls.Add(this.cbParking);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lvDataSpeed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblDataCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.dgvMonthCard);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnFilePath);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "固定车批量导入";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonthCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtThreadCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnChecking;
        private System.Windows.Forms.ComboBox cbProject;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblFailCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblSuccessCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbCarType;
        private System.Windows.Forms.ComboBox cbParking;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListView lvDataSpeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDataCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.DataGridView dgvMonthCard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnFilePath;
        private System.Windows.Forms.ComboBox cbSelectCarType;
        private System.Windows.Forms.Label label8;
    }
}