import Vue from 'vue'
import Vuex from 'vuex'
import CardManage from './cardmanage'
import System from './system'
import { getCookie } from '@/config/system.js'
Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    ProjectGuid: getCookie('ProjectGuid'),
    UserName: getCookie('UserName'),
    UserGuid: getCookie('UserGuid')
  },
  actions: {

  },
  mutations: {

  },
  modules: {
    CardManage,
    System
  }
})
