<template>
  <!-- 延期 -->
  <div class="card-operate-wrap">
    <!-- 头部显示 -->
    <Title :titleMsg='title + onSubmitArr[cardState]'></Title>

    <div class="card-box">
      <!-- 顶部信息显示 -->
      <div class="card-info" v-if="cardState!='xg'">
          <div class="info-row">

            <div class="info-li">
              <span>车牌号码：</span>
              {{cardInfo.CarNo}}
            </div>
            <div class="info-li">
              <span>状态：</span>{{parkState(cardInfo.Enable,cardInfo.Locked)}}
            </div>
            <div class="info-li">
              <span>临时卡类型：</span>{{cardInfo._CarTypeGuid}}
            </div>

          </div>

          <div class="info-row">
            <div class="info-li">
              <span>车主姓名：</span>{{cardInfo.CarOwnerName}}
            </div>
            <div class="info-li">
              <span>起始时间：</span>{{timeTransfer(cardInfo.StartDate)}}
            </div>
            <div class="info-li">
              <span>截止时间：</span>{{timeTransfer(cardInfo.EndDate)}}
            </div>
          </div>  

            <!-- 如果是激活 cardState 2 -->
          <div class="info-row" v-if='cardState==2'> 
              <div class="info-li">
                <span>报停开始日期：</span>{{timeTransfer(cardInfo.PauseDate)}}
              </div>
              <div class="info-li">
                <span>报停结束日期：</span>{{timeTransfer(cardInfo.ContinueDate)}}
              </div>
              <div class="info-li">
                <el-radio v-model="activeValue" label="1">报停期满自动激活</el-radio>
              </div>
          </div>

          <!-- 如果是注销 cardState 5 -->
          <div class="info-row" v-if='cardState=="zx"'>
              <div class="info-li">
                <span class="left-label">备注：</span> <el-input v-model="remark" placeholder="请输入内容"></el-input>
              </div>
              <div class="info-li">
              </div>
          </div>

      </div>
      <!-- 临时卡修改 -->
      <div v-else class="edit flex-box center">
        <el-form :model="cardInfo" :rules="rules" ref="ruleForm" label-width="100px" class="demo-ruleForm">
          <el-row>
            <el-col :span="12">
              <!-- <span class="left-label">停车场：</span>
              <el-input class="flex1" v-model="cardInfo._ParkName" disabled placeholder="请输入内容"></el-input> -->
              <el-form-item label="停车场：" prop="ParkingCode" >
                <el-input class="w100" v-model="cardInfo._ParkName" disabled placeholder="请输入内容"></el-input>
              </el-form-item>
            </el-col>
          <el-col :span="12">
            <!-- <span class="left-label">车牌号：</span>
            <el-input class="flex1" v-model="cardInfo.CarNo" disabled placeholder="请输入内容"></el-input> -->
            <el-form-item label="车牌号：" prop="CarNo" >
              <el-input class="w100" v-model="cardInfo.CarNo" disabled placeholder="请输入内容"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- <span class="left-label">车主姓名：</span>
            <el-input class="flex1" v-model="cardInfo.CarOwnerName" placeholder="请输入内容"></el-input> -->
            <el-form-item label="车主姓名：" prop="CarOwnerName" >
              <el-input class="w100" v-model="cardInfo.CarOwnerName" placeholder="请输入内容"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- <span class="left-label">联系方式：</span> -->
            <el-form-item label="联系方式：" prop="Mobile" >
              <el-input class="w100" v-model="cardInfo.Mobile" placeholder="请输入内容"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- <span class="left-label">车辆类型：</span> -->
            <!-- <el-input class="flex1" v-model="cardInfo.Mobile" placeholder="请输入内容"></el-input> -->
            <el-form-item label="车辆类型：" prop="CarTypeGuid" >
              <el-select v-model="cardInfo.CarTypeGuid" class="w100" placeholder="请选择">
                  <el-option label="全部" value=""></el-option>
                  <el-option
                      v-for="item in options.CarType"
                      :key="item.value"
                      :label="item.label"
                      :value="item.value">
                  </el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- <span class="left-label">状态：</span>
            <el-input class="flex1" v-model="cardInfo._StatusType" disabled placeholder="请输入内容"></el-input> -->
            <el-form-item label="状态：" prop="_StatusType" >
              {{cardInfo._StatusType}}
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- <span class="left-label">起始日期：</span>
            <el-input class="flex1" v-model="cardInfo._StartDate" disabled placeholder="请输入内容"></el-input> -->
            <el-form-item label="起始日期：" prop="_StartDate" >
              <el-input class="w100" v-model="cardInfo._StartDate" disabled placeholder="请输入内容"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <!-- <span class="left-label">截止日期：</span>
            <el-input class="flex1" v-model="cardInfo._EndDate" disabled placeholder="请输入内容"></el-input> -->
            <el-form-item label="截止日期：" prop="_EndDate" >
              <el-input class="w100" v-model="cardInfo._EndDate" disabled placeholder="请输入内容"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>

      <!-- 月卡延期 -->
      <div class="delay-box" v-if='cardState== "yq"'>
          <div class="delay-li">
            <el-radio v-model="renewValue" label="1" @change='timeChange'>按时间段延期</el-radio>
            <div class="delay-row">
              <div class="delay-input">
                <span class="left-label" :class="{'input-tip':renewValue==1}">起始时间：</span> 
                <div class="right-box">
                  <el-date-picker type="date" placeholder="选择日期" disabled  v-model="startDate" value-format="yyyy-MM-dd"  style="width: 100%;"></el-date-picker>
                </div>
              </div>
              <div class="delay-input">
                <span class="left-label" :class="{'input-tip':renewValue==1}">截止日期：</span> 
                <div class="right-box">
                  <el-date-picker type="date" placeholder="选择日期" :disabled='renewValue==2'  v-model="endDate" value-format="yyyy-MM-dd" :picker-options="pickerOptions" style="width: 100%;"></el-date-picker>
                </div>
              </div>
            </div>
          </div>

          <div class="delay-li">
            <el-radio v-model="renewValue" label="2"  @change='timeChange'>按整月延期</el-radio>
            <div class="delay-row">
              <div class="delay-input">
                <span class="left-label" :class="{'input-tip':renewValue==2}">延期时间：</span> <div class="right-box"><el-input class="month" v-model="month" placeholder="请输入内容"  :disabled='renewValue==1' ></el-input> 月</div>
              </div>
            </div>
          </div>
      </div>

      <!-- 确认按钮 -->
      <div class="btn">
        <el-button type="info" @click="cancel()">取消</el-button>
        <el-button type="primary" @click="onSubmit">{{onSubmitArr[cardState]}}</el-button>
      </div>

    </div>
    
  </div>
