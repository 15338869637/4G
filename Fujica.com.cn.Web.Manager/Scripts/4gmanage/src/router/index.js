import Vue from 'vue'
import Router from 'vue-router'
import HelloWorld from '@/components/HelloWorld'
import System from './system'
import CardManage from './cardmanage'
import reportForm from './reportForm'
import yardManagement from './yardManagement'
Vue.use(Router)

export const myRouter = [{
  path: '/',
  name: 'HelloWorld',
  component: HelloWorld
},
...CardManage,
...System,
...reportForm,
...yardManagement
]

export default new Router({
  routes: myRouter
})
