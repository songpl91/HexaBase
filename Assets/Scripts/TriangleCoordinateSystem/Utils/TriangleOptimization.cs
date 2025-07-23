using System.Collections.Generic;
using UnityEngine;
using TriangleCoordinateSystem.Core;
using TriangleCoordinateSystem.Coordinates;

namespace TriangleCoordinateSystem.Utils
{
    /// <summary>
    /// 三角形坐标系统性能优化工具
    /// 提供对象池、缓存管理等性能优化功能
    /// </summary>
    public static class TriangleOptimization
    {
        // 坐标缓存
        private static Dictionary<string, Vector3> worldPositionCache = new Dictionary<string, Vector3>();
        private static Dictionary<string, List<ITriangleCoordinate>> neighborsCache = new Dictionary<string, List<ITriangleCoordinate>>();
        private static Dictionary<string, int> distanceCache = new Dictionary<string, int>();
        
        // 对象池
        private static Queue<List<ITriangleCoordinate>> listPool = new Queue<List<ITriangleCoordinate>>();
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public static bool CacheEnabled { get; set; } = true;
        
        /// <summary>
        /// 缓存的世界坐标转换（带缓存）
        /// </summary>
        /// <param name="coordinate">三角形坐标</param>
        /// <param name="triangleSize">三角形大小</param>
        /// <returns>世界坐标位置</returns>
        public static Vector3 GetWorldPositionCached(ITriangleCoordinate coordinate, float triangleSize = 1.0f)
        {
            if (!CacheEnabled)
            {
                return coordinate.ToWorldPosition(triangleSize);
            }
            
            string key = $"{coordinate}_{triangleSize}";
            
            if (worldPositionCache.TryGetValue(key, out Vector3 cachedPosition))
            {
                return cachedPosition;
            }
            
            Vector3 position = coordinate.ToWorldPosition(triangleSize);
            
            // 限制缓存大小
            if (worldPositionCache.Count >= TriangleConstants.MAX_CACHE_SIZE)
            {
                ClearWorldPositionCache();
            }
            
            worldPositionCache[key] = position;
            return position;
        }
        
        /// <summary>
        /// 缓存的邻居查找
        /// </summary>
        /// <param name="coordinate">三角形坐标</param>
        /// <returns>邻居坐标列表</returns>
        public static List<ITriangleCoordinate> GetNeighborsCached(ITriangleCoordinate coordinate)
        {
            if (!CacheEnabled)
            {
                return coordinate.GetNeighbors();
            }
            
            string key = coordinate.ToString();
            
            if (neighborsCache.TryGetValue(key, out List<ITriangleCoordinate> cachedNeighbors))
            {
                return new List<ITriangleCoordinate>(cachedNeighbors);
            }
            
            var neighbors = coordinate.GetNeighbors();
            
            // 限制缓存大小
            if (neighborsCache.Count >= TriangleConstants.MAX_CACHE_SIZE)
            {
                ClearNeighborsCache();
            }
            
            neighborsCache[key] = new List<ITriangleCoordinate>(neighbors);
            return neighbors;
        }
        
        /// <summary>
        /// 缓存的距离计算
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <returns>距离值</returns>
        public static int GetDistanceCached(ITriangleCoordinate from, ITriangleCoordinate to)
        {
            if (!CacheEnabled)
            {
                return from.DistanceTo(to);
            }
            
            string key = $"{from}_{to}";
            string reverseKey = $"{to}_{from}";
            
            if (distanceCache.TryGetValue(key, out int cachedDistance))
            {
                return cachedDistance;
            }
            
            if (distanceCache.TryGetValue(reverseKey, out int reverseCachedDistance))
            {
                return reverseCachedDistance;
            }
            
            int distance = from.DistanceTo(to);
            
            // 限制缓存大小
            if (distanceCache.Count >= TriangleConstants.MAX_CACHE_SIZE)
            {
                ClearDistanceCache();
            }
            
            distanceCache[key] = distance;
            return distance;
        }
        
