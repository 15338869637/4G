<template>
    <el-row class="system-voice">
        <Title :titleMsg="'语音指令'"></Title>
        <el-row style="margin-top:30px;">
          <el-col :span="4">
            <div class="left-bar">
                <div class="head"></div>
                <!-- <el-menu
                  @select="WaySelect">
                    <el-submenu :index="Lot.parkCode" v-for="(Lot,index) in ParkLotList" :key="Lot.parkCode">
                        <template slot="title">
                            <span>{{Lot.parkName}}</span>
                        </template>
                        <el-menu-item :index="Way.guid" v-for="Way in Lot.DriveWayLists" :key="Way.guid">
                            <span slot="title" @click="choiceWay(Way)">{{Way.drivewayName}}</span>
                        </el-menu-item>
                    </el-submenu>
                </el-menu> -->

               <el-menu
                  @select="WaySelect"  @open="handleOpen" unique-opened>
                    <el-submenu :index="Lot.value" v-for="(Lot,index) in ParkOpt" :key="Lot.value">
                        <template slot="title">
                            <span>{{Lot.label}}</span>
                        </template>
                        <el-menu-item :index="Way.Guid" v-for="Way in DriveWayLists" :key="Way.Guid" v-on:click="choiceWay(Way)">
                            <span slot="title" >{{Way.DrivewayName}}</span>
                        </el-menu-item>
                    </el-submenu>
                </el-menu> 
            </div>
          </el-col>

          <!-- 右边显示部分 -->
          <el-col :span="19" :push="1" v-if="tableData.length >0">
            <div class='screen-right'>
              <el-button style='margin-right:20px;' type="primary" @click='reset'  size="small">重置</el-button>
              <el-radio v-model="screenValue" label="1" @change='screenChange'>一屏显示</el-radio>
              <el-radio v-model="screenValue" label="2" @change='screenChange'>两屏显示</el-radio>
              <el-radio v-model="screenValue" label="3" @change='screenChange'>三屏显示</el-radio>
            </div>

            <el-table
              class="voice-table"
              :data="tableData"
              border
              style="width: 100%"
              :row-class-name="tableRowClassName">
              <el-table-column
                prop="name"
                label="命令名称"
                :key="Math.random()"
                width="240">
                 <template slot-scope="scope">
                   {{commandText[tableData[scope.$index].CommandType]}}
                </template>
              </el-table-column>
              <el-table-column
              prop="showVoice"
              label="语音提示"
              :key="Math.random()"
              width="240">
              <template slot-scope="scope">
                <el-row>
                  <el-input v-model="tableData[scope.$index].ShowVoice" />
                </el-row>
              </template>
            </el-table-column>
            <el-table-column
              prop="showText"
               v-if='screenValue=="1"'
               :key="Math.random()"
              label="屏显文字">
                <template slot-scope="scope">
                    <el-row v-if="scope.$index < 1">
                        <el-row>
                            <el-col :span="8">
                              <el-input style="width:200px;" v-model="tableData[scope.$index].ShowText"/>
                            </el-col>
                        </el-row>
                    </el-row>
                    <el-row v-else>
                      <el-row v-for="(item,index) in 1" :key="index"  class="line-height">
                          <el-col style="width:60px;">第{{index===0?'一': '二'}}行</el-col>
                          <el-col :span="16"><el-input v-model="tableData[scope.$index]['ShowText'+index]" /></el-col>
                      </el-row>
                    </el-row>
                </template>
              </el-table-column>

              <el-table-column
              prop="showText"
               v-if='screenValue=="2"'
               :key="Math.random()"
              label="屏显文字">
                <template slot-scope="scope">
                    <el-row v-if="scope.$index < 1">
                        <el-row>
                            <el-col :span="8">
                              <el-input style="width:200px;" v-model="tableData[scope.$index].ShowText"/>
                            </el-col>
                        </el-row>
                    </el-row>
                    <el-row v-else>
                      <el-row v-for="(item,index) in 2" :key="index"  class="line-height">
                          <el-col style="width:60px;">第{{index===0?'一': '二'}}行</el-col>
                          <el-col :span="16"><el-input v-model="tableData[scope.$index]['ShowText'+index]" /></el-col>
                      </el-row>

                    </el-row>
                </template>
              </el-table-column>

              <el-table-column
              prop="showText"
              :key="Math.random()"
               v-if='screenValue=="3"'
              label="屏显文字">
                <template slot-scope="scope">
                    <el-row v-if="scope.$index < 1">
                        <el-row>
                            <el-col :span="8">
                              <el-input style="width:200px;" v-model="tableData[scope.$index].ShowText"/>
                            </el-col>
                        </el-row>
                    </el-row>
                    <el-row v-else>
                      <el-row v-for="(item,index) in 3" :key="index"  class="line-height">
                          <el-col style="width:60px;">第{{index===0?'一':index===1?'二':'三'}}行</el-col>  
                          <el-col :span="16"><el-input v-model="tableData[scope.$index]['ShowText'+index]" /></el-col>
                      </el-row>
                    </el-row>
                </template>
              </el-table-column>
            </el-table>
            
            <!-- <BtnBox @cancel="Cancel" @sure="Sure"></BtnBox> -->

          <div>
            <el-row class="btns-box">
              <el-col :span="8" style='visibility:hidden'>
                取消
              </el-col>
              <el-col :span="4">
                  <el-button type="primary" @click="sure()">确定</el-button>
              </el-col>
            </el-row>
          </div>

          </el-col>

        </el-row>
    </el-row>
