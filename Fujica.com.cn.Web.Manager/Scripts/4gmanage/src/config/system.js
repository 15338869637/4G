export const inSystemVoicTable = [
  { val: '入口空闲', type: 0 },
  { val: '临时车入场识别', type: 2 },
  { val: '月卡车入场识别', type: 6 },
  { val: '月卡车入场识别提醒延期', type: 7 },
  { val: '已过期月卡车入场识别', type: 8 },
  { val: '储值车入场识别', type: 11 },
  { val: '储值车入场识别提醒充值', type: 12 }
]

export const inSysArr = [0, 2, 6, 7, 8, 11, 12]
export const outSysArr = [1, 3, 4, 5, 9, 10, 13, 14, 15]
export const outSystemVoicTable = [
  { val: '出口空闲', type: 1 },
  { val: '未缴费临时车出场识别', type: 3 },
  { val: '已缴费临时车出场识别', type: 4 },
  { val: '已缴费临时车离场超时出场识别', type: 5 },
  { val: '月卡车出场识别', type: 9 },
  { val: '月卡车出场识别提醒延期', type: 10 },
  { val: '储值车出场识别', type: 13 },
  { val: '储值车出场识别提醒充值', type: 14 },
  { val: '储值车出场余额不足', type: 15 }
]

export const outSytemVoic = outSystemVoicTable.map((o, i) => {
  const base = {
    name: o.val,
    commandType: o.type,
    showVoice: '',
    showText: ''
  }
  if (i !== 0) {
    return { ...base, showText0: '', showText1: '' }
  } else {
    return base
  }
})

export const inSytemVoic = inSystemVoicTable.map((o, i) => {
  const base = {
    index: i,
    name: o,
    commandType: 0,
    showVoice: '',
    showText: ''
  }
  if (i !== 0) {
    return { ...base, showText0: '', showText1: '' }
  } else {
    return base
  }
})

export function getCookie (name) {
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
