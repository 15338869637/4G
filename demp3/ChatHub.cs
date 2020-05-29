using System;
using System.Collections.Generic;
using fujica_api_csharp_demo;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace demp3
{

    /// <summary>
    /// 集线器
    /// </summary>
    public class ChatHub : Hub
    {

        /// <summary>
        /// Show()方法会给所有客户端发送通知 displayDatas 
        /// </summary>
        //public  void Show()
        //{ 
        //    //调用所有客户 端的broadcastMessage方法 
        //    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        //    context.Clients.All.displayDatas();

        //} 
 
        ParkLotModel parkmodel = new ParkLotModel();
        /// <summary>
        /// 供客户端调用的方法，发送消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void GetDate()
        {
            ParkLotModel model = new ParkLotModel();
            model = GetParkLot();
            //调用所有客户 端的broadcastMessage方法  
            Clients.All.broadcastMessage(model);
        }

        /// <summary>
        /// 获取某停车场
        /// </summary>
        /// <param name="parkingcode"></param>
        /// <returns></returns>
        public ParkLotModel GetParkLot( )
        {
            #region api参数
           // string url = "http://192.168.15.114:8020/api/";
            string url = "http://localhost:45852/api/";
            string appid = "fujica_dasdetds3f15det2";
            string secret = "2f463f6f5e264b8abacfad39efc7d782";
            string privateKey = "MIICXQIBAAKBgQDGlx1TPt23C4YBkNQctonOHQ+j8ECU7edaYgbxyC85QZ8sIbzl7FArz581Nlyh/qCCbG3vX8XxaDTQ8sQNDsQNci8z6LgurrfE7WT7IIO8YMK9TSciMznNonBY55XTcmSIe8iJTXKyCUTctJLrF5HPSIwtG13FKK/ctfbL5yR1RwIDAQABAoGBAKeCb8n4DSyJG8/WShSuJC8ndDnkPZVh1vP2G8V2Bd9V6t1e1+dZHYbW6oQIBrrd/KYGr/Rp5J1sOKDHJXeeLN9uHprNd0cZ6y6zKsKgH75K3ljKJSTCzSF29Bph7/FZvdoUUHlZmYzMWWW6s9ALAccmoojeO6eGM1cPjYRTU5GBAkEA53fphsumJF3Qb4B8H4z1S0/qlYvjilAeotuupAcIy7PgEQ/O91OdZ4wAraHnFnWRm0lVzLvTZmuWKaxO3iXsGwJBANujK1rNGICiYWt5GtmusKhlPydaHrAXYXm9CnxqUkZxlbwai3YeA30EIYJ9FJtxXd6QWOWLUY5GgtkuUGz8lkUCQD+DgNnTAbjS4UHnUKfbpudOe4EjjIFEcNOhUi+CGqDCr8Yev1zQXc2u9fSvC1j3U8f3fIqcM2rUNLUkqdN9NmcCQQCKo3yCXGPTDqyfwloIfhRUt1Qd6uzkCl4lEgbEcfhtLtVtXvQZIujgyPK+A2Y2mGDAVC1I96ALRsbhgDKUGPxhAkBs4kJCv9pME4/upwAMl3RlEEYCXF7v+bITEeVZdQIy/cgmwcPAthUR4n9KR0FFNeExh1dk6NiA+oho/wO6Fqkt";
            // 生成时间戳
            string timestamp = GenerateTimeStamp();
            // 拼接签名参数
            string signparam = string.Format("secret={0}&timestamp={1}", secret, timestamp);

            // 签名
            string sign = Signature.signForSha(signparam, privateKey);
            #endregion
            string  result=  new RequestHelper().GetRequest(string.Concat(url, "/ParkLot/GetParkLot?ParkingCode=16000207520006"), appid, sign, timestamp);

            // 转为json对象
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            string zone = jo["Result"].ToString();//输出
             
           parkmodel = JsonConvert.DeserializeObject<ParkLotModel>(zone);
            return parkmodel;// result;
        }




        public class ParkLotModel
        {
            /// <summary>
            /// 项目编码
            /// </summary>
            public string ProjectGuid { get; set; }

            /// <summary>
            /// 停车场编码
            /// </summary>
            public string ParkCode { get; set; }

            /// <summary>
            /// 停车场名称
            /// </summary>
            public string ParkName { get; set; }

            /// <summary>
            /// 车牌前缀 如粤B
            /// </summary>
            public string Prefix { get; set; }

            /// <summary>
            /// 车位数
            /// </summary>
            public uint SpacesNumber { get; set; }

            /// <summary>
            /// 剩余车位
            /// </summary>
            public uint RemainingSpace { get; set; }

            /// <summary>
            /// 车场类型 0=商业 1=小区
            /// </summary>
            public int Type { get; set; }

            /// <summary>
            /// 车场地址
            /// </summary>
            public string SiteAddress { get; set; }

            /// <summary>
            /// 收款账户
            /// </summary>
            //public string gatheraccountGuid { get; set; }

            /// <summary>
            /// 该车场所有车道guid列表
            /// </summary>
            public List<string> DrivewayList { get; set; }

            /// <summary>
            /// 车场存续标志
            /// </summary>
            public bool Existence { get; set; }
        }

        /// <summary>
        /// 生成时间戳，标准北京时间，时区为东八区
        /// 自1970年1月1日 0点0分0秒以来的秒数
        /// </summary>
        /// <returns>时间戳</returns>
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

    }
}