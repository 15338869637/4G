<template>
  <!-- 延期 -->
  <div class="value-operate-wrap">
    <!-- 头部显示 -->
    <Title :titleMsg='title'></Title>

    <div class="card-box">
      <!-- 顶部信息显示 -->
      <div class="card-info">
          <div class="info-row">
            <div class="info-li">
              <span>车牌号码：</span>{{cardInfo.CarNo}}
            </div>
            <div class="info-li">
              <span>状态：</span>{{parkState(cardInfo.Enable,cardInfo.Locked)}}
            </div>
            <div class="info-li">
              <span>车辆类型：</span>{{carType}}
            </div>
          </div>
          <div class="info-row">
            <div class="info-li">
              <span>车主姓名：</span>{{cardInfo.CarOwnerName}}
            </div>
            <div class="info-li">
              <span>卡上余额：</span>{{cardInfo.Balance}}   
            </div>
            <div class="info-li">
              </div>
          </div>  
      </div>

      <!-- 充值 -->
      <div class="delay-box" v-if='cardState==0'>
        <div class="delay-li">
          <div class="delay-row">
            <div class="delay-input">
              <span class="left-label input-tip">充值金额：</span> 
              <div class="right-box">
                <el-input v-model="payAmount" placeholder="请输入内容"></el-input>
              </div>
            </div>
           <div class="delay-input">
              <span class="left-label input-tip">支付方式：</span>
              <div class="right-box">
                <el-select v-model="payStyle" placeholder="请选择支付方式" :disabled="true">
                  <el-option label="现金支付" value="现金支付"></el-option>
                  <!-- <el-option label="微信支付" value="微信支付"></el-option>
                  <el-option label="支付宝支付" value="支付宝支付"></el-option> -->
                </el-select>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 注销 -->
      <div class="delay-box" v-if='cardState==3'>
        <div class="delay-li">
          <div class="delay-row">
            <div class="delay-input">
              <span class="left-label input-tip">实际退款：</span> 
              <div class="right-box">
                <el-input v-model="refundAmount" placeholder="请输入内容"></el-input>
              </div>
            </div>
           <div class="delay-input">
              <span class="left-label">备注：</span> 
                <div class="right-box">
                  <el-input v-model="remark" placeholder="请输入内容"></el-input>
                </div>
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
import { mapActions } from "vuex";
import Title from "@/components/Title";
import rxios from "@/servers/rxios.js";
import {getCookie} from '@/config/system.js'
export default {
  name: "CardRenew",
  components: {
    Title
  },
  data() {
    return {
      title: "充值",
      cardState: 0,
      cardInfo: [],
      carType: "",
      input: "",
      month: "",
      onSubmitArr: ["充值", "锁定", "解锁", "注销"],
      renewValue: "1",
      startDate: "",
      endDate: "",
      endDateTime: "",
      payStyle: "现金支付",
      payAmount: "",
      remark: "",
      refundAmount: "",
      pickerOptions: {
        disabledDate(time) {
          // 截止日期需要大于起始日期
          let curDate = new Date().getTime();
          return time.getTime() < curDate;
        }
      }
    };
  },
  methods: {
    getSTime(val) {
      this.startDate = val; //这个sTime是在data中声明的，也就是v-model绑定的值
    },
    // 头部显示
    titleGet() {
      const titleArr = ["储值卡充值", "储值卡锁定", "储值卡解锁", "储值卡注销"];
      this.title = titleArr[this.cardState];
    },
    // 报停锁定状态判断------需要提取到外面
    parkState(enableValue, lockedValue) {
      //enable 为false-是报停 为true 判断locked true 为锁定 其他为正常
      if (enableValue) {
        if (lockedValue) {
          return "锁定";
        } else {
          return "正常";
        }
      } 
    },
    // 时间转换
    timeTransfer(time) {
      var d = new Date(time);
      return d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();
    },
    cancel() {
      this.$router.push({
        path:'/ValueCard',
        query:{parkCode:this.cardInfo.ParkCode}
      })
    },
    async onSubmit() {
      const send = {
        ParkingCode:this.cardInfo.ParkCode,
        CarNo:this.cardInfo.CarNo,
        ProjectGuid:this.cardInfo.ProjectGuid,
        RechargeOperator: decodeURI(getCookie("UserName")),
        Balance: this.cardInfo.Balance
      };

      const url = [
        "ValueCardRenew",
        "CarLocked",
        "CarUnLocked",
        "CancelCar"
      ];
      const successTip = [
        "充值成功",
        "锁定成功",
        "解锁成功",
        "注销成功"
      ];

      // 储值卡充值
      if (this.cardState == 0) {
        if (!this.payAmount) {
          this.$message({
            showClose: true,
            message: "充值金额不能为空",
            type: "warning"
          });
          return;
        }

        if (this.payAmount<0) {
          this.$message({
            showClose: true,
            message: "金额格式不正确",
            type: "warning"
          });
          return;
        }

        if(this.payAmount!=0&&!parseFloat(this.payAmount) > 0){
          this.$message({
            showClose: true,
            message: '金额格式不正确',
            type: 'warning'
          });
          return
        }

        if (!this.payStyle) {
          this.$message({
            showClose: true,
            message: "支付方式不能为空",
            type: "warning"
          });
          return;
        }

        Object.assign(send, {
          PayAmount: this.payAmount,
          PayStyle: this.payStyle,
          CarTypeGuid: this.cardInfo.CarTypeGuid
        });
      
      }

      // 储值卡锁定

      // 储值卡解锁

      // 储值卡注销
      if (this.cardState == 3) {
        if (!this.refundAmount) {
          this.$message({
            showClose: true,
            message: "实际退款不能为空",
            type: "warning"
          });
          return;
        }

        if (this.refundAmount<0) {
          this.$message({
            showClose: true,
            message: "金额格式不正确",
            type: "warning"
          });
          return;
        }

        if(this.refundAmount!=0&&!parseFloat(this.refundAmount) > 0){
          this.$message({
            showClose: true,
            message: '金额格式不正确',
            type: 'warning'
          });
          return
        }

        Object.assign(send, {
          RefundAmount: this.refundAmount,
          Remark: this.remark
        });
      }

      const res = await rxios(
        "formData",
        `/CardService/${url[this.cardState]}`,
        send
      );

      if (res.IsSuccess) {
        this.$message({
          showClose: true,
          message: successTip[this.cardState],
          type: "success"
        });
        this.$router.push({
        path:'/ValueCard',
        query:{parkCode:this.cardInfo.ParkCode}
      })
      } else {
        this.$message({
          showClose: true,
          message: "操作失败",
          type: "error"
        });
      }

    }
  },
  created() {
    // 获取参数
    this.cardState = this.$route.query.state;
    this.cardInfo = JSON.parse(this.$route.query.info);
    this.carType = this.$route.query.carType;
    // 头部显示
    this.titleGet();

    if (this.cardState == 0) {
      // 设置初始默认值
      let now = new Date();
      now.setDate(now.getDate() + 1);
      this.startDate = this.timeTransfer(now);
    } else if (this.cardState == 1) {
      // this.startDate=this.timeTransfer(new Date())
    }
  }
};
</script>

