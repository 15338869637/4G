<template>
    <div class="account">
        收款账号设置
        <el-button @click="sayname">321</el-button>
    </div>
</template>
      
<script>
import { mapActions } from 'vuex'
export default {
  name: 'HelloWorld',
  data () {
    return {
    }
  },
  methods: {
    ...mapActions('System', ['sayname'])
  },
  created () {
    console.log(this.sayname)
  }
}
</script>

<style>

</style>
      