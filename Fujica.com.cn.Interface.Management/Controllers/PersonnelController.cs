using Fujica.com.cn.Business.User;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Interface.Management.Models.InPut;
using Fujica.com.cn.Interface.Management.Models.OutPut;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
/***************************************************************************************
 * *
 * *        File Name        : PersonnelController.cs
 * *        Creator          : Ase
 * *        Create Time      : 2019-10-23 
 * *        Remark           : 人事业务接口模块
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Interface.Management.Controllers
{
    /// <summary>
    /// 人事业务模块
    /// 2019.10.23：新增接口GetUserList根据项目编码和停车场编码获取所有用户列表. Ase <br/>
    /// </summary>
    public class PersonnelController : BaseController
    {
        private RoleManager rolemanager = null;
        private UserManager usermanager = null;
        private MenuManager menumanager = null;

        /// <summary>
        /// 唯一构造函数
        /// </summary>
        /// <param name="_logger">日志接口器</param>
        /// <param name="_serializer">序列化接口器</param>
        /// <param name="_apiaccesscontrol">api接入控制器</param>
        /// <param name="_rolemanager">角色管理器</param>
        /// <param name="_usermanager">用户管理器</param>
        /// <param name="_menumanager">菜单管理器</param>
        public PersonnelController(ILogger _logger, ISerializer _serializer, 
            APIAccessControl _apiaccesscontrol, RoleManager _rolemanager, 
            UserManager _usermanager, MenuManager _menumanager) :
            base(_logger, _serializer, _apiaccesscontrol)
        {
            rolemanager = _rolemanager;
            usermanager = _usermanager;
            menumanager = _menumanager;
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBase<RoleModel>))]
        public IHttpActionResult AddNewRole(ModifyRoleRequest model)
        {
            ResponseBase<RoleModel> response = new ResponseBase<RoleModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (model.MenuSerials==null ||
                string.IsNullOrWhiteSpace(model.RoleName) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            //组建权限字符串
            string privilegeStr = "";
            MenuModel menumodel = menumanager.GetMenu(model.ProjectGuid);
            if (menumodel != null) {
                foreach (MenuDetialModel item in menumodel.MenuList)
                {
                    string menuSerial = item.MenuSerial.PadLeft(3, '0');
                    if (model.MenuSerials.Exists(o => o == menuSerial))
                    {
                        privilegeStr += (menuSerial + "1");
                    }
                    else
                    {
                        privilegeStr += (menuSerial + "0");
                    }
                }
            }

            //授权停车场编号集合
            string parkingCodeStr = "";
            if (model.ParkingCodeList != null)
            {
                foreach (string item in model.ParkingCodeList)
                {
                    parkingCodeStr += item + ",";
                }
                parkingCodeStr = parkingCodeStr.TrimEnd(',');
            }

            RolePermissionModel content = new RolePermissionModel()
            {
                Guid = Guid.NewGuid().ToString("N"),
                ProjectGuid = model.ProjectGuid,
                RoleName = model.RoleName,
                ContentDetial = privilegeStr,
                ParkingCodeList = parkingCodeStr
            };
            if (!rolemanager.AddRole(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = ApiBaseErrorCode.API_FAIL.ToString();
            }
            response.Result = new RoleModel() { Guid = content.Guid, RoleName = content.RoleName };
            return Ok(response);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyRole(ModifyRoleRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (model.MenuSerials == null ||
                string.IsNullOrWhiteSpace(model.RoleName) ||
                string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            RolePermissionModel content = rolemanager.GetRole(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    //组建权限字符串
                    string privilegeStr = "";
                    MenuModel menumodel = menumanager.GetMenu(model.ProjectGuid);
                    if (menumodel != null)
                    {
                        foreach (MenuDetialModel item in menumodel.MenuList)
                        {
                            string menuSerial = item.MenuSerial.PadLeft(3, '0');
                            if (model.MenuSerials.Exists(o => o == menuSerial))
                            {
                                privilegeStr += (menuSerial + "1");
                            }
                            else
                            {
                                privilegeStr += (menuSerial + "0");
                            }
                        }
                    }

                    //授权停车场编号集合
                    string parkingCodeStr = "";
                    if (model.ParkingCodeList != null)
                    {
                        foreach (string item in model.ParkingCodeList)
                        {
                            parkingCodeStr += item + ",";
                        }
                        parkingCodeStr = parkingCodeStr.TrimEnd(',');
                    }


                    content.RoleName = model.RoleName;
                    content.ContentDetial = privilegeStr;
                    content.ParkingCodeList = parkingCodeStr;
                    if (!rolemanager.ModifyRole(content))
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
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveRole(RoleRequest model)
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

            RolePermissionModel content = rolemanager.GetRole(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    //超级管理员不让删除（目前没有字段判断，只能针对名字来进行判断）
                    if (content.RoleName == "超级管理员")
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                        response.MessageContent = "超级管理员无法删除。";
                        return Ok(response);
                    }

                    //删除前需要验证该角色下是否含有操作员
                    List<UserAccountModel> userList = usermanager.GetUserList(model.ProjectGuid);
                    if (userList != null)
                    {
                        int userCount = userList.Where(m => m.RoleGuid == model.Guid).Count();
                        if (userCount > 0)
                        {
                            response.IsSuccess = false;
                            response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                            response.MessageContent = "该角色正在使用中，无法删除。";
                            return Ok(response);
                        }
                    }

                    if (!rolemanager.DeleteRole(content))
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
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(RoleListResponse))]
        public IHttpActionResult GetRoleList(string ProjectGuid)
        {
            RoleListResponse response = new RoleListResponse()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
                RoleList=new List<RoleModel>()
            };

            if (string.IsNullOrWhiteSpace(ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "项目标识不能为空,请检查";
                return Ok(response);
            }

            List<RolePermissionModel> rolelistmodel = rolemanager.GetRoleList(ProjectGuid);
            if (rolelistmodel != null)
            {
               foreach(var item in rolelistmodel)
                {
                    response.RoleList.Add(new RoleModel() {
                        Guid=item.Guid,
                        RoleName=item.RoleName
                    });
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(RolePermissionResponse))]
        public IHttpActionResult GetRolePermission(string Guid)
        {
            RolePermissionResponse response = new RolePermissionResponse()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString(),
                OpenUpMenuSerial = new List<string>()
            };

            if (string.IsNullOrWhiteSpace(Guid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "角色标识不能为空,请检查";
                return Ok(response);
            }

            RolePermissionModel rolemodel = rolemanager.GetRole(Guid);
            if (rolemodel != null)
            {
                
                string privilegeStr = rolemodel.ContentDetial;
                for (int i = 1; i <= privilegeStr.Length / 4; i++)
                {
                    try
                    {
                        string flag = privilegeStr.Substring(i * 4 - 1, 1);
                        string menuserial= privilegeStr.Substring((i - 1) * 4, 3);
                        if (flag=="1") response.OpenUpMenuSerial.Add(menuserial);
                    }
                    catch(Exception ex)
                    {
                        Logger.LogFatal(LoggerLogicEnum.Interface, "", "", "", rolemodel.ProjectGuid, "获取角色权限时发生异常", ex.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(rolemodel.ParkingCodeList))
                {
                    response.ParkingCodeList = rolemodel.ParkingCodeList.Split(',').ToList();
                }

                response.IsAdmin = rolemodel.IsAdmin;
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ResponseBase<MenuModel>))]
        public IHttpActionResult GetProjectAllMenu(string projectGuid)
        {
            ResponseBase<MenuModel> response = new ResponseBase<MenuModel>()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(projectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "项目标识不能为空,请检查";
                return Ok(response);
            }

            MenuModel content = menumanager.GetMenu(projectGuid);
            response.Result = content;

            return Ok(response);
        }

        /// <summary>
        /// 新增操作员
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult AddNewOperator(UserRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.UserName) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            //拿取角色权限字符串
            RolePermissionModel rolemodel = rolemanager.GetRole(model.RoleGuid);
            UserAccountModel content = new UserAccountModel()
            {
                Guid = Guid.NewGuid().ToString("N"),
                ProjectGuid = model.ProjectGuid,
                UserName=model.UserName,
                UserPswd=BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes("fujica"))).Replace("-", ""),
                Mobile=model.Mobile,
                Privilege=rolemodel.ContentDetial,
                RoleGuid=model.RoleGuid
            };
            if (!usermanager.AddUser(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = usermanager.LastErrorDescribe; //ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 修改操作员
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyOperator(ModifyUserRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.UserName) ||
                string.IsNullOrWhiteSpace(model.UserPswd) ||
                string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.ProjectGuid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            UserAccountModel content = usermanager.GetUser(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    RolePermissionModel rolemodel = rolemanager.GetRole(model.RoleGuid);
                    content.UserName = model.UserName;
                    content.UserPswd = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(model.UserPswd))).Replace("-", "");
                    content.Mobile = model.Mobile;
                    content.Privilege = rolemodel.ContentDetial;
                    content.RoleGuid = model.RoleGuid;
                    if (!usermanager.ModifyUser(content))
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                        response.MessageContent = usermanager.LastErrorDescribe;// ApiBaseErrorCode.API_FAIL.ToString();
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
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除操作员
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult RemoveOperator(DeleteUserRequest model)
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

            UserAccountModel content = usermanager.GetUser(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    if (!usermanager.DeleteUser(content))
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
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = "未找到该操作员";// ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 获取操作员列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(UserListResponse))]
        public IHttpActionResult GetUserList(string ProjectGuid)
        {
            UserListResponse response = new UserListResponse()
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

            List<UserAccountModel> userlistmodel = usermanager.GetUserList(ProjectGuid);
            if (userlistmodel != null)
            {
                response.UserList = new List<UserModel>();
                foreach (var item in userlistmodel)
                {
                    response.UserList.Add(new UserModel()
                    {
                        Guid = item.Guid,
                        UserName = item.UserName,
                        Mobile = item.Mobile,
                        RoleGuid = item.RoleGuid,
                        RoleName = rolemanager.GetRole(item.RoleGuid).RoleName
                    });
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 根据项目编码和停车场编码获取所有用户列表
        /// </summary>
        /// <param name="projectGuid">项目编码</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(UserListResponse))]
        public IHttpActionResult GetUserList(string projectGuid, string parkingCode)
        {
            UserListResponse response = new UserListResponse()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(projectGuid) ||
                string.IsNullOrWhiteSpace(parkingCode))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            List<UserAccountModel> userlistmodel = usermanager.GetUserList(projectGuid, parkingCode);
            if (userlistmodel != null)
            {
                response.UserList = new List<UserModel>();
                foreach (var item in userlistmodel)
                {
                    response.UserList.Add(new UserModel()
                    {
                        Guid = item.Guid,
                        UserName = item.UserName,
                        Mobile = item.Mobile,
                        RoleGuid = item.RoleGuid,
                        RoleName = rolemanager.GetRole(item.RoleGuid).RoleName
                    });
                }
            }
            else
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ResetPasssword(ResetUserPassword model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.Guid))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            UserAccountModel content = usermanager.GetUser(model.Guid);
            content.UserPswd = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes("fujica"))).Replace("-", "");
            if (!usermanager.ModifyUser(content))
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                response.MessageContent = usermanager.LastErrorDescribe; //ApiBaseErrorCode.API_FAIL.ToString();
            }
            return Ok(response);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(ResponseBaseCommon))]
        public IHttpActionResult ModifyPasssword(ModifyUserPasswordRequest model)
        {
            ResponseBaseCommon response = new ResponseBaseCommon()
            {
                IsSuccess = true,
                MessageCode = (int)ApiBaseErrorCode.API_SUCCESS,
                MessageContent = ApiBaseErrorCode.API_SUCCESS.ToString()
            };

            if (string.IsNullOrWhiteSpace(model.Guid) ||
                string.IsNullOrWhiteSpace(model.NewPassword) ||
                string.IsNullOrWhiteSpace(model.OldPassword)
                )
            {
                response.IsSuccess = false;
                response.MessageCode = (int)ApiBaseErrorCode.API_PARAM_ERROR;
                response.MessageContent = "必要参数缺失,请检查";
                return Ok(response);
            }

            UserAccountModel content = usermanager.GetUser(model.Guid);
            if (content != null)
            {
                if (content.ProjectGuid == model.ProjectGuid)
                {
                    if (BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(model.OldPassword))).Replace("-", "") == content.UserPswd)
                    {
                        content.UserPswd = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(model.NewPassword))).Replace("-", "");
                        if (!usermanager.ModifyUser(content))
                        {
                            response.IsSuccess = false;
                            response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                            response.MessageContent = usermanager.LastErrorDescribe; //ApiBaseErrorCode.API_FAIL.ToString();
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.MessageCode = (int)ApiBaseErrorCode.API_FAIL;
                        response.MessageContent = "旧密码验证失败";
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
                response.MessageCode = (int)ApiPersonnelErrorCode.API_DATA_NULL_ERROR;
                response.MessageContent = ApiPersonnelErrorCode.API_DATA_NULL_ERROR.ToString();
            }
            return Ok(response);
        }
    }
}