<style lang='less'>
.value-operate-wrap {
  .card-box {
    width: 878px;
    margin: 0 auto;
    margin-top: 20px;
    padding-bottom: 40px;
    .card-info {
      background: #e8f6ff;
      border: 1px solid #e6eaee;
      .info-row {
        padding: 0 30px;
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
        flex-direction: row;
        .info-li {
          width: 200px;
          margin-right: 2%;
          margin-top: 35px;
          margin-bottom: 35px;
          font-size: 14px;
          color: #333c48;
          &:nth-child(3) {
            margin-right: 0;
          }
          span {
            color: #6c7a8b;
          }
          .left-label {
            float: left;
          }
          .el-input {
            width: 130px;
            margin-top: -10px;
          }
        }
      }
    }
    .btn {
      text-align: center;
      margin-top: 100px;
      .el-button {
        padding: 12px 50px;
      }
      .el-button--info {
        margin-right: 60px;
      }
    }
    .delay-box {
      border-left: 1px solid #e6eaee;
      border-right: 1px solid #e6eaee;
      .delay-li {
        padding-top: 40px;
        padding-left: 30px;
        padding-right: 30px;
        padding-bottom: 40px;
        border-bottom: 1px solid #e6eaee;
        .el-radio__input {
          margin-right: 20px;
          margin-top: -3px;
        }
        .delay-row {
          display: flex;
          flex-wrap: wrap;
          justify-content: space-between;
          flex-direction: row;
          margin-top: 20px;
          padding-left: 42px;
          .delay-input {
            width: 260px;
            .left-label {
              float: left;
              width: 90px;
              margin-top: 10px;
              margin-right: 10px;
            }
            .right-box {
              width: 160px;
              display: inline-block;
              .month {
                width: 115px;
                margin-right: 20px;
              }
            }
          }
        }
        .delay-pause {
          .delay-input {
            width: 290px;
            .left-label {
              width: 112px;
            }
          }
        }
      }
    }
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

