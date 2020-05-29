// 临停车缴费记录
import temporary_parking from '../page/reportForm/temporary_parking'
// 月卡车缴费记录
import monthly_truck from '../page/reportForm/monthly_truck'
// 储值车充值记录
import storage_car_recharge from '../page/reportForm/storage_car_recharge'
// 储值车消费记录
import storage_ehicle_onsumption from '../page/reportForm/storage_ehicle_onsumption'
// 车辆出场记录
import vehicle_exit from '../page/reportForm/vehicle_exit'
// 异常开闸记录
import abnormal_opening from '../page/reportForm/abnormal_opening'
// 车牌修正记录
import license_plate_amendment from '../page/reportForm/license_plate_amendment'
// 入场补录记录
import admission_supplement from '../page/reportForm/admission_supplement'

export default [
  { // 临停车缴费记录
    path: '/temporary_parking',
    name: 'temporary_parking',
    component: temporary_parking
  },
  { // 月卡车缴费记录
    path: '/monthly_truck',
    name: 'monthly_truck',
    component: monthly_truck
  },
  { // 储值车充值记录
    path: '/storage_car_recharge',
    name: 'storage_car_recharge',
    component: storage_car_recharge
  },
  { // 储值车消费记录
    path: '/storage_ehicle_onsumption',
    name: 'storage_ehicle_onsumption',
    component: storage_ehicle_onsumption
  },
  { // 车辆出场记录
    path: '/vehicle_exit',
    name: 'vehicle_exit',
    component: vehicle_exit
  },
  { // 异常开闸记录
    path: '/abnormal_opening',
    name: 'abnormal_opening',
    component: abnormal_opening
  },
  { // 车牌修正记录
    path: '/license_plate_amendment',
    name: 'license_plate_amendment',
    component: license_plate_amendment
  },
  { // 入场补录记录
    path: '/admission_supplement',
    name: 'admission_supplement',
    component: admission_supplement
  }
]
