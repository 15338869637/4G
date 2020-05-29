/* 自定义钩子 */
import Vue from 'vue'

// 查看图片
Vue.directive('img', {
  bind: function (el, binding, vnode) {
    vnode.elm.onclick = function () {
      var body = document.querySelector('body')
      var dom = document.createElement('div')
      dom.className = 'flex-box center'
      dom.style = 'width: 100vw;height:100vh;position: fixed;top: 0;left:0;z-index:2;background:rgba(0, 0, 0, 0.34);'
      dom.onclick = function (e) {
        if (e.target === this) {
          body.removeChild(this)
        }
      }
      var img = document.createElement('img')
      img.src = vnode.elm.src
      img.style = 'max-width: 60%;height: auto'
      dom.appendChild(img)
      body.appendChild(dom)
    }
  }
})
