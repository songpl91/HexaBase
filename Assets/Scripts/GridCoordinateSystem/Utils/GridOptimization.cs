using System.Collections.Generic;
using UnityEngine;
using GridCoordinateSystem.Coordinates;

namespace GridCoordinateSystem.Utils
{
    /// <summary>
    /// 四边形网格坐标缓存系统
    /// 提供高效的坐标缓存和对象池管理
    /// </summary>
    public class GridCoordinateCache
    {
        #region 私有字段
        
        private Dictionary<string, object> _cache;
        private Queue<string> _accessOrder;
        private int _maxCacheSize;
        private int _hitCount;
        private int _missCount;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 最大缓存大小
        /// </summary>
        public int MaxCacheSize
        {
            get => _maxCacheSize;
            set => _maxCacheSize = Mathf.Max(1, value);
        }
        
        /// <summary>
        /// 当前缓存大小
        /// </summary>
        public int CacheSize => _cache?.Count ?? 0;
        
        /// <summary>
        /// 缓存命中次数
        /// </summary>
        public int HitCount => _hitCount;
        
        /// <summary>
        /// 缓存未命中次数
        /// </summary>
        public int MissCount => _missCount;
        
        /// <summary>
        /// 缓存命中率
        /// </summary>
        public float HitRate
        {
            get
            {
                int total = _hitCount + _missCount;
                return total > 0 ? (float)_hitCount / total : 0f;
            }
        }
        
        #endregion
        
        #region 构造函数
        
        /// <summary>
        /// 构造缓存系统
        /// </summary>
        /// <param name="maxSize">最大缓存大小</param>
        public GridCoordinateCache(int maxSize = 1000)
        {
            _maxCacheSize = maxSize;
            _cache = new Dictionary<string, object>();
            _accessOrder = new Queue<string>();
            _hitCount = 0;
            _missCount = 0;
        }
        
        #endregion
        
        #region 缓存操作
        
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">输出值</param>
        /// <returns>是否找到缓存</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            if (_cache != null && _cache.TryGetValue(key, out var cachedValue) && cachedValue is T)
            {
                value = (T)cachedValue;
                _hitCount++;
                
                // 更新访问顺序
                UpdateAccessOrder(key);
                return true;
            }
            
            value = default(T);
            _missCount++;
            return false;
        }
        
        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        public void SetValue<T>(string key, T value)
        {
            if (_cache == null) return;
            
            // 如果已存在，更新值
            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                UpdateAccessOrder(key);
                return;
            }
            
            // 检查缓存大小限制
            if (_cache.Count >= _maxCacheSize)
            {
                RemoveOldestEntry();
            }
            
