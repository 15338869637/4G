import rxios from '@/servers/rxios'

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

// 临时卡列表
export const GetTempCardList = (data) => {
  return rxios('formData', '/CardService/GetTempCardList', data)
}

// 月卡列表
export const GetMonthCardList = (data) => {
  return rxios('formData', '/CardService/GetMonthCardList', data)
}

// 储值卡列表
export const GetValueCardList = (data) => {
  return rxios('formData', '/CardService/GetValueCardList', data)
}

export const GetDriveWayList = (data) => {
  return rxios('GET', '/ParkLot/GetDriveWayList', data)
}

// 车类
export const GetCarTypeList = async function (data) {
  let res = await rxios('GET', '/ParkLot/GetCarTypeList', data)
  res = res.filter(item => item.Enable)
  return Promise.resolve(res)
}
