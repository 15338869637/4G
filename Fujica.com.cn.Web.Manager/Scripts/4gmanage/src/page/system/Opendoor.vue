<template>
    <div class="open-door flex-box column">
        <Title titleMsg="开闸原因设置">
          <el-row slot="right" style="margin:23px 30px 0px 0px;">
            <el-button @click="Add" type="primary">新增</el-button>
          </el-row>
        </Title>
        <div class="box-table table flex1 auto">
        <el-table
        class="voice-table"
        :data="mylist"
        border
        style="width: 100%"
        header-row-class-name="header-class-name"
        :row-class-name="tableRowClassName">
        <el-table-column
        prop="OpenType"
        label="开闸类型"
        width="180">
        <template slot-scope="scope">
            {{OpenOption[scope.row.OpenType]}}
        </template>
        </el-table-column>
        <el-table-column
        prop="ReasonRemark"
        label="开闸原因"
        width="240">
    </el-table-column>
    <el-table-column
    prop="showText"
    label="操作">
        <template slot-scope="scope">
            <el-button  type="primary" @click="Edit(scope.row)">编辑</el-button>
            <el-button  type="info" @click="Del(scope.row)">删除</el-button>
        </template>
    </el-table-column>
    </el-table>
    </div>
    <Pagination 
    :total="OpenGateReasonList.length"
    @PageChange="PageChange"
    ></Pagination>
    </div>
</template>
        
<script>
import { mapActions, mapMutations, mapState } from 'vuex'
import Title from '@/components/Title'
import Pagination from '@/components/Pagination'
export default {
  name: 'HelloWorld',
  data () {
    return {
      Page: 1,
      Size: 10,
      mylist:[],
      OpenOption:['手动开闸','免费开闸']
    }
  },
  components: {
    Title,
    Pagination
  },
  computed: {
    ...mapState('System', ['OpenGatePage', 'OpenGateReasonList']),
    // mylist () {
    //   return this.OpenGateReasonList.slice(this.Page, this.Size)
    // }
  },
  methods: {
    ...mapMutations('System', ['SET_DATA']),
    ...mapActions('System', ['GetOpenGateReasonList', 'DeleteOpenGateReason']),
    PageChange ({ page, size }) {
      this.Page = (page - 1) * size
      this.Size = page * size
      this.mylist=this.OpenGateReasonList.slice(this.Page, this.Size)
    },
    tableRowClassName ({ row, rowIndex }) {
      if (rowIndex % 2 === 1) {
        return 'tableBd'
      }
    },
    Add () {
      this.$router.push('/OpenDoorAdd')
    },
    Del (vod) {
      this.$confirm('此操作将永久删除该设置, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.DeleteOpenGateReason({ guid: vod.Guid })
        location.reload()
      }).catch(() => {})
    },
    Edit (vod) {
      this.SET_DATA({ name: 'OpenGate', val: vod })
      // this.$router.push('/OpenDoorAdd')

      this.$router.push({
        path: "/OpenDoorAdd",
        query: {
          state: 1,
          OpenGate: vod
        }
      });
      
    }
  },
  async created () {
    await this.GetOpenGateReasonList()
    this.mylist=this.OpenGateReasonList.slice(0, 10)
  }
}
</script>

<style lang="less">
.open-door{
  height: 100vh;
  .table-page{
    padding:20px 10px 50px 20px;
  }
}
.voice-table{
}
.box-table{
  padding:20px 20px 0 20px;
}

</style>
          