</template>
      
<script>
import {
  outSytemVoic,
  inSytemVoic,
  inSysArr,
  outSysArr
} from "@/config/system";
import BtnBox from "@/components/Btns";
import Title from "@/components/Title";
import rxios from "@/servers/rxios.js";
import { mapActions, mapState } from "vuex";
import {getCookie} from '@/config/system.js'
export default {
  name: "HelloWorld",
  data() {
    return {
      driveGuid:'',
      DriveWayLists:[],
      tableData: [],
      first: true,
      ParkOpt:[],
      screenValue:'2',
      resetList:[{
			"CommandType": 0,
			"ShowVoice": "",
			"ShowText": "深圳富士智能系统有限公司研制"
		}, {
			"CommandType": 1,
			"ShowVoice": "",
			"ShowText": "深圳富士智能系统有限公司"
		}, {
			"CommandType": 2,
			"ShowVoice": "\u003cp\u003e,欢迎光临",
			"ShowText": "\u003cp\u003e\r\n欢迎光临"
		}, {
			"CommandType": 3,
			"ShowVoice": "\u003cp\u003e,请扫码缴费",
			"ShowText": "\u003cp\u003e\r\n请扫码缴费"
		}, {
			"CommandType": 4,
			"ShowVoice": "\u003cp\u003e离场超时，请扫码补缴费用",
			"ShowText": "\u003cp\u003e，离场超时\r\n请扫码补缴费用"
		}, {
			"CommandType": 5,
			"ShowVoice": "\u003cp\u003e，离场超时，请扫码补缴费用",
			"ShowText": "\u003cp\u003e，离场超时\r\n请扫码补缴费用"
		}, {
			"CommandType": 6,
			"ShowVoice": "\u003cp\u003e，欢迎光临，可用日期\u003cd\u003e天",
			"ShowText": "\u003cp\u003e，欢迎光临\r\n可用日期\u003cd\u003e天"
		}, {
			"CommandType": 7,
			"ShowVoice": "\u003cp\u003e，欢迎光临，可用日期\u003cd\u003e天，请尽快延期",
			"ShowText": "\u003cp\u003e，欢迎光临\r\n可用日期\u003cd\u003e天，请尽快延期"
		}, {
			"CommandType": 8,
			"ShowVoice": "\u003cp\u003e，月卡已过期，请续费",
			"ShowText": "\u003cp\u003e\r\n月卡已过期，请续费"
		}, {
			"CommandType": 9,
			"ShowVoice": "\u003cp\u003e，一路顺风，可用日期\u003cd\u003e天",
			"ShowText": "\u003cp\u003e，一路顺风\r\n可用日期\u003cd\u003e天"
		}, {
			"CommandType": 10,
			"ShowVoice": "\u003cp\u003e，一路顺风，可用日期\u003cd\u003e天，请尽快延期",
			"ShowText": "\u003cp\u003e，一路顺风\r\n可用日期\u003cd\u003e天，请尽快延期"
		}, {
			"CommandType": 11,
			"ShowVoice": "\u003cp\u003e，欢迎光临，余额\u003cb\u003e元",
			"ShowText": "\u003cp\u003e，欢迎光临\r\n余额\u003cb\u003e元"
		}, {
			"CommandType": 12,
			"ShowVoice": "\u003cp\u003e，欢迎光临，余额\u003cb\u003e元，请尽快充值",
			"ShowText": "\u003cp\u003e，欢迎光临\r\n余额\u003cb\u003e元，请尽快充值"
		}, {
			"CommandType": 13,
			"ShowVoice": "\u003cp\u003e，一路顺风，此次收费\u003cc\u003e元，余额\u003cb\u003e元",
			"ShowText": "\u003cp\u003e，一路顺风\r\n此次收费\u003cc\u003e元，余额\u003cb\u003e元"
		}, {
			"CommandType": 14,
			"ShowVoice": "\u003cp\u003e，一路顺风，此次收费\u003cc\u003e元，余额\u003cb\u003e元，请尽快充值",
			"ShowText": "\u003cp\u003e，一路顺风\r\n此次收费\u003cc\u003e元，余额\u003cb\u003e元，请尽快充值"
		}, {
			"CommandType": 15,
			"ShowVoice": "\u003cp\u003e，余额不足，请充值后出场",
			"ShowText": "\u003cp\u003e\r\n余额不足，请充值后出场"
		}, {
			"CommandType": 16,
			"ShowVoice": "车道限制，禁止通行",
			"ShowText": "车道限制\r\n禁止通行"
    }, {
			"CommandType": 17,
			"ShowVoice": "<p>,无入场记录",
			"ShowText": "<p>\r\n无入场记录"
		}, {
			"CommandType": 18,
			"ShowVoice": "此次开闸需确认，请联系管理员开闸",
			"ShowText": "此次开闸需确认，请联系管理员开闸"
		}],
      commandText:['入口空闲',' 出口空闲','临时车入场识别','未缴费临时车出场识别','已缴费临时车出场识别','已缴费临时车离场超时出场识别','月卡车入场识别','月卡车入场识别提醒延期',' 已过期月卡车入场识别','月卡车出场识别','月卡车出场识别提醒延期','储值车入场识别','储值车入场识别提醒充值','储值车出场识别','储值车出场识别提醒充值',' 储值车出场余额不足','车道通行限制','无入场记录','确认开闸提示']
    };
  },
  components: {
    BtnBox,
    Title
  },
  computed: {
    ...mapState("System", ["ParkLotList", "SelectWay"])
  },
  methods: {
    ...mapActions("System", ["FetchPark", "WaySelect", "SystemSet"]),
    tableRowClassName({ row, rowIndex }) {
      if (rowIndex % 2 === 1) {
        return "tableBd";
      }
    },
    async sure() {

      // 判断是哪一个模板
      // 模板一
      if(this.screenValue=='1'){
        this.tableData.map(item=>{
          item.ShowText1=''
          item.ShowText2=''
        })
      }
      else if(this.screenValue=='2'){
        this.tableData.map(item=>{
          item.ShowText2=''
        })
      }

      console.log(this.tableData,'data数据')


      const res=await this.SystemSet(this.tableData,this.driveGuid);

      if(res==1){
        this.$message({
          showClose: true,
          message: "保存成功",
          type: "success"
        });
      }

    },
    async choiceWay(way) {

      const { Guid, Type } = way;
      await this.WaySelect(Guid);

      this.driveGuid=Guid

      const finWay = this.SelectWay.CommandList.map(o => {
        const arr = o.ShowText.split("\r\n");
        o.ShowText0 = arr[0];
        o.ShowText1 = arr[1];
        o.ShowText2 = arr[2];
        return o;
      });

      console.log(finWay,123234)
      this.tableData=finWay
    },
    Cancel() {},
    async handleOpen(key, keyPath) {

      // 获取车道
      const DriveWay = await rxios('GET', '/ParkLot/GetDriveWayList', { ParkingCode: key });
      this.DriveWayLists=DriveWay

    },
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
      await this._sleep(600);
      this.ParkOpt=arr
      console.log(this.ParkOpt,123)
    },
    screenChange(){
      console.log(this.screenValue,123)
    },
    reset(){
      this.screenValue=2
      const finWay =  this.resetList.map(o => {
        const arr = o.ShowText.split("\r\n");
        o.ShowText0 = arr[0];
        o.ShowText1 = arr[1];
        o.ShowText2 = arr[2];
        return o;
      });
      console.log(finWay,123234)
      this.tableData=finWay

    }
  },
  created() {
   this.getParkInfo()

  }
};
</script>

<style lang="less">
.system-voice {
  padding-bottom: 20px;
  .line-height {
    padding: 6px 0px;
    line-height: 40px;
    input {
      float: left;
    }
  }
  .left-bar {
    // width: 400px;
    // height: 800px;
    // overflow: auto;
    // position: fixed;
    // left: 0;
    // top:0;
    border: #e7e9ed 1px solid;
    .head {
      padding-left: 20px;
      line-height: 48px;
      text-align: left;
      font-weight: 600;
      font-size: 18px;
      height: 48px;
      background: #f3f8fb;
    }
  }
  .screen-right{
    float: right;
    padding-bottom: 20px;
    padding-right: 20px;
  }
}


@media screen and (max-width: 1400px){
 .system-voice{
   padding-bottom: 160px;
 }
}

.btns-box{
    float: left;
    width: 100%;
    margin-top: 30px;
    button{
        width:120px;
    }
}
</style>
      