<template>
    <div class="flex-body flex-box column">
      <!-- 月卡管理 -->
      <div class="month-card-wrap flex1 flex-box column">
        <!-- 搜索-start -->
        <div class="top-search">
            <el-form :inline="true" :model="formInline" class="demo-form-inline">
              <el-form-item label="停车场" style='width=80%'>
               <el-select v-model="formInline.ParkingCode"  @change='changeSelect' placeholder="停车场">
                    <el-option
                      v-for="item in parkNameOptions"
                      :key="item.value"
                      :label="item.label"
                      :value="item.value">
                    </el-option>
                </el-select>  
              </el-form-item>
              <el-form-item label="车牌号">
                <el-input v-model="formInline.CarNo" placeholder="车牌号"></el-input>
              </el-form-item>
              <el-form-item label="车主姓名">
                <el-input v-model="formInline.CarOwnerName" placeholder="车主姓名"></el-input>
              </el-form-item>
              <el-form-item label="联系电话">
                <el-input v-model="formInline.Mobile" placeholder="联系电话"></el-input>
              </el-form-item> 
              <el-form-item label="开卡时间">
                <el-date-picker type="date" placeholder="选择日期" v-model="ApplyDate" style="width: 100%;"></el-date-picker>
              </el-form-item>
              <el-form-item label="状态">
                <el-select v-model="formInline.StatusType" placeholder="状态">
                  <el-option label="全部" value="0"></el-option>
                  <el-option label="正常" value="1"></el-option>
                  <!-- <el-option label="报停" value="2"></el-option> -->
                  <el-option label="锁定" value="3"></el-option>
                </el-select>
              </el-form-item>
                <el-button type="primary" @click="onSubmit">搜索</el-button>
                <el-button type="info"  @click="clear('formInline')">重置</el-button>
            </el-form>
        </div>    
        <div class="top-bg"></div>
        <Title :titleMsg='title'></Title>
        <!-- 表格展示 -->
        <div class="table flex1 auto">
          <el-table
          :data="mylist"
          border
          style="width: 100%">
          <el-table-column
            prop="ParkCode"
            label="停车场"
             width="100">
              <template slot-scope="scope">
               {{getParkName(scope.row.ParkCode)}}
              </template>
          </el-table-column>
          <el-table-column
            prop="CarNo"
            label="车牌号"
             width="100">
          </el-table-column>
          <el-table-column
            prop="CarTypeGuid"
            label="月卡类型"
             width="100">
             <template slot-scope="scope">
               {{getCardName(scope.row.CarTypeGuid)}}
              </template>
          </el-table-column>
          <el-table-column
            prop="CarOwnerName"
            label="车主姓名"
             width="120">
          </el-table-column>
          <el-table-column
            prop="Mobile"
            label="联系电话"
             width="140">
          </el-table-column>
          <el-table-column
            prop="StartDate"
            label="起始日期"
             width="120">
              <template slot-scope="scope">
             {{timeTransfer(scope.row.StartDate)}}
            </template>
          </el-table-column>
          <el-table-column
            prop="EndDate"
            label="截止日期"
             width="120">
              <template slot-scope="scope">
             {{timeTransfer(scope.row.EndDate)}}
            </template>
          </el-table-column>
          <!-- <el-table-column
            label="授权车道"
             width="120">
             <template slot-scope="scope">
                <el-popover trigger="click" placement="top">
                  <div class="drive-box">
                      <div class="drive-title">
                          授权车道
                      </div>
                      <ul class="drive-cont">
                        <li v-for='(item,i) in DriveWayDetail' :key='i'>
                            {{item}}
                        </li>
                      </ul>
                  </div>
                  <div slot="reference" class="name-wrapper">
                    <el-button type="text" size="small" @click="getDriveWayDetail(scope.row.DrivewayGuidList)">查看详情</el-button>
                  </div>
                </el-popover>
              </template>
          </el-table-column> -->
          <el-table-column
            prop="Address"
            label="状态"
             width="100">
            <template slot-scope="scope">
              {{parkState(scope.row.Enable,scope.row.Locked)}}
            </template>
          </el-table-column>
          <el-table-column
            prop="StartDate"
            label="开卡时间"
             width="150">
             <template slot-scope="scope">
              {{timeTransferChange(scope.row.StartDate)}}
            </template>
          </el-table-column>

          <el-table-column
            prop="address"
            label="操作人"
             width="120">
             <template slot-scope="scope">
              超级管理员
              </template>
          </el-table-column>

          <el-table-column
            label="操作">
            <template slot-scope="scope" >
              <el-button  class="parkButton"  size="small" :disabled='parkState(scope.row.Enable,scope.row.Locked)=="报停"' @click='operatePush(0,JSON.stringify(scope.row),getCardName(scope.row.CarTypeGuid))'  type="text" >延期</el-button>
              <!-- <el-button  class="parkButton" size="small" v-show='parkState(scope.row.Enable,scope.row.Locked)=="正常"' @click='operatePush(1,JSON.stringify(scope.row))'  type="text" >报停</el-button> -->
              <!-- <el-button  class="parkButton"  size="small" v-show='parkState(scope.row.Enable,scope.row.Locked)=="报停"||parkState(scope.row.Enable,scope.row.Locked)=="锁定"' :disabled='parkState(scope.row.Enable,scope.row.Locked)=="锁定"' @click='operatePush(2,JSON.stringify(scope.row))'  type="text" >激活</el-button> -->
              <el-button  class="parkButton"  size="small" v-show='parkState(scope.row.Enable,scope.row.Locked)=="正常"||parkState(scope.row.Enable,scope.row.Locked)=="报停"' :disabled='parkState(scope.row.Enable,scope.row.Locked)=="报停"' @click='operatePush(3,JSON.stringify(scope.row), getCardName(scope.row.CarTypeGuid))' type="text">锁定</el-button>
              <el-button  class="parkButton" size="small" v-show='parkState(scope.row.Enable,scope.row.Locked)=="锁定"' @click='operatePush(4,JSON.stringify(scope.row), getCardName(scope.row.CarTypeGuid))' type="text">解锁</el-button>
               <el-button class="parkButton"  size="small" :disabled='parkState(scope.row.Enable,scope.row.Locked)!="正常"' @click='operatePush(5,JSON.stringify(scope.row), getCardName(scope.row.CarTypeGuid))' type="text" >注销</el-button>
               <el-button class="parkButton" size="small" :disabled='parkState(scope.row.Enable,scope.row.Locked)!="正常"' @click='operatePush(6,JSON.stringify(scope.row),scope.row.CarTypeGuid)' type="text" >修改</el-button>

            </template>
          </el-table-column>
        </el-table>
        </div>
      </div>
      <div class="month-card-wrap-page">
        <Pagination :total="tableData.length" @PageChange="PageChange"></Pagination>
      </div>
    </div>
