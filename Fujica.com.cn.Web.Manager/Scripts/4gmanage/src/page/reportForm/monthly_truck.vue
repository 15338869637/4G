<template>
  <div class="monthly-truck flex-box column">
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
                <div class="condition-tit">收费时间:</div>
                <div class="condition-con">
                    <el-date-picker
                    class="w-input"
                    v-model="dateTime"
                    :max-date="new Date()"
                    :picker-options="picker_options"
                    type="datetimerange"
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
                <div class="condition-tit">车主名:</div>
                <div class="condition-con">
                    <el-input v-model="form.OwnerName" class="w-input" placeholder="请输入内容"></el-input>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">手机号:</div>
                <div class="condition-con">
                    <el-input v-model="form.PhoneNumber" class="w-input" placeholder="请输入内容"></el-input>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">操作类型:</div>
                <div class="condition-con">
                    <el-select v-model="form.OperatType" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                            v-for="(item, i) in options.OperatType"
                            :key="i"
                            :label="item"
                            :value="i">
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
      <Title titleMsg="月卡车缴费记录">
        <el-button slot="right" type="primary" class="rightBtn" :loading="loading" @click="exportExcel">导出Excel报表</el-button>
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
            width="80"
            label="车辆类型">
          </el-table-column>
          <el-table-column
            prop="OwnerName"
            width="100"
            label="车主名">
          </el-table-column>
          <el-table-column
            prop="PhoneNumber"
            sortable
            width="120"
            label="手机号">
          </el-table-column>
          <el-table-column
            prop="OperatType"
            width="80"
            label="操作类型">
          </el-table-column>
          <el-table-column
            prop="DelayStart"
            sortable
            width="180"
            label="延期开始时间">
          </el-table-column>
          <el-table-column
            prop="DelayEnd"
            width="180"
            label="延期结束时间">
          </el-table-column>
          <el-table-column
            prop="PaymentType"
            width="80"
            label="支付方式">
          </el-table-column>
          <el-table-column
            prop="RechargeAmount"
            width="120"
            label="收款/退款金额">
            <template slot-scope="scope">
              <span :class="(scope.row.OperatType == '退卡' ? 'red' : 'green')">{{(scope.row.OperatType == '退卡' ? '-' : '') + scope.row.RechargeAmount}}</span>
            </template>
          </el-table-column>
          <el-table-column
            prop="RechargeOperator"
            width="120"
            label="收费人">
          </el-table-column>
          <el-table-column
            prop="CreateTime"
            sortable
            width="180"
            label="收费时间">
          </el-table-column>
          <el-table-column
            prop="Remarks"
            label="备注">
          </el-table-column>
        </el-table>
        <page :page="page" @PageChange="init"> </page>
      </div>
    </div>
  </div>
</template>
  
