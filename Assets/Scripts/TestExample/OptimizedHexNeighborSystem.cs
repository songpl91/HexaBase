using System.Collections.Generic;
using UnityEngine;

namespace HexagonSystem
{
    /// <summary>
    /// 针对频繁邻居查找优化的六边形坐标系统
    /// 专为游戏中高频邻居查询场景设计
    /// </summary>
    public static class OptimizedHexNeighborSystem
    {
        #region 轴向坐标结构（推荐用于频繁邻居查找）

        /// <summary>
        /// 轴向坐标 - 最适合频繁邻居查找的坐标系统
        /// 存储效率高，邻居计算简单，无需条件分支
        /// </summary>
        [System.Serializable]
        public struct AxialCoordinate
        {
            public int q, r;

            public AxialCoordinate(int q, int r)
            {
                this.q = q;
                this.r = r;
            }

            public override string ToString() => $"({q}, {r})";
            public override bool Equals(object obj) => obj is AxialCoordinate other && q == other.q && r == other.r;
            public override int GetHashCode() => (q << 16) | (r & 0xFFFF);
            
            public static bool operator ==(AxialCoordinate a, AxialCoordinate b) => a.q == b.q && a.r == b.r;
            public static bool operator !=(AxialCoordinate a, AxialCoordinate b) => !(a == b);
        }

        #endregion

        #region 预定义方向向量（性能关键）

        /// <summary>
        /// 六边形的6个邻居方向向量
        /// 预计算避免运行时计算开销
        /// </summary>
        public static readonly AxialCoordinate[] NEIGHBOR_DIRECTIONS = new AxialCoordinate[]
        {
            new AxialCoordinate(1, 0),   // 右 (East)
            new AxialCoordinate(1, -1),  // 右上 (Northeast) 
            new AxialCoordinate(0, -1),  // 左上 (Northwest)
            new AxialCoordinate(-1, 0),  // 左 (West)
            new AxialCoordinate(-1, 1),  // 左下 (Southwest)
            new AxialCoordinate(0, 1)    // 右下 (Southeast)
        };

        /// <summary>
        /// 方向枚举，提高代码可读性
        /// </summary>
        public enum HexDirection
        {
            East = 0,      // 右
            Northeast = 1, // 右上
            Northwest = 2, // 左上
            West = 3,      // 左
            Southwest = 4, // 左下
            Southeast = 5  // 右下
        }

        #endregion

        #region 高性能邻居查找方法

        /// <summary>
        /// 获取单个邻居坐标（最快的方法）
        /// 适用于只需要特定方向邻居的场景
        /// </summary>
        /// <param name="coord">当前坐标</param>
        /// <param name="direction">方向索引 (0-5)</param>
        /// <returns>邻居坐标</returns>
        public static AxialCoordinate GetNeighbor(AxialCoordinate coord, int direction)
        {
            var dir = NEIGHBOR_DIRECTIONS[direction];
            return new AxialCoordinate(coord.q + dir.q, coord.r + dir.r);
        }

        /// <summary>
        /// 获取单个邻居坐标（枚举版本，更易读）
        /// </summary>
        public static AxialCoordinate GetNeighbor(AxialCoordinate coord, HexDirection direction)
        {
            return GetNeighbor(coord, (int)direction);
        }

        /// <summary>
        /// 获取所有6个邻居坐标（标准版本）
        /// 每次调用都创建新的List，适合偶尔使用
        /// </summary>
        /// <param name="coord">中心坐标</param>
        /// <returns>6个邻居坐标的列表</returns>
        public static List<AxialCoordinate> GetAllNeighbors(AxialCoordinate coord)
        {
            var neighbors = new List<AxialCoordinate>(6);
            for (int i = 0; i < 6; i++)
            {
                var dir = NEIGHBOR_DIRECTIONS[i];
                neighbors.Add(new AxialCoordinate(coord.q + dir.q, coord.r + dir.r));
            }
            return neighbors;
        }

        /// <summary>
        /// 获取所有6个邻居坐标（数组版本，零分配）
        /// 适合超高频调用，避免List分配开销
        /// </summary>
        /// <param name="coord">中心坐标</param>
        /// <param name="neighbors">输出数组，必须长度>=6</param>
        public static void GetAllNeighborsToArray(AxialCoordinate coord, AxialCoordinate[] neighbors)
        {
            for (int i = 0; i < 6; i++)
            {
                var dir = NEIGHBOR_DIRECTIONS[i];
                neighbors[i] = new AxialCoordinate(coord.q + dir.q, coord.r + dir.r);
            }
        }

        #endregion

        #region 对象池优化版本（推荐用于频繁调用）

        /// <summary>
        /// 邻居列表对象池，避免频繁的内存分配
        /// </summary>
        private static readonly Queue<List<AxialCoordinate>> s_NeighborListPool = new Queue<List<AxialCoordinate>>();
        private static readonly object s_PoolLock = new object();

