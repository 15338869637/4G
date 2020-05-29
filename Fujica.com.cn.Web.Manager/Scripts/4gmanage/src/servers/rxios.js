import Vue from 'vue'
import axios from 'axios'
import { Loading, Message, MessageBox } from 'element-ui'
import qs from 'querystring'
// 请求超时
axios.defaults.timeout = 20000
let loadinginstace
let loadinginstaceLength = 0
// 添加请求拦截器
axios.interceptors.request.use(
  config => {
    !loadinginstaceLength && (loadinginstace = Loading.service({ fullscreen: true }))
    loadinginstaceLength++
    return config
  },
  error => {
    setTimeout(() => {
      loadinginstaceLength--
      if (loadinginstaceLength < 0) loadinginstaceLength = 0
      !loadinginstaceLength && loadinginstace.close()
    }, 300)
    Message.error({
      message: '请求超时'
    })
    return Promise.reject(error)
  }
)
// 添加响应拦截器
axios.interceptors.response.use(
  response => {
    setTimeout(() => {
      loadinginstaceLength--
      if (loadinginstaceLength < 0) loadinginstaceLength = 0
      !loadinginstaceLength && loadinginstace.close()
    }, 300)
    const { data, config } = response
    // console.log(
    //   '%c%s',
    //   'color: red; font-size: 14px;',
    //   `<<<===rxios请求返回信息===>>>  ${config.url} 丨丨start......`
    // )
    // console.log('请求参数===>>>', config.data ? JSON.parse(config.data) : {})
    // console.log('返回信息===>>>', data.Result)
    // console.log(
    //   '%c%s',
    //   'color: red; font-size: 14px;',
    //   `<<<===rxios请求返回信息===>>>  ${config.url} 丨丨end......`
    // )
    if (response.data.IsSuccess) {
      if (typeof response.data.TotalCount !== 'undefined') {
        return response.data
      }
      return response.data.Result || response.data
    } else {
      if (response.data.Redirect === 'Index') {
        MessageBox.confirm('登录超时, 是否跳转到登录页?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          if (self != top) {
            parent.location.href = '/Home/Index'
          }
        }).catch(() => {
          // 取消
        })
        return
      }
      Message.error({
        message: response.data.MessageContent
      })
    }
    return response.data
  },
  error => {
    setTimeout(() => {
      loadinginstaceLength--
      if (loadinginstaceLength < 0) loadinginstaceLength = 0
      !loadinginstaceLength && loadinginstace.close()
    }, 300)
    Message.error({
      message: '请求失败'
    })
    return Promise.reject(error)
  }
)

const getCookie = name => {
  let arr

  let reg = new RegExp('(^| )' + name + '=([^;]*)(;|$)')
  arr = document.cookie.match(reg)
  const reg3D = /%3D/
  if (arr) {
    if (reg3D.test(arr[2])) {
      return decodeURIComponent(arr[2])
    }
    return arr[2]
  } else {
    return null
  }
}

let ProjectGuidValue = getCookie('ProjectGuid')

console.log(ProjectGuidValue, 999)

export default (type, baseUrl, data = { ProjectGuid: ProjectGuidValue }) => {
  let url = process.env.NODE_ENV === 'development' && baseUrl.indexOf('/api') === 0 ? baseUrl : '/Home/Maps' + baseUrl
  const qsData = qs.stringify(data)

  const opt = {
    method: 'get',
    url
  }
  if (type === 'GET') {
    if (data && Object.keys(data).length > 0) {
      Object.assign(opt, {
        url: `${url}?${qsData}`
      })
    }
  } else if (type === 'POST') {
    Object.assign(opt, {
      method: 'post',
      data
    })
  } else if (type === 'PUT') {
    let formdata = new FormData()
    for (const [key, val] of Object.entries(data)) {
      formdata.append(key, val)
    }
    Object.assign(opt, {
      headers: {
        'Content-Type': 'multipart/form-data' // 之前说的以表单传数据的格式来传递fromdata
      },
      method: 'put',
      data: formdata
    })
  } else if (type === 'formData') {
    let formdata = new FormData()
    for (const [key, val] of Object.entries(data)) {
      formdata.append(key, val)
    }
    Object.assign(opt, {
      headers: {
        'Content-Type': 'multipart/form-data' // 之前说的以表单传数据的格式来传递fromdata
      },
      method: 'post',
      data: formdata
    })
  }
  return axios(opt)
}
