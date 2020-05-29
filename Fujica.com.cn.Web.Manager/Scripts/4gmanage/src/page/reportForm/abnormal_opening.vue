<template>
  <div class="abnormal-opening flex-box column">
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
                <div class="condition-tit">进出类型:</div>
                <div class="condition-con">
                    <el-select v-model="form.EntranceType" class="w-input" placeholder="请选择">
                        <el-option label="全部" value=""></el-option>
                        <el-option
                            v-for="(item, i) in options.EntranceType"
                            :key="i"
                            :label="item"
                            :value="i">
                        </el-option>
                    </el-select>
                </div>
            </div>
            <div class="condition">
                <div class="condition-tit">开闸时间:</div>
                <div class="condition-con">
                    <el-date-picker
                    class="w-input"
                    v-model="dateTime"
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
                <div class="condition-tit">值班人员:</div>
                <div class="condition-con">
                    <el-select v-model="form.OpenGateOperator" class="w-input" placeholder="请选择">
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
      <Title titleMsg="异常开闸记录">
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
            width="50"
            label="序号">
            <template slot-scope="scope">
              {{scope.$index + 1 + (page.page - 1) * page.size}}
            </template>
          </el-table-column>
          <el-table-column
            prop="EntranceType"
            label="进出类型">
          </el-table-column>
          <el-table-column
            prop="ThroughName"
            label="通道名称">
          </el-table-column>
          <el-table-column
            prop="OpenGateTime"
            sortable
            label="开闸时间">
          </el-table-column>
          <el-table-column
            prop="ThroughType"
            label="通行方式">
          </el-table-column>
          <el-table-column
            prop="OpenGateReason"
            label="开闸原因">
          </el-table-column>
          <el-table-column
            prop="OpenGateOperator"
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
    <el-dialog :visible.sync="dialog.show" width="680px" top="20px">
      <span slot="title" class="details-title">
        异常开闸
      </span>
      <div class="flex-box space-between">
        <div class="img flex1">
          <img :src="dialog.data.ImageUrl" width="100%" alt="" />
        </div>
        <div class="box-li flex-box column">
          <p class="flex1" v-for="(item, i1) in dialog.list" :key="i1"><span class="information-title">{{item.title}}</span>{{dialog.data[item.content]}}</p>
        </div>
      </div>
    </el-dialog>
  </div>
</template>
  
<script>
import tinytime from 'format-datetime'
import Title from '@/components/Title'
import page from '@/components/page'
import {getCookie} from '@/config/system.js'
import {
  GetUserList,
  SearchOpenGateRecord,
  GetParkLotList,
  GetRolePermission
} from './api'
export default {
  data () {
    return {
      form: {
        ParkingCode: '',
        EntranceType: '',
        OpenGateStartTime: '',
        OpenGateEndTime: '',
        OpenGateOperator: ''
      },
      Request: {},
      dateTime: [],
      options: {
        ParkingCode: [],
        OpenGateOperator: [],
        EntranceType: ['入场','出场']
      },
      InThroughType: {
        '1': '自动开闸',
        '2': '手动开闸',
        '3': '免费开闸'
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
      loading: false,
      dialog: {
        show: false,
        data: {},
        list: [
          {
            title: '进出类型:',
            content: 'EntranceType'
          },
          {
            title: '通道名称:',
            content: 'ThroughName'
          },
          {
            title: '识别相机:',
            content: 'DiscernCamera'
          },
          {
            title: '开闸时间:',
            content: 'OpenGateTime'
          },
          {
            title: '通行方式:',
            content: 'ThroughType'
          },
          {
            title: '值班人员:',
            content: 'OpenGateOperator'
          }
        ]
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
        [this.form.OpenGateStartTime, this.form.OpenGateEndTime] = [this.date_time(val[0]), this.date_time(val[1])]
      } else {
        [this.form.OpenGateStartTime, this.form.OpenGateEndTime] = ['', '']
      }
    }
  },
  methods: {
    async ParkingCodeChange () {
      await this.GetUserList();
      this.init(this.form)
    },
    init (Request) {
      if (Request) {
        Object.assign(this.Request, Request)
        this.page.page = 1
      }
      let data = Object.assign({
        FieldSort: 'OpenGateTime', 
        Sort: 1,
        PageIndex: this.page.page,
        PageSize: this.page.size
      }, this.Request)
      for(let name in data) {
        data[name] === '' && (delete(data[name]))
      }
      console.log('init =>', data)
      SearchOpenGateRecord(data).then(res => {
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
            item.OpenGateTime = this.date_time(item.OpenGateTime)
            item.EntranceType = this.options.EntranceType[item.EntranceType] 
            item.ThroughType = this.InThroughType[item.ThroughType]
            return item
          })
    },
    Reset() {
      Object.assign(this.form, {
        EntranceType: '',
        OpenGateStartTime: '',
        OpenGateEndTime: '',
        OpenGateOperator: ''
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
      this.dialog.data = item
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
        data[name] === '' && (delete(data[name]))

      }
      await SearchOpenGateRecord(Obj).then(res => {
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
      for (let i = 1;i * PageSize < TotalCount; i++) {
        await SearchOpenGateRecord(Obj).then(res => {
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
      let tHeader = ['进出类型','通道名称','开闸时间','通行方式','开闸原因','值班人员']
      let title = ['EntranceType','ThroughName','OpenGateTime','ThroughType','OpenGateReason','OpenGateOperator']
      let config = {
        header: tHeader,
        autoWidth: true
      }
      const list = formatDay(title, data, 'OpenGateTime', config)
      let arr = []
      for(let key in list) {
        arr.push(key)
      }
      export_json_to_excel({
        list,
        filename: "异常开闸记录报表" + arr[0] + '-' + arr[arr.length - 2]
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
      this.form.OpenGateOperator = ''
      this.options.OpenGateOperator = res.UserList.map((item, index) => {
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
    this.init(this.form)
  }
}
</script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
  <style lang="less">
  .abnormal-opening {
    background: #eff3f6;
    height: 100vh;
    display: flex;
    flex-direction: column;
    .details-title{
        position: relative;
        padding-left: 18px;
        font-size: 18px;
        font-weight: bold;
    }
    .details-title::before {
        content: '';
        display: block;
        height: 36px;
        width: 6px;
        // position: absolute;
        background: linear-gradient(#00C6FF, #0072FF);
        position: absolute;
        top: 50%;
        left: 0;
        margin-top: -18px;
    }
   .main {
     &.box {
       margin-top: 0;
       padding: 10px 15px;
     }
     background: #fff;
     border-radius: 5px;
     margin-top: 10px;
   }
   .box-li{
     margin: 10px;
     p {
       margin: 0;
       span{
         margin-right: 10px;
       }
     }
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
  