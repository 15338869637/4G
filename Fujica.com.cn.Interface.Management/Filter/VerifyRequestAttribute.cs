using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Interface.Management.Controllers;
using Fujica.com.cn.Interface.Management.Models.OutPut;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

/***************************************************************************************
* *
* *        File Name        : VerifyRequestAttribute.cs
* *        Creator          : Ase
* *        Create Time      : 2019-09-17 
* *        Remark           : 签名验证以及请求过程记录类
* *
* *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
* ***************************************************************************************/
namespace Fujica.com.cn.Interface.Management.Filter
{
    /// <summary>
    /// 签名验证 以及请求过程记录
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class VerifyRequestAttribute: ActionFilterAttribute
    {
        private const string m_projectInfo = "Fujica.com.cn.Interface.Management.Filters.VerifyRequestAttribute";
        ILogger i_logger = null;
        ISerializer i_serializer = null;
        DateTime m_stattime = DateTime.Now;
        APIAccessControl apiaccesscontrol = null;

        /// <summary>
        /// 拦截验证请求参数
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            //标记 AllowAnonymousAttribute 特性的Controller/Action不做验证
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }
            //默认返回值
            ResponseBaseCommon resp = new ResponseBaseCommon
            {
                IsSuccess = false,
                MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR,
                MessageContent = ApiBaseErrorCode.API_PARAM_ERROR.GetRemark(),
            };
            //如果不是继承自BaseController,返回签名错误
            if (!(actionContext.ControllerContext.Controller is BaseController))
            {
                resp.MessageContent += "，请求未授权";
                actionContext.Response = actionContext.Request.CreateResponse(resp);
                return;
            }
            m_stattime = DateTime.Now;

            var _baseController = ((BaseController)actionContext.ControllerContext.Controller);
            i_logger = _baseController.Logger;
            i_serializer = _baseController.Serializer;
            apiaccesscontrol = _baseController.Apiaccesscontrol;

            var appID = string.Empty;
            var sign = string.Empty;
            var timestamp = string.Empty;

            var appid_header = "appid";
            var sign_header = "sign";
            var timestamp_header = "timestamp";

            #region 通过APPID读取缓存的项目信息   
            if (actionContext.Request.Headers.Contains(appid_header))
            {
                appID = actionContext.Request.Headers.GetValues(appid_header).FirstOrDefault();
            }

            //没有指定标头的请求都返回签名错误
            if (string.IsNullOrWhiteSpace(appID))
            {
                resp.MessageContent += "，缺少appid";
                actionContext.Response = actionContext.Request.CreateResponse(resp);
                return;
            }
            //获取接入信息
            APIAccessModel apiaccessmodel = apiaccesscontrol.Get(appID);

            //找不到接入信息,返回签名错误
            if (apiaccessmodel == null)
            {
                resp.MessageContent += "，appid无效";
                actionContext.Response = actionContext.Request.CreateResponse(resp);
                return;
            }

            //如果不允许接入api,返回权限不足
            if (apiaccessmodel.Enable == 0)
            {
                resp.MessageCode = (int)ApiBaseErrorCode.API_Unauthorized;
                resp.MessageContent = ApiBaseErrorCode.API_Unauthorized.GetRemark();
                actionContext.Response = actionContext.Request.CreateResponse(resp);
                return;
            }
            #endregion

            //1 时间戳验证、2 签名验证、 4 接口权限验证、 8参数验证
            #region 验证时间戳      
            if ((apiaccessmodel.NeedVerify & 1) == 1)
            {
                if (actionContext.Request.Headers.Contains(timestamp_header))
                {
                    timestamp = actionContext.Request.Headers.GetValues(timestamp_header).FirstOrDefault();
                }
                if (string.IsNullOrWhiteSpace(timestamp))
                {
                    resp.MessageContent += "，时间戳无效";
                    actionContext.Response = actionContext.Request.CreateResponse(resp);
                    return;
                }
                long _timestamp = 0L;
                if (long.TryParse(timestamp, out _timestamp))
                {
                    var reqTime = DateTime.Parse("1970.1.1").AddSeconds(_timestamp);

                    //请求时间在前后5分钟之外的,返回请求过期
                    if (reqTime > DateTime.UtcNow.AddMinutes(5)
                        || reqTime < DateTime.UtcNow.AddMinutes(-5))
                    {
                        resp.MessageContent = "请求已过期";
                        actionContext.Response = actionContext.Request.CreateResponse(resp);
                        return;
                    }
                }
                else
                {
                    resp.MessageContent = "请求已过期";
                    actionContext.Response = actionContext.Request.CreateResponse(resp);

                    return;
                }
            }
            #endregion

            #region 验证签名      
            //读取请求流,获取签名数据
            var reqContent = string.Empty;

