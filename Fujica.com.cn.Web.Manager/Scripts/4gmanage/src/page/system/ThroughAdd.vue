<template>
    <div class="throug-add">
        <div class="form-box">
            <el-form :label-position="'right'" label-width="120px" :model="Send">
                <el-form-item label="所在车场：">
                    <el-select class="select-box" v-model="Send.ParkingCode" :disabled="state==1" placeholder="请选择"  @change='changeSelect'>
                        <el-option
                            v-for="item in ParkOpt"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="禁止通行时间：">
                    <el-row class="my-box">
                        <el-row :span="24" class="sub-box">

                            <!-- <el-col :span="3" class="left-labe">
                                指定时间:
                            </el-col> -->
                            <el-col :span="21"></el-col>
                                <span>限行时间</span>

                                 <el-time-picker
                                  v-model="Send.StartTime"
                                  value-format='HH:mm:ss'
                                  :picker-options="{
                                    selectableRange: '00:00:00 - 23:59:59'
                                  }"
                                  placeholder="开始时间">
                                </el-time-picker>
                                <el-time-picker
                                  v-model="Send.EndTime"
                                  value-format='HH:mm:ss'
                                  :picker-options="{
                                    selectableRange: '00:00:00 - 23:59:59'
                                  }"
                                  placeholder="截止时间">
                                </el-time-picker>

                            </el-col>
                        </el-row>
                        <el-row :span="24" class="sub-box">
                            <el-col :span="4" class="left-labe">
                              <el-checkbox :indeterminate="Send.Box[0].ind" v-model="Send.Box[0].all" @change="CheckAll(0)">指定周天(全部)</el-checkbox>
                            </el-col>
                            <el-col :span="20" class="right-box">
                              <el-checkbox-group v-model="Send.Box[0].list" @change="CheckOne(0)">
                                  <el-checkbox v-for="(item,index) in Send.Box[0].data" :label="item[0]" :key="item.value">{{item[1]}}</el-checkbox>
                              </el-checkbox-group>
                            </el-col>
                        </el-row>
                    </el-row>
                </el-form-item>
                <el-form-item label="选择受控车道：">
                  <el-row class="bro-box">
                    <el-checkbox :indeterminate="Send.Box[1].ind" v-model="Send.Box[1].all" @change="CheckAll(1)">全部车道</el-checkbox>
                    <el-checkbox-group v-model="Send.Box[1].list" @change="CheckOne(1)">
                        <el-checkbox v-for="(item,index) in Send.Box[1].data" :label="item[0]" :key="item.value">{{item[1]}}</el-checkbox>
                    </el-checkbox-group>
                  </el-row>
                </el-form-item>
                <el-form-item label="选择受控车类：">
                    <el-row class="bro-box">
                    <el-checkbox :indeterminate="Send.Box[2].ind" v-model="Send.Box[2].all" @change="CheckAll(2)">全部车类</el-checkbox>
                    <el-checkbox-group v-model="Send.Box[2].list" @change="CheckOne(2)">
                        <el-checkbox v-for="(item,index) in Send.Box[2].data" :label="item[0]" :key="index">{{item[1]}}</el-checkbox>
                    </el-checkbox-group>
                  </el-row>
                </el-form-item>
            </el-form>
        </div>
        <BtnBox @cancel="Cancel" @sure="Sure"></BtnBox>
    </div>
</template>
        
<script>
import BtnBox from '@/components/Btns'
import rxios from '@/servers/rxios.js'
import { mapActions, mapState, mapMutations } from 'vuex'
import {getCookie} from '@/config/system.js'
const Modle = {
  data: [],
  list: [],
  all: false,
  ind: false }
