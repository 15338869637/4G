import rxios from '@/servers/rxios'

/*
api1 报表
api2
*/
let testUrl1 = process.env.NODE_ENV === 'development' ? '/Report' : '/Report'
// 查询停车记录
export const SearchParkingRecord = (data) => {
  // if (process.env.NODE_ENV === 'development') {
  //   return Promise.resolve(require('./data/SearchParkingRecord.json'))
  // }
  return rxios('formData', testUrl1 + '/SearchParkingRecord', data)
}

// 查询缴费记录
export const SearchPaymentRecord = (data) => {
  // if (process.env.NODE_ENV === 'development') {
  //   return Promise.resolve(require('./data/SearchPaymentRecord.json'))
  // }
  return rxios('formData', testUrl1 + '/SearchPaymentRecord', data)
}

// 查询充值记录
export const SearchRechargeRecord = (data) => {
//   if (process.env.NODE_ENV === 'development') {
//     return Promise.resolve(require('./data/SearchRechargeRecord.json'))
//   }
  return rxios('formData', testUrl1 + '/SearchRechargeRecord', data)
}

// 查询企业付款记录
export const SearchEnterpriseRecord = (data) => {
  return rxios('formData', testUrl1 + '/SearchEnterpriseRecord', data)
}

// 查询储值卡扣费记录
export const SearchConsumeRecord = (data) => {
  // if (process.env.NODE_ENV === 'development') {
  //   return Promise.resolve(require('./data/SearchConsumeRecord.json'))
  // }
  return rxios('formData', testUrl1 + '/SearchConsumeRecord', data)
}

// 查询异常开闸信息
export const SearchOpenGateRecord = (data) => {
  return rxios('formData', testUrl1 + '/SearchOpenGateRecord', data)
}

// 查询车辆在场记录信息
export const SearchPresentRecord = (data) => {
  return rxios('formData', testUrl1 + '/SearchPresentRecord', data)
}

export const GetParkLotList = (data) => {
  return rxios('GET', '/ParkLot/GetParkLotList', data)
}

export const GetRolePermission = async function (data) {
  let res = await rxios('GET', '/Personnel/GetRolePermission', data)
  if (res.ParkingCodeList) {
    res.ParkingCodeList = [...(new Set(res.ParkingCodeList))]
  }
  return Promise.resolve(res)
}

// 车类
export const GetCarTypeList = async function (data) {
  let res = await rxios('GET', '/ParkLot/GetCarTypeList', data)
  res = res.filter(item => item.Enable)
  return Promise.resolve(res)
}

// 值班人员列表
export const GetUserList = (data) => {
  return rxios('GET', '/Personnel/GetUserList', data)
}

// 补录报表数据(报表)
export const SearchRecordInRecord = (data) => {
  return rxios('formData', testUrl1 + '/SearchRecordInRecord', data)
}

// 车牌修正报表数据(报表)
export const SearchCorrectCarnoRecord = (data) => {
  return rxios('formData', testUrl1 + '/SearchCorrectCarnoRecord', data)
}
