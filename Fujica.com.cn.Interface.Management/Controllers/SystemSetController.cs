using Fujica.com.cn.Business.ParkLot;
using Fujica.com.cn.Bussiness.Entity;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Interface.Management.Models.InPut;
using Fujica.com.cn.Interface.Management.Models.OutPut;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using FujicaService.Module.Entitys.Parking;
using FujicaService.Module.Entitys.ReportForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using static Fujica.com.cn.Context.Model.GatherAccountModel;

namespace Fujica.com.cn.Interface.Management.Controllers
{
    /// <summary>
    /// 系统设置业务模块
    /// </summary>
    public class SystemSetController : BaseController
    {
        /// <summary>
        /// 语音指令管理器
        /// </summary>
        private VoiceCommandManager _voiceCommandManager = null;
        /// <summary>
        /// 通行设置管理器
        /// </summary>
        private TrafficRestrictionManager _trafficRestrictionManager = null;
        /// <summary>
        /// 开闸管理器
        /// </summary>
        private OpenGateReasonManager _openGateReasonManager = null;
        /// <summary>
        /// 车场管理器
        /// </summary>
        private ParkLotManager _parkLotManager;

        /// <summary>
        /// 唯一构造函数
        /// </summary>
        /// <param name="logger">日志接口器</param>
        /// <param name="serializer">序列化接口器</param>
        /// <param name="apiaccesscontrol">api接入控制器</param>
        /// <param name="voiceCommandManager">语音指令管理器</param>
        /// <param name="trafficRestrictionManager">通行设置管理器</param>
        /// <param name="openGateReasonManager">开闸原因设置管理器</param>
        /// <param name="parkLotManager">车场管理器</param>
        public SystemSetController(ILogger logger, ISerializer serializer, APIAccessControl apiaccesscontrol, 
            VoiceCommandManager voiceCommandManager,
            TrafficRestrictionManager trafficRestrictionManager,
            OpenGateReasonManager openGateReasonManager, 
            ParkLotManager parkLotManager) :base(logger, serializer, apiaccesscontrol)
        {
            _voiceCommandManager = voiceCommandManager;
            _trafficRestrictionManager = trafficRestrictionManager;
            _openGateReasonManager = openGateReasonManager;
            _parkLotManager = parkLotManager;
        }
        
        #region 收款账户 转主平台支付，废弃
        ///// <summary>
        ///// 添加收款账户
        ///// </summary>
        ///// <returns></returns>
        //[ResponseType(typeof(ResponseBaseCommon))]
        //public IHttpActionResult AddNewGatherAccount(GatherAccountRequest model)
        //{
        //    ResponseBaseCommon response = new ResponseBaseCommon()
        //    {
        //        IsSuccess = true,
        //        MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
        //        MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
        //    };

        //    #region 阿里的账户
        //    AliPayAccountModel alipaycontent = new AliPayAccountModel()
        //    {
        //        appid=model.AliPayAppid,
        //        partner=model.AliPayPartner,
        //        seller=model.AliPaySeller,
        //        priverkey=model.AliPayPriverkey,
        //        publickey=model.AliPayPublickey,
        //        usev2=model.AliPayUseV2
        //    };
        //    #endregion

        //    #region 腾讯的账号
        //    WeChatAccountModel wechatcontent = new WeChatAccountModel()
        //    {
        //        appid=model.WeChatAppid,
        //        mchid=model.WeChatMchid,
        //        submchid=model.WeChatSubmchid,
        //        privatekey=model.WeChatPrivatekey,
        //        secert=model.WeChatSecert,
        //        usesubmch=model.WeChatUseSubmch
        //    };
        //    #endregion

        //    GatherAccountModel content = new GatherAccountModel()
        //    {
        //        projectGuid = model.ProjectGuid,
        //        guid = Guid.NewGuid().ToString("N"),
        //        accountName = model.AccountName,
        //        //paymentISV = new List<IBaseModel>() { alipaycontent, wechatcontent }
        //        alipayAccount=alipaycontent,
        //        wechatAccount=wechatcontent
        //    };

        //    bool flag = gatheraccountmanager.AddGatherAccount(content);
        //    if (!flag)
        //    {
        //        response.IsSuccess = false;
        //        response.MessageCode = (int)ApiSystemErrorCode.API_DATA_SAVE_ERROR;
        //        response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
        //    }
        //    return Ok(response);
        //}

        ///// <summary>
        ///// 修改收款账户
        ///// </summary>
        ///// <returns></returns>
        //[ResponseType(typeof(ResponseBaseCommon))]
        //public IHttpActionResult ModifyGatherAccount(ModifyGatherAccountRequest model)
        //{
        //    ResponseBaseCommon response = new ResponseBaseCommon()
        //    {
        //        IsSuccess = true,
        //        MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
        //        MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
        //    };