<script>
import tinytime from 'format-datetime'
import Title from '@/components/Title'
import page from '@/components/page'
import {getCookie} from '@/config/system.js'
import {
  GetCarTypeList,
  SearchRechargeRecord,
  GetParkLotList,
  GetRolePermission
} from './api'
export default {
  data () {
    return {
      form: {
        CardType: 1,
        ParkingCode: '',
        TransactionStartTime: '',
        TransactionEndTime: '',
        LicensePlate: '',
        CarType: '',
        PaymentType: '',
        OperatType: '',
        OwnerName: '',
        PhoneNumber: ''
      },
      Request: {},
      dateTime: [],
      CarTypeName: {},
      options: {
        ParkingCode: [],
        CarType: [],
        PaymentType: [],
        OperatType: ['开卡','续费','退卡']
      },
      tableData: [],
      page: {
        page:1,
        size: 10,
        total: 0
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
  components: {
    Title,
    page
  },
  watch: {
    dateTime (val) {
      if (val && val[0] && val[1]) {
        [this.form.TransactionStartTime, this.form.TransactionEndTime] = [this.date_time(val[0]), this.date_time(val[1])]
      } else {
        [this.form.TransactionStartTime, this.form.TransactionEndTime] = ['', '']
      }
    }
  },
  methods: {
    async ParkingCodeChange () {
      await this.getCarType();
      this.init(this.form)
    },
    init (Request) {
      if (Request) {
        Object.assign(this.Request, Request)
        this.page.page = 1
      }
      let data = Object.assign({
        FieldSort: 'CreateTime', 
        Sort: 1,
        PageIndex: this.page.page,
        PageSize: this.page.size
      }, this.Request)
      for(let name in data) {
        data[name] === '' && (delete(data[name]))
      }
      data['LicensePlate'] && (data['LicensePlate'] = this.LicensePlate(data['LicensePlate'].toUpperCase()))
      data['OwnerName'] && (data['OwnerName'] = data['OwnerName'] + '*')
      SearchRechargeRecord(data).then(res => {
        if (res.IsSuccess) {
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
            item.CreateTime = this.date_time(item.CreateTime)
            let Delay = this.Delay(item)
            console.log('Delay', Delay)
            item.DelayStart = Delay.start
            item.DelayEnd = Delay.end
            // 支付方式
            item.PaymentType = this.PaymentType(item.PaymentType)
            item.RechargeOperator = decodeURI(item.RechargeOperator)
            item.CarType = this.CarTypeName[item.CarType]
            item.OperatType = this.options.OperatType[item.OperatType]
            item.RechargeAmount = item.RechargeAmount.toFixed(2)
            return item
          })
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
    Reset() {
      Object.assign(this.form, {
        TransactionStartTime: '',
        TransactionEndTime: '',
        LicensePlate: '',
        CarType: '',
        PaymentType: '',
        OperatType: '',
        OwnerName: '',
        PhoneNumber: ''
      })
      this.dateTime = []
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
    date_time (time, type) {
      return tinytime(new Date(time), type || 'yyyy-MM-dd HH:mm:ss')
    },
    details (item) {
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
      await SearchRechargeRecord(Obj).then(res => {
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
        await SearchRechargeRecord(Obj).then(res => {
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
      let tHeader = ['车牌号','计费类型','车主名','手机号','操作类型','延期开始时间','延期结束时间','支付方式','收款/退款金额','收费人','收费时间','备注']
      let title = ['LicensePlate','CarType','OwnerName','PhoneNumber','OperatType','DelayStart','DelayEnd','PaymentType','RechargeAmount','RechargeOperator','CreateTime','Remarks']
      let config = {
        header: tHeader,
        autoWidth: true
      }
      const list = formatDay(title, data, 'CreateTime', config)
      let arr = []
      for(let key in list) {
        arr.push(key)
      }
      export_json_to_excel({
        list,
        filename: "月卡车缴费记录报表" + arr[0] + '-' + arr[arr.length - 2]
      });
      this.loading = false
    },
    Delay (item) {
      // 到期时间 续费类型 续费数量
      let {ExpirationTime, RenewalType, RenewalValue, OperatType} = item
      let start = this.date_time(ExpirationTime, 'yyyy-MM-dd')
      let end = this.date_time(new Date(start).getTime() + (RenewalValue * 24 * 60 * 60 * 1000), 'yyyy-MM-dd')
      if (OperatType == 2) {
        return {
            start: '',
            end: start
          }
      }
      switch (RenewalType) {
        case 1:
          return {
            start,
            end
          }
          break;
      }
      return {}
      console.error(item.LicensePlate + '数据有误！', 'RenewalType:' + RenewalType)
    },
    // 车类
    async getCarType() {
      const res = await GetCarTypeList({
        ParkingCode: this.form.ParkingCode,
        projectGuid: this.$store.state.ProjectGuid
      });
      console.log('getCarType', res)
      this.form.CarType = ''
      this.options.CarType = res.filter(item => item.CarType === 1 || item.CarType === 3).map((item, index) => {
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
    }
  },
  async created () {
    await this.getParkInfo()
    await this.getCarType()
    this.init(this.form)
  }
}
</script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
  <style lang="less">
  .monthly-truck {
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
  