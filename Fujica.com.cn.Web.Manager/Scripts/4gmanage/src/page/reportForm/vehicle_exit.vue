<template>
  <div class="vehicle-exit flex-box column">
    <div class="main box">
        <div class="query left">
            <div class="condition">
                <div class="condition-tit">停车场:</div>
                <div class="condition-con">
                    <el-select v-model="form.ParkingCode" @change="ParkingCodeChange" class="w-input" placeholder="请选择">
                      <el-option
                        v-for="item in options.ParkingCode"
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
                            v-for="item in options.CarType"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value">
                        </el-option>
                    </el-select>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">入场时间:</div>
                <div class="condition-con">
                    <el-date-picker
                    class="w-input"
                    v-model="startDateTime"
                    type="datetimerange"
                    range-separator="至"
                    start-placeholder="开始日期"
                    end-placeholder="结束日期"
                    :default-time="['00:00:00', '23:59:59']">
                    </el-date-picker>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">通行方式:</div>
                <div class="condition-con">
                    <el-select v-model="form.OutThroughType" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                            v-for="item in options.OutThroughType"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value">
                        </el-option>
                    </el-select>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">出场时间:</div>
                <div class="condition-con">
                    <el-date-picker
                    class="w-input"
                    v-model="endDateTime"
                    type="datetimerange"
                    range-separator="至"
                    start-placeholder="开始日期"
                    end-placeholder="结束日期"
                    :default-time="['00:00:00', '23:59:59']">
                    </el-date-picker>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">值班人员:</div>
                <div class="condition-con">
                    <el-select v-model="form.OutParkingOperator" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                            v-for="item in options.OutParkingOperator"
                            :key="item.value"
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
      <Title titleMsg="车辆出场记录">
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
            label="车牌号">
          </el-table-column>
          <el-table-column
            prop="_CarType"
            label="车辆类型">
          </el-table-column>
          <el-table-column
            prop="Export"
            label="出场通道">
          </el-table-column>
          <el-table-column
            prop="AdmissionDate"
            sortable
            label="入场时间">
          </el-table-column>
          <el-table-column
            prop="AppearanceDate"
            sortable
            label="出场时间">
          </el-table-column>
          <el-table-column
            prop="LongStop"
            sortable
            label="停车时长">
          </el-table-column>
          <el-table-column
            prop="OutThroughType"
            label="通行方式">
          </el-table-column>
          <el-table-column
            prop="Remark"
            label="备注">
          </el-table-column>
          <el-table-column
            prop="OutParkingOperator"
            label="值班人员">
          </el-table-column>
          <el-table-column
            label="操作">
            <template slot-scope="scope">
              <el-button type="text" @click="details(scope.row)">查看详情</el-button>
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
  GetUserList,
  GetCarTypeList,
  SearchParkingRecord,
  SearchPaymentRecord,
  SearchConsumeRecord,
  GetParkLotList,
  GetRolePermission
} from './api'
export default {
  data () {
    return {
      form: {
        CarType: '',
        ParkingCode: '',
        LicensePlate: '',
        OutThroughType: '',
        OutParkingOperator: '',
        AdmissionStartDate: '',
        AdmissionEndDate: '',
        AppearanceStartDate: '',
        AppearanceEndDate: ''
      },
      Request: {},
      startDateTime: [],
      endDateTime: [],
      options: {
        ParkingCode: [],
        CarType: [],
        PaymentType: [],
        OperatType: [],
        OutThroughType: [
          {
            label: '自动开闸',
            value: 1
          },
          {
            label: '手动开闸',
            value: 2
          },
          {
            label: '免费开闸',
            value: 3
          },
          {
            label: '收费放行',
            value: 4
          },
          {
            label: '场内删除',
            value: 5
          }
        ],
      },
      OutThroughType: {
        '1': '自动开闸',
        '2': '手动开闸',
        '3': '免费开闸',
        '4': '收费放行',
        '5': '场内删除'
      },
      CarType: {},
      tableData: [],
      page: {
        page: 1,
        size: 10,
        total: 0
      },
      picker_options: {
        disabledDate: item => {
          let time = tinytime(new Date(), 'yyyy-MM-dd 23:59:59')
          return new Date(item).getTime() > new Date(time).getTime()
        }
      },
      dialog: {
        title: '出场详情',
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
            content: '_CarType'
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
            content: '_CarType'
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
      loading: false
    }
  },
  components: {
    Title,
    page,
    'v-details': details
  },
  watch: {
    startDateTime(val) {
      if (val && val[0] && val[1]) {
        [this.form.AdmissionStartDate, this.form.AdmissionEndDate] = [this.date_time(val[0]), this.date_time(val[1])]
      } else {
        [this.form.AdmissionStartDate, this.form.AdmissionEndDate] = ['', '']
      }
    },
    endDateTime(val) {
      if (val[0] && val[1]) {
        [this.form.AppearanceStartDate, this.form.AppearanceEndDate] = [this.date_time(val[0]), this.date_time(val[1])]
      } else {
        [this.form.AppearanceStartDate, this.form.AppearanceEndDate] = ['', '']
      }
    }
  },
  methods: {
    init (Request) {
      if (Request) {
        Object.assign(this.Request, Request)
        this.page.page = 1
      }
      let data = Object.assign({
        FieldSort: 'AppearanceDate', 
        Sort: 1,
        PageIndex: this.page.page,
        PageSize: this.page.size
      }, this.Request)
      for(let name in data) {
        data[name] === '' && (delete(data[name]))
      }
      data['LicensePlate'] && (data['LicensePlate'] = this.LicensePlate(data['LicensePlate'].toUpperCase()))
      SearchParkingRecord(data).then(res => {
        console.log('init', res)
        if (res.IsSuccess && res.PaperCouponList && res.PaperCouponList.length) {
          this.tableData = this.filterData(res.PaperCouponList)
          this.page.total = res.TotalCount
        } else {
          this.tableData = []
          this.page.total = 0
        }
      })
    },
    filterData(list) {
      return list.map(item => {
        let obj = Object.assign({}, item)
        item.CarType && (obj._CarType = this.CarType[item.CarType] || '')
        item.AdmissionDate && (obj.AdmissionDate = this.date_time(item.AdmissionDate))
        item.AppearanceDate && (obj.AppearanceDate = this.date_time(item.AppearanceDate))
        item.OutThroughType && (obj.OutThroughType = this.OutThroughType[item.OutThroughType] || '')
        item.InThroughType && (obj.InThroughType = this.OutThroughType[item.InThroughType] || '')
        item.PaymentTime && (obj.PaymentTime = this.date_time(item.PaymentTime))
        // 支付方式
        typeof item.PaymentType !== 'undefined' && (obj.PaymentType = this.PaymentType(item.PaymentType))
        
        typeof item.AmountReceivable !== 'undefined' && (obj.AmountReceivable = (item.AmountReceivable || 0).toFixed(2))
        typeof item.AmountReceived !== 'undefined' && (obj.AmountReceived = (item.AmountReceived || 0).toFixed(2))
        typeof item.Discount !== 'undefined' && (obj.Discount = (item.Discount || 0).toFixed(2))
        typeof item.AmountReceived !== 'undefined' && (obj.AmountReceived = (item.AmountReceived || 0).toFixed(2))
        typeof item.FreeAdmission !== 'undefined' && (obj.FreeAdmission = (item.FreeAdmission || 0).toFixed(2))
        obj.LongStop = this.LongStop(obj.AdmissionDate, obj.AppearanceDate)
        return obj
      })
    },
    async ParkingCodeChange () {
      await this.getCarType();
      await this.GetUserList();
      this.init(this.form)
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
    date_time (time) {
      return tinytime(new Date(time), 'yyyy-MM-dd HH:mm:ss')
    },
    Reset() {
      Object.assign(this.form, {
        LicensePlate: '',
        OutThroughType: '',
        OutParkingOperator: '',
        AdmissionStartDate: '',
        AdmissionEndDate: '',
        AppearanceStartDate: '',
        AppearanceEndDate: ''
      })
      this.startDateTime = []
      this.endDateTime = []
    },
    // 获取停车场信息
    async getParkInfo(){
      var RoleGuid = getCookie("RoleGuid");
      const resListAll = await GetParkLotList()
      const resListAllfilter=resListAll.filter(item=>item.Existence==true)   //保留existence为true的
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

      this.options.ParkingCode=arr
      this.form.ParkingCode = this.$route.query.parkCode ? this.$route.query.parkCode : arr[0].value
      return Promise.resolve({})
    },
    async details (d) {
      console.log('details', d)
      let res = {}
      let title = []
      let content = []
      this.dialog.sf = []
      if (d.CardType === 3) {
        res = await SearchPaymentRecord({
          ParkingRecordCode: d.Id, 
          ParkingCode: this.form.ParkingCode,
          PageIndex: 1,
          PageSize: 10
          })
        content = ['AmountReceivable', 'AmountReceived', 'Discount', 'AmountReceived', 'FreeAdmission', 'LongStop', 'PaymentType', 'PaymentTime']
        title = ['停车应缴金额:', '线上支付金额:', '优惠金额:', '值班人员实收:', '人工免费:', '停车时长:', '支付方式:', '收费时间:']
      } else if (d.CardType === 2) {
        res = await SearchConsumeRecord({
          ParkingRecordCode: d.Id, 
          ParkingCode: this.form.ParkingCode,
          PageIndex: 1,
          PageSize: 10
          })
        content = ['DeductionAmount', 'Balance', 'LongStop']
        title = ['扣款金额:', '余额:', '停车时长:']
      }
        let data = Object.assign({}, d)
        if (res.IsSuccess) {
          if (res.PaperCouponList.length) {
            res.PaperCouponList.forEach((element, index) => {
              let obj = this.filterData(res.PaperCouponList)[index];
              for (let key in obj) {
                obj[key] && (data[key] = obj[key])
              }
              console.log('details', data)
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
              this.dialog.sf[index] = list
            });
          } else {
            this.dialog.sf = []
            // this.$message.error('未查到对应详情信息')
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
      await SearchParkingRecord(Obj).then(res => {
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
      for (let i = 1;i * 100 < TotalCount; i++) {
        await SearchParkingRecord(Obj).then(res => {
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
      let tHeader = ['车牌号','车辆类型','出场通道','入场时间','出场时间','停车时长','通行方式','备注','值班人员']
      let title =   ['LicensePlate','_CarType','Export','AdmissionDate','AppearanceDate','LongStop','OutThroughType','Remark','ParkingOperator']
      let config = {
        header: tHeader,
        autoWidth: true
      }
      const list = formatDay(title, data, 'AppearanceDate', config)
      console.log('export_json_to_excel', list)
      let arr = []
      for(let key in list) {
        arr.push(key)
      }
      export_json_to_excel({
        list,
        filename: "车辆出场记录报表" + arr[0] + '-' + arr[arr.length - 2]
      });
      this.loading = false
    },
    // 车类
    async getCarType() {
      const res = await GetCarTypeList({
        ParkingCode: this.form.ParkingCode,
        projectGuid: this.$store.state.ProjectGuid
      });
      this.form.CarType = ''
      this.options.CarType = res.map((item, index) => {
        this.CarType[item.Idx] = item.CarTypeName
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
    // 值班人员
    async GetUserList() {
      var ProjectGuid = getCookie("ProjectGuid");
      const res = await GetUserList({
        ProjectGuid: ProjectGuid,
        parkingCode: this.form.ParkingCode
      });
      this.form.OutParkingOperator = ''
      this.options.OutParkingOperator = res.UserList.map((item, index) => {
        return {
          value: item.UserName,
          label: item.UserName,
          index: index
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
    LongStop(s, e) {
      if (!s || !e) return ''
      let sta = new Date(s).getTime()
      let end = new Date(e).getTime()
      let stopTime = end - sta
      let H = parseInt(stopTime / (1000 * 60 * 60))
      let M = parseInt(stopTime % (1000 * 60 * 60) / (1000 * 60))
      let S = parseInt(stopTime % (1000 * 60) / (1000))
      return `${H}小时${M}分`
    }
  },
  async created () {
    await this.getParkInfo()
    this.GetUserList()
    await this.getCarType()
    this.init(this.form)
  }
}
</script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
  <style lang="less">
  .vehicle-exit {
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
  