</template>
      
<script>
import { mapActions } from 'vuex'
import Title from "@/components/Title";
import rxios from '@/servers/rxios.js'
var moment = require('moment');
import {getCookie} from '@/config/system.js'
export default {
  name: 'CardRenew',
  components:{
     Title,
  },
  data () {
    return {
      title:'临时卡',
      cardState: 'yq',
      data: {},
      cardInfo: {},
      options: {
        CarType: []
      },
      rules:{
        CarOwnerName: [
          {
            required: true,
            message: "请输入车主姓名",
            trigger: ["blur", "change"]
          }
        ],
        Mobile: [
          {
            required: true,
            message: "请输入车主手机号码",
            trigger: ["blur", "change"]
          }
        ],
        CarTypeGuid:[
          {
            required: true,
            message: "请选择车类",
            trigger: ["blur", "change"]
          }
        ]
      },
      carType:'',
      input:'',
      month:'',
      onSubmitArr:{
        'yq': '延期',
        'sd': '锁定',
        'zx': '注销',
        'js': '解锁',
        'xg': '修改'
      },
      renewValue:'1',   
      startDate:'',
      endDate:'',
      endDateTime:'',
      payStyle:'现金支付',
      payMount:'',
      remark:'',
      refundAmount:'',
      resValue:'',
      payTrue:false,
      activeValue:'1',
      pickerOptions:this.processDate(),
    }
  },
  computed:{
    // address() {
    //   const { renewValue, month } = this
    //   return {
    //     renewValue,
    //     month
    //   }
    // }
  },
  watch: {
    // address: {
    //   handler: function(val) {
    //     console.log('address change: ', val)
    //     if(val.renewValue==2){
    //       if(val.month){
    //         this.payMount=val.month/this.resValue.months*this.resValue.amount
    //         this.payTrue=true
    //       }
    //     }else{
    //       this.payMount=''
    //       this.payTrue=false
    //     }
    //   },
    //   deep: true
    // }
  },
  methods: {
    processDate(){
      let self = this
      return {
        disabledDate(time){
          //截止日期需要大于起始日期-----这个起始时间不是服务器当前时间，而是月卡延期的起始时间
          if(self.cardState== 'yq'){
            var curDate = (new Date(self.startDate)).getTime()
          }
          else{
            var curDate = (new Date()).getTime()
          }
          let nowDate = curDate - 24 * 60 * 60 * 1000
          return time.getTime() < nowDate
        }
      }
    },
    async timeChange(value){
      // 延期金额
      // if(value==2){
      //   const res = await rxios("GET", '/ParkLot/GetPostponeTemplate', {CarTypeGuid: this.cardInfo.carTypeGuid})
      //   this.resValue=res
      // }
    },
    getSTime(val) {
        this.startDate=val;//这个sTime是在data中声明的，也就是v-model绑定的值
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
    timeTransferAfter(time){
      var d = new Date(time) + 24*60*60*1000;
      return d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate()
    },
    getNextDate(date,day) {  
      var dd = new Date(date);
      dd.setDate(dd.getDate() + day);
      var y = dd.getFullYear();
      var m = dd.getMonth() + 1 < 10 ? "0" + (dd.getMonth() + 1) : dd.getMonth() + 1;
      var d = dd.getDate() < 10 ? "0" + dd.getDate() : dd.getDate();
      return y + "-" + m + "-" + d;
    },
    addmulMonth(dtstr, n){  
        var s = dtstr.split("-");
        var yy = parseInt(s[0]);
        var mm = parseInt(s[1]); 
        var dd = parseInt(s[2]); 
        var dt = new Date(yy, mm, dd); 

        console.log(yy, mm, dd,dt,123)
        
        var num=mm + parseInt(n);
        console.log(num,num/12,1)
        if(num/12>1){
           yy+=Math.floor(num/12) ;
           mm=num%12;
        }else{
            mm+=parseInt(n);
        }
     
        return yy + "-" + mm  + "-" + dd;
    },  
    addMonth(dtstr, n) {
            var s = dtstr.split("-");
            var yy = parseInt(s[0]);
            var mm = parseInt(s[1] - 1);
            var dd = parseInt(s[2]);
            var dt = new Date(yy, mm, dd);
            dt.setMonth(dt.getMonth() + n); 
            if ((dt.getFullYear() * 12 + dt.getMonth()) > (yy * 12 + mm + n)) {
                dt = new Date(dt.getFullYear(), dt.getMonth(), 0);
            }else if(new Date(yy, mm + 1 , 0).getDate() == dd){
              dt = new Date(dt.getFullYear(), dt.getMonth() + 1, 0);
            }
            var year = dt.getFullYear();
            var month = dt.getMonth() + 1;
            var days = dt.getDate();
            var dd = year + "-" + month + "-" + days;
            return dd;
        },
    computeYmd(val, n) {
      let str = val.split('-');
      let d = new Date(str[0], str[1], str[2]);
      // 因为getMonth()获取的月份的值只能在0~11之间所以我们在进行setMonth()之前先给其减一
      d.setMonth((d.getMonth()-1) + n);
      let yy1 = d.getFullYear();
      let mm1 = d.getMonth()+1;
      let dd1 = d.getDate()-1;
      if(dd1 == '00'){
        mm1 = parseInt(mm1)-1;
        let new_date = new Date(yy1,mm1,1);
        dd1 = (new Date(new_date.getTime()-1000*60*60*24)).getDate()
      }
      if (mm1 < 10 ) {
        mm1 = '0' + mm1;
      }
      if (dd1 < 10) {
        dd1 = '0' + dd1;
      }
      return yy1 + '-' + mm1 + '-' + dd1;
    },
    cancel(){
      this.$router.push({
        path:'/TemporaryCarList'
      })
    },
    async onSubmit(){
      const send={
        ParkingCode:this.cardInfo.ParkCode,
        CarNo:this.cardInfo.CarNo,
        ProjectGuid:this.cardInfo.ProjectGuid,
        RechargeOperator: decodeURI(getCookie("UserName")),
        Balance: 0
      }

      const url= {
        'yq': 'MonthCardRenew',
        'sd': 'CarLocked',
        'js': 'CarUnLocked',
        'zx': 'CancelCar',
        'xg': 'ModifyMonthCard'
      }
      const successTip={
        'yq': '延期成功',
        'sd': '锁定成功',
        'js': '解锁成功',
        'zx': '注销成功',
        'xg': '修改成功'
      }

      // 延期
      if(this.cardState == 'yq'){
        Object.assign(send,{
          PayAmount: 0,
          PayStyle: '无',
          CarTypeGuid: this.data.CarTypeGuid,
        })

        if(this.renewValue==1){
          // 时间段延期

          if(!this.endDate){
            this.$message({
              showClose: true,
              message: '截止日期不能为空',
              type: 'warning'
            });
            return
          }

          console.log(this.startDate,this.endDate,1)

          // 按时间段延期
          Object.assign(send,{
            StartDate:this.data.StartDate,
            EndDate:this.endDate,
          })
        }

        if(this.renewValue == 2){
          //按整月延期
          if(!this.month){
            this.$message({
              showClose: true,
              message: '延期时间不能为空',
              type: 'warning'
            });
            return
          }
          // 按整月延期------起始日期和截止日期都传错了


          // 判断日期是否是最后一天 如果是最后一天 

         console.log(this.timeTransfer(this.cardInfo.EndDate),Number(this.month),11)
         console.log(this.addMonth(this.timeTransfer(this.cardInfo.EndDate),Number(this.month)),22) 

        const threeMonth=this.addMonth(this.timeTransfer(this.cardInfo.EndDate),Number(this.month))


          console.log(threeMonth,'延期传',this.startDate)
           Object.assign(send,{
            StartDate:this.startDate,
            EndDate:this.timeTransfer(threeMonth),
          })
        }

      }
      
      // 修改
      if(this.cardState == 'xg'){
        let flage = false
        this.$refs['ruleForm'].validate(valid => {
          if (valid) {
            flage = true
          }
        });
        if (!flage) return;
        Object.assign(send,{
          CarOwnerName: this.cardInfo.CarOwnerName,
          Mobile: this.cardInfo.Mobile,
          DrivewayGuidList: this.cardInfo.DrivewayGuidList.toString(),
          CarTypeGuid: this.cardInfo.CarTypeGuid
        })
      }

    const res= await rxios("formData", `/CardService/${url[this.cardState]}`, send)

    if(res.IsSuccess){
      this.$message({
        showClose: true,
        message: successTip[this.cardState],
        type: 'success'
      });
      this.$router.push({
        path:'/TemporaryCarList',
        query:{parkCode:this.cardInfo.ParkCode}
      })
    }
    else{
      this.$message({
        showClose: true,
        message: '操作失败',
        type: 'error'
      });
    }

    }
  },
  created () {
    // 获取参数
    this.cardState = this.$route.params.type
    this.cardInfo= this.$route.params.data || {}
    this.data = Object.assign({}, this.cardInfo);
    this.options = this.$route.params.options || {}
    if (this.cardState == 'yq') {
      this.startDate = this.getNextDate(this.cardInfo.EndDate,1)
      this.endDate = this.startDate
    }
  }
}
</script>

<style lang='less'>
.card-operate-wrap{
  .card-box{
    width: 878px;
    margin:0 auto;
    margin-top: 20px;
    padding-bottom: 40px;
    .card-info{
      background: #E8F6FF;
      border:1px solid #E6EAEE;
      .info-row{
        padding:0 30px;
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between; 
        flex-direction: row;
        .info-li{
          width: 200px;
          margin-right: 2%;
          margin-top: 35px;
          margin-bottom: 35px;
          font-size: 14px;
          color: #333C48;
          &:nth-child(3){
            margin-right: 0;
          }
          span{
            color: #6C7A8B;
          }
          .left-label{
            float: left;
          }
          .el-input{
            width: 130px;
            margin-top: -10px;
          }
        }
      }
    }
    .btn{
      text-align: center;
      margin-top: 100px;
      .el-button{
        padding: 12px 50px;
      }
      .el-button--info{
        margin-right: 60px;
      }
    }
    .delay-box{
      border-left:1px solid #E6EAEE;
      border-right: 1px solid #E6EAEE;
      .delay-li{
        padding-top: 40px;
        padding-left: 30px;
        padding-right: 30px;
        padding-bottom: 40px;
        border-bottom: 1px solid #E6EAEE;
        .el-radio__input{
          margin-right: 20px;
          margin-top: -3px;
        }
        .delay-row{
          display: flex;
          flex-wrap: wrap;
          justify-content: space-between; 
          flex-direction: row;
          margin-top: 20px;
          padding-left: 42px;
          .delay-input{
            width: 260px;
            .left-label{
              float: left;
              width: 90px;
              margin-top: 10px;
              margin-right: 10px;
            }
            .right-box{
              width: 160px;
              display: inline-block;
              .month{
                width: 115px;
                margin-right: 20px;
              }
            }
          }
        }
        .delay-pause{
          .delay-input{
            width: 290px;
            .left-label{
              width: 112px;
            }
          }
        }
      }

    }
  }
  .edit{
    flex-wrap: wrap;
    &-li{
      margin-top: 20px;
      width: 50%;
      .left-label{
        width: 120px;
        line-height: 40px;
        text-align: right;
      }
      .flex1{
        margin-right: 20px;
      }
    }
    .w100 {
      width: 90%;
    }
  }
}

@media screen and (max-width: 1400px){
 .card-operate-wrap .card-box{
   padding-bottom: 180px;
  }
}
.input-tip{
  position: relative;
}
.input-tip:before {
  content: "*";
  color: #f56c6c;
  position: absolute;
  top:6px;
  left: -10px;
}

</style>

