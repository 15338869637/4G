using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 适配器模式
/// </summary>
namespace Fujica.com.cn.BaseConnect
{
    /// <summary>
    /// 池化对象包装器[Target目标抽象类]
    /// </summary>
    public interface IPoolObjectWrapper<T> : IDisposable where T : IDisposable
    {
        /// <summary>
        /// 对象创建器
        /// </summary>
        /// <param name="constructorArgs">构造内部对象需要的参数</param>
        /// <returns></returns>
        void ObjectCreator(object[] constructorArgs);
        /// <summary>
        /// 获取内部真实对象
        /// </summary>
        /// <returns></returns>
        T GetInnerObject();
        /// <summary>
        /// 重置对象状态
        /// </summary>
        void Reset();
        /// <summary>
        /// 获取内部真实对象hashcode
        /// </summary>
        int InnerObjectHashCode { get; }
        /// <summary>
        /// 对象存活检测
        /// </summary>
        /// <returns></returns>
        bool HeartbeatTest();
        /// <summary>
        /// 是否正在使用
        /// </summary>
        bool IsUsing { get; }
        /// <summary>
        /// 是否有效
        /// </summary>
        bool IsValid { get; }
    }

    /// <summary>
    /// 对象池[Adapter适配器类]
    /// </summary>
    /// <typeparam name="TWrapper">包装后的类</typeparam>
    /// <typeparam name="T">源类</typeparam>
    public class ObjectPool<TWrapper,T> where TWrapper : class, IPoolObjectWrapper<T>, new() where T : IDisposable
    {
        //默认对象池的大小
        private int _minmumPoolSize = 1;
        private int _maxmumPoolSize = 4;

        /// <summary>
        /// 对象池当前活动对象数目
        /// </summary>
        private static volatile int _activeCount = 0;

        /// <summary>
        /// 当前对象数量
        /// </summary>
        private static volatile int _currentCount = 0;

        /// <summary>
        /// 对象池中空闲对象列表
        /// </summary>
        private static ConcurrentBag<TWrapper> freeList = new ConcurrentBag<TWrapper>();

        /// <summary>
        /// 对象池中对象字典
        /// <para>key:真实对象的hashcode</para>
        /// <para>value:对象池中的对象</para>
        /// </summary>
        private static ConcurrentDictionary<int, TWrapper> dic = new ConcurrentDictionary<int, TWrapper>();


        /// <summary>
        /// 对象池当前活动对象数目
        /// </summary>
        public int ActiveCount { get { return _activeCount; } }
        /// <summary>
        /// 对象池中对象最小数目
        /// </summary>
        public int MinmumPoolSize { get { return _minmumPoolSize; } }
        /// <summary>
        /// 对象池中对象最大数目
        /// </summary>
        public int MaxmumPoolSize { get { return _maxmumPoolSize; } }
        /// <summary>
        /// 当前对象数量
        /// </summary>
        public int CurrentCount { get { return _currentCount; } }



        /// <summary>
        /// 对象池对象构造参数数组
        /// </summary>
        private object[] constructorArgs = null;
        /// <summary>
        /// 检测对象池对象的线程
        /// </summary>
        Thread checkObjectThread;
        /// <summary>
        /// 创建对象池对象线程(保证对象在同一线程中创建)
        /// </summary>
        Thread createObjectThread;

        AutoResetEvent resetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="minPoolSize">对象池中对象最小数目</param>
        /// <param name="maxPoolSize">对象池中对象最大数目</param>
        /// <param name="args">构造对象池对象的内部对象需要的参数数组</param>
        public virtual void Init(int minPoolSize, int maxPoolSize, params object[] args)
        {
            //池大小赋值与判断
            if (minPoolSize > _minmumPoolSize)  _minmumPoolSize = minPoolSize;
            if (maxPoolSize > _maxmumPoolSize)  _maxmumPoolSize = maxPoolSize;
            constructorArgs = args;

            //初始化时,先创建最小值数量的池对象
            while (_currentCount < _minmumPoolSize)
            {
                CreateObject(); //循环创建直至个数等于最小数
            }

            //正常使用中用于负责创建池中对象的线程
            createObjectThread = new Thread(Create)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true,
            };
            createObjectThread.Start();

