<template>
    <div class="page flex-box space-between">
      <el-pagination
      background
      @size-change="SizeChange"
      :current-page="page.page"
      :page-sizes="page.pageSize || pageSize"
      :page-size="page.size"
      layout="total, sizes"
      :total="page.total">
    </el-pagination>
    <el-pagination
      background
      @current-change="CurrentChange"
      :current-page="page.page"
      :page-sizes="page.pageSize || pageSize"
      :page-size="page.size"
      layout="prev, pager, next"
      :total="page.total">
    </el-pagination>
    </div>
</template>
        
<script>
export default {
  name: 'tablePage',
  data () {
    return {
      pageSize: [10,20,30,50]
    }
  },
  props: {
    page: {
      type: Object,
      default: {
        page: 1,
        size: 10,
        total: 0
      }
    }
  },
  computed: {
    // myTotal () {
    //   if (this.total === this.Size) {
    //     return this.total + 1
    //   }
    //   return this.total
    // }
  },
  methods: {
    CurrentChange (page) {
      this.page.page = page
      this.$emit('PageChange')
    },
    SizeChange (size) {
      this.page.size = size
      this.page.page = 1
      this.$emit('PageChange')
    }
  }
}
</script>

<style lang='less'>
.page {
  padding: 10px 0;
}
</style>
              