        /// <summary>
        /// 从对象池获取邻居列表（推荐用于频繁调用）
        /// 使用完毕后必须调用 ReturnNeighborList 归还
        /// </summary>
        /// <param name="coord">中心坐标</param>
        /// <returns>包含6个邻居的列表</returns>
        public static List<AxialCoordinate> GetPooledNeighborList(AxialCoordinate coord)
        {
            List<AxialCoordinate> neighbors;
            
            lock (s_PoolLock)
            {
                if (s_NeighborListPool.Count > 0)
                {
                    neighbors = s_NeighborListPool.Dequeue();
                    neighbors.Clear();
                }
                else
                {
                    neighbors = new List<AxialCoordinate>(6);
                }
            }

            // 填充邻居坐标
            for (int i = 0; i < 6; i++)
            {
                var dir = NEIGHBOR_DIRECTIONS[i];
                neighbors.Add(new AxialCoordinate(coord.q + dir.q, coord.r + dir.r));
            }

            return neighbors;
        }

        /// <summary>
        /// 归还邻居列表到对象池
        /// 必须与 GetPooledNeighborList 配对使用
        /// </summary>
        /// <param name="neighbors">要归还的列表</param>
        public static void ReturnNeighborList(List<AxialCoordinate> neighbors)
        {
            if (neighbors == null) return;

            lock (s_PoolLock)
            {
                if (s_NeighborListPool.Count < 20) // 限制池大小，避免内存泄漏
                {
                    s_NeighborListPool.Enqueue(neighbors);
                }
            }
        }

        #endregion

        #region 批量邻居查找优化

        /// <summary>
        /// 批量获取多个坐标的所有邻居
        /// 适用于需要同时处理多个六边形的场景
        /// </summary>
        /// <param name="coords">输入坐标列表</param>
        /// <param name="includeOriginals">是否包含原始坐标</param>
        /// <returns>所有邻居坐标的集合（去重）</returns>
        public static HashSet<AxialCoordinate> GetBatchNeighbors(IEnumerable<AxialCoordinate> coords, bool includeOriginals = false)
        {
            var result = new HashSet<AxialCoordinate>();
            
            foreach (var coord in coords)
            {
                if (includeOriginals)
                {
                    result.Add(coord);
                }

                for (int i = 0; i < 6; i++)
                {
                    var dir = NEIGHBOR_DIRECTIONS[i];
                    result.Add(new AxialCoordinate(coord.q + dir.q, coord.r + dir.r));
                }
            }

            return result;
        }

        /// <summary>
        /// 获取指定半径范围内的所有邻居
        /// 适用于范围查找场景
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径（1=直接邻居，2=二级邻居等）</param>
        /// <returns>范围内所有坐标</returns>
        public static List<AxialCoordinate> GetNeighborsInRadius(AxialCoordinate center, int radius)
        {
            var result = new List<AxialCoordinate>();
            
            for (int q = -radius; q <= radius; q++)
            {
                int r1 = Mathf.Max(-radius, -q - radius);
                int r2 = Mathf.Min(radius, -q + radius);
                
                for (int r = r1; r <= r2; r++)
                {
                    result.Add(new AxialCoordinate(center.q + q, center.r + r));
                }
            }
            
            return result;
        }

        #endregion

        #region 距离和路径查找

