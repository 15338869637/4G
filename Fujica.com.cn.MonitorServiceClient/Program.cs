using Fujica.com.cn.Logger;
using Fujica.com.cn.MonitorServiceClient.Business;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fujica.com.cn.MonitorServiceClient
{
    class Program
    {
        private static ILogger m_ilogger = new Logger.Logger();
        private static ISerializer m_serializer = new JsonSerializer(m_ilogger);
        private static int sleepTime = Convert.ToInt32(ConfigurationManager.AppSettings["SleepTime"]);
        private static ConcurrentQueue<WriteMessage> messageQueue = new ConcurrentQueue<WriteMessage>();

        static void Main(string[] args)
        {
            //入场数据执行线程数
            int entryThreadCount = Convert.ToInt32(ConfigurationManager.AppSettings["EntryThreadCount"]);
            //出场数据执行线程数
            int exitThreadCount = Convert.ToInt32(ConfigurationManager.AppSettings["ExitThreadCount"]);
            //出口压地感执行线程数（取消）
            //int groundSenseThreadCount = Convert.ToInt32(ConfigurationManager.AppSettings["GroundSenseThreadCount"]);
            //拦截数据执行线程数
            int gateCatchThreadCount = Convert.ToInt32(ConfigurationManager.AppSettings["GateCatchThreadCount"]);
            //支付数据执行线程数
            int payDataThreadCount = Convert.ToInt32(ConfigurationManager.AppSettings["PayDataThreadCount"]);
            //心跳数据执行线程数
            int heartBeatThreadCount = Convert.ToInt32(ConfigurationManager.AppSettings["HeartBeatThreadCount"]);

            if (entryThreadCount > 0)
            {
                Thread entryThread = new Thread(() => ThreadMain(entryThreadCount, EntryDataManager.DataHandle));
                entryThread.IsBackground = true;
                entryThread.Start();
            }

            if (exitThreadCount > 0)
            {
                Thread exitThread = new Thread(() => ThreadMain(exitThreadCount, ExitDataManager.DataHandle));
                exitThread.IsBackground = true;
                exitThread.Start();
            }

            //if (groundSenseThreadCount > 0)
            //{
            //    Thread groundSenseThread = new Thread(() => ThreadMain(groundSenseThreadCount, ExitPayDataManager.DataHandle));
            //    groundSenseThread.IsBackground = true;
            //    groundSenseThread.Start();
            //}

            if (gateCatchThreadCount > 0)
            {
                Thread groundSenseThread = new Thread(() => ThreadMain(gateCatchThreadCount, GateCatchDataManager.DataHandle));
                groundSenseThread.IsBackground = true;
                groundSenseThread.Start();
            }

            if (payDataThreadCount > 0)
            {
                Thread groundSenseThread = new Thread(() => ThreadMain(payDataThreadCount, PayDataManager.DataHandle));
                groundSenseThread.IsBackground = true;
                groundSenseThread.Start();
            }

            if (heartBeatThreadCount > 0)
            {
                Thread heartBeatThread = new Thread(() => ThreadMain(heartBeatThreadCount, HeartBeatDataManager.DataHandle));
                heartBeatThread.IsBackground = true;
                heartBeatThread.Start();
            }


            Task.Run(() =>
            {
                while (true)
                {
                    while (messageQueue.Any())
                    {
                        WriteMessage m;
                        messageQueue.TryDequeue(out m);
                        ShowMessage(m);
                    }
                    System.Threading.Thread.Sleep(100);
                }

            });



            if (Console.ReadLine().ToLower() == "q")
            {
                return;
            }
        }

        /// <summary>
        /// 主线程
        /// </summary>
        /// <param name="threadCount">线程数量</param>
        /// <param name="methodName">子方法名称</param>
        private static void ThreadMain(int threadCount, Func<ILogger, ISerializer,ResponseCommon> methodName)
        {
            for (int i = 0; i < threadCount; i++)
            {
                Thread t = new Thread(() => MonitorResult(methodName));
                t.Start();
            }
        }
        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="methodName"></param>
        private static void MonitorResult(Func<ILogger, ISerializer, ResponseCommon> methodName)
        {
            while (true)
            {
                Thread.Sleep(sleepTime);
                Stopwatch watch = new Stopwatch();

                ResponseCommon result = new ResponseCommon();
                try
                {
                    watch.Start();

                    string redisContent = string.Empty;
                    result = methodName(m_ilogger, m_serializer);
                    ConsoleColor msgColor = ConsoleColor.White;
                    if (!result.IsSuccess)
                        msgColor = ConsoleColor.White;
                    else
                        msgColor = ConsoleColor.Green;

                    watch.Stop();

                    messageQueue.Enqueue(new WriteMessage() { ShowColor = msgColor, ShowText = $"[{result.MsgType}]-[耗时：{ watch.Elapsed.TotalMilliseconds}]{result.MessageContent}" });
                    

                    //==========压力测试代码==========
                    if (result.IsSuccess)
                    {
                        StackExchange.Redis.IDatabase db = BaseConnect.RedisHelper.GetDatabase(8);
                        db.ListRightPush("test:" + result.MessageContent.Substring(0, 7), "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]处理"+ result.MsgType + "业务：" + result.MessageContent);

                        if (result.MsgType == MsgType.InParking)
                        {
                            //入场成功后，再记录数据到出场车牌列表（以便执行出场业务）
                            db.ListRightPush("ExitDataList", result.RedisContent);
                        }
                    }

                    //==========压力测试代码==========

                }
                catch (Exception ex)
                {
                    m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", "", "", "Fujica.com.cn.MonitorServiceClient.MonitorResult", $"执行{result.MsgType}数据发生异常；参数：{result.RedisContent}", ex.ToString());

                    messageQueue.Enqueue(new WriteMessage() { ShowColor = ConsoleColor.Yellow, ShowText = $"[{result.MsgType}]-[{DateTime.Now}]系统发生异常，redis数据：{result.RedisContent}；系统异常：{ex.ToString()}" });

                }
            }
        }

        public static void ShowMessage(WriteMessage msg)
        {
            if (msg.ShowColor == ConsoleColor.Red)
            {
                Console.WriteLine("red");
            }
            Console.ForegroundColor = msg.ShowColor;
            Console.WriteLine(msg.ShowText);
            Console.ResetColor();
        }


    }

    public class WriteMessage
    {
        public ConsoleColor ShowColor { get; set; }
        public string ShowText { get; set; }
    }
}
