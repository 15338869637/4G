using ExcelDataReader;
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardImportTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public delegate void ShowMsg(ShowMsgType st, int msgIndex, string msg);
        public static ShowMsg ShowMsgDistance;

        public string ProjectGuid = string.Empty;
        public string ParkingCode = string.Empty;
        public string CarTypeGuid = string.Empty;
        public string CarTypeIdx = string.Empty;
        public List<string> DrivewayList = null;

        public DataTable ExcelMonthCardDt = null;

        private static ILogger m_ilogger = new Logger();
        private static ISerializer m_serializer = new JsonSerializer(m_ilogger);
        private static RabbitMQSender m_rabbitMQ = new RabbitMQSender(m_ilogger, m_serializer);

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitDataGridView();
            InitProjectData();

            ShowMsgDistance = new ShowMsg(ShowMsgForm);
        }

        /// <summary>
        /// 初始化DataGridView数据
        /// </summary>
        private void InitDataGridView()
        {
            this.dgvMonthCard.AutoGenerateColumns = false;
            this.dgvMonthCard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            DataGridViewColumn column1 = new DataGridViewTextBoxColumn();
            column1.HeaderText = "车牌号码";
            column1.DataPropertyName = "Column14";
            column1.Name = "carnumber";
            this.dgvMonthCard.Columns.Add(column1);

            DataGridViewColumn column2 = new DataGridViewTextBoxColumn();
            column2.HeaderText = "姓名";
            column2.DataPropertyName = "Column4";
            column2.Name = "name";
            this.dgvMonthCard.Columns.Add(column2);

            DataGridViewColumn column3 = new DataGridViewTextBoxColumn();
            column3.HeaderText = "手机号码";
            column3.DataPropertyName = "Column7";
            column3.Name = "mobile";
            this.dgvMonthCard.Columns.Add(column3);

            DataGridViewColumn column4 = new DataGridViewTextBoxColumn();
            column4.HeaderText = "开始时间";
            column4.DataPropertyName = "Column12";
            column4.Name = "beginDate";
            this.dgvMonthCard.Columns.Add(column4);

            DataGridViewColumn column5 = new DataGridViewTextBoxColumn();
            column5.HeaderText = "结束时间";
            column5.DataPropertyName = "Column13";
            column5.Name = "endDate";
            this.dgvMonthCard.Columns.Add(column5);

            DataGridViewColumn column6 = new DataGridViewTextBoxColumn();
            column6.HeaderText = "发行时间";
            column6.DataPropertyName = "Column18";
            column6.Name = "starDate";
            this.dgvMonthCard.Columns.Add(column6);

            DataGridViewColumn column7 = new DataGridViewTextBoxColumn();
            column7.HeaderText = "开卡金额";
            column7.DataPropertyName = "Column11";
            column7.Name = "payAmount";
            this.dgvMonthCard.Columns.Add(column7);

            DataGridViewColumn column8 = new DataGridViewTextBoxColumn();
            column8.HeaderText = "状态";
            //column8.DataPropertyName = "Column11";
            column8.Name = "state";
            this.dgvMonthCard.Columns.Add(column8);
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

        /// <summary>
        /// 数据源筛选下拉列表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSelectCarType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbSelectCarType.SelectedIndex > 0)
            {
                string selectCarType = this.cbSelectCarType.Text;
                DataTable dtNew = ExcelMonthCardDt.Clone();
                foreach (var item in ExcelMonthCardDt.Select($"Column2 = '{selectCarType}'"))
                {
                    dtNew.Rows.Add(item.ItemArray);
                }
                this.dgvMonthCard.DataSource = null;                
                this.dgvMonthCard.DataSource = dtNew;

                this.lblDataCount.Text = dtNew.Rows.Count.ToString();
            }
            else
            {
                this.dgvMonthCard.DataSource = null;
                this.dgvMonthCard.DataSource = ExcelMonthCardDt;
            }
        }

        /// <summary>
        /// 项目集合下拉列表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.cbParking.Items.Clear();
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

        /// <summary>
        /// 停车场集合下拉列表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbParking_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.cbCarType.Items.Clear();
            string projectGuid = Convert.ToString(this.cbProject.SelectedValue);
            string parkingCode = Convert.ToString(this.cbParking.SelectedValue);
            if (string.IsNullOrEmpty(projectGuid) || string.IsNullOrEmpty(parkingCode))
                return;

            Dictionary<string, string> dicCarType = GetAllCarType(projectGuid, parkingCode);
            if (dicCarType == null || dicCarType.Count <= 0)
            {
                this.cbCarType.DataSource = null;
                this.cbCarType.Items.Add("==暂未创建车类==");
                this.cbCarType.SelectedIndex = 0;
                return;
            }

            BindingSource bs = new BindingSource();
            bs.DataSource = dicCarType;
            this.cbCarType.ValueMember = "Key";
            this.cbCarType.DisplayMember = "Value";
            this.cbCarType.DataSource = bs;
            this.cbCarType.SelectedIndex = -1;
        }

        /// <summary>
        /// 文件选择事件
        /// 仅支持xlsx格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(fileDialog.FileName);
                string[] str = new string[] { ".xlsx" };
                if (!((IList)str).Contains(extension))
                {
                    MessageBox.Show("仅能上传xlsx格式的文件！");
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(fileDialog.FileName);
                    this.txtFilePath.Text = fileInfo.FullName;

                    DataTable dtMonthCard = ReadExcelToTable(fileInfo.FullName);

                    //格式化去掉头部和尾部多余数据（e7导出）
                    //删除尾部3行
                    dtMonthCard.Rows.RemoveAt(dtMonthCard.Rows.Count - 1);
                    dtMonthCard.Rows.RemoveAt(dtMonthCard.Rows.Count - 1);
                    dtMonthCard.Rows.RemoveAt(dtMonthCard.Rows.Count - 1);
                    //删除头部2行
                    dtMonthCard.Rows.RemoveAt(0);
                    dtMonthCard.Rows.RemoveAt(0);

                    this.dgvMonthCard.DataSource = null;
                    this.dgvMonthCard.DataSource = dtMonthCard;


                    if (dtMonthCard != null && dtMonthCard.Rows.Count > 0)
                    {
                        ExcelMonthCardDt = dtMonthCard;

                        this.lblDataCount.Text = dtMonthCard.Rows.Count.ToString();
                        this.btnImport.Enabled = true;

                        this.cbSelectCarType.Items.Add("==全部==");
                        this.cbSelectCarType.SelectedIndex = 0;
                        this.cbSelectCarType.Items.AddRange(GetDistinctValues(dtMonthCard, "Column2"));
                        this.cbSelectCarType.SelectedIndexChanged += new EventHandler(cbSelectCarType_SelectedIndexChanged);
                    }
                    else
                    {
                        ExcelMonthCardDt = null;

                        this.lblDataCount.Text = "0";
                        this.btnImport.Enabled = true;

                        this.cbSelectCarType.Items.Clear();
                    }

                }
            }

        }

        /// <summary>
        /// 将excel导出成DataTable
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        private static DataTable ReadExcelToTable(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:

                        // 1. Use the reader methods
                        do
                        {
                            while (reader.Read())
                            {
                                // reader.GetDouble(0);
                            }
                        } while (reader.NextResult());

                        // 2. Use the AsDataSet extension method
                        var result = reader.AsDataSet();

                        // The result of each spreadsheet is in result.Tables                        
                        return result == null ? null : result.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }


        }

        /// <summary>
        /// 数据上传导入点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否已完成数据核对？", "请确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }
            if (MessageBox.Show("要不再确认一遍？", "请确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
            {
                return;
            }

            ProjectGuid = Convert.ToString(this.cbProject.SelectedValue);
            ParkingCode = Convert.ToString(this.cbParking.SelectedValue);
            string cbCarTypeValue = Convert.ToString(this.cbCarType.SelectedValue);
            if (!string.IsNullOrEmpty(cbCarTypeValue))
            {
                CarTypeGuid = cbCarTypeValue.Split(',')[0];
                CarTypeIdx = cbCarTypeValue.Split(',')[1];
            }

            if (string.IsNullOrEmpty(ProjectGuid) || string.IsNullOrEmpty(ParkingCode) || string.IsNullOrEmpty(CarTypeGuid) || string.IsNullOrEmpty(CarTypeIdx))
            {
                MessageBox.Show("请先选择车类", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //Dictionary<string,string> dicDriveway = GetAllDriveway(ProjectGuid, ParkingCode);
            //if (dicDriveway != null)
            //{
            //    DrivewayList = new List<string>();
            //    foreach (var item in dicDriveway.Keys)
            //    {
            //        DrivewayList.Add(item);
            //    }
            //}

            Thread tMain = new Thread(() => MonthCardImport());
            tMain.IsBackground = true;
            tMain.Start();

            AllControlEnabled(false);
        }

        /// <summary>
        /// 月卡导入方法
        /// </summary>
        private void MonthCardImport()
        {
            if (this.dgvMonthCard.DataSource != null)
            {
                this.Invoke(ShowMsgDistance, ShowMsgType.ShowMsgAll, this.dgvMonthCard.Rows.Count, "");

                int threadCount = 1;
                Int32.TryParse(this.txtThreadCount.Text, out threadCount);
                var waits = new AutoResetEvent[threadCount];

                int i = 0;
                foreach (DataGridViewRow item in this.dgvMonthCard.Rows)
                {
                    var threadIndex = 0;
                    AutoResetEvent wait;
                    if (i < threadCount)
                    {
                        waits[i] = new AutoResetEvent(false);
                        wait = waits[i];
                        threadIndex = i;
                    }
                    else
                    {
                        threadIndex = WaitHandle.WaitAny(waits);
                        wait = waits[threadIndex];
                    }
                    i++;
                    var newI = i;
                    Thread t = new Thread(() => ImportData(item, newI, wait));
                    t.IsBackground = true;
                    t.Start();
                }
            }
            //AllControlEnabled(true);
        }

        /// <summary>
        /// 月卡信息导入到4G项目
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="rowIndex"></param>
        /// <param name="e"></param>
        private void ImportData(DataGridViewRow dataRow, int rowIndex, AutoResetEvent e)
        {
            string carOwnerName = Convert.ToString(dataRow.Cells[1].Value);
            string carNo = Convert.ToString(dataRow.Cells[0].Value);
            if (string.IsNullOrEmpty(carOwnerName) || string.IsNullOrEmpty(carNo))
            {
                this.Invoke(ShowMsgDistance, ShowMsgType.ShowFail, rowIndex, "空数据");
                e.Set();
                return;
            }

            CardServiceModel cardServiceModel = new CardServiceModel()
            {
                ProjectGuid = ProjectGuid,
                ParkCode = ParkingCode,
                CarOwnerName = carOwnerName,
                CarNo = carNo,
                CarTypeGuid = CarTypeGuid,
                DrivewayGuidList = new List<string>(),//DrivewayList,
                Mobile = dataRow.Cells[2].Value.ToString(),
                PayAmount = Convert.ToDecimal(dataRow.Cells[6].Value),
                PayStyle = "现金",
                Remark = "批量导入",
                Balance = Convert.ToDecimal(dataRow.Cells[6].Value), //新开卡时余额就等于支付金额(仅当储值卡时后端逻辑才会读取此值)
                StartDate = Convert.ToDateTime(dataRow.Cells[5].Value),
                PrimaryEndDate = Convert.ToDateTime(dataRow.Cells[5].Value),
                EndDate = Convert.ToDateTime(dataRow.Cells[4].Value),
                RechargeOperator = "系统管理员",
                PauseDate = default(DateTime),
                Locked = false,
                Enable = true
            };


            //先验证月卡数据是否已存在
            bool carExist = ExistCardService(cardServiceModel.ParkCode, cardServiceModel.CarNo);            
            if (carExist)
            {
                this.Invoke(ShowMsgDistance, ShowMsgType.ShowFail, rowIndex, "已存在");
                e.Set();
                return;
            }

            bool flag = ImportDataToFujica(cardServiceModel, CarTypeIdx);
            if (!flag)
            {
                this.Invoke(ShowMsgDistance, ShowMsgType.ShowFail, rowIndex, "保存fujica失败");
                e.Set();
                return;
            }
            flag = ImportDataToDB(cardServiceModel);
            if (!flag)
            {
                this.Invoke(ShowMsgDistance, ShowMsgType.ShowFail, rowIndex, "保存数据库失败");
                e.Set();
                return;
            }

            flag = ImportDataToMQ(cardServiceModel);
            if (!flag)
            {
                this.Invoke(ShowMsgDistance, ShowMsgType.ShowFail, rowIndex, "发送MQ失败");
                e.Set();
                return;
            }

            if (flag)
                this.Invoke(ShowMsgDistance, ShowMsgType.ShowSuccess, rowIndex, "");
            
            

            //Thread.Sleep(Convert.ToInt32(new Random().Next(5) + "000"));

            ////ShowMsgDistance.Invoke(ShowMsgType.ShowTip, rowIndex);

            //if (rowIndex.ToString().IndexOf("20") > -1)
            //{

            //    this.Invoke(ShowMsgDistance, ShowMsgType.ShowFail, rowIndex);
            //}
            //else
            //{

            //    this.Invoke(ShowMsgDistance, ShowMsgType.ShowSuccess, rowIndex);
            //}

            //ShowMsgDistance.Invoke(ShowMsgType.ShowMsgOne, rowIndex + 1);
            e.Set();
        }

        private bool ImportDataToDB(CardServiceModel model)
        {
            return SaveCardService(model);
        }
        private bool ImportDataToMQ(CardServiceModel model)
        {
            try
            {
                bool result = false;
                MonthCardModel sendmodel = new MonthCardModel()
                {
                    CarOwnerName = model.CarOwnerName,
                    CarNo = model.CarNo,
                    Delete = model.Enable ? false : true,
                    CarTypeGuid = model.CarTypeGuid,
                    Locked = model.Locked,
                    StartDate = model.StartDate.Date.AddHours(0).AddMinutes(0).AddSeconds(0).ToString("yyyy-MM-dd HH:mm:ss"),
                    EndDate = model.EndDate.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") //结束时间截止到23:59:59
                };

                CommandEntity<MonthCardModel> entity = new CommandEntity<MonthCardModel>()
                {
                    command = BussineCommand.MonthCar,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = sendmodel
                };

                result = m_rabbitMQ.SendMessageForRabbitMQ("发送月卡命令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
                return result;
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", model.ParkCode, model.CarNo, "CameraUpdateTool.MainForm.ImportDataToMQ", "下发月卡时发生异常", ex.ToString());
                return false;
            }
        }
        private bool ImportDataToFujica(CardServiceModel model, string idx)
        {
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            string beginDt = model.StartDate.Date.ToString("yyyy-MM-dd HH:mm:ss");//开始时间从0分秒
            string endDt = model.EndDate.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");  //结束时间截止到23:59:59
            model.EndDate = Convert.ToDateTime(model.EndDate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            model.PrimaryEndDate = Convert.ToDateTime(model.PrimaryEndDate.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "Park/UnderLineMonthCard";
            dicParam["OperatType"] = 0; //操作类型 -新增
            //请求参数  
            dicParam["CarType"] = idx;
            dicParam["PhoneNumber"] = model.Mobile;
            dicParam["OwnerName"] = model.CarOwnerName;
            dicParam["StartDate"] = Convert.ToDateTime(beginDt);
            dicParam["PrimaryEndDate"] = model.PrimaryEndDate; //计算前结束日期 
            dicParam["EndDate"] = Convert.ToDateTime(endDt); //计算后结束日期
            dicParam["RenewalType"] = 1; //目前默认都是按天延期  
            dicParam["RenewalValue"] = (model.EndDate - model.PrimaryEndDate).Days;
            dicParam["ParkingCode"] = model.ParkCode;
            dicParam["CarNo"] = model.CarNo;
            dicParam["RechargeAmount"] = model.PayAmount;
            dicParam["PaymentType"] = 98;//支付类型：4G支付
            dicParam["OrderType"] = 99;//订单类型：现金支付
            dicParam["TransactionTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dicParam["Remarks"] = model.Remark;
            dicParam["OrderStauts"] = 4; //线下确认
            dicParam["RechargeOperator"] = model.RechargeOperator;
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }


        public void ShowMsgForm(ShowMsgType st, int msgIndex, string msg)
        {
            if (st == ShowMsgType.ShowMsgAll)
            {
                //if (this.progressBar1.InvokeRequired)
                //{
                //    this.progressBar1.Invoke(ShowMsgDistance, new object[] { st, msgIndex });
                //}
                //else
                this.progressBar1.Maximum = msgIndex;

            }
            else if (st == ShowMsgType.ShowFail)
            {
                //修改数据源的状态
                this.dgvMonthCard.Rows[msgIndex - 1].Cells[7].Value = msg;
                this.dgvMonthCard.Rows[msgIndex - 1].Cells[7].Style.ForeColor = Color.Red;
                this.dgvMonthCard.FirstDisplayedScrollingRowIndex = msgIndex - 1;

                //修改进度条的当前值
                this.progressBar1.Value = Convert.ToInt32(this.progressBar1.Value) + 1;
                //修改失败条数
                this.lblFailCount.Text = (Convert.ToInt32(this.lblFailCount.Text) + 1).ToString();
            }
            else if (st == ShowMsgType.ShowSuccess)
            {
                //修改数据源的状态
                //if (this.dgvMonthCard.InvokeRequired)
                //{
                //    this.dgvMonthCard.Invoke(ShowMsgDistance, new object[] { st, msgIndex });
                //}
                //else
                //{
                //    this.dgvMonthCard.Rows[msgIndex - 1].Cells[7].Value = "成功";
                //    this.dgvMonthCard.Rows[msgIndex - 1].Cells[7].Style.ForeColor = Color.Green;
                //    this.dgvMonthCard.FirstDisplayedScrollingRowIndex = msgIndex - 1;

                //}
                //修改数据源的状态
                this.dgvMonthCard.Rows[msgIndex - 1].Cells[7].Value = "成功";
                this.dgvMonthCard.Rows[msgIndex - 1].Cells[7].Style.ForeColor = Color.Green;
                this.dgvMonthCard.FirstDisplayedScrollingRowIndex = msgIndex - 1;

                //修改进度条的当前值
                this.progressBar1.Value = Convert.ToInt32(this.progressBar1.Value) + 1;
                //修改成功条数
                this.lblSuccessCount.Text = (Convert.ToInt32(this.lblSuccessCount.Text) + 1).ToString();
            }

        }

        public enum ShowMsgType
        {
            /// <summary>
            /// 显示所有数量
            /// </summary>
            ShowMsgAll,
            /// <summary>
            /// 成功提示
            /// </summary>
            ShowSuccess,
            /// <summary>
            /// 失败提示
            /// </summary>
            ShowFail,
        }

        private void AllControlEnabled(bool enabled)
        {
            this.btnFilePath.Enabled = enabled;
            this.btnImport.Enabled = enabled;
            this.cbProject.Enabled = enabled;
            this.cbParking.Enabled = enabled;
            this.cbCarType.Enabled = enabled;
        }

        /// <summary>
        /// 数据验证按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChecking_Click(object sender, EventArgs e)
        {
            MessageBox.Show("晚点做这个功能吧！");
        }

        /// <summary>
        /// 数据源的序号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvMonthCard_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               this.dgvMonthCard.RowHeadersWidth - 4,
               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,
                (e.RowIndex + 1).ToString(),
                this.dgvMonthCard.RowHeadersDefaultCellStyle.Font,
                rectangle,
                this.dgvMonthCard.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        /// <summary>
        /// DataTable某列去重复，返回所有值
        /// </summary>
        /// <param name="dtable"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public string[] GetDistinctValues(DataTable dtable, string colName)
        {
            Hashtable hTable = new Hashtable();
            foreach (DataRow drow in dtable.Rows)
            {
                try
                {
                    hTable.Add(drow[colName], string.Empty);
                }
                catch { }
            }
            string[] objArray = new string[hTable.Keys.Count];
            hTable.Keys.CopyTo(objArray, 0);
            return objArray;
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
            catch (Exception) { }
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
        /// 获取所有月卡车类
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetAllCarType(string projectGuid, string parkingCode)
        {
            Dictionary<string, string> dicCarType = new Dictionary<string, string>();
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
                command.CommandText = "select * from t_cartype where projectGuid = @projectGuid and parkCode = @parkCode and (carType = 1 or carType = 3) order by idx";// and (carType = 1 or carType = 3 )
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

                        foreach (DataRow item in table.Rows)
                        {
                            dicCarType.Add(item["guid"].ToString()+","+ item["idx"].ToString(), item["carTypeName"].ToString());
                        }
                    }
                }

                dbc.Close();
            }
            catch (Exception) { }
            return dicCarType;
        }

        /// <summary>
        /// 获取所有车道
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetAllDriveway(string projectGuid, string parkingCode)
        {
            Dictionary<string, string> dicCarType = new Dictionary<string, string>();
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
                command.CommandText = "select * from t_driveway where projectGuid = @projectGuid and parkCode = @parkCode ";// and (carType = 1 or carType = 3 )
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

                        foreach (DataRow item in table.Rows)
                        {
                            dicCarType.Add(item["guid"].ToString(), item["drivewayName"].ToString());
                        }
                    }
                }

                dbc.Close();
            }
            catch (Exception) { }
            return dicCarType;
        }

        /// <summary>
        /// 保存月卡信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool SaveCardService(CardServiceModel model)
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
                command.CommandText = @"insert into t_monthcard(projectGuid,identifying,parkCode,carNo,carOwnerName,mobile,carTypeGuid,remark,enable,locked,startDate,endDate,pauseDate,continueDate,drivewayListContent,rechargeOperator)
                                values(@projectGuid,@identifying,@parkCode,@carNo,@carOwnerName,@mobile,@carTypeGuid,@remark,@enable,@locked,@startDate,@endDate,@pauseDate,@continueDate,@drivewayListContent,@rechargeOperator)";
                command.CommandType = CommandType.Text;


                DbParameter projectGuid = factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = model.ProjectGuid;

                DbParameter identifying = factory.CreateParameter();
                identifying.ParameterName = "@identifying";
                identifying.Value = model.ParkCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(model.CarNo));

                DbParameter parkCode = factory.CreateParameter();
                parkCode.ParameterName = "@parkCode";
                parkCode.Value = model.ParkCode;

                DbParameter carNo = factory.CreateParameter();
                carNo.ParameterName = "@carNo";
                carNo.Value = model.CarNo;

                DbParameter carOwnerName = factory.CreateParameter();
                carOwnerName.ParameterName = "@carOwnerName";
                carOwnerName.Value = model.CarOwnerName;

                DbParameter mobile = factory.CreateParameter();
                mobile.ParameterName = "@mobile";
                mobile.Value = model.Mobile;

                DbParameter carTypeGuid = factory.CreateParameter();
                carTypeGuid.ParameterName = "@carTypeGuid";
                carTypeGuid.Value = model.CarTypeGuid;

                DbParameter remark = factory.CreateParameter();
                remark.ParameterName = "@remark";
                remark.Value = model.Remark;

                DbParameter enable = factory.CreateParameter();
                enable.ParameterName = "@enable";
                enable.Value = model.Enable;

                DbParameter locked = factory.CreateParameter();
                locked.ParameterName = "@locked";
                locked.Value = model.Locked;

                DbParameter startDate = factory.CreateParameter();
                startDate.ParameterName = "@startDate";
                startDate.Value = model.StartDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter endDate = factory.CreateParameter();
                endDate.ParameterName = "@endDate";
                endDate.Value = model.EndDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter pauseDate = factory.CreateParameter();
                pauseDate.ParameterName = "@pauseDate";
                pauseDate.Value = model.PauseDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter continueDate = factory.CreateParameter();
                continueDate.ParameterName = "@continueDate";
                continueDate.Value = model.ContinueDate.ToString("yyyy-MM-dd HH:mm:ss");

                DbParameter drivewayListContent = factory.CreateParameter();
                drivewayListContent.ParameterName = "@drivewayListContent";
                drivewayListContent.Value = m_serializer.Serialize(model.DrivewayGuidList ?? new List<string>());


                DbParameter rechargeOperator = factory.CreateParameter();
                rechargeOperator.ParameterName = "@rechargeOperator";
                rechargeOperator.Value = model.RechargeOperator;

                DbParameter[] parameter = new DbParameter[] { projectGuid, identifying, parkCode, carNo, carOwnerName, mobile, carTypeGuid, remark, enable, locked, startDate, endDate, pauseDate, continueDate, drivewayListContent, rechargeOperator };

                command.Parameters.AddRange(parameter);

                return command.ExecuteNonQuery() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "CameraUpdateTool.MainForm.SaveCardService", string.Format("保存卡信息时发生异常，入参:{0}", m_serializer.Serialize(model)), ex.ToString());
            }
            return false;
        }

        private bool ExistCardService(string parkingCode, string carNo)
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
                command.CommandText = "select count(1) from t_monthcard where identifying=@identifying";
                command.CommandType = CommandType.Text;


                DbParameter identifying = factory.CreateParameter();
                identifying.ParameterName = "@identifying";
                identifying.Value = parkingCode + Convert.ToBase64String(Encoding.UTF8.GetBytes(carNo));


                DbParameter[] parameter = new DbParameter[] { identifying };

                command.Parameters.AddRange(parameter);

                return Convert.ToInt32(command.ExecuteScalar()) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "CameraUpdateTool.MainForm.ExistCardService", "验证卡信息时发生异常", ex.ToString());
            }
            return false;
        }

        #endregion

    }
}
