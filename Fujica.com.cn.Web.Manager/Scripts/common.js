// 消息提示
function $message (type, html, time = 3000, width = 400) {
  if (self != top) {
    parent.window.$message(type, html, time, width)
    return
  }
  var className = ''
  var id = 'id' + Math.floor(Math.random() * 10000)
  switch (type) {
    case 's': // success
      className = 'success'
      break
    case 'i': // info
      className = 'info'
      break
    case 'w': // warning
      className = 'warning'
      break
    case 'd': // danger
      className = 'danger'
      break
  }
  dom = $('<div id="' + id + '" ' +
  'style="position: absolute;min-width: ' + width + 'px; top: 15%; left: 50%; margin-left: -' + (width / 2) + 'px;" ' +
  'class="alert alert-' + className + '">' +
        '<a href="#" class="close" data-dismiss="alert" onclick="$(\'#' + id + '\').remove()">&times;</a>' +
        html +
    '</div>')
  $('body').append(dom)
  setTimeout(function () {
    var d = $('#' + id)
    d.length && d.remove()
  }, time)
}

// 确认弹框
function $alert (obj) {
  if (self != top) {
    parent.window.$alert(obj)
    return
  }
  var id = 'id' + Math.floor(Math.random() * 10000)
  var config = { // 默认配置 obj传对应字段覆盖默认
    title: '温馨提示', // 弹框标题
    html: '', // html内容
    width: 400, // 宽度，高度按内容自动
    cancelName: '取消', // 取消按钮名称
    confirmName: '确认' // 确认按钮名称
  }
  Object.assign(config, obj)
  config.cancel = function () { // 取消回调
    obj.cancel && obj.cancel()
    $('#' + id).remove()
  }
  config.confirm = function () { // 确认回调
    obj.confirm && obj.confirm()
    $('#' + id).remove()
  }
  dom = $('<div id="' + id + '" style="position: absolute;top: 0;left: 0;width: 100vw;height: 100vh;">' +
'<div class="modal-backdrop" style="opacity: 0.5;"></div>' +
  '<div class="modal-dialog" style="z-index: 1041;width: ' + config.width + 'px;left: 0; margin-top:20%;position: relative;">' +
      '<div class="modal-content">' +
          '<div class="modal-header">' +
            '<h4 class="modal-title" style="flex:1">' +
            config.title +
            '</h4>' +
              (config.cancelName ? '<button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="$(\'#' + id + '\').remove()">×' +
              '</button>' : '') +
          '</div>' +
          '<div class="modal-body">' +
            config.html +
          '</div>' +
          '<div class="modal-footer">' +
              (config.cancelName ? '<button type="button" class="btn btn-default" data-dismiss="modal">' +
              config.cancelName +
              '</button>' : '') +
              '<button type="button" class="btn btn-primary">' +
              config.confirmName +
              '</button>' +
          '</div>' +
      '</div>' +
  '</div>' +
'</div>' +
'</div>')
  $('body').append(dom)
  $('#' + id + ' .btn-primary').click(config.confirm)
  config.cancelName && $('#' + id + ' .btn-default').click(config.cancel)
}

/*
  loading加载图标
  id 元素id 可不传 （不传的时候多个loading无序关闭）
  text loading提示语
  关闭调用 $loading_hide
 */
var loading_obj = {}
function $loading_show (id, text) {
  console.log('loading_show')
  if (self != top) {
    parent.window.$loading_show(id, text)
    return
  }
  var id = id || 'loading' + Math.floor(Math.random() * 10000)
  var dom = $('<div id="' + id + '" style="position: fixed; top: 0px; left: 0px; width: 100%; height: 100%; background: rgb(76, 76, 76); opacity: 0.6; z-index: 1000; display: block;">' +
        '<div id="' + id + '_gif" style="text-align:center;position:fixed;z-index: 9999;top:50%;left: 50%;">' +
            '<div class="sk-spinner sk-spinner-cube-grid" style="text-align:center;color:#fff">' +
                '<img src="/picture/loader.gif" style="height:50px">' +
                (text ? '<br />' + text : '') +
            '</div>' +
        '</div>' +
    '</div>')
  $('body').append(dom)
  loading_obj[id] = function () {
    // screen
    $('#' + id).fadeOut(600)
    setTimeout(function () {
      $('#' + id).remove()
    }, 1000)
    delete (loading_obj[id])
  }
}

function $loading_hide (id) {
  if (self != top) {
    parent.window.$loading_hide(id)
    return
  }
  if (id) {
    loading_obj[id] && loading_obj[id]()
  } else {
    for (var a in loading_obj) {
      loading_obj[a]()
      return
    }
  }
}

// 登录超时全局拦截
$.ajaxSetup({
  complete: function (XMLHttpRequest, textStatus) {
    if (XMLHttpRequest.status === 200 &&
      typeof XMLHttpRequest.responseJSON === 'object' &&
      XMLHttpRequest.responseJSON.Redirect &&
      XMLHttpRequest.responseJSON.Redirect === 'Index') {
      $alert({
        html: '登陆超时,是否跳转登录页！',
        confirm: function () {
          if (self != top) {
            parent.location.href = '/Home/Index'
          }
        }
      })
    }
  }
})

// 权限跳转
$(function () {
  var role = JSON.parse(localStorage.getItem('role') || '{"arr":[]}')
  role.arr && (role.arr = role.arr.concat(['/Home/Trans/layout', '/Home/Index', '/Home/Index/', '/']))
  var currUrl = location.pathname
  if (currUrl == '/Home/Trans/VueSinglePage') {
    currUrl += location.hash.split('?')[0]
  }
  if (role.all && role.all[currUrl]) {
    if (role.arr && currUrl !== '/' && role.arr.indexOf(currUrl) == -1) {
      $alert({
        html: '您没有权限进入此页面。',
        cancelName: false,
        confirm: function () {
          location.href = role.arr[0]
        }
      })
    }
  }
})
