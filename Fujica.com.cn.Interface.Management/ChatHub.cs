using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Fujica.com.cn.Interface.Management
{
    /// <summary>
    ///Hub（集线器）上下文对象：用来解决实时(realtime)信息交换的功能.连接到这个Hub，就能与所有的客户端共享发送到服务器上的信息，同时服务端可以调用客户端的脚本。
    /// </summary>
    public class ChatHub : Hub
    {

        public static List<string> ConnectionList = new List<string>();

        /// <summary>
        /// 当链接的时候
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            //var DriveWay = Context.QueryString["DriveWay"];
            //Groups.Add(this.Context.ConnectionId, this.Context.ConnectionId + DriveWay); 
            return base.OnConnected();
        }


        /// <summary>
        /// 断开的时候
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        ///// <summary>
        ///// 客户端重新连接的时候调用
        ///// </summary>
        ///// <returns></returns>
        //public override Task OnReconnected()
        //{
        //    List.Add(this.Context.ConnectionId);
        //    return base.OnReconnected();
        //}

        /// <summary>
        /// 断开连接使用
        /// </summary>
        /// <param name="connectionId"></param> 
        /// <param name="groupName"></param> 
        public void RemoveGroup(string connectionId,string groupName)
        {
            this.Groups.Remove(connectionId, groupName);
        } 

        /// /// <summary>
        /// 定义一个添加分组函数，用来被客户端调用 
        /// </summary>
        /// <param name="groupId"></param> 
        /// <param name="connectionId"></param> 
        public void AddToGroup(string connectionId, string groupId)
        {
            this.Groups.Add(connectionId, groupId);
        }
    }
}