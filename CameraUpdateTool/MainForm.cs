using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.ListViewItem;

namespace CameraUpdateTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.listCamera.CheckBoxes = true;
            //int listWidth = this.listCamera.Width;
            //int listColumnCount = this.listCamera.Columns.Count;
            //foreach (ColumnHeader columnItem in this.listCamera.Columns)
            //{
            //    columnItem.Width = listWidth / listColumnCount;
            //    columnItem.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //}
            InitProjectData();

        }

        /// <summary>
        /// 初始化项目数据集合
        /// </summary>
        private void InitProjectData()
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = GetAllProject();
            this.cbProject.ValueMember = "Key";
            this.cbProject.DisplayMember = "Value";
            this.cbProject.DataSource = bs;
            this.cbProject.SelectedIndex = -1;

            this.cbProject.SelectedIndexChanged += new EventHandler(cbProject_SelectedIndexChanged);
        }

        private void cbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            string projectGuid = Convert.ToString(this.cbProject.SelectedValue);
            if (string.IsNullOrEmpty(projectGuid))
                return;

            Dictionary<string, string> dicParking = GetAllParking(projectGuid);
            if (dicParking == null || dicParking.Count <= 0)
            {
                this.cbParking.DataSource = null;
                this.cbParking.Items.Add("==暂未创建停车场==");
                this.cbParking.SelectedIndex = 0;
                return;
            }

            BindingSource bs = new BindingSource();
            bs.DataSource = dicParking;
            this.cbParking.ValueMember = "Key";
            this.cbParking.DisplayMember = "Value";
            this.cbParking.DataSource = bs;
            this.cbParking.SelectedIndex = -1;
            
            this.cbParking.SelectedIndexChanged += new EventHandler(cbParking_SelectedIndexChanged);
        }


        private void cbParking_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listCamera.Items.Clear();

            string projectGuid = Convert.ToString(this.cbProject.SelectedValue);
            string parkingCode = Convert.ToString(this.cbParking.SelectedValue);
            if (string.IsNullOrEmpty(projectGuid) || string.IsNullOrEmpty(parkingCode))
                return;

            DataTable dt = GetAllDriveway(projectGuid, parkingCode);
            if (dt == null || dt.Rows.Count <= 0)
            {
                //MessageBox.Show("暂未创建车道", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow itemRow = dt.Rows[i];

                ListViewSubItem subItemMacAddress = new ListViewSubItem();
                subItemMacAddress.Name = "deviceMacAddress";
                subItemMacAddress.Text = Convert.ToString(itemRow["deviceMacAddress"]);

                ListViewItem listItem = new ListViewItem();
                listItem.Text = (i+1).ToString();
                listItem.SubItems.Add(Convert.ToString(itemRow["drivewayName"].ToString()));
                listItem.SubItems.Add(Convert.ToInt32(itemRow["type"]) == 0 ? "入口" : "出口");
                listItem.SubItems.Add(subItemMacAddress);
                listItem.SubItems.Add("");
                this.listCamera.Items.Add(listItem);
            }

        }

        #region 查询数据库

        /// <summary>
        /// 获取所有的项目列表
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetAllProject()
        {
            Dictionary<string, string> dicProject = new Dictionary<string, string>();
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
                            dicProject.Add(item["projectGuid"].ToString(), item["projectName"].ToString());
                        }
                    }
                }

                dbc.Close();
            }
            catch(Exception) { }
            return dicProject;
        }
        
        /// <summary>
        /// 根据项目获取所有停车场
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetAllParking(string projectGuid)
        {
            Dictionary<string, string> dicParking = new Dictionary<string, string>();
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
                command.CommandText = "select * from t_parklot where projectGuid = @projectGuid";
                command.CommandType = CommandType.Text;


                DbParameter param1 = factory.CreateParameter();
                param1.ParameterName = "@projectGuid";
                param1.Value = projectGuid;

                command.Parameters.Add(param1);

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
                            dicParking.Add(item["parkCode"].ToString(), item["parkName"].ToString());
                        }
                    }
                }

                dbc.Close();
            }
            catch (Exception) { }
            return dicParking;
        }
        
        /// <summary>
        /// 根据停车场获取所有车道相机数据
        /// </summary>
        /// <returns></returns>
        private DataTable GetAllDriveway(string projectGuid, string parkingCode)
        {
            DataTable dt = new DataTable();
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
                command.CommandText = "select * from t_driveway where projectGuid = @projectGuid and parkCode = @parkCode";
                command.CommandType = CommandType.Text;


                DbParameter param1 = factory.CreateParameter();
                param1.ParameterName = "@projectGuid";
                param1.Value = projectGuid;
                command.Parameters.Add(param1);

                DbParameter param2 = factory.CreateParameter();
                param2.ParameterName = "@parkCode";
                param2.Value = parkingCode;
                command.Parameters.Add(param2);

                DbDataAdapter adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable table = ds.Tables[0];
                        dt = table;
                    }
                }

                dbc.Close();
            }
            catch (Exception) { }
            return dt;
        }

        #endregion

        #region MQ命令发送

        public bool CarNumberRepushToCamera(CameraUpdateModel model,string parkingCode)
        {
            CommandEntity<CameraUpdateModel> entity = new CommandEntity<CameraUpdateModel>()
            {
                command = BussineCommand.CameraUpdate,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = model
            };
            ILogger logger = new Logger();
            ISerializer serializer = new JsonSerializer(logger);
            RabbitMQSender m_rabbitMQ = new RabbitMQSender(logger, serializer);

            return m_rabbitMQ.SendMessageForRabbitMQ("发送相机升级命令", serializer.Serialize(entity), entity.idMsg, parkingCode);
        }

        #endregion

        private void btnSendUpdate_Click(object sender, EventArgs e)
        {
            string parkingCode = Convert.ToString(this.cbParking.SelectedValue);
            if (string.IsNullOrEmpty(parkingCode))
            {
                MessageBox.Show("请先选择停车场", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            CheckedListViewItemCollection checkedItem = this.listCamera.CheckedItems;
            if (checkedItem == null || checkedItem.Count <= 0)
            {
                MessageBox.Show("请选择需要更新的相机", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string inputUrl = this.txtUpdateUrl.Text;
            string inputMd5 = this.txtUpdateMd5.Text;
            string inputVersion = this.txtVersion.Text;
            if (string.IsNullOrEmpty(inputUrl) || string.IsNullOrEmpty(inputMd5) || string.IsNullOrEmpty(inputVersion))
            {
                MessageBox.Show("请将更新信息填写完整", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult tipResult = MessageBox.Show("请仔细核对更新内容？\n该操作可能导致现场相机无法正常使用", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (tipResult != DialogResult.OK)
            {
                return;
            }

            CameraUpdateModel updateModel = new CameraUpdateModel();
            updateModel.UpdateUrl = inputUrl;
            updateModel.UpdateMd5 = inputMd5;
            updateModel.UpdateVersion = inputVersion;
            foreach (ListViewItem item in checkedItem)
            {
                string macAddress = item.SubItems["deviceMacAddress"].Text;
                if (string.IsNullOrEmpty(macAddress)) continue;
                updateModel.DeviceIdentify = macAddress;

                CarNumberRepushToCamera(updateModel, parkingCode);
            }

            //暂不参考mq返回结果
            MessageBox.Show("更新命令发送成功", "提示", MessageBoxButtons.OK);
        }
    }

    public class CameraUpdateModel
    {
        /// <summary>
        /// 设备标识
        /// </summary>
        public string DeviceIdentify { get; set; }
        /// <summary>
        /// 更新地址
        /// </summary>
        public string UpdateUrl { get; set; }
        /// <summary>
        /// 更新MD5值
        /// </summary>
        public string UpdateMd5 { get; set; }
        /// <summary>
        /// 更新版本号
        /// </summary>
        public string UpdateVersion { get; set; }
    }
}
