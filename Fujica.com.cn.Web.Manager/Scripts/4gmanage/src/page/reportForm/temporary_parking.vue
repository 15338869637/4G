<template>
  <div class="temporary-parking flex-box column">
    <div class="main box">
        <div class="query left">
            <div class="condition">
                <div class="condition-tit">停车场:</div>
                <div class="condition-con">
                    <el-select v-model="form.ParkingCode" @change="ParkingCodeChange" class="w-input" placeholder="请选择">
                      <el-option
                        v-for="item in option.ParkingCode"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value">
                      </el-option>
                    </el-select>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">车牌号:</div>
                <div class="condition-con">
                    <el-input v-model="form.LicensePlate" class="w-input" placeholder="请输入内容"></el-input>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">车辆类型:</div>
                <div class="condition-con">
                    <el-select v-model="form.CarType" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                            v-for="item in option.CarType"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value">
                        </el-option>
                    </el-select>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">计费截止时间:</div>
                <div class="condition-con">
                    <el-date-picker
                    class="w-input"
                    v-model="dateTime"
                    type="datetimerange"
                    :picker-options="picker_options"
                    range-separator="至"
                    start-placeholder="开始日期"
                    end-placeholder="结束日期"
                    :default-time="['00:00:00', '23:59:59']">
                    </el-date-picker>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">支付方式:</div>
                <div class="condition-con">
                    <el-select v-model="form.PaymentType" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option label="现金支付" :value="98"></el-option>
                        <el-option label="微信支付" :value="2"></el-option>
                        <el-option label="支付宝支付" :value="4"></el-option>
                    </el-select>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">值班人员:</div>
                <div class="condition-con">
                    <el-select v-model="form.TollOperator" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                            v-for="item in option.OpenGateOperator"
                            :key="item.index"
                            :label="item.label"
                            :value="item.value">
                        </el-option>
                    </el-select>
                </div>
            </div>
        <div>
        <el-button type="primary" @click="init(form)">搜索</el-button>
        <el-button type="primary" @click="Reset">重置</el-button>
        </div>
        </div>
    </div>
    <div class="main flex1 flex-box column">
      <Title titleMsg="临时车缴费记录">
        <el-button slot="right" type="primary" class="rightBtn" @click="exportExcel" :loading="loading">导出Excel报表</el-button>
      </Title>
      <div class="table flex1 auto">
        <el-table
          :data="tableData"
          border
          header-row-class-name="header-class-name">
          <span class="empty-ico" slot="empty">
            <i>
            </i>
            <span>暂无数据</span>
          </span>
          <el-table-column
            label="序号"
            width="50">
            <template slot-scope="scope">
              {{scope.$index + 1 + (page.page - 1) * page.size}}
            </template>
          </el-table-column>
          <el-table-column
            prop="LicensePlate"
            width="100"
            label="车牌号">
          </el-table-column>
          <el-table-column
            prop="CarType"
            width="100"
            label="车辆类型">
          </el-table-column>
          <el-table-column
            prop="AdmissionDate"
            sortable
            width="180"
            label="入场时间">
          </el-table-column>
          <el-table-column
            prop="BillingStartTime"
            sortable
            width="180"
            label="计费开始时间">
          </el-table-column>
          <el-table-column
            prop="BillingDeadline"
            sortable
            width="180"
            label="计费截止时间">
          </el-table-column>
          <el-table-column
            prop="LongStop"
            sortable
            width="120"
            label="停车时长">
          </el-table-column>
          <el-table-column
            prop="AmountReceivable"
            width="120"
            label="停车应缴金额">
            <template slot-scope="scope">
              {{scope.row.AmountReceivable || '0.00'}}
            </template>
          </el-table-column>
          <el-table-column
            prop="AmountReceived_xs"
            width="120"
            label="线上支付金额">
            <template slot-scope="scope">
              {{scope.row.AmountReceived_xs || '0.00'}}
            </template>
          </el-table-column>
          <el-table-column
            width="80"
            prop="Discount"
            label="优惠金额">
            <template slot-scope="scope">
              {{scope.row.Discount || '0.00'}}
            </template>
          </el-table-column>
          <el-table-column
            prop="AmountReceived_xx"
            width="120"
            label="值班人员实收">
            <template slot-scope="scope">
              {{scope.row.AmountReceived_xx || '0.00'}}
            </template>
          </el-table-column>
          <el-table-column
            prop="FreeAdmission"
            width="80"
            label="人工免费">
            <template slot-scope="scope">
              {{scope.row.FreeAdmission || '0.00'}}
            </template>
          </el-table-column>
          <el-table-column
            prop="PaymentType"
            width="100"
            label="支付方式">
          </el-table-column>
          <el-table-column
            prop="Remarks"
            width="120"
            label="备注">
          </el-table-column>
          <el-table-column
            prop="TollOperator"
            width="80"
            label="值班人员">
          </el-table-column>
          <el-table-column
            width="120"
            label="操作">
            <template slot-scope="scope">
              <el-button type="text" @click="details(scope.row)">详细信息</el-button>
            </template>
          </el-table-column>
        </el-table>
        <page :page="page" @PageChange="init"> </page>
      </div>
    </div>
    <v-details :dialog="dialog" v-if="dialog.show"></v-details>
  </div>
