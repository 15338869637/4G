<template>
    <div class="sthrough flex-box column">
      <Title :titleMsg="'通行设置'">
          <el-row slot="right" style="margin:23px 30px 0px 0px;">
            <el-button @click="Add" type="primary">添加</el-button>
          </el-row>
      </Title>

      <div class="box-table table flex1">
      
      <el-table
      class="voice-table"
      :data="mylist"
      border
      style="width: 100%"
      header-row-class-name="header-class-name"
      :row-class-name="tableRowClassName">
      <el-table-column
        prop="ParkCode"
        label="停车场"
        width="220">
          <template slot-scope="scope">
              {{getParkName(scope.row.ParkCode)}}
        </template>
      </el-table-column>
      <el-table-column
      label="车道名称"
      width="220">
        <template slot-scope="scope">
          <el-popover trigger="click" placement="top">
            <div class="drive-box">
                <div class="drive-title">
                    车道名称
                </div>
                <ul class="drive-cont">
                  <!-- <li v-for='(item,i) in DriveWayDetail' :key='i'>
                      {{item}}
                  </li> -->
                  {{DriveWayDetail.join('、')}}
                </ul>
            </div>
            <div slot="reference" class="name-wrapper">
              <el-button type="text" size="small" @click="getDriveWayDetail(scope.row.DrivewayGuid,scope.row.ParkCode)">查看详情</el-button>
            </div>
          </el-popover>
        </template>
    </el-table-column>

    <el-table-column
      prop="CarType"
      label="车类型"
      width="180">
        <template slot-scope="scope">
          <el-popover trigger="click" placement="top">
            <div class="drive-box">
                <div class="drive-title">
                    车类型
                </div>
                <ul class="drive-cont">
                  <!-- <li v-for='(item,i) in CarDetail' :key='i'>
                      {{item}}
                  </li> -->
                  {{CarDetail.join('、')}}
                </ul>
            </div>
            <div slot="reference" class="name-wrapper">
              <el-button type="text" size="small" @click="getCardTypeDetail(scope.row.CarTypeGuid,scope.row.ParkCode)">查看详情</el-button>
            </div>
          </el-popover>
        </template>
    </el-table-column>
    <el-table-column
      prop="Week"
      label="禁止通行日"
      width="160">
      <template slot-scope="scope">
          <el-popover
            placement="top"
            title="禁止通行日"
            width="200"
            trigger="click"
            :content="scope.row.Week.join('、')">
            <el-button slot="reference" type="text" size="small">查看详情</el-button>
          </el-popover>
      </template>
    </el-table-column>

    <el-table-column
      prop="startTime"
      label="禁止通行开始时间"
      width="150">
      <template slot-scope="scope">
        <span v-if="scope.row.StartTime">{{scope.row.StartTime}}</span>
        <span v-else>无</span>
      </template>
    </el-table-column>

    <el-table-column
      prop="endTime"
      label="禁止通行截止时间"
      width="150">
      <template slot-scope="scope">
        <span v-if="scope.row.EndTime">{{scope.row.EndTime}}</span>
        <span v-else>无</span>
      </template>
    </el-table-column>

    <el-table-column
    prop="showText"
    label="操作">
      <template slot-scope="scope">
         <el-button @click="Edit(scope.row)" type="primary">编辑</el-button>
         <el-button @click="Del(scope.row)"  type="info"> 删除</el-button>
      </template>
    </el-table-column>

    </el-table>

    </div>

     <Pagination 
        :total="dataList.length"
        @PageChange="PageChange"
        ></Pagination>
    </div>
  </template>
      
  <script>