        /// <summary>
        /// 计算两个六边形之间的距离
        /// </summary>
        public static int Distance(AxialCoordinate a, AxialCoordinate b)
        {
            return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.q + a.r - b.q - b.r) + Mathf.Abs(a.r - b.r)) / 2;
        }

        /// <summary>
        /// 检查两个坐标是否相邻
        /// </summary>
        public static bool AreNeighbors(AxialCoordinate a, AxialCoordinate b)
        {
            return Distance(a, b) == 1;
        }

        /// <summary>
        /// 获取从a到b的方向索引（如果相邻）
        /// </summary>
        /// <returns>方向索引(0-5)，如果不相邻返回-1</returns>
        public static int GetDirectionTo(AxialCoordinate from, AxialCoordinate to)
        {
            if (!AreNeighbors(from, to)) return -1;

            var diff = new AxialCoordinate(to.q - from.q, to.r - from.r);
            
            for (int i = 0; i < 6; i++)
            {
                if (NEIGHBOR_DIRECTIONS[i] == diff)
                {
                    return i;
                }
            }
            
            return -1;
        }

        #endregion

        #region 性能测试和调试工具

        /// <summary>
        /// 性能测试：比较不同邻居查找方法的性能
        /// </summary>
        public static void PerformanceTest(int iterations = 100000)
        {
            var testCoord = new AxialCoordinate(5, -3);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // 测试1：标准方法
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var neighbors = GetAllNeighbors(testCoord);
            }
            stopwatch.Stop();
            Debug.Log($"标准方法 {iterations} 次调用耗时: {stopwatch.ElapsedMilliseconds}ms");

            // 测试2：对象池方法
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var neighbors = GetPooledNeighborList(testCoord);
                ReturnNeighborList(neighbors);
            }
            stopwatch.Stop();
            Debug.Log($"对象池方法 {iterations} 次调用耗时: {stopwatch.ElapsedMilliseconds}ms");

            // 测试3：数组方法
            var neighborsArray = new AxialCoordinate[6];
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                GetAllNeighborsToArray(testCoord, neighborsArray);
            }
            stopwatch.Stop();
            Debug.Log($"数组方法 {iterations} 次调用耗时: {stopwatch.ElapsedMilliseconds}ms");

            // 测试4：单个邻居查找
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                for (int dir = 0; dir < 6; dir++)
                {
                    GetNeighbor(testCoord, dir);
                }
            }
            stopwatch.Stop();
            Debug.Log($"单个邻居查找 {iterations * 6} 次调用耗时: {stopwatch.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// 验证邻居查找的正确性
        /// </summary>
        public static bool ValidateNeighborFinding()
        {
            var center = new AxialCoordinate(0, 0);
            var expectedNeighbors = new AxialCoordinate[]
            {
                new AxialCoordinate(1, 0),   // 右
                new AxialCoordinate(1, -1),  // 右上
                new AxialCoordinate(0, -1),  // 左上
                new AxialCoordinate(-1, 0),  // 左
                new AxialCoordinate(-1, 1),  // 左下
                new AxialCoordinate(0, 1)    // 右下
            };

            // 测试标准方法
            var neighbors = GetAllNeighbors(center);
            for (int i = 0; i < 6; i++)
            {
                if (neighbors[i] != expectedNeighbors[i])
                {
                    Debug.LogError($"邻居查找错误: 期望 {expectedNeighbors[i]}, 实际 {neighbors[i]}");
                    return false;
                }
            }

            // 测试单个邻居方法
            for (int i = 0; i < 6; i++)
            {
                var neighbor = GetNeighbor(center, i);
                if (neighbor != expectedNeighbors[i])
                {
                    Debug.LogError($"单个邻居查找错误: 方向 {i}, 期望 {expectedNeighbors[i]}, 实际 {neighbor}");
                    return false;
                }
            }

            Debug.Log("邻居查找验证通过！");
            return true;
        }

        #endregion

        #region 实用工具方法

        /// <summary>
        /// 获取相对方向的邻居
        /// </summary>
        /// <param name="coord">当前坐标</param>
        /// <param name="baseDirection">基础方向</param>
        /// <param name="offset">相对偏移(-2到2，表示逆时针到顺时针)</param>
        public static AxialCoordinate GetRelativeNeighbor(AxialCoordinate coord, HexDirection baseDirection, int offset)
        {
            int direction = ((int)baseDirection + offset + 6) % 6;
            return GetNeighbor(coord, direction);
        }

        /// <summary>
        /// 获取对面的邻居
        /// </summary>
        public static AxialCoordinate GetOppositeNeighbor(AxialCoordinate coord, HexDirection direction)
        {
            return GetRelativeNeighbor(coord, direction, 3);
        }

        /// <summary>
        /// 清理对象池（在场景切换时调用）
        /// </summary>
        public static void ClearPool()
        {
            lock (s_PoolLock)
            {
                s_NeighborListPool.Clear();
            }
        }

        #endregion
    }

    #region 使用示例

    /// <summary>
    /// 使用示例：展示如何在游戏中高效使用邻居查找
    /// </summary>
    public class HexNeighborUsageExample : MonoBehaviour
    {
        [Header("测试参数")]
        public int testIterations = 10000;
        
        void Start()
        {
            // 验证正确性
            OptimizedHexNeighborSystem.ValidateNeighborFinding();
            
            // 性能测试
            OptimizedHexNeighborSystem.PerformanceTest(testIterations);
            
            // 使用示例
            DemonstrateUsage();
        }

        void DemonstrateUsage()
        {
            var center = new OptimizedHexNeighborSystem.AxialCoordinate(5, -3);

            // 方法1：获取单个邻居（最快）
            var rightNeighbor = OptimizedHexNeighborSystem.GetNeighbor(center, OptimizedHexNeighborSystem.HexDirection.East);
            Debug.Log($"右邻居: {rightNeighbor}");

            // 方法2：获取所有邻居（标准）
            var allNeighbors = OptimizedHexNeighborSystem.GetAllNeighbors(center);
            Debug.Log($"所有邻居数量: {allNeighbors.Count}");

            // 方法3：使用对象池（推荐用于频繁调用）
            var pooledNeighbors = OptimizedHexNeighborSystem.GetPooledNeighborList(center);
            // ... 使用邻居列表 ...
            OptimizedHexNeighborSystem.ReturnNeighborList(pooledNeighbors); // 记得归还

            // 方法4：零分配版本（超高频场景）
            var neighborsArray = new OptimizedHexNeighborSystem.AxialCoordinate[6];
            OptimizedHexNeighborSystem.GetAllNeighborsToArray(center, neighborsArray);

            // 范围查找
            var nearbyHexes = OptimizedHexNeighborSystem.GetNeighborsInRadius(center, 2);
            Debug.Log($"半径2内的六边形数量: {nearbyHexes.Count}");
        }

        void OnDestroy()
        {
            // 清理对象池
            OptimizedHexNeighborSystem.ClearPool();
        }
    }

    #endregion
}