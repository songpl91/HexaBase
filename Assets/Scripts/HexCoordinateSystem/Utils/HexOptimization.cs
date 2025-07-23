using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;
using HexCoordinateSystem.Coordinates;

namespace HexCoordinateSystem.Utils
{
    /// <summary>
    /// 六边形坐标系统对象池管理器
    /// 用于优化频繁创建和销毁的对象，提高性能
    /// </summary>
    public static class HexObjectPool
    {
        #region 列表对象池
        
        /// <summary>
        /// 轴向坐标列表对象池
        /// </summary>
        private static readonly Queue<List<AxialCoordinate>> s_AxialListPool = new Queue<List<AxialCoordinate>>();
        
        /// <summary>
        /// 立方坐标列表对象池
        /// </summary>
        private static readonly Queue<List<CubeCoordinate>> s_CubeListPool = new Queue<List<CubeCoordinate>>();
        
        /// <summary>
        /// 偏移坐标列表对象池
        /// </summary>
        private static readonly Queue<List<OffsetCoordinateOddQ>> s_OffsetListPool = new Queue<List<OffsetCoordinateOddQ>>();
        
        /// <summary>
        /// 双宽坐标列表对象池
        /// </summary>
        private static readonly Queue<List<DoubledCoordinate>> s_DoubledListPool = new Queue<List<DoubledCoordinate>>();
        
        /// <summary>
        /// 通用接口坐标列表对象池
        /// </summary>
        private static readonly Queue<List<IHexCoordinate>> s_InterfaceListPool = new Queue<List<IHexCoordinate>>();
        
        #endregion
        
        #region 获取对象池对象
        
        /// <summary>
        /// 获取轴向坐标列表（从对象池）
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <returns>轴向坐标列表</returns>
        public static List<AxialCoordinate> GetAxialList(int capacity = 6)
        {
            if (s_AxialListPool.Count > 0)
            {
                var list = s_AxialListPool.Dequeue();
                list.Clear();
                if (list.Capacity < capacity)
                {
                    list.Capacity = capacity;
                }
                return list;
            }
            return new List<AxialCoordinate>(capacity);
        }
        
        /// <summary>
        /// 获取立方坐标列表（从对象池）
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <returns>立方坐标列表</returns>
        public static List<CubeCoordinate> GetCubeList(int capacity = 6)
        {
            if (s_CubeListPool.Count > 0)
            {
                var list = s_CubeListPool.Dequeue();
                list.Clear();
                if (list.Capacity < capacity)
                {
                    list.Capacity = capacity;
                }
                return list;
            }
            return new List<CubeCoordinate>(capacity);
        }
        
        /// <summary>
        /// 获取偏移坐标列表（从对象池）
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <returns>偏移坐标列表</returns>
        public static List<OffsetCoordinateOddQ> GetOffsetList(int capacity = 6)
        {
            if (s_OffsetListPool.Count > 0)
            {
                var list = s_OffsetListPool.Dequeue();
                list.Clear();
                if (list.Capacity < capacity)
                {
                    list.Capacity = capacity;
                }
                return list;
            }
            return new List<OffsetCoordinateOddQ>(capacity);
        }
        
        /// <summary>
        /// 获取双宽坐标列表（从对象池）
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <returns>双宽坐标列表</returns>
        public static List<DoubledCoordinate> GetDoubledList(int capacity = 6)
        {
            if (s_DoubledListPool.Count > 0)
            {
                var list = s_DoubledListPool.Dequeue();
                list.Clear();
                if (list.Capacity < capacity)
                {
                    list.Capacity = capacity;
                }
                return list;
            }
            return new List<DoubledCoordinate>(capacity);
        }
        
        /// <summary>
        /// 获取通用接口坐标列表（从对象池）
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <returns>通用接口坐标列表</returns>
        public static List<IHexCoordinate> GetInterfaceList(int capacity = 6)
        {
            if (s_InterfaceListPool.Count > 0)
            {
                var list = s_InterfaceListPool.Dequeue();
                list.Clear();
                if (list.Capacity < capacity)
                {
                    list.Capacity = capacity;
                }
                return list;
            }
            return new List<IHexCoordinate>(capacity);
        }
        
        #endregion
        
        #region 归还对象池对象
        
        /// <summary>
        /// 归还轴向坐标列表到对象池
        /// </summary>
        /// <param name="list">要归还的列表</param>
        public static void ReturnAxialList(List<AxialCoordinate> list)
        {
            if (list == null) return;
            
            list.Clear();
            if (s_AxialListPool.Count < 10) // 限制对象池大小
            {
                s_AxialListPool.Enqueue(list);
            }
        }
        