            if ((apiaccessmodel.NeedVerify & 2) == 2)
            {
                //如果公钥或secret为空,返回签名错误
                if (string.IsNullOrWhiteSpace(apiaccessmodel.PublicKey) || string.IsNullOrWhiteSpace(apiaccessmodel.Secret))
                {
                    resp.MessageContent = "参数配置错误";
                    actionContext.Response = actionContext.Request.CreateResponse(resp);
                    return;
                }
                if (actionContext.Request.Method.Method == "GET")
                {
                    reqContent = HttpContext.Current.Request.QueryString.ToString();
                    if (!string.IsNullOrEmpty(reqContent))
                        reqContent = reqContent.TrimStart('?');
                }
                else
                {
                    var stream = await actionContext.Request.Content.ReadAsStreamAsync();
                    stream.Position = 0;
                    var reader = new StreamReader(stream);
                    reqContent = reader.ReadToEnd();
                    //重置请求流文件的状态
                    stream.Position = 0;
                }

                if (string.IsNullOrWhiteSpace(reqContent))
                {
                    resp.MessageContent += ",未检测到请求参数";
                    actionContext.Response = actionContext.Request.CreateResponse(resp);
                    return;
                }

                if (actionContext.Request.Headers.Contains(sign_header))
                {
                    sign = actionContext.Request.Headers.GetValues(sign_header).FirstOrDefault();
                }

                // sign标头值为空,则返回签名错误
                if (string.IsNullOrWhiteSpace(sign))
                {
                    resp.MessageContent += ",签名无效";
                    actionContext.Response = actionContext.Request.CreateResponse(resp);
                    return;
                }
                else
                {
                    string signparam = string.Empty;
                    try
                    {
                        //要签名的字符串为参数拼接后的字符串(或json字符串)+secret+timestamp
                        signparam = string.Concat("param=", reqContent, "&secret=", apiaccessmodel.Secret, "&timestamp=", timestamp);
                        var result = Signature.verify(signparam, sign, apiaccessmodel.PublicKey, "UTF-8");

                        if (!result)
                        {
                            resp.MessageContent = "验证签名失败";
                            actionContext.Response = actionContext.Request.CreateResponse(resp);
                            i_logger.LogLogic(
                                         LoggerLogicEnum.Filter,
                                         "", "", "",
                                         string.Concat(m_projectInfo, ".OnActionExecuting"),
                                         string.Format("请求参数：{0}；appid：{1}；sign：{2}；signparam：{3}；返回参数：{4}", reqContent, appID, sign, signparam, i_serializer.Serialize(resp))
                                         );
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.MessageContent = "验证签名失败";
                        actionContext.Response = actionContext.Request.CreateResponse(resp);
                        i_logger.LogError(
                                          LoggerLogicEnum.Filter,
                                          "", "", "",
                                          string.Concat(m_projectInfo, ".OnActionExecuting"),
                                          string.Format("请求参数：{0}；appid：{1}；sign：{2}；signparam：{3}；返回参数：{4}", reqContent, appID, sign, signparam, i_serializer.Serialize(resp)),
                                          ex.ToString()
                                          );

                        return;
                    }
                }
            }

            #endregion

            #region 接口权限验证
            if ((apiaccessmodel.NeedVerify & 4) == 4)
            {
                var url = actionContext.ControllerContext.Request.RequestUri.ToString().Split('/');
                if (url.Length == 6)
                {
                    bool result = false;
                    string[] test = { };
                    //todo 权限验证
                    foreach (var item in test)
                    {
                        if (url[5].Equals(item))
                        {
                            result = true;
                            break;
                        }
                    }
                    if (!result)
                    {
                        resp.MessageCode = (int)ApiBaseErrorCode.API_INTERFACENAME_ERROR;
                        resp.MessageContent = ApiBaseErrorCode.API_INTERFACENAME_ERROR.GetRemark();
                        actionContext.Response = actionContext.Request.CreateResponse(resp);
                        return;
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(resp);
                    return;
                }
            }
            #endregion

            #region 验证参数规范
            if ((apiaccessmodel.NeedVerify & 8) == 8)
            {
                if (!actionContext.ModelState.IsValid)
                {
                    int num = 1;
                    List<string> values = new List<string>();

                    //获取所有错误的Key
                    List<string> Keys = actionContext.ModelState.Keys.ToList();

                    //获取每一个key对应的ModelStateDictionary
                    foreach (var key in Keys)
                    {
                        var errors = actionContext.ModelState[key].Errors.ToList();
                        //将错误描述添加到sb中
                        foreach (var error in errors)
                        {
                            values.Add(string.Concat(num, ".", key + ":", error.ErrorMessage));
                            num++;
                        }
                    }
                    resp.MessageContent = string.Concat(ApiBaseErrorCode.API_PARAM_ERROR.GetRemark(), ":", string.Join(";", values));
                    actionContext.Response = actionContext.Request.CreateResponse(resp);

                    i_logger.LogLogic(
                                        LoggerLogicEnum.Filter,
                                        "", "", "",
                                        string.Concat(m_projectInfo, ".OnActionExecuting"),
                                        string.Format("请求参数：{0}；appid：{1}；sign：{2}；返回参数：{3}", reqContent, appID, sign, i_serializer.Serialize(resp))
                                        );
                    return;
                }
            }
            #endregion            
        }

        /// <summary>
        /// 记录请求日志
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public async override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //标记 AllowAnonymousAttribute 特性的Controller/Action不做验证
            if (actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() || actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }
            base.OnActionExecuted(actionExecutedContext);
            var appid = string.Empty;
            var sign = string.Empty;
            var timestamp = string.Empty;

            if (actionExecutedContext.Request.Headers.Contains("appid"))
                appid = actionExecutedContext.Request.Headers.GetValues("appid").FirstOrDefault();
            if (actionExecutedContext.Request.Headers.Contains("sign"))
                sign = actionExecutedContext.Request.Headers.GetValues("sign").FirstOrDefault();
            if (actionExecutedContext.Request.Headers.Contains("timestamp"))
                timestamp = actionExecutedContext.Request.Headers.GetValues("timestamp").FirstOrDefault();

            var reqContent = string.Empty;
            var resContent = string.Empty;
            var isError = 0;

            try
            {
                if (actionExecutedContext.Request.Method.Method == "GET")
                {
                    reqContent = actionExecutedContext.Request.RequestUri.Query;
                    if (!string.IsNullOrEmpty(reqContent))
                        reqContent = reqContent.TrimStart('?');
                }
                else
                {
                    var stream = await actionExecutedContext.Request.Content.ReadAsStreamAsync();
                    stream.Position = 0;
                    var reader = new StreamReader(stream);
                    reqContent = reader.ReadToEnd();
                    //重置请求流文件的状态
                    stream.Position = 0;
                }

                if (actionExecutedContext.Response == null)
                {
                    isError = 1;
                    // 获取异常信息
                    ResponseBaseCommon response = new ResponseBaseCommon()
                    {
                        IsSuccess = false,
                        MessageCode = (int)ApiBaseErrorCode.API_ERROR,
                        MessageContent = string.Concat(actionExecutedContext.Exception.Message, actionExecutedContext.Exception.StackTrace)
                    };
                    // 重新封装回传格式
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(System.Net.HttpStatusCode.OK, response);
                    resContent = i_serializer.Serialize(response);
                }
                else
                {
                    resContent = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                isError = 1;
                i_logger.LogError(LoggerLogicEnum.Filter, "", "", "",
                 actionExecutedContext.Request.RequestUri.ToString(),
                 string.Format("Filter出现异常：\r\n请求参数：{0}\r\nappid：{1}\r\nsign：{2}\r\ntimestamp：{3}\r\n返回结果：{4}", reqContent, appid, sign, timestamp, resContent), ex.ToString());
            }
            var nowtime = DateTime.Now;
            var time = (nowtime - m_stattime).TotalSeconds;
            if (actionExecutedContext.Request.RequestUri.Segments.Length > 3)
            {
                var ip = string.Empty;
                var serviceName = string.Empty;
                string hostName = System.Net.Dns.GetHostName();
                System.Net.IPAddress[] addresses = System.Net.Dns.GetHostAddresses(hostName);

                if (actionExecutedContext.Request.RequestUri.Segments.Length == 4)
                    serviceName = string.Concat(actionExecutedContext.Request.RequestUri.Segments[2].TrimEnd('/'), ".", actionExecutedContext.Request.RequestUri.Segments[3]);
                else if (actionExecutedContext.Request.RequestUri.Segments.Length == 5)
                    serviceName = string.Concat(actionExecutedContext.Request.RequestUri.Segments[3].TrimEnd('/'), ".", actionExecutedContext.Request.RequestUri.Segments[4]);

                for (int i = 0; i < addresses.Length; i++)
                {
                    var item = addresses[i].ToString();
                    if (item.StartsWith("10.") || item.StartsWith("192.168.")) ip = item;
                };

                //int isSuccess = 0;
                //var result = i_serializer.Deserialize<ResponseBaseCommon>(resContent);
                //isSuccess = result.MessageCode;
            }

            i_logger.LogInfo(LoggerLogicEnum.Filter, "", "", "",
            actionExecutedContext.Request.RequestUri.ToString(),
            string.Format("\r\n请求参数：{0}\r\nappid：{1}\r\nsign：{2}\r\ntimestamp：{3}\r\n返回结果：{4}\r\n接口耗时：{5}秒", reqContent, appid, sign, timestamp, resContent, time)
            );
        }

    }
}