
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using ServiceStack.Text;
using ServiceStack.Common;
using ServiceStack.Redis.Generic;


namespace Unitoys.Core
{
    public sealed class RedisHelper : IDisposable
    {
        public static readonly RedisHelper Instance = new RedisHelper(true);

        //private string redisIp = "120.76.240.82";//;"121.42.167.44";
        //private static string redisIp = "127.0.0.1";
        private string redisPassWord = "LcpcQjZ0Onzcexwm";

        //缓存池
        PooledRedisClientManager prcm = new PooledRedisClientManager();

        //默认缓存过期时间单位秒
        public int secondsTimeOut = 20 * 60;

        /// <summary>
        /// 缓冲池
        /// </summary>
        /// <param name="readWriteHosts"></param>
        /// <param name="readOnlyHosts"></param>
        /// <returns></returns>
        public static PooledRedisClientManager CreateManager(string[] readWriteHosts, string[] readOnlyHosts)
        {
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts,
                new RedisClientManagerConfig
                {
                    MaxWritePoolSize = 100,
                    MaxReadPoolSize = 100,
                    AutoStart = true
                });
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="OpenPooledRedis">是否开启缓冲池</param>
        private RedisHelper(bool OpenPooledRedis = false)
        {
            if (OpenPooledRedis)
            {
                prcm = CreateManager(new string[] { String.Format("{0}@{1}:6379", redisPassWord, UTConfig.SiteConfig.RedisIp) }, new string[] { String.Format("{0}@{1}:6379", redisPassWord, UTConfig.SiteConfig.RedisIp) });

            }
        }
        /// <summary>
        /// 距离过期时间还有多少秒
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long TTL(string key)
        {
            using (RedisClient Redis = prcm.GetReadOnlyClient() as RedisClient)
            {
                return Redis.Ttl(key);
            }

        }
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeout"></param>
        public void Expire(string key, int timeout = 0)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        secondsTimeOut = timeout;
                    }
                    Redis.Expire(key, secondsTimeOut);
                }
            }
        }

        #region Key/Value存储
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存建</param>
        /// <param name="t">缓存值</param>
        /// <param name="timeout">过期时间，单位秒,-1：不过期，0：默认过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T t, int timeout = 0)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                Redis.Set<T>(key, t);
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        secondsTimeOut = timeout;
                    }
                    Redis.Expire(key, secondsTimeOut);
                }
                return true;
            }

        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            using (RedisClient Redis = prcm.GetReadOnlyClient() as RedisClient)
            {
                return Redis.Get<T>(key);
            }
        }

        /// <summary>
        /// 获取所有keys对应的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            using (RedisClient Redis = prcm.GetReadOnlyClient() as RedisClient)
            {
                return Redis.GetAll<T>(keys);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                return Redis.Remove(key);
            }
        }
        /// <summary>
        /// Exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long Exists(string key)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                return Redis.Exists(key);
            }
        }
        #endregion

        #region 链表操作
        /// <summary>
        /// 根据IEnumerable数据添加链表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="values"></param>
        /// <param name="timeout"></param>
        public void AddList<T>(string listId, IEnumerable<T> values, int timeout = 0)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                var redisList = iredisClient.Lists[listId];
                redisList.AddRange(values);
                iredisClient.Save();
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        secondsTimeOut = timeout;
                    }
                    Redis.Expire(listId, secondsTimeOut);
                }
            }
        }
        /// <summary>
        /// 添加单个实体到链表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="Item"></param>
        /// <param name="timeout"></param>
        public void AddEntityToList<T>(string listId, T Item, int timeout = 0)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        secondsTimeOut = timeout;
                    }
                    Redis.Expire(listId, secondsTimeOut);
                }

                var redisList = iredisClient.Lists[listId];
                redisList.Add(Item);
                iredisClient.Save();
            }
        }
        /// <summary>
        /// 获取链表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string listId)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                return iredisClient.Lists[listId];
            }
        }
        /// <summary>
        /// 获取链表中的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public T GetEntityFromList<T>(string listId, Func<T, bool> func)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                var redisList = iredisClient.Lists[listId];
                return redisList.Where(func).FirstOrDefault();
            }
        }
        /// <summary>
        /// 在链表中删除单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="t"></param>
        public void RemoveEntityFromList<T>(string listId, T t)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                var redisList = iredisClient.Lists[listId];
                redisList.RemoveValue(t);
                iredisClient.Save();
            }
        }
        /// <summary>
        /// 根据lambada表达式删除符合条件的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="func"></param>
        public void RemoveEntityFromList<T>(string listId, Func<T, bool> func)
        {
            using (RedisClient Redis = prcm.GetClient() as RedisClient)
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                var redisList = iredisClient.Lists[listId];
                T value = redisList.Where(func).FirstOrDefault();
                redisList.RemoveValue(value);
                iredisClient.Save();
            }
        }
        #endregion
        //释放资源
        public void Dispose()
        {
            if (prcm != null)
            {
                prcm.Dispose();
                prcm = null;
            }
            GC.Collect();

        }
    }
}
