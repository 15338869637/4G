<template>
  <div class="on-site-vehicles flex-box column">
    <div class="main box">
        <div class="query">
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
        <div>
        <el-button type="primary" @click="init(form)">搜索</el-button>
        <el-button type="primary" @click="Reset">重置</el-button>
        </div>
        </div>
    </div>
    <div class="main flex1 flex-box column">
      <Title titleMsg="车辆在场记录">
        <el-button slot="right" type="primary" class="rightBtn" @click="blInfo">入场信息补录</el-button>
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
            align="center"
            label="序号"
            width="50">
            <template slot-scope="scope">
              {{scope.$index + 1 + (page.page - 1) * page.size}}
            </template>
          </el-table-column>
          <el-table-column
            align="center"
            prop="ParkName"
            label="停车场">
          </el-table-column>
          <el-table-column
            align="center"
            prop="LicensePlate"
            label="车牌号">
          </el-table-column>
          <el-table-column
            align="center"
            prop="_CarType"
            label="车辆类型">
          </el-table-column>
          <el-table-column
            align="center"
            prop="Entrance"
            label="入场通道">
          </el-table-column>
          <el-table-column
            align="center"
            prop="AdmissionDate"
            sortable
            label="入场时间">
          </el-table-column>
          <el-table-column
            align="center"
            prop="LongStop"
            sortable
            label="停车时长">
          </el-table-column>
          <el-table-column
            align="center"
            label="入场图片">
            <template slot-scope="scope">
              <img :src="scope.row.InImgUrl" v-img style="cursor: pointer;" width="30px" height="25px" />
            </template>
          </el-table-column>
          <el-table-column
            align="center"
            prop="Remark"
            label="备注">
          </el-table-column>
          <el-table-column
            align="center"
            label="操作">
            <template slot-scope="scope">
              <el-button type="text" @click="edit(scope.row)">车牌修正</el-button>
              <el-button type="text" @click="dele(scope.row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <page :page="page" @PageChange="init"> </page>
      </div>
    </div>
    <el-dialog title="入场补录" :visible.sync="dialog" v-if="dialog" width="500px">
      <el-form :model="bl" :rules="rules" ref="form">
        <el-form-item label="车牌号" label-width="90px" prop="CarNo">
          <el-input v-model="bl.CarNo" autocomplete="off" class="w100"></el-input>
        </el-form-item>
        <el-form-item label="入场时间" label-width="90px" prop="InTime">
          <el-date-picker
            v-model="bl.InTime"
            type="datetime"
            class="w100"
            value-format="yyyy-MM-dd HH:mm"
            :picker-options="picker_options"
            format="yyyy-MM-dd HH:mm"
            placeholder="选择日期时间">
          </el-date-picker>
        </el-form-item>
        <el-form-item label="车类型" label-width="90px" prop="CarTypeGuid">
          <el-select v-model="bl.CarTypeGuid" class="w100" placeholder="请选择">
              <el-option
                  v-for="item in options.CarType"
                  :key="item.value"
                  :label="item.label"
                  :value="CarType[item.value].Guid">
              </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="入场通道" label-width="90px" prop="DrivewayGuid">
          <el-select class="w100" v-model="Guid" placeholder="请选择" @change="Driveway">
              <el-option
                  v-for="item in options.DrivewayGuid"
                  :key="item.Guid"
                  :label="item.DrivewayName"
                  :value="item.Guid + '||' + item.DrivewayName">
              </el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialog = false">取 消</el-button>
        <el-button type="primary" @click="submit">确 定</el-button>
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
  SearchPresentRecord,
  GetRolePermission,
  GetCarTypeList,
  CorrectCarNo,
  GetDriveWayList,
  AddInRecord,
  GetParkLotList,
  InVehicleDelete
  } from './api'