</template>
      
<script>
import { mapActions } from 'vuex'
import Title from "@/components/Title";
import rxios from '@/servers/rxios.js'
import tinytime from 'tinytime';
import { defaultCoreCipherList } from 'constants';
import Pagination from '@/components/Pagination'
import  { getCookie }  from "@/config/system.js";
// import dateFormat from 'format-datetime';

// alert(template1.render(new Date()))


export default {
  name: 'MonthCard',
  components: {
    Title,
    Pagination,
  },
  data () {
    return {
      title:'月卡列表',
      formInline: {
          ParkingCode: '',
          CarNo: '',
          CarOwnerName:'',
          Mobile: '',
          ApplyDate: '',
          StatusType:'0',
          PageIndex:1,
          PageSize:100000,
      },
      tableData: [],
      ApplyDate:'',
      parkNameOptions:[],
      parkCode:'',
      CarTypeOptions:[],
      DriveWayGidArr:[],
      DriveWayDetail:[],
      mylist:[],
    }
  },
  methods: {
     _sleep(time) {
        return new Promise((resolve, reject) => {
          setTimeout(resolve, time);
        })
      },
    // 获取停车场信息
    async getParkInfo(){
      let roleGuid=getCookie('RoleGuid')
      const resListAll = await rxios("GET", '/ParkLot/GetParkLotList')
      const resListAllfilter=resListAll.filter(item=>item.Existence==true)   //保留existence为true的
      const resList = await rxios("GET", "/Personnel/GetRolePermission",{
        Guid:roleGuid
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

      console.log(arr,123)


      this.parkNameOptions=arr

      this.formInline.ParkingCode=this.$route.query.parkCode?this.$route.query.parkCode:arr[0].value
      this.parkCode=this.$route.query.parkCode?this.$route.query.parkCode:arr[0].value

      console.log(this.parkCode,1234)

      this.getCarType()
      this.onSubmit()
    },
    // 停车场名称显示
    getParkName(code){
      const arr= this.parkNameOptions.filter(item=>item.value==code)
      return arr[0].label
    },
    
    // 停车场切换
    changeSelect(parkCode){
       // 获取当前的停车场
      this.parkCode=parkCode
      this.mylist=[]
      this.getCarType()
      this.onSubmit()
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
    },
    // 时间转换
    timeTransfer(time){
      var d = new Date(time);
      return d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate() 
    },
    // 时间转换
    timeTransferChange(time){
      var d = new Date(time);
      return d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate() +" " + d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds()
    },
    //获取卡类型
    async getCarType(){

      const res = await rxios("GET", '/ParkLot/GetCarTypeList', {ParkingCode: this.parkCode, projectGuid: this.$store.state.ProjectGuid})
      this.CarTypeOptions=res.map((item,index)=>{
        return {
          value:item.Guid,
          label: item.CarTypeName,
          index:index,
          type:item.CarType,
          enable:item.Enable,
        }
      })
    },
    // 卡类显示
    getCardName(guid){
      const arr=this.CarTypeOptions.filter(item=>item.value==guid)
      const arrList=arr?arr:[]
      if(arrList.length>0){
         return arrList[0].label
      }
    },
    // 授权车道信息获取
    async getDriveWayDetail(DriveWay){
      const res = await rxios("GET", '/ParkLot/GetDriveWayList', {ParkingCode: this.parkCode})
      this.DriveWayGidArr=res.map(item=>{
        return {
          value:item.DrivewayName,
          guid: item.Guid,
        }
      })

      const arr=[]
      for(var i=0;i<this.DriveWayGidArr.length;i++){
        for(var j=0;j<DriveWay.length;j++){
          if(this.DriveWayGidArr[i].guid==DriveWay[j]){
            arr.push(this.DriveWayGidArr[i].value)
          }
        }
      }

      console.log(this.DriveWayGidArr,DriveWay,12)

      this.DriveWayDetail=arr
    },
    clear(formName){
      this.formInline.CarNo=''
      this.formInline.CarOwnerName=''
      this.formInline.Mobile=''
      this.formInline.ApplyDate=''
      this.onSubmit()
    },
    handleClick(){
    },
    // 查询提交
    async onSubmit(){

      const getCookie = name => {
          let arr,
            reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
          arr = document.cookie.match(reg);
          const reg3D = /%3D/;
          if (arr) {
            if (reg3D.test(arr[2])) {
              return decodeURIComponent(arr[2]);
            }
            return arr[2];
          } else {
            return null;
          }
        };

        let ProjectGuidValue = getCookie("ProjectGuid");


      this.formInline.ProjectGuid=ProjectGuidValue
      if(this.ApplyDate){
        const template = tinytime('{YYYY}-{Mo}-{DD}');
         this.formInline.ApplyDate=template.render(this.ApplyDate);
      }else{
        this.formInline.ApplyDate=''
      }

      let Base64 = require('js-base64').Base64;
      const send= Base64.encode(JSON.stringify(this.formInline))


      const res = await rxios("GET", '/CardService/GetMonthCardList', {Base64args: send})
      this.tableData=res

      // this.mylist=this.tableData.slice(this.Page, this.Size)
      this.mylist=this.tableData.slice(0, 10)

      console.log(this.mylist,124345)
    },
    operatePush(state,info,carType){
      // const pathName=''
      // switch(state){
      //   case 0:  //延期
      //     pathName='/CardRenew'
      //     break;
      //   case 1:   //报停
      //     pathName='/CardRenew'
      //     break;
      //   case 2:   //激活
      //     pathName='/CardRenew'
      //     break;
      //   case 3:   //锁定
      //     pathName='/CardRenew'
      //     break; 
      //   case 4:   //解锁
      //     pathName='/CardRenew'
      //     break;
      //   case 5:   //注销
      //     pathName='/CardRenew'
      //     break;
      //   // case 6:   //修改
      //   //   pathName='/CardRenew'
      //   //   break;
      //   default:
      // }
      if(state==6){
        this.$router.push({path: '/SetCard',query:{info:info,carType:carType,status:2,parkCode:this.formInline.ParkingCode}});
        return
      }
      this.$router.push({path: '/MonthOperate',query:{state:state,info:info,carType:carType}});
    },
    PageChange ({ page, size }) {
      this.Page = (page - 1) * size
      this.Size = page * size
      this.mylist=this.tableData.slice(this.Page, this.Size)
    },
    // 时间转换
    dateForm(time){
      const template = tinytime('{YYYY}-{Mo}-{DD} {h}:{mm}:{ss}');
      return template.render(time)
    },
  },
  created () {
    this.getParkInfo()
    this.mylist=this.tableData.slice(0, 10)

    
  }
}
</script>

<style lang='less'>
.month-card-wrap{
  padding-bottom:30px;
  .top-search{
    padding-top: 17px;
    padding-left: 15px;
    border-radius:4px;
    border: 1px solid #EDEEF1;
    padding-bottom: 10px;
    .el-form-item{
      margin-bottom: 10px;
    }
    .el-form-item__content{
      width: 150px;
    }
    // border-bottom:7px solid #EFF3F6;
  }
  .top-bg{
      background: #EFF3F6;
      height: 17px;
  }
  .table{
    // width:95%;
    margin-top: 20px;
    margin-right: 20px;
    margin-left: 20px;
    .el-table thead th{
      background: #E8F6FF;
      color: #333C48;
      font-size: 14px;
      text-align: center; 
    }
    td{
       text-align: center; 
       font-size: 14px;
    }
    .parkButton{
      margin-top: 10px;
    }
  }
}
.month-card-wrap-page {
  .table-page {
    padding: 0 20px;
  }
}
.drive-box{
  padding:10px 20px;
  .drive-title{
    font-size: 16px;
    font-weight: bold;
    color: #000;
  }
  .drive-cont{
    margin-top: 10px;
    li{
      height: 30px;
      line-height: 30px;
    }
  }
}
*{
  padding:0;
  margin:0;
}
li {
  list-style: none;
}



</style>

