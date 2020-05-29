<template>
  <div id="app">
    <div>
        <router-view/>
    </div>
  </div>
</template>
<script>
import { myRouter } from './router'
export default {
  name: 'App',
  data () {
    return {
      winshow: window.localStorage.getItem('window')
    }
  },
  created () {
    const winshow = window.localStorage.getItem('window')
    const nowRouter = myRouter.filter(o => o.pageType).find(o => o.pageType === winshow)
    console.log(nowRouter,123)
    // this.$router.push(nowRouter.path)

    console.log(this.$route.path,123456)

    // this.$router.push(this.$route.path)

    // 权限跳转
    var role = JSON.parse(localStorage.getItem('role') || '{"arr":[]}')
    role.arr && (role.arr = role.arr.concat(['/Home/Trans/layout', '/Home/Index', '/']))
    var currUrl = location.pathname + location.hash.split('?')[0]
    if (role.all && role.all[currUrl]) {
      if (role.arr && currUrl !== '/' && role.arr.indexOf(currUrl) == -1) {
        this.$confirm('您没有权限进入此页面。', '提示', {
            confirmButtonText: '确定',
            type: 'warning'
          }).then(() => {
            location.href = role.arr[0]
          }).catch(() => {
            location.href = role.arr[0]
          });
      }
    }
  }
}
</script>

<style lang="less">
body{
  margin:0;
  padding:0;
}
.m_t10{
  margin-top: 10px;
}
.m_r10 {
  margin-right: 10px;
}
.red {
  color: red;
}
.green {
  color: green;
}
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  background: #fff;
  height: 100vh;
  // margin:8px;
  overflow-y: auto;
}
.el-table .tableBd{
  background: #F3F8FB;
}
.table {
  .header-class-name{
    .gutter{
      width: 100px !important;
      display: block !important;
    }
      &>th{
          background: #e8f6ff;
          color:#212529;
          font-size: 14px;
      }
  }
  td{
    font-size: 14px;
  }
  &.auto{
    display: flex;
    flex-direction: column;
    .el-table {
      flex:1;
      height: 100%;
      display: flex;
      flex-direction: column;
      .el-table__body-wrapper{
        flex:1;
        overflow-y: auto;
      }
    }
  }
}
.flex-body{
  width: 100vw;
  height: 100vh;
  margin: 0;
  padding: 0;
}
.flex-box{
    display: flex;
    &.center{
      justify-content: center;
      align-items: center;
    }
    &.space-between{
      justify-content: space-between;
    }
    &.column{
      flex-direction: column;
    }
    .flex1{
        flex:1;
        overflow: hidden;
    }
}
.query{
  display: flex;
  flex-wrap: wrap;
  font-size: 14px;
  &.left{
      display: block;
      &:after{
        content: '';
        display: block;
        height: 0;
        width: 0;
        clear: both;
      }
      &>div{
          float: left;
          margin: 5px 0;
      }
      .condition{
          width: 25%;
      }
  }
  &>div{
    margin: 5px 0;
  }
  .condition{
      flex: 1;
      display: flex;
      &-tit{
          line-height: 40px;
          margin-right: 10px;
      }
      &-con{
          flex: 1;
          .w-input{
              width: 80%;
          }
      }
  }
}

.right-box .rightBtn {
  margin-top: 19px;
  margin-right: 15px; 
}
.empty-ico {
  i {
    background: url('/picture/empty.png') no-repeat;
    background-size: 100% 100%;
    width: 60px;
    height: 60px;
    margin: 50px auto 0;
    display: block;
  }
  span{
    display: block;
    line-height: 30px;
  }
}
</style>
