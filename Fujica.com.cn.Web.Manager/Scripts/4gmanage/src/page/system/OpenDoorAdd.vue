<template>
    <div class="open-add">
      <div class="form-box">
        <el-form :label-position="'right'" label-width="120px" :model="Send">
          <el-form-item label="开闸类型：">
              <el-select class="select-box" v-model="Send.OpenType" placeholder="请选择">
                <el-option
                  v-for="item in options"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value">
                </el-option>
              </el-select>
          </el-form-item>
          <el-form-item label="开闸原因：">
              <el-input 
              :autosize="{ minRows: 6, maxRows: 10}"
              v-model="Send.ReasonRemark"
              type="textarea" />
          </el-form-item>
        </el-form>
      </div>
      <BtnBox @cancel="Cancel" @sure="Sure"></BtnBox>
    </div>
</template>
        
<script>
import BtnBox from '@/components/Btns'
import { mapActions, mapState } from 'vuex'
export default {
  name: 'opendorradd',
  data () {
    return {
      options: [
        {
          value: 0,
          label: '手动开闸'
        },
        {
          value: 1,
          label: '免费开闸'
        }
      ],
      Send: {
        OpenType: '',
        ReasonRemark: ''
      }
    }
  },
  components: {
    BtnBox,
  },
  computed: {
    ...mapState('System', ['OpenGate'])
  },
  methods: {
    ...mapActions('System', ['AddNewOpenGateReason', 'ModifyOpenGateReason']),
    Cancel () {
      this.$router.push('/OpenDoor')
    },
    async Sure () {
      let res = null
      
      if(!this.Send.ReasonRemark){
        this.$message({
          showClose: true,
          message: '请填入开闸原因',
          type: 'warning'
        });
        return
      }


      if (this.OpenGate.Guid) {
        this.Send.Guid = this.OpenGate.Guid
        res = await this.ModifyOpenGateReason(this.Send)
      } else {
        res = await this.AddNewOpenGateReason(this.Send)
      }
      if (res.IsSuccess) {
        this.$router.push('/OpenDoor')
      }
    },
    SetData () {
      const editState=this.$route.query.state?this.$route.query.state:0
      if(editState==1){
         const { OpenType, ReasonRemark } = this.OpenGate
         this.Send = {
          OpenType: OpenType,
          ReasonRemark: ReasonRemark
        }
      }
      else{
        this.Send.OpenType=0
      }
    },
  },
  created () {
    this.SetData()
  }
}
</script>

<style lang="less">
.open-add{
  .form-box{
    width: 600px;
    margin: auto;
    margin-top: 100px;
  }
  .el-button--info{
      margin-left: 180px;
  }
  .el-button--primary{
    margin-left: 120px;
  }
}
</style>
              