import Title from "@/components/Title";
import { mapActions, mapState, mapMutations } from "vuex";
import Pagination from "@/components/Pagination";
import {getCookie} from '@/config/system.js'
import rxios from "@/servers/rxios.js";
export default {
  name: "Through",
  data() {
    return {
      tableData: [
        {
          drivewayGuid: 111,
          carTypeGuid: 222,
          assignDays: 333,
          startTime: 111,
          endTime: 222
        },
        {
          drivewayGuid: 111,
          carTypeGuid: 222,
          assignDays: 333,
          startTime: 111,
          endTime: 222
        }
      ],
      parkNameOptions: [],
      DriveWayDetail: [],
      DriveWayGidArr: [],
      CarDetail: [],
      CarGidArr: [],
      mylist: []
    };
  },
  components: {
    Title,
    Pagination
  },
  computed: {
    ...mapState("System", ["TrafficAddList"]),
    dataList () {
      return this.TrafficAddList.filter(item => !!this.getParkName(item.ParkCode))
    }
  },
  methods: {
    ...mapMutations("System", ["SET_DATA"]),
    ...mapActions("System", [
      "GetTrafficRestrictionList",
      "RemoveTrafficRestriction",
      "GetAddNewTrafficRestriction"
    ]),
    tableRowClassName({ row, rowIndex }) {
      if (rowIndex % 2 === 1) {
        return "tableBd";
      }
    },
    Add() {
      this.SET_DATA({ name: "TrafficAdd", val: {} });
      this.$router.push("/ThroughAdd");
    },
    Edit(vod) {
      this.GetAddNewTrafficRestriction(vod);

      this.$router.push({
        path: "/ThroughAdd",
        query: {
          state: 1,
          parkCode: vod.ParkCode
        }
      });
    },
    Del(vod) {
      this.$confirm("此操作将永久删除该设置, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(() => {
          console.log(vod);
          this.RemoveTrafficRestriction(vod);
          location.reload()
        })
        .catch(() => {});
    },
    // 获取停车场信息
    async getParkInfo() {
      let roleGuid=getCookie('RoleGuid')
      const resListAll = await rxios("GET", '/ParkLot/GetParkLotList')
      const resListAllfilter=resListAll.filter(item=>item.Existence==true)   //保留existence为true的
      const resList = await rxios("GET", "/Personnel/GetRolePermission",{
        Guid:roleGuid
      });
      const arr={}
      for(var i=0;i<resListAllfilter.length;i++){
        for(var j=0;j<resList.ParkingCodeList.length;j++){
          if(resListAllfilter[i].ParkCode==resList.ParkingCodeList[j]){
          //   arr.push({
          //     value: resListAllfilter[i].ParkCode,
          //     label: resListAllfilter[i].ParkName
          //   });
            arr[resListAllfilter[i].ParkCode] = resListAllfilter[i].ParkName
          }
        }
      }
      this.parkNameOptions=arr

    },
    // 停车场名称显示
    getParkName(code) {
      return this.parkNameOptions[code]
    },
    // 授权车道信息获取
    async getDriveWayDetail(DriveWay, ParkCode) {
      const res = await rxios("GET", "/ParkLot/GetDriveWayList", {
        ParkingCode: ParkCode
      });
      // 获取到这个停车场的车道
      this.DriveWayGidArr = res.map(item => {
        return {
          value: item.DrivewayName,
          guid: item.Guid
        };
      });

      const arr = [];
      for (var i = 0; i < this.DriveWayGidArr.length; i++) {
        for (var j = 0; j < DriveWay.length; j++) {
          if (this.DriveWayGidArr[i].guid == DriveWay[j]) {
            arr.push(this.DriveWayGidArr[i].value);
          }
        }
      }

      this.DriveWayDetail = arr;
    },
    // 授权卡类信息
    async getCardTypeDetail(CarType, ParkCode) {
      const res = await rxios("GET", "/ParkLot/GetCarTypeList", {
        ParkingCode: ParkCode,
        projectGuid: this.$store.state.ProjectGuid
      });
      // 获取到这个停车场的卡类
      this.CarGidArr = res.map(item => {
        return {
          value: item.CarTypeName,
          guid: item.Guid
        };
      });
      const arr = [];
      for (var i = 0; i < this.CarGidArr.length; i++) {
        for (var j = 0; j < CarType.length; j++) {
          if (this.CarGidArr[i].guid == CarType[j]) {
            arr.push(this.CarGidArr[i].value);
          }
        }
      }
      this.CarDetail = arr;
    },
    PageChange({ page, size }) {
      this.Page = (page - 1) * size;
      this.Size = page * size;
      this.mylist = this.dataList.slice(this.Page, this.Size);
    }
  },
  async created() {
    await this.getParkInfo();
    await this.GetTrafficRestrictionList();
    this.mylist = this.dataList.slice(0, 10);

    console.log(this.mylist,9812)
  }
};
</script>
  
<style lang="less">
.sthrough{
  height: 100vh;
  .table-page {
    padding: 20px 10px 60px 20px;
  }
}
.drive-box {
  width: 250px;
  padding-right: 30px;
}
.box-table{
  padding:20px 20px 0 20px;
}
</style>
      