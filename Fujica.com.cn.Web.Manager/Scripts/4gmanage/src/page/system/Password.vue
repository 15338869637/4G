<template>
    <div class="password">
      <Title :titleMsg="'密码修改'"></Title>
      <el-form class="pass-form" :model="ruleForm" :rules="rules" ref="ruleForm" :label-position="'right'" label-width="120px" >
          <el-form-item label="原密码：" prop="OldPassword">
              <el-input type="password"  v-model="ruleForm.OldPassword" class="inp"></el-input>
          </el-form-item>
          <el-form-item label="新密码：" prop="NewPassword">
              <el-input type="password"  v-model="ruleForm.NewPassword" class="inp"></el-input>
          </el-form-item>
          <el-form-item label="确认密码：" prop="ReNewPassword">
              <el-input type="password"  v-model="ruleForm.ReNewPassword" class="inp"></el-input>
          </el-form-item>
      </el-form>    
      <!-- <BtnBox @cancel="Cancel('ruleForm')" @sure="Sure('ruleForm')"></BtnBox> -->

      <div>
        <el-row class="btns-box">
          <el-col :span="10" style='visibility:hidden'>
            取消
          </el-col>
          <el-col :span="10">
              <el-button type="primary" @click="Sure('ruleForm')">确定</el-button>
          </el-col>
        </el-row>
      </div>

    </div>
</template>
      
<script>
import Title from '@/components/Title'
import BtnBox from '@/components/Btns'
import { mapActions } from 'vuex'
export default {
  name: 'HelloWorld',
  data () {
    const CheckPassWord = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入密码'))
      } else if (value !== this.ruleForm.NewPassword) {
        callback(new Error('两次输入密码不一致!'))
      } else {
        callback()
      }
    }
    const NewPassWord = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入新密码'))
      } else if (value === this.ruleForm.OldPassword) {
        callback(new Error('新密码与旧密码一致!'))
      } else {
        callback()
      }
    }
    return {
      ruleForm: {
        OldPassword: '',
        NewPassword: '',
        ReNewPassword: ''
      },
      rules: {
        OldPassword: [
          { required: true, message: '请输入原密码', trigger: 'blur' }
        ],
        NewPassword: [
          { validator: NewPassWord, trigger: 'blur' }
        ],
        ReNewPassword: [
          { validator: CheckPassWord, trigger: 'blur' }
        ]
      }
    }
  },
  components: {
    Title,
    BtnBox
  },
  methods: {
    ...mapActions('System', ['ModifyPasssword']),
    Sure (formName) {
      let _this = this
      this.$refs[formName].validate(async function (valid) {
        if (valid) {
          const res = await _this.ModifyPasssword(_this.ruleForm)
          if (res.data.IsSuccess) {
            _this.$message({
              showClose: true,
              message: "修改密码成功",
              type: "success"
            });

            _this.ruleForm = {
              OldPassword: '',
              NewPassword: '',
              ReNewPassword: ''
            }
          }
        } else {
          console.log('error submit!!')
          return false
        }
      })
    },
    Cancel () {}
  }
}
</script>

<style lang='less'>
.pass-form{
  margin: 30px auto;
  width: 600px;
  .inp{
    width: 400px;
  }
}
.password{
  .el-button--info{
      margin-left: 130px;
  }
  .el-button--primary{
    margin-left: 80px;
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
      