        //    GatherAccountModel content = gatheraccountmanager.GetGatherAccount(model.Guid);
        //    if (content != null)
        //    {
        //        if (content.projectGuid == model.ProjectGuid)
        //        {
        //            #region 阿里的账户
        //            AliPayAccountModel alipaycontent = gatheraccountmanager.GetAliPayAccount(model.Guid);
        //            if (alipaycontent != null)
        //            {
        //                alipaycontent.appid = model.AliPayAppid;
        //                alipaycontent.partner = model.AliPayPartner;
        //                alipaycontent.seller = model.AliPaySeller;
        //                alipaycontent.priverkey = model.AliPayPriverkey;
        //                alipaycontent.publickey = model.AliPayPublickey;
        //                alipaycontent.usev2 = model.AliPayUseV2;
        //            }
        //            #endregion

        //            #region 腾讯的账号
        //            WeChatAccountModel wechatcontent = gatheraccountmanager.GetWeChatAccount(model.Guid);
        //            if (wechatcontent != null)
        //            {
        //                wechatcontent.appid = model.WeChatAppid;
        //                wechatcontent.mchid = model.WeChatMchid;
        //                wechatcontent.submchid = model.WeChatSubmchid;
        //                wechatcontent.privatekey = model.WeChatPrivatekey;
        //                wechatcontent.secert = model.WeChatSecert;
        //                wechatcontent.usesubmch = model.WeChatUseSubmch;
        //            }
        //            #endregion

        //            content.guid = model.Guid;
        //            content.accountName = model.AccountName;
        //            //content.paymentISV = new List<IBaseModel>() { alipaycontent, wechatcontent };
        //            content.alipayAccount = alipaycontent;
        //            content.wechatAccount = wechatcontent;

        //            bool flag = gatheraccountmanager.ModifyGatherAccount(content);
        //            if (!flag)
        //            {
        //                response.IsSuccess = false;
        //                response.MessageCode = (int)ApiSystemErrorCode.API_DATA_SAVE_ERROR;
        //                response.MessageContent = ApiSystemErrorCode.API_DATA_SAVE_ERROR.ToString();
        //            }
        //        }else
        //        {
        //            response.IsSuccess = false;
        //            response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
        //            response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
        //        }
        //    }else
        //    {
        //        response.IsSuccess = false;
        //        response.MessageCode = (int)ApiSystemErrorCode.API_DATA_NULL_ERROR;
        //        response.MessageContent = ApiSystemErrorCode.API_DATA_NULL_ERROR.ToString();
        //    }
        //    return Ok(response);
        //}

        ///// <summary>
        ///// 获取收款账户
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[ResponseType(typeof(ResponseBase<GatherAccountModel>))]
        //public IHttpActionResult GetParkLot(string Guid)
        //{
        //    ResponseBase<GatherAccountModel> response = new ResponseBase<GatherAccountModel>()
        //    {
        //        IsSuccess = true,
        //        MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
        //        MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
        //    };
        //    GatherAccountModel content = gatheraccountmanager.GetGatherAccount(Guid);
        //    response.Result = content;

        //    return Ok(response);
        //}

        ///// <summary>
        ///// 获取收款账户列表
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[ResponseType(typeof(ResponseBaseList<GatherAccountModel>))]
        //public IHttpActionResult GetCarTypeList(string ParkingCode)
        //{
        //    ResponseBaseList<GatherAccountModel> response = new ResponseBaseList<GatherAccountModel>()
        //    {
        //        IsSuccess = true,
        //        MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
        //        MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
        //    };

        //    List<GatherAccountModel> content = gatheraccountmanager.GetGatherAccountList(ParkingCode);
        //    response.Result = content;

        //    return Ok(response);
        //}

        #endregion

        #region 语音指令
        /// <summary>
        /// 获取语音指令
        /// </summary>
        /// <param name="DrivewayGuid">车道标识</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<VoiceCommandModel>))]
        public IHttpActionResult GetVoiceCommand(string DrivewayGuid)
        {
            ResponseBase<VoiceCommandModel> response = new ResponseBase<VoiceCommandModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(DrivewayGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车道标识不能为空,请检查";
                return Ok(response);
            }

            VoiceCommandModel content = _voiceCommandManager.GetCommand(DrivewayGuid);
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 设置语音指令
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult SetVoiceCommand(VoiceCommandRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.DrivewayGuid) ||
                string.IsNullOrWhiteSpace(model.ParkingCode) ||
                model.CommandList.Count < 15 ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数不全,请检查";
                return Ok(response);
            }

