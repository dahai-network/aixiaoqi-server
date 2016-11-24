using System;
using System.Web;
using System.Collections;
using System.Web.Caching;

namespace Unitoys.Core
{
    public class CacheHelper
    {
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        public static object Get(string key)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[key];
        }

        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <typeparam name="T">需要返回的对象</typeparam>
        /// <param name="key">键</param>
        /// <returns>T</returns>
        public static T Get<T>(string key)
        {
            object obj = Get(key);
            return obj == null ? default(T) : (T)obj;
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void Set(string key, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void Set(string key, object objObject, TimeSpan Timeout)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject, null, System.Web.Caching.Cache.NoAbsoluteExpiration, Timeout, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void Set(string key, object objObject, DateTime absoluteExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }

        /// <summary>
        /// 设置数据缓存与依赖 
        /// 监视一组（到文件或目录的）路径、缓存键的更改情况或同时监视二者的更改情况。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objObject"></param>
        /// <param name="cachekeys">依赖项</param>
        public static void Set(string key, object objObject, string[] cachekeys)
        {
            CacheDependency dep = new CacheDependency(null, cachekeys);
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 设置数据缓存与依赖 
        /// 监视一组（到文件或目录的）路径、缓存键的更改情况或同时监视二者的更改情况。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objObject"></param>
        /// <param name="cachekeys">依赖项</param>
        /// <param name="onUpdateCallback">从缓存中移除对象之前将调用的委托。 可以使用它来更新缓存项并确保缓存项不会从缓存中移除。</param>
        public static void Set(string key, object objObject, string[] filenames, string[] cachekeys, CacheItemUpdateCallback onUpdateCallback)
        {
            CacheDependency dep = new CacheDependency(filenames, cachekeys);
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, onUpdateCallback);
        }

        /// <summary>
        /// 设置数据缓存与依赖 
        /// 监视一组（到文件或目录的）路径、缓存键的更改情况或同时监视二者的更改情况。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objObject"></param>
        /// <param name="cachekeys">依赖项</param>
        /// <param name="onUpdateCallback">从缓存中移除对象之前将调用的委托。 可以使用它来更新缓存项并确保缓存项不会从缓存中移除。
        /// 警告：请不要在页面中将 CacheItemRemovedCallback 设置为一个方法。除了在释放页面后回调无法使用页面方法以外，将回调指向页面方法还会阻碍垃圾回收将页面使用的内存回收。由于回调包含对页面的引用，而垃圾回收器不会从内存中移除包含任何引用的项，因此会出现这种情况。在加载应用程序期间，这可能会导致内存很快被用光。
        /// 
        /// </param>
        public static void Set(string key, object objObject, string[] filenames, string[] cachekeys, CacheItemRemovedCallback onRemoveCallback)
        {
            CacheDependency dep = new CacheDependency(filenames, cachekeys);
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, onRemoveCallback);
        }

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public static void Remove(string key)
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            _cache.Remove(key);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAll()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }
        }
    }
}