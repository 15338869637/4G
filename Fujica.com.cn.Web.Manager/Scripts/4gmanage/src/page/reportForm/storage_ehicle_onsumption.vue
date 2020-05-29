<template>
  <div class="storage-ehicle-onsumption flex-box column">
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
                <div class="condition-tit">计费截止时间:</div>
                <div class="condition-con">
                    <el-date-picker
                    class="w-input"
                    v-model="dateTime"
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
                    <el-select v-model="form.ConsumeOperator" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                            v-for="item in options.ConsumeOperator"
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
      <Title titleMsg="储值车消费记录">
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
            prop="AdmissionDate"
            sortable
            label="入场时间">
          </el-table-column>
          <el-table-column
            prop="BillingStartTime"
            sortable
            label="计费开始时间">
          </el-table-column>
          <el-table-column
            prop="BillingDeadline"
            sortable
            label="计费截止时间">
          </el-table-column>
          <el-table-column
            prop="LongStop"
            label="停车时长">
          </el-table-column>
          <el-table-column
            prop="DeductionAmount"
            label="扣款金额">
          </el-table-column>
          <el-table-column
            prop="Balance"
            label="余额">
          </el-table-column>
          <el-table-column
            prop="Remark"
            label="备注">
          </el-table-column>
          <el-table-column
            prop="ConsumeOperator"
            label="值班人员">
          </el-table-column>
          <el-table-column
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
  GetUserList,
  GetCarTypeList,
  SearchConsumeRecord,
  SearchParkingRecord,
  GetParkLotList,
  GetRolePermission
} from './api'
export default {
  data () {
    return {
      form: {
        CardType: 1,
        ParkingCode: '',
        BillingStartTime: '',
        BillingDeadline: '',
        LicensePlate: '',
        CarType: '',
        PaymentType: '',
        ConsumeOperator: '',
        OperatType: ''
      },
      Request: {},
      dateTime: [],
      options: {
        ParkingCode: [],
        CarType: [],
        PaymentType: [],
        OperatType: []
      },
      OutThroughType: {
        '1': '自动开闸',
        '2': '手动开闸',
        '3': '免费开闸',
        '4': '收费放行'
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
      loading: false
    }
  },
  components: {
    Title,
    page,
    'v-details': details
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
      SearchConsumeRecord(data).then(res => {
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
      return list.map(obj => {
            let item = Object.assign({}, obj);
            item.CarType = this.CarType[item.CarType]
            item.AdmissionDate = this.date_time(item.AdmissionDate)
            item.AppearanceDate && (item.AppearanceDate = this.date_time(item.AppearanceDate))
            item.BillingStartTime = this.date_time(item.BillingStartTime)
            item.BillingDeadline = this.date_time(item.BillingDeadline)
            // 停车时长
            let H = parseInt(item.LongStop / 60);
            let M = item.LongStop % 60
            item.LongStop = `${H}小时${M}分钟`;
            obj.Balance && (item.Balance = (item.Balance || 0).toFixed(2))
            obj.DeductionAmount && (item.DeductionAmount = (obj.DeductionAmount || 0).toFixed(2))
            obj.InThroughType && (item.InThroughType = this.OutThroughType[obj.InThroughType])
            obj.OutThroughType && (item.OutThroughType = this.OutThroughType[obj.OutThroughType])
            return item
          })
    },
    date_time (time) {
      return tinytime(new Date(time), 'yyyy-MM-dd HH:mm:ss')
    },
    Reset() {
      Object.assign(this.form, {
        BillingStartTime: '',
        BillingDeadline: '',
        LicensePlate: '',
        CarType: '',
        PaymentType: '',
        OperatType: ''
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
            res.PaperCouponList.forEach((element, index) => {
              let obj = this.filterData(res.PaperCouponList)[index];
              for (let key in obj) {
                obj[key] && (data[key] = obj[key])
              }
              console.log('details', data)
              let title = ['扣款金额:', '余额:', '停车时长:']
              let content = ['DeductionAmount', 'Balance', 'LongStop']
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
            this.$message.error('未查到对应详情信息')
          }
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
      await SearchConsumeRecord(Obj).then(res => {
        if (res.IsSuccess && res.PaperCouponList && res.PaperCouponList.length) {
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
        await SearchConsumeRecord(Obj).then(res => {
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
      let tHeader = ['车牌号','车辆类型','入场时间','计费开始时间','计费截止时间','停车时长','扣款金额','余额','备注','值班人员']
      let title =   ['LicensePlate','CarType','AdmissionDate','BillingStartTime','BillingDeadline','LongStop','DeductionAmount','Balance','Remark','ConsumeOperator']
      let config = {
        header: tHeader,
        autoWidth: true
      }
      console.log('data', data)
      const list = formatDay(title, data, 'BillingStartTime', config)
      let arr = []
      for(let key in list) {
        arr.push(key)
      }
      export_json_to_excel({
        list,
        filename: "储值车消费记录报表" + arr[0] + '-' + arr[arr.length - 2]
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
      this.options.CarType = res.filter(item => item.CarType === 2).map((item, index) => {
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
      this.form.ConsumeOperator = ''
      this.options.ConsumeOperator = res.UserList.map((item, index) => {
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
  .storage-ehicle-onsumption {
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
  