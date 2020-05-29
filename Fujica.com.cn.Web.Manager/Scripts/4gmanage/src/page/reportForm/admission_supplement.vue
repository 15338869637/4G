<template>
  <div class="admission-supplement flex-box column">
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
                <div class="condition-tit">车辆类型:</div>
                <div class="condition-con">
                    <el-select v-model="form.CarTypeGuid" class="w-input" placeholder="请选择">
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
                <div class="condition-tit">补录时间:</div>
                <div class="condition-con">
                    <el-date-picker
                    class="w-input"
                    v-model="form.dateTime"
                    type="datetimerange"
                    range-separator="至"
                    start-placeholder="开始日期"
                    end-placeholder="结束日期">
                    </el-date-picker>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">值班人员:</div>
                <div class="condition-con">
                    <el-select v-model="form.Operator" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                            v-for="item in options.OpenGateOperator"
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
      <Title titleMsg="入场补录记录">
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
            prop="CarNo"
            label="车牌号">
          </el-table-column>
          <el-table-column
            prop="_CarTypeGuid"
            label="车辆类型">
          </el-table-column>
          <el-table-column
            prop="Entrance"
            label="入场通道">
          </el-table-column>
          <el-table-column
            prop="_InTime"
            sortable
            label="入场时间">
          </el-table-column>
          <el-table-column
            prop="_RecordTime"
            sortable
            label="补录时间">
          </el-table-column>
          <el-table-column
            prop="_Operator"
            label="补录操作员">
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
  GetUserList,
  GetCarTypeList,
  SearchRecordInRecord,
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
        CarTypeGuid: '',
        StrTime: '',
        EndTime: '',
        Operator: ''
      },
      Request: {},
      dateTime: [],
      CarTypeName: {},
      loading: false,
      options: {
        ParkingCode: [],
        OpenGateOperator: [],
        CarType: [],
        EntranceType: ['出场','入场']
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
      }
    }
  },
  components: {
    Title,
    page
  },
  watch: {
    dateTime (val) {
      if (val && val[0] && val[1]) {
        [this.form.StrTime, this.form.EndTime] = [this.date_time(val[0]), this.date_time(val[1])]
      } else {
        [this.form.StrTime, this.form.EndTime] = ['', '']
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
        PageIndex: this.page.page,
        PageSize: this.page.size
      }, this.Request)
      for(let name in data) {
        data[name] === '' && (delete(data[name]))
      }
      data['CarNo'] && (data['CarNo'] = this.LicensePlate(data['CarNo'].toUpperCase()))
      console.log('init =>', data)
      SearchRecordInRecord(data).then(res => {
        console.log('init', res)
        if (res.IsSuccess && res.Result && res.Result.length) {
          this.tableData = this.filterData(res.Result)
          this.page.total = res.TotalCount
        } else {
          this.tableData = []
          this.page.total = 0
        }
      })
    },
    filterData(list) {
      return list.map(obj => {
        let item = Object.assign({}, obj)
        item._CarTypeGuid = this.CarTypeName[obj.CarTypeGuid]
        item._InTime = this.date_time(obj.InTime)
        item._RecordTime = this.date_time(obj.RecordTime)
        item._Operator = decodeURI(item.Operator)
        return item
      })
    },
    Reset() {
      Object.assign(this.form, {
        CarNo: '',
        CarTypeGuid: '',
        StrTime: '',
        EndTime: '',
        Operator: ''
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
    date_time (time) {
      return tinytime(new Date(time), 'yyyy-MM-dd HH:mm:ss')
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
      Obj['CarNo'] && (Obj['CarNo'] = this.LicensePlate(Obj['CarNo'].toUpperCase()))
      await SearchRecordInRecord(Obj).then(res => {
        if (res.IsSuccess) {
          data = data.concat(this.filterData(res.Result))
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
        await SearchRecordInRecord(Obj).then(res => {
          if (res.IsSuccess) {
            data = data.concat(this.filterData(res.Result))
            Obj.NextToken = res.NextToken
          }
        })
      }
      const {
        export_json_to_excel,
        formatDay
      } = require("@/vendor/Export2Excel.js");
      let tHeader = ['车牌号','车辆类型','入场通道','入场时间','补录时间','补录操作员']
      let title = ['CarNo','_CarTypeGuid','Entrance','_InTime','_RecordTime','_Operator']
      let config = {
        header: tHeader,
        autoWidth: true
      }
      const list = formatDay(title, data, '_RecordTime', config)
      export_json_to_excel({
        list,
        filename: "入场补录记录报表"
      });
      this.loading = false
    },
    // 值班人员
    async GetUserList() {
      var ProjectGuid = getCookie("ProjectGuid");
      const res = await GetUserList({
        ProjectGuid: ProjectGuid,
        parkingCode: this.form.ParkingCode
      });
      this.form.Operator = ''
      this.options.OpenGateOperator = res.UserList.map((item, index) => {
        return {
          value: item.UserName,
          label: item.UserName,
          index: index
        };
      });
      return Promise.resolve()
    },
    // 车类
    async getCarType() {
      const res = await GetCarTypeList({
        ParkingCode: this.form.ParkingCode,
        projectGuid: this.$store.state.ProjectGuid
      });
      console.log('getCarType', res)
      this.form.CarType = ''
      this.options.CarType = res.map((item, index) => {
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
    await this.GetUserList()
    await this.getCarType()
    this.init(this.form)
  }
}
</script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
  <style lang="less">
  .admission-supplement {
    background: #eff3f6;
    height: 100vh;
    display: flex;
    flex-direction: column;
   .main {
     &.box {
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
  