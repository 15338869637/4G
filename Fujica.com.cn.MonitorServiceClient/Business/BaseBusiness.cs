using Fujica.com.cn.BaseConnect;

namespace Fujica.com.cn.MonitorServiceClient.Business
{
    public class BaseBusiness
    {
        public static MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
    }
}
