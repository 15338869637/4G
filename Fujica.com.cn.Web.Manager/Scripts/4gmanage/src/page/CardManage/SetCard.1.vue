<template>
  <div>
    <!-- 开卡-start -->
    <div class="set-card-wrap">
        <Title :titleMsg='title'></Title>
        <div class="form-band">
          <el-form :model="ruleForm" :rules="rules" ref="ruleForm" label-width="100px" class="demo-ruleForm">
            <el-row>
              <el-col :span="12">
                <el-form-item label="停车场：" required prop="parkName" class="wid80">
                  <el-select v-model="ruleForm.parkName" :disabled="changeStatus!=1"  @change='changeSelect' placeholder="请选择停车场">
                    <el-option
                      v-for="item in parkNameOptions"
                      :key="item.value"
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
                <el-form-item label="卡类型：" required  class="wid80" prop="CarTypeGuid"  >
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

            <el-row v-show='cardType==1'>
              <el-col :span="12">
                <el-form-item label="起始日期：" required  class="wid80">
                  <el-date-picker type="date" disabled placeholder="选择日期" v-model="ruleForm.StartDate" value-format="yyyy-MM-dd" :picker-options="pickerOptions" style="width: 100%;"></el-date-picker>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="截止日期：" class="wid80"    prop="EndDate">
                   <el-date-picker type="date" :disabled='changeStatus==2' placeholder="选择日期" value-format="yyyy-MM-dd" v-model="ruleForm.EndDate" :picker-options="pickerOptions" style="width: 100%;"></el-date-picker>
                </el-form-item>
              </el-col>
            </el-row> 

            <el-row v-show='changeStatus==1'>
              <el-col :span="12">
                <el-form-item label="支付金额：" prop="PayAmount"  class="wid80" required>
                  <el-input v-model="ruleForm.PayAmount"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="支付方式：" required  class="wid80" prop="PayStyle" > 
                  <el-select v-model="ruleForm.PayStyle" placeholder="请选择支付方式">
                    <el-option label="现金支付" value="现金支付"></el-option>
                    <el-option label="微信支付" value="微信支付"></el-option>
                    <el-option label="支付宝支付" value="支付宝支付"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row> 

            <!-- 授权车道 -->
            <el-row>
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
            </el-row> 

            <el-row>
              <el-form-item class="btn-box">
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
const cityOptions = ['上海', '北京', '广州', '深圳'];
import { mapActions } from "vuex";
import Title from "@/components/Title";
import rxios from '@/servers/rxios.js'
import tinytime from 'tinytime';
export default {
  name: "SetCard",
  components: {
    Title
  },
  data() {
    return {
      changeStatus:1,
      cardInfo:'',
      title:'开卡',
      EndDateTime:'',
      balance:'',
      ruleForm: {
        parkName: "",
        CarNo: "",
        CarOwnerName: "",
        Mobile: "",
        CarTypeGuid: '',
        Remark: '',
        StartDate:new Date(),
        EndDate:'',
        PayAmount:'',
        PayStyle:'',
        DrivewayGuidList:[],
      },
      send:{},
      parkNameOptions:[{
        value: '',
        label: ''
      }],
      CarTypeOptions:[{
        value: '',
        label: ''
      }],
      checkAll: false,
      DriveWayList:[],
      DriveWayListOptions:[],
      DriveWayGidArr:[],
      isIndeterminate: true,
      parkCode:'',
      cardType:'',
      rules: {
        CarNo: [
          { required: true, message: "请输入车牌号", trigger: ['blur', 'change']  },
          // { min: 3, max: 5, message: "长度在 3 到 5 个字符", trigger: "blur" }
        ],
        CarOwnerName: [
          { required: true, message: "请输入车主姓名", trigger: ['blur', 'change']  }
        ],
        Mobile: [
          { required: true, message: "请输入联系方式", trigger:  ['blur', 'change'] },
          // { type: 'number', message: '请输入正确的联系方式', trigger: ['blur', 'change'] }
        ],
        CarTypeGuid: [
          { required: true, message: "请选择卡类型", trigger:  ['blur', 'change'] },
        ],
        EndDate: [
          {
            required: this.cardType==1?true:false,
            message: "请选择截止日期日期",
            trigger: "change"
          }
        ],
        PayAmount: [
          { required: true, message: "请输入支付金额", trigger: ['blur', 'change'] }
        ],
        PayStyle: [
          { required: true, message: "请输入支付方式", trigger: "change" }
        ],
        DrivewayGuidList: [
          {
            type: "array",
            required: true,
            message: "请至少选择一个授权车道",
            trigger: "change"
          }
        ],
        
      },
      pickerOptions: {
        disabledDate(time) {
          // 截止日期需要大于起始日期
          let curDate = (new Date()).getTime()
          // let nowDate = curDate - 24 * 60 * 60 * 1000
          // return time.getTime() < nowDate || time.getTime() > threeMonths
          return time.getTime() < curDate
        }
      },
    };
  },
  methods: {
    //获取车场信息
    async getParkInfo(){
      // 获取停车场信息
      const res = await rxios("GET", '/ParkLot/GetParkLotList')
      this.ruleForm.parkName=res[0].parkName
      
      const arr=[]
      res.forEach(item=>{
        arr.push({
          value:item.parkCode,
          label: item.parkName,
        })
      })

      this.parkNameOptions=arr
      this.parkCode=this.parkNameOptions[0].value
      this.getCarType()
      this.getDriveWay()
    },
    //获取卡类型
    async getCarType(){
      const res = await rxios("GET", '/ParkLot/GetCarTypeList', {ParkingCode: this.parkCode, projectGuid: this.$store.state.ProjectGuid})
      this.CarTypeOptions=res.map((item,index)=>{
        return {
          value:item.guid,
          label: item.carTypeName,
          index:index,
          type:item.carType,
          enable:item.enable,
        }
      })
      this.CarTypeOptions=this.CarTypeOptions.filter(item=>item.type!==0&&item.enable==true)

      // 月卡修改
      if(this.changeStatus==2){
        this.CarTypeOptions=this.CarTypeOptions.filter(item=>item.type==1&&item.enable==true)
      }

      // 储值卡修改
      if(this.changeStatus==3){
        this.CarTypeOptions=this.CarTypeOptions.filter(item=>item.type==2&&item.enable==true)
      }
      

    },
    // 获取车道
    async getDriveWay(){
      const res = await rxios("GET", '/ParkLot/GetDriveWayList', {ParkingCode: this.parkCode})

      this.DriveWayGidArr=res.map(item=>{
        return {
          value:item.drivewayName,
          guid: item.guid,
        }
      })
      console.log(this.DriveWayGidArr,888)
      
    },
    // 停车场切换
    changeSelect(parkCode){
       // 获取当前的停车场
      console.log(parkCode)
      this.parkCode=parkCode
      this.getCarType()
      this.getDriveWay()
    },
    //卡类型切换
    changeCard(gid){
      const arr=this.CarTypeOptions.filter(item=>item.value==gid)
      this.cardType=arr[0].type
    },
    // 提交
    submitForm(formName) {

      this.$refs[formName].validate(valid => {
        if (valid) {
          const {CarOwnerName,Mobile,DrivewayGuidList,CarTypeGuid,Remark,StartDate,EndDate,PayAmount,PayStyle,CarNo}=this.ruleForm

          const send={
            CarOwnerName:CarOwnerName,
            Mobile:Mobile,
            DrivewayGuidList:DrivewayGuidList.join(','),
            CarTypeGuid:CarTypeGuid,
            StartDate:StartDate,
            EndDate:EndDate,
            ParkingCode:this.parkCode,
            CarNo:CarNo,
            RechargeOperator: decodeURI(getCookie("UserName"))
          }
          
          if(this.changeStatus==1){
            Object.assign(send,{
              PayAmount:PayAmount,
              PayStyle:PayStyle,
              Remark:Remark
            })
            rxios("formData", '/CardService/ApplyNewCard', send).then(res=>{
              console.log(res,9999)
              if(res.IsSuccess){
                this.$message({
                  showClose: true,
                  message: '开卡成功',
                  type: 'success'
                });
                this.$router.push('/MonthCard')
              }
              else{
                this.$message({
                  showClose: true,
                  message: '开卡失败',
                  type: 'error'
                });
              }

            })
          }

          let url=''
          if(this.changeStatus==2){
            url='/CardService/ModifyMonthCard'
          }
          if(this.changeStatus==3){
            url='/CardService/ModifyValueCard'
          }
          
          rxios("formData", url, send).then(res=>{
            console.log(res,9999)
            if(res.IsSuccess){
              this.$message({
                showClose: true,
                message: '修改成功',
                type: 'success'
              });
              const link=['/Month','/MonthCard','/ValueCard']
              this.$router.push(link[this.changeStatus-1])
            }
            else{
              this.$message({
                showClose: true,
                message: '修改失败',
                type: 'error'
              });
            }
          })
         

        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
    // 全选
    handleCheckAllChange(val) {
        this.ruleForm.DrivewayGuidList = val ? this.DriveWayListOptions : [];
        this.isIndeterminate = false;
    },
    handleCheckedCitiesChange(value) {
        let checkedCount = value.length;
        this.checkAll = checkedCount === this.DriveWayGidArr.length;
        this.isIndeterminate = checkedCount > 0 && checkedCount < this.DriveWayGidArr.length;
      },
     // 时间转换
    timeTransfer(time){
      var d = new Date(time);
      return d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate()
    },
  },
  created() {
    const template = tinytime('{YYYY}-{Mo}-{DD} {h}-{mm}-{ss}');
    this.ruleForm.StartDate=template.render(new Date());

    this.getParkInfo()

    this.submitValue='开卡'

    // 修改的时候做的处理
    this.changeStatus=this.$route.query.status?this.$route.query.status:1
    if(this.changeStatus==2||this.changeStatus==3){
      this.title='修改'
      this.submitValue='修改'
      this.cardInfo=JSON.parse(this.$route.query.info)
      console.log(this.changeStatus,this.cardInfo,999)
      this.ruleForm.CarNo=this.cardInfo.carNo
      this.ruleForm.EndDate=this.cardInfo.endDate
      this.ruleForm.parkName=this.$route.query.parkName
      this.ruleForm.CarOwnerName=this.cardInfo.carOwnerName
      this.ruleForm.Mobile=this.cardInfo.mobile
      this.ruleForm.DrivewayGuidList=this.cardInfo.drivewayGuidList
      this.ruleForm.balance=this.cardInfo.balance

      this.ruleForm.PayAmount=1
      this.ruleForm.PayStyle=1
    }
   
  }
};
</script>

<style lang='less'>
.set-card-wrap {
  .form-band {
    width: 55%;
    margin: 0 auto;
    margin-top: 55px;
  }
  .el-select{
    display: block;
  }
  .wid80{
    width: 80%;
  }
  .wid90{
    width: 90%;
  }
  .driveway-box{
    border:1px solid #E7E9ED;
    .driveway-top{
      padding-left: 20px;
      height: 40px;
      line-height: 40px;
      font-size: 14px;
      background: #F3F8FB;
      border-bottom: 1px solid #E7E9ED;
    }
    .driveway-cont{
      padding-left: 20px;
      padding-top: 10px;
      padding-bottom: 10px;
      .el-checkbox{
        width: 32%;
        margin-bottom: 10px;
      }
      .el-checkbox+.el-checkbox{
        margin-left: 0;
      }
    }
  }
  .btn-box{
    text-align: center;
    width: 80%;
    button{
      width: 130px;
      margin-top: 62px;
    }
  }
}
</style>
     

      