        /// <summary>
        /// 归还立方坐标列表到对象池
        /// </summary>
        /// <param name="list">要归还的列表</param>
        public static void ReturnCubeList(List<CubeCoordinate> list)
        {
            if (list == null) return;
            
            list.Clear();
            if (s_CubeListPool.Count < 10)
            {
                s_CubeListPool.Enqueue(list);
            }
        }
        
        /// <summary>
        /// 归还偏移坐标列表到对象池
        /// </summary>
        /// <param name="list">要归还的列表</param>
        public static void ReturnOffsetList(List<OffsetCoordinateOddQ> list)
        {
            if (list == null) return;
            
            list.Clear();
            if (s_OffsetListPool.Count < 10)
            {
                s_OffsetListPool.Enqueue(list);
            }
        }
        
        /// <summary>
        /// 归还双宽坐标列表到对象池
        /// </summary>
        /// <param name="list">要归还的列表</param>
        public static void ReturnDoubledList(List<DoubledCoordinate> list)
        {
            if (list == null) return;
            
            list.Clear();
            if (s_DoubledListPool.Count < 10)
            {
                s_DoubledListPool.Enqueue(list);
            }
        }
        
        /// <summary>
        /// 归还通用接口坐标列表到对象池
        /// </summary>
        /// <param name="list">要归还的列表</param>
        public static void ReturnInterfaceList(List<IHexCoordinate> list)
        {
            if (list == null) return;
            
            list.Clear();
            if (s_InterfaceListPool.Count < 10)
            {
                s_InterfaceListPool.Enqueue(list);
            }
        }
        
        #endregion
        
        #region 清理对象池
        
        /// <summary>
        /// 清理所有对象池
        /// </summary>
        public static void ClearAllPools()
        {
            s_AxialListPool.Clear();
            s_CubeListPool.Clear();
            s_OffsetListPool.Clear();
            s_DoubledListPool.Clear();
            s_InterfaceListPool.Clear();
        }
        
        /// <summary>
        /// 获取对象池状态信息
        /// </summary>
        /// <returns>对象池状态字符串</returns>
        public static string GetPoolStatus()
        {
            return $"对象池状态 - 轴向:{s_AxialListPool.Count}, 立方:{s_CubeListPool.Count}, " +
                   $"偏移:{s_OffsetListPool.Count}, 双宽:{s_DoubledListPool.Count}, 接口:{s_InterfaceListPool.Count}";
        }
        
        #endregion
    }
    
    /// <summary>
    /// 六边形坐标缓存管理器
    /// 用于缓存频繁转换的坐标，避免重复计算
    /// </summary>
    public class HexCoordinateCache
    {
        #region 缓存字典
        
        /// <summary>
        /// 轴向到立方坐标的缓存
        /// </summary>
        private readonly Dictionary<AxialCoordinate, CubeCoordinate> _axialToCubeCache = 
            new Dictionary<AxialCoordinate, CubeCoordinate>();
        
        /// <summary>
        /// 轴向到偏移坐标的缓存
        /// </summary>
        private readonly Dictionary<AxialCoordinate, OffsetCoordinateOddQ> _axialToOffsetCache = 
            new Dictionary<AxialCoordinate, OffsetCoordinateOddQ>();
        
        /// <summary>
        /// 轴向到世界坐标的缓存
        /// </summary>
        private readonly Dictionary<AxialCoordinate, Vector3> _axialToWorldCache = 
            new Dictionary<AxialCoordinate, Vector3>();
        
        /// <summary>
        /// 距离计算缓存
        /// </summary>
        private readonly Dictionary<(AxialCoordinate, AxialCoordinate), int> _distanceCache = 
            new Dictionary<(AxialCoordinate, AxialCoordinate), int>();
        
        /// <summary>
        /// 邻居坐标缓存
        /// </summary>
        private readonly Dictionary<AxialCoordinate, List<AxialCoordinate>> _neighborsCache = 
            new Dictionary<AxialCoordinate, List<AxialCoordinate>>();
        
        #endregion
        
        #region 缓存大小限制
        
        /// <summary>
        /// 最大缓存条目数
        /// </summary>
        public int MaxCacheSize { get; set; } = 1000;
        
        #endregion
        
        #region 缓存操作
        
        /// <summary>
        /// 获取或计算立方坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>立方坐标</returns>
        public CubeCoordinate GetOrComputeCube(AxialCoordinate axial)
        {
            if (_axialToCubeCache.TryGetValue(axial, out var cube))
            {
                return cube;
            }
            
            cube = axial.ToCube();
            
            if (_axialToCubeCache.Count < MaxCacheSize)
            {
                _axialToCubeCache[axial] = cube;
            }
            
            return cube;
        }
        
