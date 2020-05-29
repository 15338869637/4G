using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fujica.com.cn.RabbitMQFactory.Helpers
{
    /// <summary>
    /// 中心服务和相机MQ 交换器名称
    /// </summary>
    public static class MQYunCameraExchange
    {
        /*
        /// <summary>
        /// 激活相机广播 每个项目都一样
        /// </summary>
        public const string FanoutFuJiCaActivationCameraExchange = "FuJiCaActivationCameraExchange";
        /// <summary>
        /// 云停车场项目  停车场编码_CameraExchange  eg:17000105950001_CameraExchange
        /// </summary>
        public const string FanoutParkingCode_CameraExchange = "_CameraExchange";
        /// <summary>
        /// 压地感 相机上传计费金额  路由键使用停车场编码
        /// </summary>
        public const string DirectGroundSensorCameraExchange = "GroundSensorCameraExchange";
        */
        /// <summary>
        /// 接收线上下发的缴费数据    富士云平台到===> 相机
        /// </summary>
        public const string DirectRecOnLinePayData = "IssuedPayCost";


        #region direct    
        /// <summary>
        /// 动态添加区域CityCode交换器  默认路由键 //NewCityCode4007004008
        /// </summary>
        public const string DirectDynamicAddNewCityCode = "FuJiCaDynamicAddNewCityCode.direct";

        #endregion

        #region topic    2019年3月21日 topic 模式  来接收数据

        /*
         *topic 路由设置   FUJICA.区号.业务 
         *例如深圳的车场:  FUJICA.0755.InPark          
         */
        /// <summary>
        /// 相机推送数据过来   topic 
        /// </summary>
        public const string TopicFuJiCaYunCameraParkPushExchange = "FuJiCaYunCameraParkPushData.topic";  //发送到【Topic】类型【消息交换机】的消息不能有任意的routing_key - 它必须是由点分隔的单词列表


        #endregion

    }


}