export default {
  name: 'througadd',
  data () {
    return {
      Week: ['一', '二', '三', '四', '五', '六', '日'],
      parkNameOptions:[],
      ParkOpt: [],
      Test: [],
      state:0,
       selectVal: "停车场",
      Send: {
        ParkingCode: '',
        Box: [{ ...Modle }, { ...Modle }, { ...Modle }],
        StartTime: '00:00:00',
        EndTime: '23:59:59',
        HaveTime: false,
        Time:[],
      },
      value1: '',
      value2: '',
    }
  },
  components: {
    BtnBox
  },
  computed: {
    ...mapState('System', ['OpenGate', 'Base', 'TrafficAdd','ParkNow','ParkLotList'])
  },
  watch: {
  
  },
  methods: {
    ...mapActions('System', ['GetBaseData', 'AddNewTrafficRestriction']),
    ...mapMutations('System', ['SET_PARK']),
    Cancel () {
      this.$router.push('/Through')
    },
    async Sure () {
      const { ParkingCode, Box: [Assign, Driveway, CarType], StartTime, EndTime, HaveTime } = this.Send
      let Week = [0, 0, 0, 0, 0, 0, 0]
      Assign.list.forEach(o => { Week[o] = 1 })
      const Send = {
        ParkingCode,
        DrivewayGuid: Driveway.list.map(o => o).join(','),
        CarTypeGuid: CarType.list.map(o => o).join(','),
        AssignDays: Week.join(''),
        StartTime:  StartTime ,
        EndTime: EndTime
      }

      // 受控车道和受控车类必填
      if(!Send.DrivewayGuid){
        this.$message({
          showClose: true,
          message: "受控车道必填",
          type: "warning"
        });
        return false
      }
      if(!Send.CarTypeGuid){
        this.$message({
          showClose: true,
          message: "受控车类必填",
          type: "warning"
        });
        return false
      }

      const res = await this.AddNewTrafficRestriction(Send)
      if (res.IsSuccess) {
        this.$router.push('/Through')
      }
    },
    CheckAll (num) {
      const { data, all } = this.Send.Box[num]
      this.Send.Box[num].list = all ? data.map(o => o[0]) : []
    },
    CheckOne (num) {
      const { list, data } = this.Send.Box[num]
      if (list.length === data.length) {
        this.Send.Box[num].all = true
      } else {
        this.Send.Box[num].all = false
      }
    },
     // 停车场切换
    async changeSelect(parkCode){

       // 获取当前的停车场
      this.SET_PARK({ newVal:parkCode })
      await this.GetBaseData()
      this.changeBox()

      // 需要重置复选框选择
      this.Send.Box[1].all = false
      this.Send.Box[2].all = false
    },
    SetData () {

    },
    // 受控车道和受控车类的数据转换
    changeBox(){
      this.Send.Box[0].data = this.Week.map((o, i) => [i, `周${o}`])
      this.Send.Box[1].data = this.Base.DriveWayList.map(o => [o.Guid, o.DrivewayName])
      this.Send.Box[2].data = this.Base.CarTypeList.map(o => [o.Guid, o.CarTypeName])

      console.log(this.TrafficAdd,5555)

      if (this.TrafficAdd.Guid) {
        const { BoxSet } = this.TrafficAdd
        this.Send = Object.assign(this.Send, this.TrafficAdd)
        console.log(this.Send,666666)


        BoxSet.forEach((o, i) => {
          this.Send.Box[i].list = o
          if (o.length === this.Send.Box[i].data.length) {
            this.Send.Box[i].all = true
            this.Send.Box[i].ind = true
          }
        })
      }
    },
  },
  async created () {

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

    this.ParkOpt=arr

    // 先设置默认值  车场默认显示第一个
    //获取是否是编辑还是添加
    this.state=this.$route.query.state?this.$route.query.state:0
    if(this.state==0){
      this.Send.ParkingCode=this.ParkOpt[0].value
    }else{
      this.Send.ParkingCode=this.$route.query.parkCode
    }
    this.SET_PARK({ newVal:this.Send.ParkingCode })
    await this.GetBaseData()
    this.changeBox()
  }
}
</script>

<style lang="less">
.throug-add{
  padding-left: 20px;
  padding-top: 40px;
}
.throug-add{
  min-width:1200px;
  .my-box{
    width: 800px;
    border:1px solid #E7E9ED;
    padding:20px;
    .sub-box{
      margin-bottom: 10px;
    }
    .left-labe{
      height: 40px;
      label{
       margin-top:10px;
      }
    }
    .right-box{
      height: 40px;
      >div{
        margin-top:10px;
      }
    }
  }
  .bro-box{
    width: 800px;
    border:1px solid #E7E9ED;
    padding:20px;
    .el-checkbox{
      margin-top: 10px;
      margin-right: 10px;
    }
    .el-checkbox+.el-checkbox{
      margin-left: 0!important;
    }
  }
}
</style>
          