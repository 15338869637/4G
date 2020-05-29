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
                    <el-input v-model="form.CarNo" class="w-input" placeholder="请输入内容"></el-input>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">车主姓名:</div>
                <div class="condition-con">
                    <el-input v-model="form.CarOwnerName" class="w-input" placeholder="请输入内容"></el-input>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">联系电话:</div>
                <div class="condition-con">
                    <el-input v-model="form.Mobile" class="w-input" placeholder="请输入内容"></el-input>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">开卡时间:</div>
                <div class="condition-con">
                    <el-date-picker
                    class="w-input"
                    v-model="dateTime"
                    :max-date="new Date()"
                    :picker-options="picker_options"
                    type="date">
                    </el-date-picker>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">状态:</div>
                <div class="condition-con">
                    <el-select v-model="form.StatusType" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option label="正常" :value="1"></el-option>
                        <!-- <el-option label="报停" :value="2"></el-option> -->
                        <el-option label="锁定" :value="3"></el-option>
                    </el-select>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">车类:</div>
                <div class="condition-con">
                    <el-select v-model="form.CarTypeGuid" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                          v-for="item in options.CarTypeGuid"
                          :key="item.value"
                          :label="item.label"
                          :value="item.value">
                        </el-option>
                    </el-select>
                </div>
            </div>
        <div>
        <el-button type="primary" @click="init(form)" :loading="loading">搜索</el-button>
        <el-button type="primary" @click="Reset">重置</el-button>
        </div>
        </div>
    </div>
    <div class="main flex1 flex-box column">
      <Title titleMsg="临时车列表">
        <!-- <el-button slot="right" type="primary" class="rightBtn" :loading="loading" @click="exportExcel">导出Excel报表</el-button> -->
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
            prop="CarNo"
            width="100"
            label="车牌号">
          </el-table-column>
          <el-table-column
            prop="_CarTypeGuid"
            width="80"
            label="车辆类型">
          </el-table-column>
          <el-table-column
            prop="CarOwnerName"
            width="100"
            label="车主名">
          </el-table-column>
          <el-table-column
            prop="Mobile"
            width="120"
            label="联系电话">
          </el-table-column>
          <el-table-column
            prop="_StartDate"
            width="120"
            label="起始日期">
          </el-table-column>
          <el-table-column
            prop="_EndDate"
            width="120"
            label="截止日期">
          </el-table-column>
          <!-- <el-table-column
            prop="_DrivewayGuidList"
            width="80"
            show-overflow-tooltip
            label="授权车道">
          </el-table-column> -->
          <el-table-column
            prop="_StatusType"
            width="120"
            label="状态">
          </el-table-column>
          <el-table-column
            prop="_CreatedTime"
            width="180"
            label="开卡时间">
          </el-table-column>
          <el-table-column
            prop="RechargeOperator"
            width="120"
            label="操作人">
          </el-table-column>
          <el-table-column
            prop="Remark"
            label="备注">
          </el-table-column>
          <el-table-column
            prop="Remarks"
            label="操作">
            <template slot-scope="scope">
                <el-button type="text" @click="Operate('yq', scope.row)">延期</el-button>
                <el-button type="text" @click="Operate(scope.row.Locked ? 'js' : 'sd', scope.row)">{{scope.row.Locked ? '解锁' : '锁定'}}</el-button>
                <el-button type="text" @click="Operate('zx', scope.row)">注销</el-button>
                <el-button type="text" @click="Operate('xg', scope.row)">修改</el-button>
            </template>
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
  GetDriveWayList,
  GetTempCardList,
  GetParkLotList,
  GetRolePermission
} from './api'
export default {
  data () {
    return {
      form: {
        ProjectGuid: getCookie('ProjectGuid'),
        ParkingCode: '',
        CarNo: '',
        CarOwnerName: '',
        CarTypeGuid: '',
        Mobile: '',
        ApplyDate: '',
        StatusType: ''
      },
      Request: {},
      dateTime: '',
      CarTypeName: {},
      GetDriveWay: {},
      ParkName: {},
      options: {
        ParkingCode: [],
        CarTypeGuid: []
      },
      tableData: [],
      PaymentType: ['全部', '正常', '报停', '锁定'],
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
      if (val) {
        this.form.ApplyDate = this.date_time(val, 'yyyy-MM-dd')
      } else {
        this.form.ApplyDate = ''
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
      this.loading = true
      let data = Object.assign({
        PageIndex: this.page.page,
        PageSize: this.page.size
      }, this.Request)
      for(let name in data) {
        data[name] === '' && (delete(data[name]))
      }
      console.log('init =>', data)
      GetTempCardList(data).then(res => {
        console.log('init', res)
        if (res.IsSuccess && res.Result) {
          this.tableData = this.filterData(res.Result)
          this.page.total = res.TotalCount
        } else {
          this.tableData = []
          this.page.total = 0
        }
        this.loading = false
      })
    },
    filterData(list) {
      return list.map(item => {
            item._DrivewayGuidList = this.GetDriveType(item.DrivewayGuidList)
            item._CarTypeGuid = this.CarTypeName[item.CarTypeGuid]
            item._StatusType = this.parkState(item.Enable, item.Locked)
            item._EndDate = this.date_time(item.EndDate, 'yyyy-MM-dd')
            new Date(item._EndDate + ' 23:59:59').getTime() < new Date().getTime() && (item._StatusType = '过期')
            item._ParkName = this.ParkName[item.ParkCode]
            item._AdmissionDate = this.date_time(item.AdmissionDate, 'yyyy-MM-dd')
            item._BillingStartTime = this.date_time(item.BillingStartTime)
            item._ContinueDate = this.date_time(item.ContinueDate, 'yyyy-MM-dd')
            item._PauseDate = this.date_time(item.PauseDate, 'yyyy-MM-dd')
            item._StartDate = this.date_time(item.StartDate, 'yyyy-MM-dd')
            item._CreatedTime = this.date_time(item.StartDate)
            return item
          })
    },
    Reset() {
      Object.assign(this.form, {
        // ParkingCode: '',
        CarNo: '',
        CarOwnerName: '',
        Mobile: '',
        ApplyDate: '',
        StatusType: ''
      })
      this.dateTime = ''
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
            this.ParkName[resListAllfilter[i].ParkCode] = resListAllfilter[i].ParkName
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
    GetDriveWayList () {
        let {ParkingCode} = this.form
        return GetDriveWayList({ParkingCode}).then(res => {
            console.log('GetDriveWay', res)
            res.forEach(element => {
                this.GetDriveWay[element.Guid] = element.DrivewayName
            });
            this.GetDriveWay['length'] = res.length
        })
    },
    GetDriveType (list) {
        if (list.length === this.GetDriveWay.length) return '全部';
        let str = list.map(item => {
            return this.GetDriveWay[item]
        })
        return str
    },
    Operate(type, data) {
        let options = this.options;
        this.$router.push({name: 'TemporaryCarOperate', params:{type, data, options}});
    },
    // 车类
    async getCarType() {
      const res = await GetCarTypeList({
        ParkingCode: this.form.ParkingCode,
        projectGuid: this.$store.state.ProjectGuid
      });
      this.form.CarTypeGuid = ''
      this.options.CarTypeGuid = res.filter(item => item.CarType === 0).map((item, index) => {
        this.CarTypeName[item.Guid] = item.CarTypeName
        return {
          value: item.Guid,
          label: item.CarTypeName,
          index: index,
          type: item.CarType,
          enable: item.Enable
        };
      });
      return Promise.resolve()
    },
    // 报停锁定状态判断------需要提取到外面
    parkState(enableValue,lockedValue){
      //enable 为false-是报停 为true 判断locked true 为锁定 其他为正常
      if(enableValue){
        if(lockedValue){
          return '锁定'
        }
        else{
          return '正常'
        }
      }
      else{
        return '报停'
      }
    }
  },
  async created () {
    await this.getParkInfo()
    await this.GetDriveWayList()
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
  