export default {
  data () {
    return {
      form: {
        ParkingCode: '',
        LicensePlate: '',
        OutThroughType: '',
        OutParkingOperator: '',
        AdmissionStartDate: '',
        AdmissionEndDate: '',
        AppearanceStartDate: '',
        CarType: '',
        AppearanceEndDate: ''
      },
      Request: {},
      bl: {
        ProjectGuid: this.$store.state.ProjectGuid,
        ParkingCode: '',
        CarNo: '',
        InTime: '',
        CarTypeGuid: '',
        DrivewayGuid: '',
        Entrance: '',
        Operator: this.$store.state.UserName,
        ImgUrl: ''
      },
      rules: {
        CarNo: [{required: true, message: '请输入车牌', trigger: 'blur'}],
        InTime: [{required: true, message: '请选择时间', trigger: 'change'}],
        CarTypeGuid: [{required: true, message: '请选择车类型', trigger: 'change'}],
        DrivewayGuid: [{required: true, message: '请选择车道', trigger: 'change'}]
      },
      startDateTime: [],
      endDateTime: [],
      options: {
        ParkingCode: [],
        CarType: [],
        PaymentType: [],
        OperatType: [],
        DrivewayGuid: null
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
      Guid: null,
      picker_options: {
        disabledDate: item => {
          let time = tinytime(new Date(), 'yyyy-MM-dd 23:59:59')
          return new Date(item).getTime() > new Date(time).getTime()
        }
      },
      dialog: false,
      loading: false
    }
  },
  components: {
    Title,
    page
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
        FieldSort: 'AdmissionDate', 
        Sort: 1,
        PageIndex: this.page.page,
        PageSize: this.page.size
      }, this.Request)
      for(let name in data) {
        data[name] === '' && (delete(data[name]))
      }
      data['LicensePlate'] && (data['LicensePlate'] = this.LicensePlate(data['LicensePlate'].toUpperCase()))
      SearchPresentRecord(data).then(res => {
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
      let _this = this
      return list.map(item => {
        let obj = Object.assign({}, item)
        console.log('filterData', item.CarType)
        obj._CarType = (_this.CarType[item.CarType] || {}).CarTypeName
        obj.AdmissionDate = _this.date_time(item.AdmissionDate)
        obj.LongStop = _this.LongStop(item.AdmissionDate, new Date())
        return obj
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
    date_time (time) {
      return tinytime(new Date(time), 'yyyy-MM-dd HH:mm:ss')
    },
    // 删除
    dele (row) {
      this.$confirm('是否删除车辆?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          this.deleSet(row)
        })
    },
    deleSet (row) {
      console.log('dele', row, this.$store.state.UserName)
      let data = {
        RecordGuid: row.Id,
        CarNo: row.LicensePlate,
        ParkingCode: row.ParkingCode,
        ImgUrl: row.InImgUrl,
        Operator: decodeURI(this.$store.state.UserName)
      }
      InVehicleDelete(data).then(res => {
        console.log('dele', res)
        if (res.IsSuccess) {
          this.$message.success('删除成功')
        } else {
          this.$message.error(res.MessageContent)
        }
      })
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
    // 车类
    async getCarType() {
      const res = await GetCarTypeList({
        ParkingCode: this.form.ParkingCode,
        projectGuid: this.$store.state.ProjectGuid
      });
      this.form.CarType = ''
      this.options.CarType = res.map((item, index) => {
        this.CarType[item.Idx] = item
        return Object.assign({
          value: item.Idx,
          label: item.CarTypeName,
          index: index,
          type: item.CarType,
          enable: item.Enable
        }, item);
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
    },
    edit (row) {
      this.$prompt('当前车牌号: ' + row.LicensePlate, '车牌修正', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        inputPlaceholder: '请输入新的车牌号',
        inputPattern: /^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$/,
        inputErrorMessage: '车牌格式不正确'
      }).then(({ value }) => {
        let data = {
          ProjectGuid: this.$store.state.ProjectGuid,
          ParkingCode: this.form.ParkingCode,
          OldCarNo: row.LicensePlate,
          NewCarNo: value,
          Operator: this.$store.state.UserName
        }
        CorrectCarNo(data).then(res => {
          if (res.IsSuccess) {
            this.$message({
              type: 'success',
              message: '修改成功'
            });
            row.Remark = `管理后台车牌修正：原车牌粤${row.LicensePlate}改为${value}`
            row.LicensePlate = value;
          }
        })
      }).catch(() => {
      });
    },
    Driveway(val) {
      console.log('Driveway', val)
      let arr = val.split('||')
      this.bl.DrivewayGuid = arr[0]
      this.bl.Entrance = arr[1]
    },
    blInfo () {
      Object.assign(this.bl, {
        ProjectGuid: this.$store.state.ProjectGuid,
        ParkingCode: this.form.ParkingCode,
        CarNo: '',
        InTime: '',
        CarTypeGuid: '',
        DrivewayGuid: '',
        Entrance: '',
        Operator: this.$store.state.UserName,
        ImgUrl: '/picture/shownocapture.png'
      })
      GetDriveWayList({ParkingCode: this.bl.ParkingCode}).then(res => {
        if (res) {
          this.options.DrivewayGuid = (res || []).filter(item => item.Type == 0)
        }
      })
      this.dialog = true
    },
    submit () {
      this.$refs.form.validate(valid => {
        if (valid) {
          AddInRecord(this.bl).then(res => {
            console.log('AddInRecord', res)
            if (res.IsSuccess) {
              this.$message.success('补录成功')
              this.dialog = false
            }
          })
        }
      })
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
  .on-site-vehicles {
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
   div.w100{
     width: 90%;
   }
  }
  </style>
  