            // 添加新条目
            _cache[key] = value;
            _accessOrder.Enqueue(key);
        }
        
        /// <summary>
        /// 移除缓存条目
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveValue(string key)
        {
            if (_cache != null && _cache.ContainsKey(key))
            {
                _cache.Remove(key);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 检查是否包含指定键
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否包含</returns>
        public bool ContainsKey(string key)
        {
            return _cache != null && _cache.ContainsKey(key);
        }
        
        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            _cache?.Clear();
            _accessOrder?.Clear();
            _hitCount = 0;
            _missCount = 0;
        }
        
        #endregion
        
        #region 私有方法
        
        /// <summary>
        /// 更新访问顺序
        /// </summary>
        /// <param name="key">访问的键</param>
        private void UpdateAccessOrder(string key)
        {
            // 简单的LRU实现，将访问的键移到队列末尾
            var tempQueue = new Queue<string>();
            
            while (_accessOrder.Count > 0)
            {
                var currentKey = _accessOrder.Dequeue();
                if (currentKey != key)
                {
                    tempQueue.Enqueue(currentKey);
                }
            }
            
            // 重建队列
            while (tempQueue.Count > 0)
            {
                _accessOrder.Enqueue(tempQueue.Dequeue());
            }
            
            // 将当前键添加到末尾
            _accessOrder.Enqueue(key);
        }
        
        /// <summary>
        /// 移除最旧的条目
        /// </summary>
        private void RemoveOldestEntry()
        {
            if (_accessOrder.Count > 0)
            {
                var oldestKey = _accessOrder.Dequeue();
                _cache?.Remove(oldestKey);
            }
        }
        
        #endregion
        
        #region 统计方法
        
        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>统计信息字符串</returns>
        public string GetStats()
        {
            return $"缓存大小: {CacheSize}/{MaxCacheSize}, 命中率: {HitRate:P2} ({HitCount}/{HitCount + MissCount})";
        }
        
        /// <summary>
        /// 重置统计信息
        /// </summary>
        public void ResetStats()
        {
            _hitCount = 0;
            _missCount = 0;
        }
        
        #endregion
    }
    
    /// <summary>
    /// 四边形网格对象池
    /// 提供坐标对象的复用机制，减少GC压力
    /// </summary>
    public static class GridObjectPool
    {
        #region 私有字段
        
        private static Dictionary<System.Type, Queue<object>> _pools;
        private static Dictionary<System.Type, int> _poolSizes;
        private static readonly int DEFAULT_POOL_SIZE = 100;
        
        #endregion
        
        #region 静态构造函数
        
        static GridObjectPool()
        {
            _pools = new Dictionary<System.Type, Queue<object>>();
            _poolSizes = new Dictionary<System.Type, int>();
        }
        
        #endregion
        
        #region 对象池操作
        
        /// <summary>
        /// 从对象池获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>对象实例</returns>
        public static T Get<T>() where T : new()
        {
            var type = typeof(T);
            
            if (_pools.TryGetValue(type, out var pool) && pool.Count > 0)
            {
                return (T)pool.Dequeue();
            }
            
            return new T();
        }
        
        /// <summary>
        /// 将对象返回到对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">要返回的对象</param>
        public static void Return<T>(T obj)
        {
            if (obj == null) return;
            
            var type = typeof(T);
            
            if (!_pools.TryGetValue(type, out var pool))
            {
                pool = new Queue<object>();
                _pools[type] = pool;
                _poolSizes[type] = DEFAULT_POOL_SIZE;
            }
            
            // 检查池大小限制
            if (pool.Count < _poolSizes[type])
            {
                pool.Enqueue(obj);
            }
        }
        
        /// <summary>
        /// 获取列表对象
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <returns>列表实例</returns>
        public static List<T> GetList<T>()
        {
            var list = Get<List<T>>();
            list.Clear();
            return list;
        }
        
        /// <summary>
        /// 返回列表对象
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="list">要返回的列表</param>
        public static void ReturnList<T>(List<T> list)
        {
            if (list != null)
            {
                list.Clear();
                Return(list);
            }
        }
        
        /// <summary>
        /// 设置对象池大小
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="size">池大小</param>
        public static void SetPoolSize<T>(int size)
        {
            var type = typeof(T);
            _poolSizes[type] = Mathf.Max(1, size);
        }
        
        /// <summary>
        /// 清理指定类型的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public static void ClearPool<T>()
        {
            var type = typeof(T);
            if (_pools.TryGetValue(type, out var pool))
            {
                pool.Clear();
            }
        }
        
        /// <summary>
        /// 清理所有对象池
        /// </summary>
        public static void ClearAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();
            _poolSizes.Clear();
        }
        
        /// <summary>
        /// 获取对象池统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        public static string GetPoolStats()
        {
            var stats = new System.Text.StringBuilder();
            stats.AppendLine("对象池统计:");
            
            foreach (var kvp in _pools)
            {
                var typeName = kvp.Key.Name;
                var poolCount = kvp.Value.Count;
                var maxSize = _poolSizes.TryGetValue(kvp.Key, out var size) ? size : DEFAULT_POOL_SIZE;
                
                stats.AppendLine($"  {typeName}: {poolCount}/{maxSize}");
            }
            
            return stats.ToString();
        }
        
        #endregion
    }
}