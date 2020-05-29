<template>
  <div>
    <!-- 开卡-start -->
    <div class="set-card-wrap">
        <Title :titleMsg='title'></Title>
        <div class="form-band">
          <el-form :model="ruleForm" :rules="rules" ref="ruleForm" label-width="100px" class="demo-ruleForm">
            <el-row>
              <el-col :span="12">
                <el-form-item label="停车场：" required prop="ParkingCode" class="wid80">
                  <el-select v-model="ruleForm.ParkingCode" :disabled="changeStatus!=1"  @change='changeSelect' placeholder="请选择停车场">
                    <el-option
                      v-for="item in parkNameOptions"
                      :key="item.index"
                      :label="item.label"
                      :value="item.value">
                    </el-option>
                  </el-select>

                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item  label="车牌号：" required prop="CarNo"  class="wid80" >
                  <el-input v-model="ruleForm.CarNo" :disabled="changeStatus!=1"></el-input>
                </el-form-item>
              </el-col>
            </el-row>

            <el-row>
              <el-col :span="12">
                <el-form-item label="车主姓名：" required prop="CarOwnerName" class="wid80">
                  <el-input v-model="ruleForm.CarOwnerName" ></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="联系方式："  required  prop="Mobile"  class="wid80">
                  <el-input v-model="ruleForm.Mobile" ></el-input>
                </el-form-item>
              </el-col>
            </el-row> 

            <el-row>
              <el-col :span="12">
                <el-form-item label="车辆类型：" required  class="wid80" prop="CarTypeGuid"  >
                  <el-select v-model="ruleForm.CarTypeGuid"  @change='changeCard'  placeholder="请选择卡类型">
                    <el-option
                      v-for="item in CarTypeOptions"
                      :key="item.index"
                      :label="item.label"
                      :value="item.value">
                    </el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="12" v-show='changeStatus==1'>
                <el-form-item  label="备注：" prop="Remark"  class="wid80">                                                                                                                                                                                                                                                                                                          
                  <el-input v-model="ruleForm.Remark"></el-input>
                </el-form-item>
              </el-col>

              <el-col :span="12" v-show='changeStatus!=1'>
                <el-form-item label="状态：" prop="Remark"  class="wid80">
                  正常
                </el-form-item>
              </el-col>
            </el-row> 

            <el-row>
              <el-col :span="12" v-if='changeStatus==3'>
                <el-form-item label="账户余额："  required  prop="Mobile"  class="wid80">
                  {{ruleForm.balance}}
                </el-form-item>
              </el-col>

              <el-col :span="12">
               
              </el-col>
            </el-row> 

            <el-row v-show='cardType==1||cardType==3||cardType==0'>
              <el-col :span="12">
                <el-form-item label="起始日期：" required  class="wid80">
                  <el-date-picker type="date" disabled placeholder="选择日期" v-model="ruleForm.StartDate" value-format="yyyy-MM-dd" :picker-options="pickerOptions" style="width: 100%;"></el-date-picker>
                </el-form-item>
              </el-col>
              <el-col :span="12" class='endDate'>
                <el-form-item label="截止日期：" class="wid80"    prop="EndDate">
                   <el-date-picker type="date" :disabled='changeStatus==2' placeholder="选择日期" value-format="yyyy-MM-dd" v-model="ruleForm.EndDate" :picker-options="pickerOptions" style="width: 100%;"></el-date-picker>
                </el-form-item>
              </el-col>
            </el-row> 

            <el-row v-show='changeStatus==1' v-if="cardType!=0">
              <el-col :span="12">
                <el-form-item label="支付金额：" prop="PayAmount"  class="wid80" required>
                  <el-input v-model="ruleForm.PayAmount"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="支付方式：" required  class="wid80" prop="PayStyle" > 
                  <el-select v-model="ruleForm.PayStyle" placeholder="请选择支付方式" :disabled="true">
                    <el-option label="现金支付" value="现金支付"></el-option>
                    <!-- <el-option label="微信支付" value="微信支付"></el-option>
                    <el-option label="支付宝支付" value="支付宝支付"></el-option> -->
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <!-- 授权车道 -->
            <!-- <el-row>
              <el-form-item label="授权车道："  class="wid90" required prop="DrivewayGuidList">
                <div class="driveway-box">
                  <div class="driveway-top">请选择授权车道</div>
                  <div class="driveway-cont" v-if='DriveWayGidArr.length>0'>
                    <el-checkbox :indeterminate="isIndeterminate" v-model="checkAll" @change="handleCheckAllChange">全部车道</el-checkbox>
                    <el-checkbox-group v-model="ruleForm.DrivewayGuidList" @change="handleCheckedCitiesChange">
                      <el-checkbox v-for="DriveWay in DriveWayGidArr" :label="DriveWay.guid" :key="DriveWay.value">{{DriveWay.value}}</el-checkbox>
                    </el-checkbox-group>
                  </div>
                </div>
              </el-form-item>
            </el-row>  -->

            <el-row>

              <el-form-item class="btn-box">
                <!-- 返回 -->

                <el-button type="info" v-show='changeStatus!=1'  @click="cancel()">取消</el-button>

                <el-button  type="primary" @click="submitForm('ruleForm')">{{submitValue}}</el-button>


                
              </el-form-item>
            </el-row> 

          </el-form>
        </div>
    </div>
    <!-- 开卡-end -->
  </div>
