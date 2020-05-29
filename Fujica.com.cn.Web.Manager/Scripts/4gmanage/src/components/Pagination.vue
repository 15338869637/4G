<template>
    <el-row 
    class="table-page"
    type="flex"
    justify="space-between">
        <el-col class="left" :span="12">
            <span style="padding-right: 10px">单页显示</span>
            <div class="page-size">
                <el-pagination
                    @size-change="SizeChange"
                    layout="sizes"
                    :current-page="Page"
                    :total="myTotal">
                </el-pagination>
            </div>
            <span style="padding-left: 10px">条</span>
        </el-col>
        <el-col class="right" :span="12">
            <el-pagination
                layout="prev, next"
                prev-text="上一页"
                next-text="下一页"
                :page-size="Size"
                :current-page="Page"
                @current-change="CurrentChange"
                :total="myTotal">
            </el-pagination>
        </el-col>
    </el-row>
</template>
        
<script>
export default {
  name: 'tablePage',
  data () {
    return {
      Size: 10,
      Page: 1
    }
  },
  props: {
    total: {
      type: Number,
      default: 0
    }
  },
  computed: {
    myTotal () {
      if (this.total === this.Size) {
        return this.total + 1
      }
      return this.total
    }
  },
  methods: {
    CurrentChange (page) {
      this.Page = page
      this.$emit('PageChange', { page, size: this.Size })
    },
    SizeChange (size) {
      this.Size = size
      this.Page = 1
      this.$emit('PageChange', { page: this.Page, size })
    }
  }
}
</script>

<style lang='less'>
.table-page{
    padding: 20px 10px 100px 20px;
    width: 100%;
    height: 48px;
    line-height: 48px;
    .select-box{
        width: 80px;
    }
    .left{
        overflow: hidden;
        height: 48px;
        >span{
            float: left;
            font-size:14px;
        }
        .page-size{
            width: 80px;
            float: left;
            margin-top: 8px;
        }
    }
    .right{
        text-align: right;
        .next{
            /* margin-right: 40px; */
        }
        .el-pagination span:not([class*=suffix]){
            border: 1px solid;
            border-radius: 4px;
            padding: 0 20px;
            height:30px;
            line-height: 30px;
        }
    }
}

@media screen and (max-width: 1400px){
  .table-page{
    padding: 20px 10px 200px 20px!important;
  }
}
</style>
              