</template>
  
<script>
import tinytime from 'format-datetime'
import Title from '@/components/Title'
import page from '@/components/page'
import {getCookie} from '@/config/system.js'
import details from './details'
import {
  GetCarTypeList,
  GetUserList,
  SearchParkingRecord,
  SearchPaymentRecord,
  GetParkLotList,
  GetRolePermission
} from './api'
export default {
  data () {
    return {
      form: {
        ParkingCode: '',
        BillingStartTime: '',
        BillingDeadline: '',
        LicensePlate: '',
        CarType: '',
        PaymentType: '',
        presentationCategory: '',
        TollOperator: ''
      },
      Request: {},
      option: {
        ParkingCode: [],
        OpenGateOperator: [],
        CarType: [],
      },
      OutThroughType: {
        '1': '自动开闸',
        '2': '手动开闸',
        '3': '免费开闸',
        '4': '收费放行'
      },
      CarTypeName: {},
      dateTime: [],
      tableData: [],
      page: {
        page: 1,
        size: 10,
        total: 0
      },
      dialog: {
        title: '缴费详情',
        data: {}, // 数据字段
        show: false,
        rcImg: 'InImgUrl',
        ccImg: 'OutImgUrl',
        rc: [
          {
            title: '车牌号:',
            content: 'LicensePlate'
          },
          {
            title: '车辆类型:',
            content: 'CarType'
          },
          {
            title: '入场时间:',
            content: 'AdmissionDate'
          },
          {
            title: '识别相机:',
            content: 'InDiscernCamera'
          },
          {
            title: '入场通道:',
            content: 'Entrance'
          },
          {
            title: '通行方式:',
            content: 'InThroughType'
          },
          {
            title: '值班人员:',
            content: 'InParkingOperator'
          }
        ],
        cc: [
          {
            title: '车牌号:',
            content: 'LicensePlate'
          },
          {
            title: '车辆类型:',
            content: 'CarType'
          },
          {
            title: '出场时间:',
            content: 'AppearanceDate'
          },
          {
            title: '识别相机:',
            content: 'OutDiscernCamera'
          },
          {
            title: '出场通道:',
            content: 'Export'
          },
          {
            title: '通行方式:',
            content: 'OutThroughType'
          },
          {
            title: '值班人员:',
            content: 'OutParkingOperator'
          }
        ],
        sf: [
          // {
          //   title: '收费信息',
          //   data: [
          //     {
          //       title: '测试收费标题',
          //       content: '测试收费内容'
          //     }
          //   ]
          // }
        ]
      },
      picker_options: {
        disabledDate: item => {
          let time = tinytime(new Date(), 'yyyy-MM-dd 23:59:59')
          return new Date(item).getTime() > new Date(time).getTime()
        }
      },
      loading: false
    }
  },
  watch: {
    dateTime (val) {
      if (val && val[0] && val[1]) {
        [this.form.BillingStartTime, this.form.BillingDeadline] = [this.date_time(val[0]), this.date_time(val[1])]
      } else {
        [this.form.BillingStartTime, this.form.BillingDeadline] = ['', '']
      }
    }
  },
  components: {
    Title,
    page,
    'v-details': details
  },
  methods: {
    async ParkingCodeChange () {
      await this.getCarType();
      await this.GetUserList();
      this.init(this.form)
    },
    init (Request) {
      if (Request) {
        Object.assign(this.Request, Request)
        this.page.page = 1
      }
      console.log('init')
      let data = Object.assign({
        FieldSort: 'BillingDeadline', 
        Sort: 1,
        PageIndex: this.page.page,
        PageSize: this.page.size
      }, this.Request)
      for(let name in data) {
        data[name] === '' && (delete(data[name]))
      }
      data['LicensePlate'] && (data['LicensePlate'] = this.LicensePlate(data['LicensePlate'].toUpperCase()))
      SearchPaymentRecord(data).then(res => {
        if (res.IsSuccess && res.PaperCouponList && res.PaperCouponList.length) {
          this.tableData = this.filterData(res.PaperCouponList)
          this.page.total = res.TotalCount
        } else {
          this.tableData = []
          this.page.total = 0
        }
      })
    },
    // 过滤数据格式
    filterData(list) {
      return list.map(item => {
            let obj = Object.assign({}, item)
            item.AdmissionDate && (obj.AdmissionDate = this.date_time(item.AdmissionDate))
            item.AppearanceDate && (obj.AppearanceDate = this.date_time(item.AppearanceDate))
            item.BillingStartTime && (obj.BillingStartTime = this.date_time(item.BillingStartTime))
            item.BillingDeadline && (obj.BillingDeadline = this.date_time(item.BillingDeadline))
            item.PaymentTime && (obj.PaymentTime = this.date_time(item.PaymentTime))
            item.InvoiceTime && (obj.InvoiceTime = this.date_time(item.InvoiceTime))
            item.InThroughType && (obj.InThroughType = this.OutThroughType[item.InThroughType])
            item.OutThroughType && (obj.OutThroughType = this.OutThroughType[item.OutThroughType])
            // 支付方式
            typeof item.PaymentType !== 'undefined' && (obj.PaymentType = this.PaymentType(item.PaymentType))
            // 停车时长
            if (item.BillingStartTime) {
              obj.LongStop = this.LongStop(item.AdmissionDate, item.BillingDeadline)
            } else {
              obj.LongStop = undefined
            }
            // 车类
            typeof item.CarType !== 'undefined' && (obj.CarType = this.CarTypeName[item.CarType])

            if (item.AmountReceived) {
              // 优惠金额
              obj.Discount = item.AmountReceivable - item.AmountReceived
              // 线上支付
              obj.AmountReceived_xs = [98, 99, 0].indexOf(item.PaymentType) === -1 ? item.AmountReceived : ''
              // 线下支付
              obj.AmountReceived_xx = [99, 98].indexOf(item.PaymentType) > -1 ? item.AmountReceived : ''
              obj.AmountReceivable = (obj.AmountReceivable || 0).toFixed(2)
              obj.AmountReceived_xs = (obj.AmountReceived_xs || 0).toFixed(2)
              obj.AmountReceived_xx = (obj.AmountReceived_xx || 0).toFixed(2)
              obj.Discount = (obj.Discount || 0).toFixed(2)
            }

            typeof item.AmountReceivable !== 'undefined' && (obj.AmountReceivable = (item.AmountReceivable || 0).toFixed(2))
            typeof item.AmountReceived !== 'undefined' && (obj.AmountReceived = (item.AmountReceived || 0).toFixed(2))
            typeof item.Discount !== 'undefined' && (obj.Discount = (item.Discount || 0).toFixed(2))
            typeof item.AmountReceived !== 'undefined' && (obj.AmountReceived = (item.AmountReceived || 0).toFixed(2))
            typeof item.FreeAdmission !== 'undefined' && (obj.FreeAdmission = (item.FreeAdmission || 0).toFixed(2))
            return obj
          })
    },
    date_time (time) {
      if (!time) return ''
      return tinytime(new Date(time), 'yyyy-MM-dd HH:mm:ss')
    },
    LongStop(s, e) {
      if (!s || !e) return ''
      let sta = new Date(s).getTime()
      let end = new Date(e).getTime()
      let stopTime = end - sta
      let H = parseInt(stopTime / (1000 * 60 * 60))
      let M = parseInt(stopTime % (1000 * 60 * 60) / (1000 * 60))
      let S = parseInt(stopTime % (1000 * 60) / (1000))
      return `${H}小时${M}分`
    },
    Reset() {
      Object.assign(this.form, {
        BillingStartTime: '',
        BillingDeadline: '',
        LicensePlate: '',
        CarType: '',
        PaymentType: '',
        presentationCategory: '',
        TollOperator: ''
      })
      this.dateTime = []
    },
    // 获取停车场信息
    async getParkInfo(){
      var RoleGuid = getCookie("RoleGuid");
      const resListAll = await GetParkLotList()
      const resListAllfilter = resListAll.filter(item=>item.Existence==true)   //保留existence为true的
      const resList = await GetRolePermission({
        Guid:RoleGuid
      });
      const arr=[]
      for(var i=0;i<resListAllfilter.length;i++){
        for(var j=0;j<resList.ParkingCodeList.length;j++){
          if(resListAllfilter[i].ParkCode==resList.ParkingCodeList[j]){
            arr.push({
              value: resListAllfilter[i].ParkCode,
              label: resListAllfilter[i].ParkName
            });
          }
        }
      }

      this.option.ParkingCode=arr
      this.form.ParkingCode = this.$route.query.parkCode ? this.$route.query.parkCode : arr[0].value
      return Promise.resolve({})
    },
    async details (d) {
      let res = await SearchParkingRecord({
        id: d.ParkingRecordCode, 
        ParkingCode: this.form.ParkingCode,
        PageIndex: 1,
        PageSize: 10
        })
        let data = Object.assign({}, d)
        if (res.IsSuccess) {
          if (res.PaperCouponList.length) {
              let obj = this.filterData(res.PaperCouponList)[0];
              for (let key in obj) {
                obj[key] && (data[key] = obj[key])
              }
              console.log('details', data)
              let title = ['停车应缴金额:', '线上支付金额:', '优惠金额:', '值班人员实收:', '人工免费:', '停车时长:', '支付方式:', '收费时间:']
              let content = ['AmountReceivable', 'AmountReceived_xs', 'Discount', 'AmountReceived_xx', 'FreeAdmission', 'LongStop', 'PaymentType', 'PaymentTime']
              let list = {
                title: '收费信息',
                data: title.map((item, i) => {
                  let obj = {
                    title: item,
                      content: data[content[i]]
                    }
                    return obj
                  })
              }
            this.dialog.sf[0] = list
          } else {
            this.dialog.sf = []
          }
        } else {
          res.MessageContent && this.$message.error(res.MessageContent)
        }
        this.dialog.data = data
        this.dialog.show = true
    },
    // 导出
    async exportExcel () {
      this.loading = true
      let data = []
      let TotalCount = 0;
      const PageSize = 100;
      let Obj = Object.assign({
        PageIndex: 1,
        PageSize: PageSize
      }, this.form)
      for(let name in Obj) {
        Obj[name] === '' && (delete(Obj[name]))
      }
      Obj['LicensePlate'] && (Obj['LicensePlate'] = this.LicensePlate(Obj['LicensePlate'].toUpperCase()))
      await SearchPaymentRecord(Obj).then(res => {
        if (res.IsSuccess) {
          data = data.concat(this.filterData(res.PaperCouponList))
          TotalCount = res.TotalCount
          Obj.NextToken = res.NextToken
        }
      })
      if (TotalCount === 0) {
        this.$message.warning('无数据');
        this.loading = false
        return 
      }
      if (TotalCount > this.Upperlimit) {
        this.$message.warning('数据量较大，为保证下载速度和结果，建议你可添加搜索条件分多次完成导出');
        this.loading = false
        return
      }
      for (let i = 1;i*PageSize < TotalCount; i++) {
        await SearchPaymentRecord(Obj).then(res => {
          if (res.IsSuccess) {
            data = data.concat(this.filterData(res.PaperCouponList))
            Obj.NextToken = res.NextToken
          }
        })
      }
      const {
        export_json_to_excel,
        formatDay
      } = require("@/vendor/Export2Excel.js");
      let tHeader = ['车牌号','计费类型','入场时间','计费开始时间','计费截止时间','停车时长','停车应缴金额','线上支付金额','优惠金额','值班人员实收','人工免费','支付方式','备注','值班人员']
      let title =   ['LicensePlate','CarType','AdmissionDate','BillingStartTime','BillingDeadline','LongStop','AmountReceivable','AmountReceived_xs','Discount','AmountReceived_xs','FreeAdmission','PaymentType','Remarks','TollOperator']
      let config = {
        header: tHeader,
        autoWidth: true
      }
      const list = formatDay(title, data, 'BillingDeadline', config)
      console.log('export_json_to_excel', list, data)
      let arr = []
      for(let key in list) {
        arr.push(key)
      }
      export_json_to_excel({
        list,
        filename: "时租车缴费记录报表" + arr[0] + '-' + arr[arr.length - 2]
      })
      this.loading = false
    },
    // 支付方式
    PaymentType (val) {
      switch (val) {
        case 0:
          return '未知';
        case 99:
          return '现金支付';
        case 98:
          return '现金支付';
        case 1:
          return '微信支付';
        case 3:
          return '支付宝支付';
      }
      return ''
    },
    // 车类
    async getCarType() {
      const res = await GetCarTypeList({
        ParkingCode: this.form.ParkingCode,
        projectGuid: this.$store.state.ProjectGuid
      });
      this.form.CarType = ''
      this.option.CarType = res.filter(item => item.CarType === 0).map((item, index) => {
        this.CarTypeName[item.Idx] = item.CarTypeName
        return {
          value: item.Idx,
          label: item.CarTypeName,
          index: index,
          type: item.CarType,
          enable: item.Enable
        };
      });
      return Promise.resolve()
    },
    LicensePlate(carNo) {
      let str = carNo.trim()
      let re = /^[\u4e00-\u9fa5]/
      if (re.test(str)) {
        return carNo + '*'
      } else {
        return '?*' + carNo + '*'
      }
    },
    // 值班人员
    async GetUserList() {
      var ProjectGuid = getCookie("ProjectGuid");
      const res = await GetUserList({
        ProjectGuid: ProjectGuid,
        parkingCode: this.form.ParkingCode
      });
      this.form.TollOperator = ''
      this.option.OpenGateOperator = res.UserList.map((item, index) => {
        return {
          value: item.UserName,
          label: item.UserName,
          index: index
        };
      });
      return Promise.resolve()
    }
  },
  async created () {
    await this.getParkInfo()
    await this.getCarType()
    await this.GetUserList()
    this.init(this.form)
  }
}
  </script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
  <style lang="less">
  .temporary-parking {
    background: #eff3f6;
    height: 100vh;
    display: flex;
    flex-direction: column;
   .main {
     &.box {
       margin-top: 0;
       padding: 10px 15px;
     }
     background: #fff;
     border-radius: 5px;
     margin-top: 10px;
   }
   .table {
     padding: 0px 15px;
     width: 100%;
     box-sizing: border-box;
   }
   .title-box {
     border: 0;
   }
   .el-table__empty-block {
     height: auto;
   }
   .query .condition-tit{
    min-width: 60px;
    text-align: right;
   }
  }
  </style>
  