</template>
      
<script>
const cityOptions = ["上海", "北京", "广州", "深圳"];
import { mapActions } from "vuex";
import Title from "@/components/Title";
import rxios from "@/servers/rxios.js";
import  { getCookie }  from "@/config/system.js";
import tinytime from "format-datetime";
export default {
  name: "SetCard",
  components: {
    Title
  },
  data() {
    var  validPhone = (rules, value, callback) => {
        if (value === '') {
          callback(new Error('请输入手机号'));
        } else {
          if(value){
            value = value.trim();
            console.log(value)
          }
          var TEL_REGEXP = /^1([38][0-9]|4[579]|5[0-3,5-9]|6[6]|7[0135678]|9[89])\d{8}$/;
          console.log(TEL_REGEXP.test(value))

          if(!TEL_REGEXP.test(value)) {
            callback(new Error('手机号格式不正确'));
          } else {
            callback();
          }
        }
      };

      var  validCard = (rules, value, callback) => {
        if (value === '') {
          callback(new Error('请输入车牌号'));
        } else {
          if(value){
            value = value.trim();
            console.log(value)
          }
          var Card_REGEXP =  /^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z0-9]{1}[A-Z0-9挂学警港澳]{4,5}$/;
          console.log(Card_REGEXP.test(value))

          if(!Card_REGEXP.test(value)) {
            callback(new Error('车牌号格式不正确'));
          } else {
            callback();
          }
        }
      };

      var  validAmount = (rules, value, callback) => {
        if (value === '') {
          callback(new Error('请输入金额'));
        } else {
          console.log(!parseFloat(value))
          if ((value!=0&&!parseFloat(value) > 0) || parseFloat(value)<0){
            callback(new Error('金额格式不正确'));
          } else {
            callback();
          }
        }
      };

    return {
      carTypeGuid:'',
      changeType:1,
      changeStatus: 1,
      cardInfo: "",
      title: "登记发行",
      EndDateTime: "",
      balance: "",
      carType: {},
      ruleForm: {
        ParkingCode: "",
        CarNo: "",
        CarOwnerName: "",
        Mobile: "",
        CarTypeGuid: "",
        Remark: "",
        StartDate: new Date(),
        EndDate: "",
        PayAmount: "",
        PayStyle: "现金支付",
        cardType:1,
        // DrivewayGuidList:[],
      },
      DriveWayGidArrGid: [],
      send: {},
      parkNameOptions: [
        {
          value: "",
          label: ""
        }
      ],
      CarTypeOptions: [
        {
          value: "",
          label: ""
        }
      ],
      checkAll: false,
      DriveWayList: [],
      DriveWayListOptions: [],
      DriveWayGidArr: [],
      isIndeterminate: true,
      parkCode: "",
      cardType: "",
      rules: {
        CarNo: [
          {
            required: true,
            trigger: ["blur", "change"],
            validator: validCard,
          },
          // { min: 3, max: 5, message: "长度在 3 到 5 个字符", trigger: "blur" }
        ],
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
            trigger: ["blur", "change"],
            validator: validPhone,
          },
        ],
        CarTypeGuid: [
          {
            required: true,
            message: "请选择卡类型",
            trigger: ["blur", "change"]
          }
        ],
        EndDate: [
          {
            required: this.cardType == 1 ? true : false,
            message: "请选择截止日期日期",
            trigger: "change"
          }
        ],    
        PayAmount: [
          {
            required: true,
            trigger: ["blur", "change"],
            validator: validAmount,
          },
        ],
        PayStyle: [
          { required: true, message: "请输入支付方式", trigger: "change" }
        ]
        // DrivewayGuidList: [
        //   {
        //     type: "array",
        //     required: true,
        //     message: "请至少选择一个授权车道",
        //     trigger: "change"
        //   }
        // ],
      },
      pickerOptions: {
        disabledDate(time) {
          // 截止日期需要大于等于起始日期
          let curDate = new Date().getTime();
          let nowDate = curDate - 24 * 60 * 60 * 1000
          // return time.getTime() < nowDate || time.getTime() > threeMonths
           return time.getTime() < nowDate
        }
      }
    };
  },
  methods: {
    //获取车场信息
    async getParkInfo() {
      // 获取停车场信息
      let roleGuid=getCookie('RoleGuid')

      const resListAll = await rxios("GET", '/ParkLot/GetParkLotList')

      const resListAllfilter=resListAll.filter(item=>item.Existence==true)   //保留existence为true的

      console.log(resListAllfilter,1213)

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


      console.log(this.parkNameOptions,123456)

      this.parkNameOptions=arr

      this.ruleForm.ParkingCode = this.$route.query.parkCode
        ? this.$route.query.parkCode
        : this.parkNameOptions[0].value;

      this.parkCode = this.$route.query.parkCode
        ? this.$route.query.parkCode
        : this.parkNameOptions[0].value;

      this.getCarType();
      this.getDriveWay();
    },
    //获取卡类型
    async getCarType() {
      const res = await rxios("GET", "/ParkLot/GetCarTypeList", {
        ParkingCode: this.parkCode,
        projectGuid: this.$store.state.ProjectGuid
      });

      // const arr = [];
      // res.forEach(item => {
      //   arr.push({
      //     value: item.Guid,
      //     label: item.CarTypeName,
      //     type: item.CarType,
      //     enable: item.Enable
      //   });
      // });

      // this.CarTypeOptions = arr;
      // console.log(arr)

      this.CarTypeOptions = res.map((item, index) => {
        return {
          value: item.Guid,
          label: item.CarTypeName,
          index: index,
          type: item.CarType,
          enable: item.Enable
        };
      });

      console.log(this.CarTypeOptions,2)

      this.CarTypeOptions = this.CarTypeOptions.filter(
        item => item.enable == true
      );

      this.ruleForm.CarTypeGuid=this.CarTypeOptions[0].value
      console.log(this.CarTypeOptions[0].value,this.CarTypeOptions[0].label)

      this.ruleForm.cardType=this.CarTypeOptions[0].type;

      if (this.$route.query.info) {
        const cardInfo = JSON.parse(this.$route.query.info);
        this.carTypeGuid=cardInfo.CarTypeGuid
        this.ruleForm.CarTypeGuid = cardInfo.CarTypeGuid
          ? cardInfo.CarTypeGuid
          : res[0].Guid;
      }


      const carTypeArr=this.CarTypeOptions.filter(item=>item.value== this.carTypeGuid)
      const arrList=carTypeArr?carTypeArr:[]
      if(carTypeArr.length>0){
         this.cardType=arrList[0].type
      }
      
      if (this.changeStatus == 2) {
        // 月卡修改 1   //贵宾卡修改 3

        this.CarTypeOptions = this.CarTypeOptions.filter(
          item => item.type == this.cardType  && item.enable == true
        );

      }

      // 储值卡修改
      if (this.changeStatus == 3) {
        this.CarTypeOptions = this.CarTypeOptions.filter(
          item => item.type == 2 && item.enable == true
        );
      }
    },
    // 获取车道
    async getDriveWay() {
      const res = await rxios("GET", "/ParkLot/GetDriveWayList", {
        ParkingCode: this.parkCode
      });

      this.DriveWayGidArr = res.map(item => {
        return {
          value: item.DrivewayName,
          guid: item.Guid
        };
      });

      const arr = [];
      this.DriveWayGidArr.forEach(i => {
        arr.push(i.guid);
      });
      this.DriveWayGidArrGid = arr;

      console.log(this.DriveWayGidArrGid,12345678)


      if (this.DriveWayGidArrGid.length == 0) {
        this.$message({
          showClose: true,
          message: "此车场还未创建车道，如需开卡请先创建",
          type: "warning"
        });
      }
    },
    // 停车场切换
    changeSelect(parkCode) {
      // 获取当前的停车场
      this.parkCode = parkCode;
      this.getCarType();
      this.getDriveWay();
    },
    //卡类型切换
    changeCard(gid) {
      const arr = this.CarTypeOptions.filter(item => item.value == gid);
      this.cardType = arr[0].type;
      this.ruleForm.cardType = arr[0].type;
    },
    // 提交
    submitForm(formName) {
      // 如果选的是月卡车，截止日期需要填写

      this.$refs[formName].validate(valid => {
        if (valid) {
          let {
            CarOwnerName,
            Mobile,
            CarTypeGuid,
            Remark,
            StartDate,
            EndDate,
            PayAmount,
            PayStyle,
            CarNo
          } = this.ruleForm;
          !PayAmount && (PayAmount = 0);
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

          if (this.ruleForm.cardType == 1||this.ruleForm.cardType == 3) {
            if (!EndDate) {
              this.$message({
                showClose: true,
                message: "月卡须填入截止日期",
                type: "warning"
              });
              return false;
            }
          }
          
          console.log(this.DriveWayGidArrGid,9999)

          if (this.DriveWayGidArrGid.length == 0) {
            this.$message({
              showClose: true,
              message: "此车场还未创建车道，如需开卡请先创建",
              type: "warning"
            });
            return false;
          }

          let ProjectGuidValue = getCookie("ProjectGuid");
          let RechargeOperator = decodeURI(getCookie("UserName"));
          const send = {
            CarOwnerName: CarOwnerName,
            Mobile: Mobile,
            DrivewayGuidList: this.DriveWayGidArrGid.join(","),
            CarTypeGuid: CarTypeGuid,
            StartDate: StartDate,
            EndDate: EndDate,
            ParkingCode: this.parkCode,
            CarNo: CarNo,
            RechargeOperator,
            ProjectGuid: ProjectGuidValue
          };
          !send.EndDate && (delete(send.EndDate))
          if (this.changeStatus == 1) {
            Object.assign(send, {
              PayAmount: PayAmount,
              PayStyle: PayStyle,
              Remark: Remark
            });

            rxios("formData", "/CardService/ApplyNewCard", send).then(res => {

              console.log(res,'结果')

              if (res && res.IsSuccess) {
                this.$message({
                  showClose: true,
                  message: "开卡成功",
                  type: "success"
                });
                const arr = ['/TemporaryCarList', '/MonthCard', "/ValueCard"];
                const pathUrl = arr[this.ruleForm.cardType];
                this.$router.push({
                  path: pathUrl,
                  query: { parkCode: this.parkCode }
                });
              } else {
                this.$message({
                  showClose: true,
                  message: res.MessageContent,
                  type: "error"
                });
              }

            });

            return;
          }

          let url = "";
          if (this.changeStatus == 2) {//月卡
            url = "/CardService/ModifyMonthCard";
          } else if (this.changeStatus == 3) {//储值卡
            url = "/CardService/ModifyValueCard";
          }

          rxios("formData", url, send).then(res => {
            if (res.IsSuccess) {
              this.$message({
                showClose: true,
                message: "修改成功",
                type: "success"
              });
              const link=['/Month','/MonthCard','/ValueCard']
              this.$router.push({
                path: link[this.changeStatus-1],
                query: { parkCode: this.parkCode }
              });
            } else {
              this.$message({
                showClose: true,
                message: "修改失败",
                type: "error"
              });
            }
          });
         
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
    cancel() {
      let pathUrl=''
      if (this.changeStatus == 2) {//月卡
        pathUrl = "/MonthCard";
      } else if (this.changeStatus == 3) {//储值卡
        pathUrl = "/ValueCard";
      } 
      this.$router.push({
        path: pathUrl,
        query: { parkCode: this.parkCode }
      });
    },
    // // 全选
    // handleCheckAllChange(val) {
    //     this.ruleForm.DrivewayGuidList = val ? this.DriveWayListOptions : [];
    //     this.isIndeterminate = false;
    // },
    // handleCheckedCitiesChange(value) {
    //     let checkedCount = value.length;
    //     this.checkAll = checkedCount === this.DriveWayGidArr.length;
    //     this.isIndeterminate = checkedCount > 0 && checkedCount < this.DriveWayGidArr.length;
    //   },
    // 时间转换
    timeTransfer(time) {
      var d = new Date(time);
      return d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();
    }
  },
  created() {
    const template = tinytime(new Date(), 'yyyy-MM-dd HH:mm:ss');
    this.ruleForm.StartDate = template;

    this.getParkInfo();
    this.submitValue = "登记";

    // 修改的时候做的处理
    this.changeStatus = this.$route.query.status ? this.$route.query.status : 1;
    if (this.changeStatus == 2 || this.changeStatus == 3) {
      this.parkCode = this.$route.query.parkCode;

      this.carType=this.$route.query.carType

      this.title = "修改";
      this.submitValue = "修改";
      this.cardInfo = JSON.parse(this.$route.query.info);
      this.ruleForm.CarNo = this.cardInfo.CarNo;
      this.ruleForm.EndDate = this.cardInfo.EndDate;
      this.ruleForm.parkCode = this.$route.query.parkCode;
      this.ruleForm.CarOwnerName = this.cardInfo.CarOwnerName;
      this.ruleForm.Mobile = this.cardInfo.Mobile;
      this.ruleForm.DrivewayGuidList = this.cardInfo.DrivewayGuidList;
      this.ruleForm.balance = this.cardInfo.Balance;

      this.ruleForm.PayAmount = 1;
      this.ruleForm.PayStyle = 1;
    }
  }
};
</script>

<style lang='less'>
.set-card-wrap {
  padding-bottom:40px;
  .form-band {
    width: 55%;
    margin: 0 auto;
    margin-top: 55px;
  }
  .el-select {
    display: block;
  }
  .wid80 {
    width: 80%;
  }
  .wid90 {
    width: 90%;
  }
  .driveway-box {
    border: 1px solid #e7e9ed;
    .driveway-top {
      padding-left: 20px;
      height: 40px;
      line-height: 40px;
      font-size: 14px;
      background: #f3f8fb;
      border-bottom: 1px solid #e7e9ed;
    }
    .driveway-cont {
      padding-left: 20px;
      padding-top: 10px;
      padding-bottom: 10px;
      .el-checkbox {
        width: 32%;
        margin-bottom: 10px;
      }
      .el-checkbox + .el-checkbox {
        margin-left: 0;
      }
    }
  }
  .btn-box {
    text-align: center;
    width: 80%;
    button {
      width: 130px;
      margin-top: 62px;
    }
  }
}

.endDate {
  .el-form-item__label:before {
    content: "*";
    color: #f56c6c;
    margin-right: 4px;
  }
}


@media screen and (max-width: 1400px){
 .set-card-wrap{
  .form-band {
    margin-top: 9px;
  }
  .el-form-item{
    margin-bottom: 17px;
  }
 }
}
</style>
     

      