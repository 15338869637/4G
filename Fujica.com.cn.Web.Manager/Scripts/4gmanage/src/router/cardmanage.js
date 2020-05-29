import SetCard from '../page/CardManage/SetCard'
import MonthCard from '../page/CardManage/MonthCardNew'
import ValueCard from '../page/CardManage/ValueCardNew'
import MonthOperate from '../page/CardManage/MonthOperate'
import ValueOperate from '../page/CardManage/ValueOperate'
import TemporaryCarList from '../page/CardManage/TemporaryCarList'
import TemporaryCarOperate from '../page/CardManage/TemporaryCarOperate'

export default [
  {
    path: '/SetCard',
    name: 'SetCard',
    component: SetCard,
    pageType: 'cardmanage0'
  },
  {
    path: '/MonthCard',
    name: 'MonthCard',
    component: MonthCard,
    pageType: 'cardmanage1'
  },
  {
    path: '/ValueCard',
    name: 'ValueCard',
    component: ValueCard,
    pageType: 'cardmanage2'
  },
  {
    path: '/MonthOperate',
    name: 'MonthOperate',
    component: MonthOperate,
    pageType: 'cardmanage3'
  },
  {
    path: '/ValueOperate',
    name: 'ValueOperate',
    component: ValueOperate,
    pageType: 'cardmanage4'
  },
  {
    path: '/TemporaryCarList',
    name: 'TemporaryCarList',
    component: TemporaryCarList
  },
  {
    path: '/TemporaryCarOperate',
    name: 'TemporaryCarOperate',
    component: TemporaryCarOperate
  }
]