        /// <summary>
        /// 从对象池获取列表
        /// </summary>
        /// <returns>列表对象</returns>
        public static List<ITriangleCoordinate> GetListFromPool()
        {
            if (listPool.Count > 0)
            {
                var list = listPool.Dequeue();
                list.Clear();
                return list;
            }
            
            return new List<ITriangleCoordinate>();
        }
        
        /// <summary>
        /// 将列表归还到对象池
        /// </summary>
        /// <param name="list">要归还的列表</param>
        public static void ReturnListToPool(List<ITriangleCoordinate> list)
        {
            if (list == null) return;
            
            list.Clear();
            
            if (listPool.Count < TriangleConstants.MAX_POOL_SIZE)
            {
                listPool.Enqueue(list);
            }
        }
        
        /// <summary>
        /// 批量坐标转换
        /// </summary>
        /// <param name="coordinates">坐标列表</param>
        /// <param name="triangleSize">三角形大小</param>
        /// <returns>世界坐标位置列表</returns>
        public static List<Vector3> BatchWorldPositionConversion(List<ITriangleCoordinate> coordinates, float triangleSize = 1.0f)
        {
            var positions = new List<Vector3>(coordinates.Count);
            
            for (int i = 0; i < coordinates.Count; i++)
            {
                positions.Add(GetWorldPositionCached(coordinates[i], triangleSize));
            }
            
            return positions;
        }
        
        /// <summary>
        /// 批量距离计算
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="targets">目标坐标列表</param>
        /// <returns>距离列表</returns>
        public static List<int> BatchDistanceCalculation(ITriangleCoordinate from, List<ITriangleCoordinate> targets)
        {
            var distances = new List<int>(targets.Count);
            
            for (int i = 0; i < targets.Count; i++)
            {
                distances.Add(GetDistanceCached(from, targets[i]));
            }
            
            return distances;
        }
        
        /// <summary>
        /// 清理世界坐标缓存
        /// </summary>
        public static void ClearWorldPositionCache()
        {
            worldPositionCache.Clear();
        }
        
        /// <summary>
        /// 清理邻居缓存
        /// </summary>
        public static void ClearNeighborsCache()
        {
            neighborsCache.Clear();
        }
        
        /// <summary>
        /// 清理距离缓存
        /// </summary>
        public static void ClearDistanceCache()
        {
            distanceCache.Clear();
        }
        
        /// <summary>
        /// 清理所有缓存
        /// </summary>
        public static void ClearAllCaches()
        {
            ClearWorldPositionCache();
            ClearNeighborsCache();
            ClearDistanceCache();
        }
        
        /// <summary>
        /// 清理对象池
        /// </summary>
        public static void ClearObjectPool()
        {
            listPool.Clear();
        }
        
        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存统计信息</returns>
        public static string GetCacheStats()
        {
            return $"缓存统计:\n" +
                   $"世界坐标缓存: {worldPositionCache.Count}\n" +
                   $"邻居缓存: {neighborsCache.Count}\n" +
                   $"距离缓存: {distanceCache.Count}\n" +
                   $"对象池大小: {listPool.Count}";
        }
        
        /// <summary>
        /// 预热缓存（为常用坐标预先计算并缓存结果）
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">预热半径</param>
        /// <param name="triangleSize">三角形大小</param>
        public static void WarmupCache(TriangleAxialCoordinate center, int radius, float triangleSize = 1.0f)
        {
            var coordinates = TriangleConverter.GetCoordinatesInRange(center, radius);
            
            // 预热世界坐标转换
            foreach (var coord in coordinates)
            {
                GetWorldPositionCached(coord, triangleSize);
            }
            
            // 预热邻居查找
            foreach (var coord in coordinates)
            {
                GetNeighborsCached(coord);
            }
            
            // 预热距离计算（从中心到所有坐标）
            foreach (var coord in coordinates)
            {
                GetDistanceCached(center, coord);
            }
        }
    }
}