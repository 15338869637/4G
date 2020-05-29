import OpenDoor from '../page/System/OpenDoor'
import OpenDoorAdd from '../page/System/OpenDoorAdd'
import PassWord from '../page/System/PassWord'
import Through from '../page/System/Through'
import ThroughAdd from '../page/System/ThroughAdd'
import Voice from '../page/System/Voice'
export default [
  {
    path: '/OpenDoor',
    name: 'OpenDoor',
    component: OpenDoor,
    pageType: 'system2'
  },
  {
    path: '/OpenDoorAdd',
    name: 'OpenDoorAdd',
    component: OpenDoorAdd
  },
  {
    path: '/PassWord',
    name: 'PassWord',
    component: PassWord,
    pageType: 'system3'
  },
  {
    path: '/Through',
    name: 'Through',
    component: Through,
    pageType: 'system1'
  },
  {
    path: '/ThroughAdd',
    name: 'ThroughAdd',
    component: ThroughAdd
  },
  {
    path: '/Voice',
    name: 'Voice',
    component: Voice,
    pageType: 'system0'
  }
]
