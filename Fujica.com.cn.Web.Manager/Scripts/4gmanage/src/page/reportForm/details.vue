<template>
  <el-dialog :visible.sync="dialog.show" width="1280px" top="20px">
    <span slot="title" class="details-title">
        {{dialog.title}}
    </span>
    <div class="flex-box space-between rc">
        <div class="box flex1 m_r10">
            <div class="title">入场记录信息</div>
            <div class="content flex-box space-between">
                <div class="img flex1">
                    <img :src="dialog.data[dialog.rcImg]" style="max-width: 100%; height: 220px" />
                </div>
                <div class="information flex-box column">
                    <p v-for="(item,i) in dialog.rc" :key="'rc'+i"><span class="information-title">{{item.title}}</span>{{dialog.data[item.content]}}</p>
                </div>
            </div>
        </div>
        <div class="box flex1 cc">
            <div class="title">出场记录信息</div>
            <div class="content flex-box space-between">
                <div class="img flex1">
                    <img :src="dialog.data[dialog.ccImg]" style="max-width: 100%; height: 220px" />
                </div>
                <div class="information flex-box column">
                    <p v-for="(item,i) in dialog.cc" :key="'cc'+i"><span class="information-title">{{item.title}}</span>{{dialog.data[item.content]}}</p>
                </div>
            </div>
        </div>
    </div>
    <div class="flex-box space-between m_t10 sf">
        <div class="box" v-for="(item,i) in dialog.sf" :key="'sf'+i">
            <div class="title">
                {{item.title + (i+1)}}
            </div>
            <div class="content">
                <p v-for="(item1, i1) in item.data" :key="i1"><span class="information-title">{{item1.title}}</span>{{item1.content}}</p>
            </div>
        </div>
        <div class="box no-border" v-for="i in (3 - dialog.sf.length % 3)" :key="i"></div>
    </div>
  </el-dialog>
</template>

<script>
export default {
  data () {
    return {
    }
  },
  props: ['dialog']
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="less" scoped>
.img{
    text-align: center;
}
.details-title{
    position: relative;
    padding-left: 18px;
    font-size: 18px;
    font-weight: bold;
}
.details-title::before {
    content: '';
    display: block;
    height: 36px;
    width: 6px;
    // position: absolute;
    background: linear-gradient(#00C6FF, #0072FF);
    position: absolute;
    top: 50%;
    left: 0;
    margin-top: -18px;
}
.sf {
    flex-wrap: wrap;
    .box {
        width: 32%;
        margin-top: 10px;
    }
}
.box {
    // min-height: 300px;
    border: 1px solid #e6e6e6;
    background: #fff;
    &.no-border{
        border: 0;
    }
    .title {
        height: 38px;
        line-height: 38px;
        padding: 0px 10px;
        font-size: 14px;
        background:#e8f6ff;
        font-weight: bold;
        color: #2c3e50;
    }
    .content {
        .img {
            margin: 10px;
        }
        .information{
            padding: 10px 0;
            width: 200px;
            >p{
                margin: 0;
                flex: 1;
            }
        }
        >p{
            margin: 10px 0;
            padding: 0 10px;
        }
        .information-title{
            margin-right: 5px;
        }
    }
}
</style>