            //前端内容带有html字符，传输前进行了url编码
            foreach (var item in model.CommandList)
            {
                item.ShowText = System.Web.HttpUtility.UrlDecode(item.ShowText);
                item.ShowVoice = System.Web.HttpUtility.UrlDecode(item.ShowVoice);
            }


            DrivewayModel drivewayModel = _parkLotManager.GetDriveway(model.DrivewayGuid);
            if (drivewayModel == null)
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "车道不存在";
                return Ok(response);
            }

            VoiceCommandModel content = new VoiceCommandModel()
            {
                ProjectGuid=model.ProjectGuid,
                ParkCode=model.ParkingCode,
                DrivewayGuid = model.DrivewayGuid,
                DeviceMacAddress = drivewayModel.DeviceMacAddress,
                CommandList=model.CommandList
            };


            if (!_voiceCommandManager.SaveCommand(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiSystemErrorCode.API_DATA_SAVE_ERROR;
                response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
            }

            return Ok(response);
        }

        #endregion

        #region 通行设置
        /// <summary>
        /// 新增限行规则
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddNewTrafficRestriction(TrafficRestrictionRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            string[] drivewayguids = (model.DrivewayGuid ?? "").Split(',');
            string[] cartypeguids = (model.CarTypeGuid ?? "").Split(',');
            TrafficRestrictionModel content = new TrafficRestrictionModel()
            {
                Guid = Guid.NewGuid().ToString("N"),
                ProjectGuid = model.ProjectGuid,
                ParkCode=model.ParkingCode,
                DrivewayGuid= drivewayguids.ToList(),
                CarTypeGuid= cartypeguids.ToList(),
                AssignDays=model.AssignDays,
                StartTime=model.StartTime,
                EndTime=model.EndTime
            };
            List<DrivewayModel> drivewayList = _parkLotManager.AllDriveway(model.ParkingCode);
            if (!_trafficRestrictionManager.SaveTrafficRestriction(content, drivewayList))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "保存数据失败";// ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 修改限行规则
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyTrafficRestriction(ModifyTrafficRestrictionRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            TrafficRestrictionModel content = _trafficRestrictionManager.GetTrafficRestriction(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    string[] drivewayguids = (model.DrivewayGuid ?? "").Split(',');
                    string[] cartypeguids = (model.CarTypeGuid ?? "").Split(',');

                    content.DrivewayGuid = drivewayguids.ToList();
                    content.CarTypeGuid = cartypeguids.ToList(); ;
                    content.AssignDays = model.AssignDays;
                    content.StartTime = model.StartTime;
                    content.EndTime = model.EndTime;

                    List<DrivewayModel> drivewayList = _parkLotManager.AllDriveway(model.ParkingCode);
                    if (!_trafficRestrictionManager.SaveTrafficRestriction(content, drivewayList))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                        response.MessageContent = "保存数据失败";// ApiBaseErrorCode.API_FAIL.ToString();
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiSystemErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiSystemErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除限行规则
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveTrafficRestriction(ModifyTrafficRestrictionRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            TrafficRestrictionModel content = _trafficRestrictionManager.GetTrafficRestriction(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid && content.ParkCode == model.ParkingCode)
                {
                    List<DrivewayModel> drivewayList = _parkLotManager.AllDriveway(content.ParkCode);
                    if (!_trafficRestrictionManager.DeleteTrafficRestriction(content, drivewayList))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                        response.MessageContent = _trafficRestrictionManager.LastErrorDescribe;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiSystemErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = "未找到此限行规则";// ApiSystemErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取限行规则列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBaseList<TrafficRestrictionModel>))]
        public IHttpActionResult GetTrafficRestrictionList(string ProjectGuid)
        {
            ResponseBaseList<TrafficRestrictionModel> response = new ResponseBaseList<TrafficRestrictionModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "项目标识不能为空,请检查";
                return Ok(response);
            }

            List<TrafficRestrictionModel> trafficrestrictionlistmodel = _trafficRestrictionManager.GetTrafficRestrictionList(ProjectGuid);
            if (trafficrestrictionlistmodel != null)
            {
                response.Result = trafficrestrictionlistmodel;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        #endregion

        #region 手动开闸原因设置
        /// <summary>
        /// 新增开闸原因
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddNewOpenGateReason(OpenGateReasonRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ReasonRemark) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            OpenGateReasonModel content = new OpenGateReasonModel()
            {
                Guid = Guid.NewGuid().ToString("N"),
                ProjectGuid = model.ProjectGuid,
                OpenType = model.OpenType,
                ReasonRemark = model.ReasonRemark
            };
            if (!_openGateReasonManager.SaveOpenReason(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = "保存数据失败";// ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 修改开闸原因
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyOpenGateReason(ModifyOpenGateReasonRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            OpenGateReasonModel content = _openGateReasonManager.GetOpenReason(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    content.OpenType = model.OpenType;
                    content.ReasonRemark = model.ReasonRemark;
                    if (!_openGateReasonManager.SaveOpenReason(content))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                        response.MessageContent = "保存数据失败";// ApiBaseErrorCode.API_FAIL.ToString();
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiSystemErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiSystemErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除开闸原因
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveOpenGateReason(DeleteOpenGateReasonRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            OpenGateReasonModel content = _openGateReasonManager.GetOpenReason(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    if (!_openGateReasonManager.DeleteOpenReason(content.Guid))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                        response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                    response.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.ToString();
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiSystemErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = "未找到此开闸原因";// ApiSystemErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取开闸原因列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBaseList<OpenGateReasonModel>))]
        public IHttpActionResult GetOpenGateReasonList(string ProjectGuid)
        {
            ResponseBaseList<OpenGateReasonModel> response = new ResponseBaseList<OpenGateReasonModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "项目标识不能为空,请检查";
                return Ok(response);
            }

            List<OpenGateReasonModel> opengatereasonlistmodel = _openGateReasonManager.GetOpenReasonList(ProjectGuid);
            if (opengatereasonlistmodel != null)
            {
                response.Result = opengatereasonlistmodel;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        #endregion

        #region  三种开闸方式 
        /// <summary>
        /// 收费开闸
        /// </summary> 
        /// <returns></returns> 
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ChargeOpenGate(FreeOpenGateRequest model)
        {
            //考虑用异步的接口，然后实现返回拍照后的图片地址
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.DeviceIdentify) || //设备标识
                string.IsNullOrWhiteSpace(model.CarNo) ||
                string.IsNullOrWhiteSpace(model.TolloPerator))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            FreeOpenGateModel content = new FreeOpenGateModel()
            {
                ParkingCode= model.ParkingCode,
                DeviceIdentify=model.DeviceIdentify,
                CarNo= model.CarNo,
                TolloPerator= model.TolloPerator,
                Remark=model.Remark 
            };
            if (!_openGateReasonManager.ChargeOpenGate(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response); ;
        }

        /// <summary>
        /// 手动开闸--出口未识别到车牌(添加到异常记录报表) 
        ///         --出口识别到车牌（果是临时车则添加免费临停车缴费记录）
        ///         --入口开闸(添加到异常记录报表)
        /// </summary>
        /// <param name="model">异常开闸记录</param> 
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult OpenGate(Models.InPut.OpenGateRecordRequest model)
        {
            //考虑用异步的接口，然后实现返回拍照后的图片地址
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
                string.IsNullOrWhiteSpace(model.DeviceIdentify))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            OpenGateRecordModel content = new OpenGateRecordModel()
            {
                //   Guid = Guid.NewGuid().ToString("N"),
                ParkingCode = model.ParkingCode,
                DeviceIdentify = model.DeviceIdentify,
                CarType = model.CarType,
                EntranceType = model.EntranceType,
                ThroughName = model.ThroughName,
                DiscernCamera = model.DiscernCamera,
                ThroughType = model.ThroughType,
                OpenGateOperator = model.OpenGateOperator,
                OpenGateReason = model.OpenGateReason,
                Remark = model.Remark,
                ImageUrl = model.ImageUrl,
                ErrorCode=model.ErrorCode,
                CarNo=model.CarNo
            };
            if (!_openGateReasonManager.OpenGate(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response);
        }


        /// <summary>
        /// 免费放行
        /// </summary>  
        /// <returns></returns> 
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult FreeOpenGate(FreeOpenGateRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.ParkingCode) ||
               string.IsNullOrWhiteSpace(model.DeviceIdentify) || //设备标识
               string.IsNullOrWhiteSpace(model.CarNo) ||
               string.IsNullOrWhiteSpace(model.TolloPerator))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }
            FreeOpenGateModel content = new FreeOpenGateModel()
            {
                ParkingCode = model.ParkingCode,
                DeviceIdentify = model.DeviceIdentify,
                CarNo = model.CarNo,
                TolloPerator = model.TolloPerator,
                Remark=model.Remark 

            };
            if (!_openGateReasonManager.FreeOpenGate(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response);

        }
        #endregion
    }
}
