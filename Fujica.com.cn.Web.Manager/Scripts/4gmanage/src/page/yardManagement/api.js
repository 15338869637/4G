import rxios from '@/servers/rxios'
// 查询充值记录
export const SearchPresentRecord = (data) => {
//   if (process.env.NODE_ENV === 'development') {
//     return Promise.resolve(require('./data/SearchPresentRecord.json'))
//   }
  return rxios('formData', '/Report/SearchPresentRecord', data)
}

// 获取停车场列表
export const GetParkLotList = (data) => {
  return rxios('GET', '/ParkLot/GetParkLotList', data)
}

// 获取权限
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

// 车道
export const GetDriveWayList = async function (data) {
  let res = await rxios('GET', '/ParkLot/GetDriveWayList', data)
  return Promise.resolve(res)
}

// 车牌修正
export const CorrectCarNo = async function (data) {
  let res = await rxios('formData', '/ParkLot/CorrectCarNo', data)
  return Promise.resolve(res)
}

// 补录入场数据
export const AddInRecord = async function (data) {
  let res = await rxios('formData', '/ParkLot/AddInRecord', data)
  return Promise.resolve(res)
}

// 删除
export const InVehicleDelete = function (data) {
  return rxios('formData', '/ParkLot/InVehicleDelete', data)
}