            //正常使用中用于负责监视线程池的线程
            checkObjectThread = new Thread(Check)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true,
            };
            checkObjectThread.Start();
        }

        /// <summary>
        /// 创建池对象
        /// </summary>
        private void Create()
        {
            while (!_disposed)
            {
                //等待其他线程发出对象池对象数量不足的信号
                resetEvent.WaitOne();

                //如果当前池中对象数量小于最大池大小,则可以继续创建对象
                if (_currentCount < _maxmumPoolSize)
                {
                    //收到信号后创建对象
                    CreateObject();
                }
            }
        }

        /// <summary>
        /// 创建池对象
        /// </summary>
        private void CreateObject()
        {
            if (_disposed) return;

            //当前池中对象数量递增(线程安全)
            Interlocked.Increment(ref _currentCount);

            Console.WriteLine("create object.currentCount:{0}", _currentCount);

            //创建对象
            var item = new TWrapper();
            //创建真实对象
            item.ObjectCreator(constructorArgs);
            //获取真实对象的hashcode
            var key = item.InnerObjectHashCode;
            //添加到对象池字典中
            dic.TryAdd(key, item);
            //添加到对象池可用对象列表中
            freeList.Add(item);
        }

        /// <summary>
        /// 定时检测
        /// </summary>
        private void Check()
        {
            while (!_disposed)
            {
                Console.WriteLine("检测对象存活线程运行中.");

                try
                {
                    //临时存储将被释放的对象
                    var list = new List<int>(); 
                    //遍历对象字典
                    foreach (TWrapper item in dic.Values)
                    {
                        try
                        {
                            //调用对象的存活检测方法,检查对象是否有效,无效则将对象的内部对象的hashcode存入列表
                            if (!item.HeartbeatTest() || !item.IsValid)
                            {
                                list.Add(item.InnerObjectHashCode);
                            }
                        }
                        catch { }
                    }
                    if (list.Count == 0) continue;
                    //遍历列表,通过字典找到无效对象,释放对象并从字典中移除
                    list.ForEach((key) =>
                    {
                        TWrapper item;
                        if (dic.TryRemove(key, out item))
                        {
                            item.Dispose();
                            //当前对象数量递减
                            Interlocked.Decrement(ref _currentCount);

                            Console.WriteLine("清理过期或无效对象.currentCount:{0}", _currentCount);
                        }
                    });
                    //发送信号,通知负责创建池对象的线程(resetEvent.WaitOne()所在线程)可以开始创建对象
                    resetEvent.Set();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Console.WriteLine("线程休眠10秒");

                    Thread.Sleep(10 * 1000);

                    Console.WriteLine("线程休眠结束");
                }
            }
            Console.WriteLine("检测对象存活线程退出.");
        }

        /// <summary>
        /// 从对象池中获取真实对象,可能返回null对象
        /// </summary>
        /// <para>如果对象池当前活动对象已达上限,则返回null</para>
        /// <para>如果对象池已释放,则返回null</para>
        /// <para>当返回null对象时可以多次尝试延时并重新获取</para>
        /// <returns></returns>
        public T GetObjectFromPool()
        {
            reTry:
            TWrapper wrapper;

            //如果对象池已释放,直接返回
            if (_disposed) return default(T);

            //对象池空闲可用对象队列数量大于0,取出对象
            if (freeList.Count > 0 && freeList.TryTake(out wrapper))
            {
                if (wrapper != null && wrapper.IsValid)
                {
                    //对象池当前活动对象数量自增
                    Interlocked.Increment(ref _activeCount);

                    //返回真实对象
                    return wrapper.GetInnerObject();
                }
                else
                {
                    //取出的对象可能已经标记为无效,重新获取一次
                    goto reTry;
                }
            }
            //如果池空闲可用对象队列数量为0
            //且池中对象数量未超过最大值,可以继续创建对象
            if (_currentCount < _maxmumPoolSize)
            {
                //发送信号,通知负责创建池对象的线程开始创建对象
                resetEvent.Set();
            }
            return default(T);
        }

        /// <summary>
        /// 将对象释放回对象池
        /// </summary>
        /// <param name="innerObj">真实对象</param>
        public void FreeObjectToPool(T innerObj)
        {
            if (innerObj == null) return;

            //对象池当前活动对象数量递减
            Interlocked.Decrement(ref _activeCount);

            Console.WriteLine("free object,activeCount:{0}", _activeCount);

            //获取真实对象的hashcode
            var key = innerObj.GetHashCode();

            TWrapper freeObj;

            //如果当前对象池已经释放了
            if (_disposed)
            {
                //则直接释放真实对象
                innerObj.Dispose();
            }
            else
            {
                //否则,通过真实对象的hashcode,在对象池对象字典中找到对象池对象
                if (!dic.TryGetValue(key, out freeObj)) return;

                //重置对象状态
                freeObj.Reset();

                //放回池中
                freeList.Add(freeObj);
            }
        }

        /// <summary>
        /// 移除无效对象
        /// </summary>
        /// <param name="innerObj"></param>
        protected void RemoveObject(T innerObj)
        {
            if (innerObj == null) return;

            //对象池当前活动对象数量递减
            Interlocked.Decrement(ref _activeCount);

            //获取真实对象的hashcode
            var key = innerObj.GetHashCode();

            TWrapper freeObj;

            //如果当前对象池已经释放了
            if (_disposed)
            {
                //则直接释放真实对象
                innerObj.Dispose();
            }
            else
            {
                //否则,通过真实对象的hashcode,在对象池对象字典中移除对象池对象
                if (!dic.TryRemove(key, out freeObj)) return;

                //释放对象:标记为无效对象
                freeObj.Dispose();
            }
        }

        /// <summary>
        /// 标识对象池是否已释放
        /// </summary>
        private volatile bool _disposed = false;

        /// <summary>
        /// 对象池中对象是否已释放
        /// </summary>
        public bool Disposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// 释放对象池中的对象
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                TWrapper item;

                //循环获取对象池中空闲可用对象
                while (freeList.Count > 0 && freeList.TryTake(out item))
                {
                    if (item == null) continue;

                    //释放真实对象
                    item.Dispose();
                }
                //清空对象池空闲可用对象列表
                freeList = null;
                //清空中对象池对象字典
                dic.Clear();
            }
        }
    }
}
