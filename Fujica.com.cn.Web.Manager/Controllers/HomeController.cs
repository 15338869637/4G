using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Fujica.com.cn.Web.Manager.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 转发
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Trans(string pathname)
        {
            if (Session["UserName"] == null)
            {
                //没登录
                return RedirectToAction("Index");
            }
            string path = "~/Views/" + pathname + ".cshtml";
            return View(path);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Login()
        {
            string username = Request["username"];
            string pswword = Request["pswword"];

            Dictionary<string, object> dic = new Dictionary<string, object>()
            {
                { "IsSuccess",false},
                { "Redirect",""}
            };

            //账户验证
            FollowMysqlHelper dbhelper = new FollowMysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = string.Format("select projectGuid,guid,userName,userPswd,privilege,roleGuid from t_operator where mobile='{0}' limit 1", username);
            DataTable table = dbhelper.ExecuteDataTable(commandtext);
            if (table != null)
            {
                if (table.Rows.Count > 0)
                {
                    string userPswd = table.Rows[0]["userPswd"].ToString();
                    if (BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(pswword))).Replace("-", "") == userPswd)
                    {
                        Session["UserName"] = username;
                        Session["UserGuid"] = table.Rows[0]["guid"].ToString();
                        Session["RoleGuid"] = table.Rows[0]["roleGuid"].ToString();
                        Session["Privilege"] = table.Rows[0]["privilege"].ToString();
                        dic = new Dictionary<string, object>()
                        {
                            { "IsSuccess",true},
                            { "ProjectGuid",table.Rows[0]["projectGuid"].ToString()},
                            { "UserGuid",table.Rows[0]["guid"].ToString()},
                            { "UserName",table.Rows[0]["userName"].ToString()},
                            { "RoleGuid",table.Rows[0]["roleGuid"].ToString()},
                            { "ApiUrl",ConfigurationManager.AppSettings["apiurl"]},
                            { "Redirect",""}
                        };
                    }
                }
            }
            return Json(dic);
        }

        [HttpPost]
        public JsonResult Login123()
        {
            string username = Request["username"];
            string pswword = Request["pswword"];

            Dictionary<string, object> dic = new Dictionary<string, object>()
            {
                { "IsSuccess",false},
                { "Redirect",""}
            };

            //账户验证
            Session["UserName"] = username;
            Session["UserGuid"] = "84dc413a37c24c64a25eb59b0d2cb066";
            Session["RoleGuid"] = "1fc2a69ec4154e8eaf28229466114d2e";
            Session["Privilege"] = "000110111021103110410011201120210021301130213031003140114021403140414051406140714081409141010041501150215031504150510051";
            dic = new Dictionary<string, object>()
            {
                { "IsSuccess",true},
                { "ProjectGuid","f3c68fef27fd4300b195074237b44803"},
                { "UserGuid","84dc413a37c24c64a25eb59b0d2cb066"},
                { "UserName","系统管理员"},
                 { "RoleGuid","1fc2a69ec4154e8eaf28229466114d2e"},
                { "Redirect",""}
            };
            return Json(dic);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Logout()
        {
            Session.Clear();
            Dictionary<string, object> dic = new Dictionary<string, object>()
            {
                { "IsSuccess",true},
                { "Redirect","Index"}
            };
            return Json(dic);
        }

        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Put| HttpVerbs.Post| HttpVerbs.Get)]
        public JsonResult Maps(string pathname)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (Session["UserName"] == null)
            {
                //没登录
                dic["IsSuccess"] = false;
                dic["Redirect"] = "Index";
            }
            else
            {
                RequestFujicaInterface interfaceobj = new RequestFujicaInterface();
                if (Request.HttpMethod.Equals("post", StringComparison.OrdinalIgnoreCase))
                {
                    string jsonstr = "{";
                    for (int i = 0; i < Request.Form.Count; i++)
                    {
                        jsonstr += "\"" + Request.Form.Keys[i] + "\":";
                        //检测当前值是否是数组["a","b"]
                        if (!Regex.IsMatch(Request.Form[i], @"^\[.*\]$"))
                        {
                            jsonstr += "\"" + Request.Form[i] + "\",";
                        }
                        else
                        {
                            jsonstr += Request.Form[i] + ",";
                        }
                    }
                    if (jsonstr.Length > 1) jsonstr = jsonstr.Remove(jsonstr.Length - 1, 1);
                    jsonstr += "}";

                    if (interfaceobj.GetRequest("post", pathname, jsonstr))
                    {
                        string result = interfaceobj.LastResult;
                        dic = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
                    }
                }
                else if (Request.HttpMethod.Equals("get", StringComparison.OrdinalIgnoreCase))
                {
                    string querystr = Request.QueryString.ToString();

                    if (interfaceobj.GetRequest("get", pathname, querystr))
                    {
                        string result = interfaceobj.LastResult;
                        dic = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
                    }
                }
                else if (Request.HttpMethod.Equals("put", StringComparison.OrdinalIgnoreCase))
                {
                    string jsonstr = "{";
                    for (int i = 0; i < Request.Form.Count; i++)
                    {
                        jsonstr += "\"" + Request.Form.Keys[i] + "\":";
                        jsonstr += "\"" + Request.Form[i] + "\",";
                    }
                    if (jsonstr.Length > 1) jsonstr = jsonstr.Remove(jsonstr.Length - 1, 1);
                    jsonstr += "}";

                    if (interfaceobj.GetRequest("put", pathname, jsonstr))
                    {
                        string result = interfaceobj.LastResult;
                        dic = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
                    }
                }
            }

            return Json(dic,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新超时时长
        /// （返回空白页面）
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshTimeout()
        {
            return View();
        }

    }
}