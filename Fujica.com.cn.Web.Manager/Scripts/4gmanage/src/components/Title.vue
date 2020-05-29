<template>
  <div class="title-box">
     <span></span>
     {{titleMsg}}
     <div class="right-box">
       <slot name='right'></slot>
     </div>
  </div>
</template>

<script>
export default {
  name: '',
  props: {
    titleMsg: String
  },
  data () {
    return {
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang='less'>
.title-box{
  height: 74px;
  line-height: 74px;
  font-size: 22px;
  color: #333C48;
  font-weight: bold;
  border-bottom:1px solid #E6EAEE;
  >span{
    height: 36px;
    width: 6px;
    margin-top: 19px;
    margin-left: 15px;
    margin-right: 15px;
    float: left;
    display: inline-block;
    background: -webkit-linear-gradient(#00C6FF, #0072FF); /* Safari 5.1 - 6.0 */
    background: -o-linear-gradient(#00C6FF, #0072FF); /* Opera 11.1 - 12.0 */
    background: -moz-linear-gradient(#00C6FF, #0072FF); /* Firefox 3.6 - 15 */
    background: linear-gradient(#00C6FF, #0072FF); /* 标准的语法（必须放在最后） */
  }
  .right-box{
    float:right;
    height: 100%;
    line-height: 75px;
  }
}
</style>
