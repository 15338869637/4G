<template>
    <el-row class="btns-box">
        <el-col :span="4" :offset="8">
            <el-button type="info" @click="$emit('cancel')">取消</el-button>
        </el-col>
        <el-col :span="4">
            <el-button type="primary" @click="$emit('sure')">确定</el-button>
        </el-col>
    </el-row>
</template>
        
<script>
export default {
  name: 'HelloWorld',
  data () {
    return {
    }
  }
}
</script>

<style lang="less">
.btns-box{
    float: left;
    width: 100%;
    margin-top: 30px;
    button{
        width:120px;
    }
}
</style>
        