        /// <summary>
        /// 获取或计算偏移坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>偏移坐标</returns>
        public OffsetCoordinateOddQ GetOrComputeOffset(AxialCoordinate axial)
        {
            if (_axialToOffsetCache.TryGetValue(axial, out var offset))
            {
                return offset;
            }
            
            offset = OffsetCoordinateOddQ.FromAxial(axial);
            
            if (_axialToOffsetCache.Count < MaxCacheSize)
            {
                _axialToOffsetCache[axial] = offset;
            }
            
            return offset;
        }
        
        /// <summary>
        /// 获取或计算世界坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>世界坐标</returns>
        public Vector3 GetOrComputeWorldPosition(AxialCoordinate axial, float hexSize = 1.0f)
        {
            // 只缓存标准大小的世界坐标
            if (Mathf.Approximately(hexSize, 1.0f))
            {
                if (_axialToWorldCache.TryGetValue(axial, out var world))
                {
                    return world;
                }
                
                world = axial.ToWorldPosition(hexSize);
                
                if (_axialToWorldCache.Count < MaxCacheSize)
                {
                    _axialToWorldCache[axial] = world;
                }
                
                return world;
            }
            
            return axial.ToWorldPosition(hexSize);
        }
        
        /// <summary>
        /// 获取或计算距离
        /// </summary>
        /// <param name="a">坐标A</param>
        /// <param name="b">坐标B</param>
        /// <returns>距离</returns>
        public int GetOrComputeDistance(AxialCoordinate a, AxialCoordinate b)
        {
            var key = (a, b);
            var reverseKey = (b, a);
            
            if (_distanceCache.TryGetValue(key, out var distance))
            {
                return distance;
            }
            
            if (_distanceCache.TryGetValue(reverseKey, out distance))
            {
                return distance;
            }
            
            distance = AxialCoordinate.Distance(a, b);
            
            if (_distanceCache.Count < MaxCacheSize)
            {
                _distanceCache[key] = distance;
            }
            
            return distance;
        }
        
        /// <summary>
        /// 获取或计算邻居坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>邻居坐标列表</returns>
        public List<AxialCoordinate> GetOrComputeNeighbors(AxialCoordinate axial)
        {
            if (_neighborsCache.TryGetValue(axial, out var neighbors))
            {
                return neighbors;
            }
            
            neighbors = new List<AxialCoordinate>(6);
            for (int i = 0; i < HexConstants.AXIAL_DIRECTIONS.Length; i++)
            {
                var direction = HexConstants.AXIAL_DIRECTIONS[i];
                neighbors.Add(new AxialCoordinate(axial.q + direction.x, axial.r + direction.y));
            }
            
            if (_neighborsCache.Count < MaxCacheSize)
            {
                _neighborsCache[axial] = neighbors;
            }
            
            return neighbors;
        }
        
        #endregion
        
        #region 缓存管理
        
        /// <summary>
        /// 清理所有缓存
        /// </summary>
        public void ClearCache()
        {
            _axialToCubeCache.Clear();
            _axialToOffsetCache.Clear();
            _axialToWorldCache.Clear();
            _distanceCache.Clear();
            _neighborsCache.Clear();
        }
        
        /// <summary>
        /// 清理指定类型的缓存
        /// </summary>
        /// <param name="cacheType">缓存类型</param>
        public void ClearCache(CacheType cacheType)
        {
            switch (cacheType)
            {
                case CacheType.AxialToCube:
                    _axialToCubeCache.Clear();
                    break;
                case CacheType.AxialToOffset:
                    _axialToOffsetCache.Clear();
                    break;
                case CacheType.AxialToWorld:
                    _axialToWorldCache.Clear();
                    break;
                case CacheType.Distance:
                    _distanceCache.Clear();
                    break;
                case CacheType.Neighbors:
                    _neighborsCache.Clear();
                    break;
            }
        }
        
        /// <summary>
        /// 获取缓存状态信息
        /// </summary>
        /// <returns>缓存状态字符串</returns>
        public string GetCacheStatus()
        {
            return $"缓存状态 - 立方:{_axialToCubeCache.Count}, 偏移:{_axialToOffsetCache.Count}, " +
                   $"世界:{_axialToWorldCache.Count}, 距离:{_distanceCache.Count}, 邻居:{_neighborsCache.Count}";
        }
        
        /// <summary>
        /// 获取缓存使用率
        /// </summary>
        /// <returns>缓存使用率（0-1）</returns>
        public float GetCacheUsage()
        {
            int totalEntries = _axialToCubeCache.Count + _axialToOffsetCache.Count + 
                              _axialToWorldCache.Count + _distanceCache.Count + _neighborsCache.Count;
            return (float)totalEntries / (MaxCacheSize * 5);
        }
        
        #endregion
    }
    
    /// <summary>
    /// 缓存类型枚举
    /// </summary>
    public enum CacheType
    {
        AxialToCube,
        AxialToOffset,
        AxialToWorld,
        Distance,
        Neighbors
    }
}