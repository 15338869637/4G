// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import store from './store'
import elementUI from './config/elementUi'
import router from './router'
import 'element-ui/lib/theme-chalk/display.css'
import rxios from '@/servers/rxios.js'
import './config/directive'
Vue.config.productionTip = false
Vue.prototype.rxios = rxios
Vue.prototype.Upperlimit = 12000 // 导出上限
elementUI(Vue)
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  components: { App },
  template: '<App/>'
})
