namespace ProjectInitializeTools
{
    partial class MainFrom
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("首页");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("车场设置");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("在场车辆管理");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("黑名单车辆");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("移动岗亭");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("车场管理", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("角色权限管理");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("操作员管理");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("人事管理", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("登记发行");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("临时车管理");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("月卡车管理");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("储值车管理");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("卡务管理", new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13});
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("临时车缴费记录");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("月卡车缴费记录");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("储值车缴费记录");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("储值车消费记录");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("车辆出场记录");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("异常开闸记录");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("车牌修正记录");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("入场补录记录");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("报表管理", new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20,
            treeNode21,
            treeNode22});
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("通行设置");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("开闸原因设置");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("密码修改");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("系统设置", new System.Windows.Forms.TreeNode[] {
            treeNode24,
            treeNode25,
            treeNode26});
            this.MenuChecked = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ProjectList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProjectName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProjectAccount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProjectGuid = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddProject = new System.Windows.Forms.Button();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.btnLaunchRight = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.CheckAllMenu = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // MenuChecked
            // 
            this.MenuChecked.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MenuChecked.CheckBoxes = true;
            this.MenuChecked.Enabled = false;
            this.MenuChecked.Location = new System.Drawing.Point(563, 23);
            this.MenuChecked.Name = "MenuChecked";
            treeNode1.Name = "Home";
            treeNode1.Tag = "0";
            treeNode1.Text = "首页";
            treeNode1.ToolTipText = "/picture/home.png";
            treeNode2.Name = "ParkSet";
            treeNode2.Tag = "101";
            treeNode2.Text = "车场设置";
            treeNode2.ToolTipText = "/Home/Trans/ParklotIndex";
            treeNode3.Name = "Inside";
            treeNode3.Tag = "103";
            treeNode3.Text = "在场车辆管理";
            treeNode3.ToolTipText = "/Home/Trans/VueSinglePage#/on_site_vehicles";
            treeNode4.Name = "BlackList";
            treeNode4.Tag = "104";
            treeNode4.Text = "黑名单车辆";
            treeNode4.ToolTipText = "/Home/Trans/ParkLot_Blacklist";
            treeNode5.Name = "SentryBox";
            treeNode5.Tag = "105";
            treeNode5.Text = "移动岗亭";
            treeNode5.ToolTipText = "/Home/Trans/ParkLot_MobileWatchHouse";
            treeNode6.Name = "ParkLot";
            treeNode6.Tag = "1";
            treeNode6.Text = "车场管理";
            treeNode6.ToolTipText = "/picture/parkLot.png";
            treeNode7.Name = "Role";
            treeNode7.Tag = "201";
            treeNode7.Text = "角色权限管理";
            treeNode7.ToolTipText = "/Home/Trans/RoleAuthManage";
            treeNode8.Name = "User";
            treeNode8.Tag = "202";
            treeNode8.Text = "操作员管理";
            treeNode8.ToolTipText = "/Home/Trans/OperaManage";
            treeNode9.Name = "Personnel";
            treeNode9.Tag = "2";
            treeNode9.Text = "人事管理";
            treeNode9.ToolTipText = "/picture/User.png";
            treeNode10.Name = "OpenCard";
            treeNode10.Tag = "301";
            treeNode10.Text = "登记发行";
            treeNode10.ToolTipText = "/Home/Trans/VueSinglePage#/SetCard";
            treeNode11.Name = "TempCard";
            treeNode11.Tag = "302";
            treeNode11.Text = "临时车管理";
            treeNode11.ToolTipText = "/Home/Trans/VueSinglePage#/TemporaryCarList";
            treeNode12.Name = "MonthCard";
            treeNode12.Tag = "303";
            treeNode12.Text = "月卡车管理";
            treeNode12.ToolTipText = "/Home/Trans/VueSinglePage#/MonthCard";
            treeNode13.Name = "ValueCard";
            treeNode13.Tag = "304";
            treeNode13.Text = "储值车管理";
            treeNode13.ToolTipText = "/Home/Trans/VueSinglePage#/ValueCard";
            treeNode14.Name = "Card";
            treeNode14.Tag = "3";
            treeNode14.Text = "卡务管理";
            treeNode14.ToolTipText = "/picture/car.png";
            treeNode15.Name = "temporary_parking";
            treeNode15.Tag = "401";
            treeNode15.Text = "临时车缴费记录";
            treeNode15.ToolTipText = "/Home/Trans/VueSinglePage#/temporary_parking";
            treeNode16.Name = "monthly_truck";
            treeNode16.Tag = "402";
            treeNode16.Text = "月卡车缴费记录";
            treeNode16.ToolTipText = "/Home/Trans/VueSinglePage#/monthly_truck";
            treeNode17.Name = "storage_car_recharge";
            treeNode17.Tag = "403";
            treeNode17.Text = "储值车缴费记录";
            treeNode17.ToolTipText = "/Home/Trans/VueSinglePage#/storage_car_recharge";
            treeNode18.Name = "storage_car_recharge";
            treeNode18.Tag = "404";
            treeNode18.Text = "储值车消费记录";
            treeNode18.ToolTipText = "/Home/Trans/VueSinglePage#/storage_car_recharge";
            treeNode19.Name = "vehicle_exit";
            treeNode19.Tag = "405";
            treeNode19.Text = "车辆出场记录";
            treeNode19.ToolTipText = "/Home/Trans/VueSinglePage#/vehicle_exit";
            treeNode20.Name = "abnormal_opening";
            treeNode20.Tag = "406";
            treeNode20.Text = "异常开闸记录";
            treeNode20.ToolTipText = "/Home/Trans/VueSinglePage#/abnormal_opening";
            treeNode21.Name = "license_plate_amendment";
            treeNode21.Tag = "407";
            treeNode21.Text = "车牌修正记录";
            treeNode21.ToolTipText = "/Home/Trans/VueSinglePage#/license_plate_amendment";
            treeNode22.Name = "admission_supplement";
            treeNode22.Tag = "408";
            treeNode22.Text = "入场补录记录";
            treeNode22.ToolTipText = "/Home/Trans/VueSinglePage#/admission_supplement";
            treeNode23.Name = "Report";
            treeNode23.Tag = "4";
            treeNode23.Text = "报表管理";
            treeNode23.ToolTipText = "/picture/report.png";
            treeNode24.Name = "TrafficSet";
            treeNode24.Tag = "503";
            treeNode24.Text = "通行设置";
            treeNode24.ToolTipText = "/Home/Trans/VueSinglePage#/Through";
            treeNode25.Name = "ManualReason";
            treeNode25.Tag = "504";
            treeNode25.Text = "开闸原因设置";
            treeNode25.ToolTipText = "/Home/Trans/VueSinglePage#/OpenDoor";
            treeNode26.Name = "ChangePswd";
            treeNode26.Tag = "505";
            treeNode26.Text = "密码修改";
            treeNode26.ToolTipText = "/Home/Trans/VueSinglePage#/PassWord";
            treeNode27.Name = "System";
            treeNode27.Tag = "5";
            treeNode27.Text = "系统设置";
            treeNode27.ToolTipText = "/picture/system.png";
            this.MenuChecked.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode6,
            treeNode9,
            treeNode14,
            treeNode23,
            treeNode27});
            this.MenuChecked.Size = new System.Drawing.Size(195, 437);
            this.MenuChecked.TabIndex = 0;
            this.MenuChecked.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.MenuChecked_AfterCheck);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(561, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "项目开通功能:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(314, 433);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "管理人手机号:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "项目列表:";
            // 
            // ProjectList
            // 
            this.ProjectList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.ProjectName,
            this.ProjectAccount,
            this.ProjectGuid});
            this.ProjectList.FullRowSelect = true;
            this.ProjectList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ProjectList.Location = new System.Drawing.Point(12, 23);
            this.ProjectList.Name = "ProjectList";
            this.ProjectList.Size = new System.Drawing.Size(510, 397);
            this.ProjectList.TabIndex = 2;
            this.ProjectList.UseCompatibleStateImageBehavior = false;
            this.ProjectList.View = System.Windows.Forms.View.Details;
            this.ProjectList.SelectedIndexChanged += new System.EventHandler(this.ProjectList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.DisplayIndex = 3;
            this.columnHeader1.Width = 1;
            // 
            // ProjectName
            // 
            this.ProjectName.DisplayIndex = 0;
            this.ProjectName.Text = "项目名";
            this.ProjectName.Width = 180;
            // 
            // ProjectAccount
            // 
            this.ProjectAccount.DisplayIndex = 1;
            this.ProjectAccount.Text = "管理人手机号";
            this.ProjectAccount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ProjectAccount.Width = 100;
            // 
            // ProjectGuid
            // 
            this.ProjectGuid.DisplayIndex = 2;
            this.ProjectGuid.Text = "项目Guid";
            this.ProjectGuid.Width = 220;
            // 
            // btnAddProject
            // 
            this.btnAddProject.Location = new System.Drawing.Point(187, 466);
            this.btnAddProject.Name = "btnAddProject";
            this.btnAddProject.Size = new System.Drawing.Size(151, 44);
            this.btnAddProject.TabIndex = 3;
            this.btnAddProject.Text = "保存项目";
            this.btnAddProject.UseVisualStyleBackColor = true;
            this.btnAddProject.Click += new System.EventHandler(this.btnAddProject_Click);
            // 
            // txtAccount
            // 
            this.txtAccount.Location = new System.Drawing.Point(394, 430);
            this.txtAccount.Name = "txtAccount";
            this.txtAccount.Size = new System.Drawing.Size(128, 21);
            this.txtAccount.TabIndex = 4;
            // 
            // btnLaunchRight
            // 
            this.btnLaunchRight.Enabled = false;
            this.btnLaunchRight.Location = new System.Drawing.Point(579, 466);
            this.btnLaunchRight.Name = "btnLaunchRight";
            this.btnLaunchRight.Size = new System.Drawing.Size(151, 44);
            this.btnLaunchRight.TabIndex = 3;
            this.btnLaunchRight.Text = "开通功能";
            this.btnLaunchRight.UseVisualStyleBackColor = true;
            this.btnLaunchRight.Click += new System.EventHandler(this.btnLaunchRight_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 433);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "项目名称:";
            // 
            // txtProjectName
            // 
            this.txtProjectName.Location = new System.Drawing.Point(68, 430);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(223, 21);
            this.txtProjectName.TabIndex = 4;
            // 
            // CheckAllMenu
            // 
            this.CheckAllMenu.AutoSize = true;
            this.CheckAllMenu.Enabled = false;
            this.CheckAllMenu.Location = new System.Drawing.Point(650, 3);
            this.CheckAllMenu.Name = "CheckAllMenu";
            this.CheckAllMenu.Size = new System.Drawing.Size(96, 16);
            this.CheckAllMenu.TabIndex = 5;
            this.CheckAllMenu.Text = "开通全部功能";
            this.CheckAllMenu.UseVisualStyleBackColor = true;
            this.CheckAllMenu.CheckedChanged += new System.EventHandler(this.CheckAllMenu_CheckedChanged);
            // 
            // MainFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 523);
            this.Controls.Add(this.CheckAllMenu);
            this.Controls.Add(this.txtProjectName);
            this.Controls.Add(this.txtAccount);
            this.Controls.Add(this.btnLaunchRight);
            this.Controls.Add(this.btnAddProject);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ProjectList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MenuChecked);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "项目管理工具";
            this.Load += new System.EventHandler(this.MainFrom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView MenuChecked;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView ProjectList;
        private System.Windows.Forms.ColumnHeader ProjectName;
        private System.Windows.Forms.ColumnHeader ProjectAccount;
        private System.Windows.Forms.Button btnAddProject;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.Button btnLaunchRight;
        private System.Windows.Forms.ColumnHeader ProjectGuid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.CheckBox CheckAllMenu;
    }
}

