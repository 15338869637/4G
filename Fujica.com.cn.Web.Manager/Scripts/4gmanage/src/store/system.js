import Rxios from '../servers/rxios'
import { getCookie } from '@/config/system.js'

import {
  flow as _flow,
  find as _find,
  get as _get,
  map as _map
} from 'lodash'

const PROJECTGUID = getCookie('ProjectGuid')
const UserGuid = getCookie('UserGuid')

export default {
  namespaced: true,
  state: {
    ParkLotList: [],
    SelectWay: {},
    /** 手动开闸 */
    OpenGateReasonList: [],
    OpenGate: {},
    /** 通行设置 */
    TrafficAddList: [],
    TrafficAdd: {},
    Base: {
      DriveWayList: [],
      CarTypeList: [],
      ParkLotList: []
    },
    ParkNow: ''
  },
  actions: {
    /** 语音指令 */
    async FetchPark ({ commit }) {
      // 获取 停车场
      // const ParkLot = await Rxios('GET', '/ParkLot/GetParkLotList', { ProjectGuid: PROJECTGUID })

      let roleGuid = getCookie('RoleGuid')
      const resListAll = await Rxios('GET', '/ParkLot/GetParkLotList')
      const resListAllfilter = resListAll.filter(item => item.Existence == true) // 保留existence为true的
      const resList = await Rxios('GET', '/Personnel/GetRolePermission', {
        Guid: roleGuid
      })
      const arr = []
      for (var i = 0; i < resListAllfilter.length; i++) {
        for (var j = 0; j < resList.ParkingCodeList.length; j++) {
          if (resListAllfilter[i].parkCode == resList.ParkingCodeList[j]) {
            arr.push({
              value: resListAllfilter[i].ParkCode,
              label: resListAllfilter[i].ParkName
            })
          }
        }
      }
      this.ParkOpt = arr

      // 获取车道
      // const DriveWay = ParkLot.map(o => Rxios('GET', '/ParkLot/GetDriveWayList', { ParkingCode: o.parkCode }))
      // const DriveWayRes = await Promise.all(DriveWay)
      // const ParkLotList = ParkLot.map(log => {
      //   DriveWayRes.forEach(way => {
      //     if (way[0] && log.parkCode === way[0].parkCode) {
      //       log.DriveWayLists = way
      //     }
      //   })
      //   return log
      // })
      commit('SET_DATA', { name: 'ParkLotList', val: this.ParkLot })
    },
    async WaySelect ({ commit }, guid) {
      const VoiceCommand = await Rxios('GET', '/SystemSet/GetVoiceCommand', { DrivewayGuid: guid })
      if (VoiceCommand.ProjectGuid) {
        commit('SET_DATA', { name: 'SelectWay', val: VoiceCommand })
      }
    },
    // 保存语音指令的数据
    async SystemSet ({ commit, state }, tableData) {
      console.log(tableData, '接口数据0')

      const Send = tableData.map(o => {
        if (o.ShowText0 && o.ShowText1) {
          o.ShowText = o.ShowText0 + '\r\n' + o.ShowText1
        }
        const { CommandType, ShowVoice, ShowText } = o
        return {
          CommandType,
          ShowVoice,
          ShowText
        }
      })

      const tableDataList = JSON.parse(JSON.stringify(tableData))

      console.log(tableDataList, '接口数据1')

      const SendChange = tableDataList.map(o => {
        // 这里需要判断  是否要拼接传值！！！！ ShowText1 ShowText2是否有值！！！
        if (o.ShowText && o.ShowText1 && o.ShowText2) {
          o.ShowText = o.ShowText0 + '\r\n' + o.ShowText1 + '\r\n' + o.ShowText2
        } else if (o.ShowText && o.ShowText1) {
          o.ShowText = o.ShowText0 + '\r\n' + o.ShowText1
        } else {
          o.ShowText = o.ShowText0
        }

        o.ShowText = encodeURI(o.ShowText)
        o.ShowVoice = encodeURI(o.ShowVoice)

        const { CommandType, ShowVoice, ShowText } = o
        return {
          CommandType,
          ShowVoice,
          ShowText
        }
      })

      console.log(SendChange, '接口数据2')

      const finSend = state.SelectWay.CommandList.map(o => {
        const oway = Send.find(a => o.CommandType === a.CommandType)
        if (oway) {
          return { ...o, ...oway }
        } else {
          return o
        }
      })

      const { DrivewayGuid, ParkCode, ProjectGuid } = state.SelectWay

      console.log(state.SelectWay, 1233444)

      const VoiceCommand = await Rxios('formData', '/SystemSet/SetVoiceCommand', {
        DrivewayGuid: DrivewayGuid,
        ParkingCode: ParkCode,
        ProjectGuid: ProjectGuid,
        CommandList: JSON.stringify(SendChange)
      })

      if (VoiceCommand.IsSuccess) {
        return 1
      }
    },
    /** 通行设置 */
    // 获取受控车道
    async GetBaseData ({ commit, state }) {
      let { Base: { DriveWayList, CarTypeList, ParkLotList }, ParkNow } = state

      DriveWayList = await Rxios('GET', '/ParkLot/GetDriveWayList', { ParkingCode: ParkNow })
      CarTypeList = await Rxios('GET', '/ParkLot/GetCarTypeList', { ParkingCode: ParkNow, projectGuid: PROJECTGUID })
      ParkLotList = await Rxios('GET', '/ParkLot/GetParkLotList', { ProjectGuid: PROJECTGUID })
      commit('SET_DATA', { name: 'Base', val: { DriveWayList, CarTypeList, ParkLotList } })
      commit('SET_PARK', { newVal: ParkNow })
    },
    // 获取列表
    async GetTrafficRestrictionList ({ commit, dispatch, state }) {
      // await dispatch('GetBaseData')

      const list = await Rxios('GET', '/SystemSet/GetTrafficRestrictionList', { ProjectGuid: PROJECTGUID })
      const { DriveWayList, CarTypeList, ParkLotList } = state.Base
      const TrafficAddList = list.map(o => {
        const ParkCode = _flow([o => _find(ParkLotList, ['ParkCode', o.ParkCode]), o => _get(o, 'ParkName')])
        const GetList = (list, type, name) => _flow([
          o => _get(o, type),
          o => _map(o, d => _find(list, ['Guid', d])),
          o => _map(o, name)
        ])
        const Week = _flow([
          o => _get(o, 'AssignDays'),
          o => o.split(''),
          o => o.reduce((list, w, i) => {
            const group = ['一', '二', '三', '四', '五', '六', '日']
            if (w == 1) {
              list.push(`周${group[i]}`)
            }
            return list
          }, [])
        ])
        o.ParkName = ParkCode(o)
        // o.DriveWay = GetList(DriveWayList, 'drivewayGuid', 'deviceName')(o)
        // o.CarType = GetList(CarTypeList, 'carTypeGuid', 'carTypeName')(o)
        o.Week = Week(o)
        return o
      })

      commit('SET_DATA', { name: 'TrafficAddList', val: TrafficAddList })
    },
    async GetAddNewTrafficRestriction ({ commit, state }, vod) {
      const { Guid, DrivewayGuid, CarTypeGuid, ParkCode, AssignDays, StartTime, EndTime } = vod
      const Week = _flow([
        o => o.split(''),
        o => o.reduce((list, w, i) => {
          if (w == 1) {
            list.push(i)
          }
          return list
        }, [])
      ])
      const TrafficAdd = {
        Guid: Guid,
        ParkingCode: ParkCode,
        BoxSet: [Week(AssignDays), DrivewayGuid, CarTypeGuid],
        StartTime: StartTime,
        EndTime: EndTime,
        HaveTime: !!(StartTime && EndTime)
      }
      commit('SET_DATA', { name: 'TrafficAdd', val: TrafficAdd })
    },
    // 添加
    async AddNewTrafficRestriction ({ commit, state }, send) {
      const { Guid } = state.TrafficAdd
      let res = {}
      if (Guid) {
        res = await Rxios('formData', '/SystemSet/ModifyTrafficRestriction', { ...send, Guid, ProjectGuid: PROJECTGUID })
      } else {
        res = await Rxios('formData', '/SystemSet/AddNewTrafficRestriction', { ...send, ProjectGuid: PROJECTGUID })
      }
      return res
    },
    // 删除
    async RemoveTrafficRestriction ({ commit, state }, send) {
      const { Guid, DrivewayGuid, CarTypeGuid, AssignDays, StartTime, EndTime } = send
      const ParkingCode = send.ParkCode

      const res = await Rxios('formData', '/SystemSet/RemoveTrafficRestriction',
        {
          Guid,
          DrivewayGuid,
          CarTypeGuid,
          AssignDays,
          StartTime,
          EndTime,
          ParkingCode: ParkingCode,
          ProjectGuid: PROJECTGUID })
      if (res.IsSuccess) {
        const list = state.TrafficAddList.filter(o => o.guid !== send.Guid)
        commit('SET_DATA', { name: 'TrafficAddList', val: list })
      }
    },
    /** 手动开闸 */
    // 列表
    async GetOpenGateReasonList ({ commit }) {
      const list = await Rxios('GET', '/SystemSet/GetOpenGateReasonList', { ProjectGuid: PROJECTGUID })
      commit('SET_DATA', { name: 'OpenGateReasonList', val: list })
    },
    //  新增
    async AddNewOpenGateReason ({ commit }, send) {
      const res = await Rxios('formData', '/SystemSet/AddNewOpenGateReason', { ...send, ProjectGuid: PROJECTGUID })
      return res
    },
    // 修改
    async ModifyOpenGateReason ({ commit }, send) {
      const res = await Rxios('formData', '/SystemSet/ModifyOpenGateReason', { ...send, ProjectGuid: PROJECTGUID })
      commit('SET_DATA', { name: 'OpenGate', val: {} })
      return res
    },
    // 删除
    async DeleteOpenGateReason ({ commit, state }, send) {
      const res = await Rxios('formData', '/SystemSet/RemoveOpenGateReason', { Guid: send.guid, ProjectGuid: PROJECTGUID })
      if (res.IsSuccess) {
        const list = state.OpenGateReasonList.filter(o => o.guid !== send.guid)
        commit('SET_DATA', { name: 'OpenGateReasonList', val: list })
      }
    },
    /** *********修改密码 */
    async ModifyPasssword ({ commit, state }, send) {
      const res = await Rxios('PUT', '/Personnel/ModifyPasssword', { Guid: UserGuid, ProjectGuid: PROJECTGUID, ...send })
      return res.IsSuccess
    }
  },
  mutations: {
    SET_DATA (state, { name, val }) {
      state[name] = val
    },
    SET_PARK (state, { newVal }) {
      state.ParkNow = newVal
    }
  },
  getters: {}
}
