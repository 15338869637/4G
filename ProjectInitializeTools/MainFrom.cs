using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectInitializeTools
{
    public partial class MainFrom : Form
    {
        string chooseproject = "";
        string chooseaccount = "";

        public MainFrom()
        {
            InitializeComponent();
        }

        private void MainFrom_Load(object sender, EventArgs e)
        {
            MenuChecked.ExpandAll();
            List<Dictionary<string, string>> listproject = GetAllProject();
            foreach(var item in listproject)
            {
                ListViewItem row = new ListViewItem();
                row.SubItems.Add(item["projectName"]);
                row.SubItems.Add(item["projectAccount"]);
                row.SubItems.Add(item["projectGuid"]);
                ProjectList.Items.Add(row);
            }
        }

        private void btnAddProject_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccount.Text) || string.IsNullOrWhiteSpace(txtProjectName.Text))
            {
                MessageBox.Show("项目名与管理人手机号不能为空");
                return;
            }

            if (hasAccount(txtAccount.Text) && chooseproject=="")
            { 
                //判断为相同手机号存在且非新增时候
                MessageBox.Show("存在相同管理人手机号账户");
                return;
            }
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();

                DbCommand command = factory.CreateCommand();
                command.Connection = dbc;
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                command.CommandTimeout = 120;
                command.CommandText = @"replace into t_project(projectGuid,projectName,projectAccount,createTime) values(@projectGuid, @projectName, @projectAccount,@createTime)";
                command.CommandType = CommandType.Text;

                DbParameter projectGuid = factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                if (chooseproject != "")
                {
                    projectGuid.Value = chooseproject;
                }
                else
                {
                    projectGuid.Value = Guid.NewGuid().ToString("N");
                }

                DbParameter projectName = factory.CreateParameter();
                projectName.ParameterName = "@projectName";
                projectName.Value = txtProjectName.Text;

                DbParameter projectAccount = factory.CreateParameter();
                projectAccount.ParameterName = "@projectAccount";
                projectAccount.Value = txtAccount.Text;

                DbParameter createTime = factory.CreateParameter();
                createTime.ParameterName = "@createTime";
                createTime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                command.Parameters.Add(projectGuid);
                command.Parameters.Add(projectName);
                command.Parameters.Add(projectAccount);
                command.Parameters.Add(createTime);
                int retval = command.ExecuteNonQuery();

                dbc.Close();
                if (chooseproject == "")
                    MessageBox.Show("添加成功");
                else
                    MessageBox.Show("保存成功");

                ProjectList.Items.Clear();

                List<Dictionary<string, string>> listproject = GetAllProject();
                foreach (var item in listproject)
                {
                    ListViewItem row = new ListViewItem();
                    row.SubItems.Add(item["projectName"]);
                    row.SubItems.Add(item["projectAccount"]);
                    row.SubItems.Add(item["projectGuid"]);
                    ProjectList.Items.Add(row);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("保存失败");
            }
            
        }

        private List<Dictionary<string,string>> GetAllProject()
        {
            List<Dictionary<string, string>> listproject = new List<Dictionary<string, string>>();
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();

                DbCommand command = factory.CreateCommand();
                command.Connection = dbc;
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                command.CommandTimeout = 120;
                command.CommandText = "select * from t_project";
                command.CommandType = CommandType.Text;

                DbDataAdapter adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable table = ds.Tables[0];

                        foreach (DataRow item in table.Rows)
                        {
                            listproject.Add(new Dictionary<string, string>()
                            {
                                { "projectGuid",item["projectGuid"].ToString()},
                                { "projectName",item["projectName"].ToString()},
                                { "projectAccount",item["projectAccount"].ToString()}
                            });
                        }
                    }
                }

                dbc.Close();
            }
            catch { }
            return listproject;
        }

        private void ShowMenuChecked()
        {
            MenuChecked.Nodes.OfType<TreeNode>().ToList().ForEach(x => x.Checked = false);
            foreach (TreeNode node in MenuChecked.Nodes)
            {
                node.Nodes.OfType<TreeNode>().ToList().ForEach(x => x.Checked = false);
            }

            if (chooseproject == "") return;
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode node in MenuChecked.Nodes)
            {
                node.Nodes.OfType<TreeNode>().ToList().ForEach(x => x.Checked = false);
                nodes.Add(node);
                foreach (TreeNode childnode in node.Nodes)
                {
                    //再查一级子节点
                    nodes.Add(childnode);
                }
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();

                DbCommand command = factory.CreateCommand();
                command.Connection = dbc;
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                command.CommandTimeout = 120;
                command.CommandText = string.Format("select menuSerial from t_projectmenu where projectGuid='{0}'",chooseproject);
                command.CommandType = CommandType.Text;

                DbDataAdapter adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable table = ds.Tables[0];

                        foreach (DataRow item in table.Rows)
                        {
                            TreeNode targetNode = nodes.Find(x => x.Tag.ToString() == item["menuSerial"].ToString());
                            if (targetNode != default(TreeNode))
                                targetNode.Checked = true;
                        }
                    }
                }

                dbc.Close();
            }
            catch { }
        }

        private bool hasAccount(string Account)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();

                DbCommand command = factory.CreateCommand();
                command.Connection = dbc;
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                command.CommandTimeout = 120;
                command.CommandText = string.Format("select projectAccount from t_project where projectAccount='{0}'", Account);
                command.CommandType = CommandType.Text;
                object retval = command.ExecuteScalar();

                dbc.Close();

                if (retval != null) return true;
            }
            catch(Exception ex)
            {

            }
            return false; ;
        }
        private string GetUserGuid(string projectGuid,string mobile)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();

                DbCommand command = factory.CreateCommand();
                command.Connection = dbc;
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                command.CommandTimeout = 120;
                command.CommandText = string.Format("select guid from t_operator where projectGuid='{0}' and mobile = '{1}' ", projectGuid, mobile);
                command.CommandType = CommandType.Text;
                object retval = command.ExecuteScalar();

                dbc.Close();

                if (retval != null) return retval.ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
            return "";
        }
        private bool SaveUser(string privilegeStr,string roleGuidStr)
        {
            try
            {
                string userGuid = GetUserGuid(chooseproject, chooseaccount);
                if (string.IsNullOrEmpty(userGuid))
                    userGuid = Guid.NewGuid().ToString("N");

                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();

                DbCommand command = factory.CreateCommand();
                command.Connection = dbc;
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                command.CommandTimeout = 120;
                command.CommandText = @"replace into t_operator(projectGuid,guid,userName,userPswd,mobile,privilege,roleGuid) 
                                        values(@projectGuid,@guid,@userName,@userPswd,@mobile,@privilege,@roleGuid)";
                command.CommandType = CommandType.Text;

                DbParameter projectGuid = factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = chooseproject;

                DbParameter guid = factory.CreateParameter();
                guid.ParameterName = "@guid";
                guid.Value = userGuid;

                DbParameter userName = factory.CreateParameter();
                userName.ParameterName = "@userName";
                userName.Value = "系统管理员";

                DbParameter userPswd = factory.CreateParameter();
                userPswd.ParameterName = "@userPswd";
                userPswd.Value = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes("fujica"))).Replace("-", "");

                DbParameter mobile = factory.CreateParameter();
                mobile.ParameterName = "@mobile";
                mobile.Value = chooseaccount;

                DbParameter privilege = factory.CreateParameter();
                privilege.ParameterName = "@privilege";
                privilege.Value = privilegeStr;

                DbParameter roleGuid = factory.CreateParameter();
                roleGuid.ParameterName = "@roleGuid";
                roleGuid.Value = roleGuidStr;

                command.Parameters.Add(projectGuid);
                command.Parameters.Add(guid);
                command.Parameters.Add(userName);
                command.Parameters.Add(userPswd);
                command.Parameters.Add(mobile);
                command.Parameters.Add(privilege);
                command.Parameters.Add(roleGuid);

                int retval = command.ExecuteNonQuery();
                dbc.Close();
                if (retval > 0) return true;
            }
            catch(Exception ex)
            {

            }

            return false;
        }
        private string GetRoleGuid(string projectGuid)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();

                DbCommand command = factory.CreateCommand();
                command.Connection = dbc;
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                command.CommandTimeout = 120;
                command.CommandText = string.Format("select guid from t_rolepermission where projectGuid='{0}' and isAdmin = 1 ", projectGuid);
                command.CommandType = CommandType.Text;
                object retval = command.ExecuteScalar();

                dbc.Close();

                if (retval != null) return retval.ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
            return "";
        }

        private string  SaveRole(string privilegeStr)
        {
            try
            {
                string roleguid = GetRoleGuid(chooseproject);
                if (string.IsNullOrEmpty(roleguid))
                    roleguid = Guid.NewGuid().ToString("N");
                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();

                DbCommand command = factory.CreateCommand();
                command.Connection = dbc;
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                command.CommandTimeout = 120;
                command.CommandText = @"replace into t_rolepermission(projectGuid,guid,roleName,contentDetial,parkingCodeList,isAdmin) 
                                        values(@projectGuid,@guid,@roleName,@contentDetial,@parkingCodeList,@isAdmin)";
                command.CommandType = CommandType.Text;

                DbParameter projectGuid = factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = chooseproject;

                DbParameter guid = factory.CreateParameter();
                guid.ParameterName = "@guid";
                guid.Value = roleguid;

                DbParameter roleName = factory.CreateParameter();
                roleName.ParameterName = "@roleName";
                roleName.Value = "超级管理员";

                DbParameter contentDetial = factory.CreateParameter();
                contentDetial.ParameterName = "@contentDetial";
                contentDetial.Value = privilegeStr;

                DbParameter parkingCodeList = factory.CreateParameter();
                parkingCodeList.ParameterName = "@parkingCodeList";
                parkingCodeList.Value = "";

                DbParameter isAdmin = factory.CreateParameter();
                isAdmin.ParameterName = "@isAdmin";
                isAdmin.Value = true;

                command.Parameters.Add(projectGuid);
                command.Parameters.Add(guid);
                command.Parameters.Add(roleName);
                command.Parameters.Add(contentDetial);
                command.Parameters.Add(parkingCodeList);
                command.Parameters.Add(isAdmin);

                int retval = command.ExecuteNonQuery();
                dbc.Close();
                if (retval > 0) return roleguid;
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        private void btnLaunchRight_Click(object sender, EventArgs e)
        {
            if (chooseproject == "")
            {
                MessageBox.Show("请选中一个项目再编辑权限");
                return;
            }
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode node in MenuChecked.Nodes)
            {
                bool hascheckedchild = false;
                foreach (TreeNode childnode in node.Nodes)
                {
                    //再查一级子节点
                    nodes.Add(childnode);
                    if (childnode.Checked) hascheckedchild = true;//有选中的子级
                }
                if (!hascheckedchild && node.Nodes.Count > 0)
                {
                    node.Checked = false;// 有子级节点而又没选中任何子级时候，强制未选中
                }
                nodes.Add(node);
            }

            //保存
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["parklotManager"].ConnectionString;
                DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                DbConnection dbc = factory.CreateConnection();
                dbc.ConnectionString = connectionString;
                dbc.Open();
                //保存权限
                foreach (TreeNode node in nodes)
                {
                    if (node.Checked)
                    {
                        DbCommand command = factory.CreateCommand();
                        command.Connection = dbc;
                        if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                        command.CommandTimeout = 120;
                        command.CommandText = @"replace into t_projectmenu(projectGuid,menuSerial,menuName,pageUrl) values(@projectGuid, @menuSerial, @menuName,@pageUrl)";
                        command.CommandType = CommandType.Text;

                        DbParameter projectGuid = factory.CreateParameter();
                        projectGuid.ParameterName = "@projectGuid";
                        projectGuid.Value = chooseproject;

                        DbParameter menuSerial = factory.CreateParameter();
                        menuSerial.ParameterName = "@menuSerial";
                        menuSerial.Value = node.Tag;

                        DbParameter menuName = factory.CreateParameter();
                        menuName.ParameterName = "@menuName";
                        menuName.Value = node.Text;

                        DbParameter pageUrl = factory.CreateParameter();
                        pageUrl.ParameterName = "@pageUrl";
                        pageUrl.Value = node.ToolTipText;

                        command.Parameters.Add(projectGuid);
                        command.Parameters.Add(menuSerial);
                        command.Parameters.Add(menuName);
                        command.Parameters.Add(pageUrl);

                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        DbCommand command = factory.CreateCommand();
                        command.Connection = dbc;
                        if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                        command.CommandTimeout = 120;
                        command.CommandText = @"delete from t_projectmenu where projectGuid=@projectGuid and menuSerial=@menuSerial";
                        command.CommandType = CommandType.Text;

                        DbParameter projectGuid = factory.CreateParameter();
                        projectGuid.ParameterName = "@projectGuid";
                        projectGuid.Value = chooseproject;

                        DbParameter menuSerial = factory.CreateParameter();
                        menuSerial.ParameterName = "@menuSerial";
                        menuSerial.Value = node.Tag;

                        command.Parameters.Add(projectGuid);
                        command.Parameters.Add(menuSerial);
                        command.ExecuteNonQuery();
                    }
                }
                dbc.Close();

                //组建权限字符串
                string privilegeStr = "";
                foreach (TreeNode node in nodes)
                {
                    if (node.Checked)
                    {
                        privilegeStr += (node.Tag.ToString().PadLeft(3, '0') + "1");
                    }
                    else
                    {
                        privilegeStr += (node.Tag.ToString().PadLeft(3, '0') + "0");
                    }
                 }
                string roleGuid = SaveRole(privilegeStr);
                if (string.IsNullOrEmpty(roleGuid))
                {
                    MessageBox.Show("保存角色出错");
                    return;
                }
                bool userState = SaveUser(privilegeStr, roleGuid);
                if (!userState)
                {
                    MessageBox.Show("保存用户出错");
                    return;
                }
                MessageBox.Show("菜单修改成功");
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show("菜单修改失败" + ex.ToString());
            }
        }

        private void ProjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProjectList.SelectedIndices.Count > 0)
            {
                chooseproject = ProjectList.SelectedItems[0].SubItems[3].Text;
                chooseaccount = ProjectList.SelectedItems[0].SubItems[2].Text;
                txtAccount.Text = ProjectList.SelectedItems[0].SubItems[2].Text;
                txtProjectName.Text = ProjectList.SelectedItems[0].SubItems[1].Text;
            }
            if (ProjectList.SelectedIndices.Count == 0)
            {
                chooseproject = "";
                txtAccount.Text = "";
                txtProjectName.Text = "";
            }

            if (chooseproject == "")
            {
                btnLaunchRight.Enabled = false;
                MenuChecked.Enabled = false;
                CheckAllMenu.Enabled = false;
                CheckAllMenu.Checked = false;

                this.txtAccount.Enabled = true;
            }
            if (chooseproject != "")
            {
                btnLaunchRight.Enabled = true;
                MenuChecked.Enabled = true;
                CheckAllMenu.Enabled = true;

                this.txtAccount.Enabled = false;
            }

            ShowMenuChecked();
        }

        /// <summary>
        /// 选中行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuChecked_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            //子节点联动父节点
            if (node.Parent != null)
            {
                if (!node.Checked)
                {
                    //取消选择的时候
                    foreach (TreeNode brother in node.Parent.Nodes)
                    {
                        //兄弟节点有选中的,跳过
                        if (brother.Checked)
                        {
                            return;
                        }
                    }
                }
                node.Parent.Checked = node.Checked;
            }
        }

        private void CheckAllMenu_CheckedChanged(object sender, EventArgs e)
        {
            foreach (TreeNode node in MenuChecked.Nodes)
            {
                if (node.Nodes.Count != 0)
                {
                    node.Nodes.OfType<TreeNode>().ToList().ForEach(x => x.Checked = CheckAllMenu.Checked);
                }else
                {
                    node.Checked = CheckAllMenu.Checked;
                }
            }
            if (CheckAllMenu.Checked)
            {
                CheckAllMenu.Text = "关闭全部功能";
            }else
            {
                CheckAllMenu.Text = "开通全部功能";
            }
        }
    }
}
