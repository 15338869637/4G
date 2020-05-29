using Fujica.com.cn.Context.Model;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;

namespace ToolsTestProject
{
    public  class ChatHub: Hub
    {

        ParkLotModel parkmodel = new ParkLotModel();

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<ParkLotModel> databaseoperate = null;
        private IBaseRedisOperate<ParkLotModel> redisoperate = null;


        public ChatHub(IBaseRedisOperate<ParkLotModel> _redisoperate, IBaseDataBaseOperate<ParkLotModel> _databaseoperate)
        {
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;
        }


        /// <summary>
        /// 供客户端调用的方法，发送消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void sendToService(string code)
        {
            //调用所有客户 端的broadcastMessage方法
            parkmodel = GetParkLot(code);
            string aaa = "eeeeee";
            Clients.All.broadcastMessage(aaa);
        }
        /// <summary>
        /// 获取某停车场
        /// </summary>
        /// <param name="parkingcode"></param>
        /// <returns></returns>
        public ParkLotModel GetParkLot(string parkcode)
        {
            ParkLotModel model = null;
            redisoperate.model = new ParkLotModel() { ParkCode = parkcode };
            model = redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(parkcode);
                //缓存到redis
                if (model != null)
                {
                    redisoperate.model = model;
                    redisoperate.SaveToRedis();
                }
            }

            return model;
        }
    }
}
