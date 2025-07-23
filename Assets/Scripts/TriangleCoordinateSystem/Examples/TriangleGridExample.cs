using UnityEngine;
using TriangleCoordinateSystem;
using TriangleCoordinateSystem.Coordinates;
using System.Collections.Generic;
using TriangleCoordinateSystem.Core;

namespace TriangleCoordinateSystem.Examples
{
    /// <summary>
    /// 三角形网格系统使用示例
    /// 演示基本功能和常见用法
    /// </summary>
    public class TriangleGridExample : MonoBehaviour
    {
        [Header("示例设置")]
        [SerializeField] private TriangleCoordinateManager coordinateManager;
        [SerializeField] private bool runExamplesOnStart = true;
        
        [Header("交互示例")]
        [SerializeField] private bool enableClickInteraction = true;
        [SerializeField] private bool showClickedTriangle = true;
        [SerializeField] private Color clickedTriangleColor = Color.yellow;
        
        [Header("路径示例")]
        [SerializeField] private TriangleAxialCoordinate pathStart = new TriangleAxialCoordinate(0, 0);
        [SerializeField] private TriangleAxialCoordinate pathEnd = new TriangleAxialCoordinate(3, -2);
        [SerializeField] private bool showPath = true;
        [SerializeField] private Color pathColor = Color.green;
        
        [Header("范围示例")]
        [SerializeField] private TriangleAxialCoordinate rangeCenter = new TriangleAxialCoordinate(0, 0);
        [SerializeField] private int rangeRadius = 2;
        [SerializeField] private bool showRange = true;
        [SerializeField] private Color rangeColor = Color.blue;
        
        private TriangleAxialCoordinate lastClickedCoordinate;
        private List<TriangleAxialCoordinate> currentPath;
        private List<TriangleAxialCoordinate> currentRange;
        
        private void Start()
        {
            // 确保有坐标管理器
            if (coordinateManager == null)
            {
                coordinateManager = FindObjectOfType<TriangleCoordinateManager>();
                if (coordinateManager == null)
                {
                    coordinateManager = gameObject.AddComponent<TriangleCoordinateManager>();
                }
            }
            
            // 订阅事件
            coordinateManager.OnCoordinateClicked += OnCoordinateClicked;
            coordinateManager.OnCoordinateHovered += OnCoordinateHovered;
            
            if (runExamplesOnStart)
            {
                RunAllExamples();
            }
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件
            if (coordinateManager != null)
            {
                coordinateManager.OnCoordinateClicked -= OnCoordinateClicked;
                coordinateManager.OnCoordinateHovered -= OnCoordinateHovered;
            }
        }
        
        /// <summary>
        /// 运行所有示例
        /// </summary>
        public void RunAllExamples()
        {
            Debug.Log("=== 三角形网格系统示例 ===");
            
            CoordinateCreationExample();
            CoordinateConversionExample();
            NeighborExample();
            DistanceExample();
            PathExample();
            RangeExample();
            PerformanceExample();
        }
        
        /// <summary>
        /// 坐标创建示例
        /// </summary>
        private void CoordinateCreationExample()
        {
            Debug.Log("--- 坐标创建示例 ---");
            
            // 创建不同类型的坐标
            var axial = coordinateManager.CreateAxial(2, -1);
            var cube = coordinateManager.CreateCube(2, -1, -1);
            var offset = coordinateManager.CreateOffset(3, 1);
            
            Debug.Log($"轴向坐标: {axial}");
            Debug.Log($"立方坐标: {cube}");
            Debug.Log($"偏移坐标: {offset}");
            
            // 验证坐标
            Debug.Log($"轴向坐标有效: {coordinateManager.ValidateCoordinate(axial)}");
            Debug.Log($"立方坐标有效: {coordinateManager.ValidateCoordinate(cube)}");
            Debug.Log($"偏移坐标有效: {coordinateManager.ValidateCoordinate(offset)}");
        }
        
        /// <summary>
        /// 坐标转换示例
        /// </summary>
        private void CoordinateConversionExample()
        {
            Debug.Log("--- 坐标转换示例 ---");
            
            var axial = coordinateManager.CreateAxial(1, -1);
            
            // 转换为世界坐标
            Vector3 worldPos = coordinateManager.CoordinateToWorld(axial);
            Debug.Log($"轴向坐标 {axial} 的世界坐标: {worldPos}");
            
            // 世界坐标转回三角形坐标
            var backToAxial = coordinateManager.WorldToAxial(worldPos);
            Debug.Log($"世界坐标 {worldPos} 转回轴向坐标: {backToAxial}");
            
            // 不同坐标系统间转换
            var cube = axial.ToCubeCoordinate();
            var offset = axial.ToOffsetCoordinate();
            
            Debug.Log($"轴向 {axial} -> 立方 {cube}");
            Debug.Log($"轴向 {axial} -> 偏移 {offset}");
        }
        
        /// <summary>
        /// 邻居查找示例
        /// </summary>
        private void NeighborExample()
        {
            Debug.Log("--- 邻居查找示例 ---");
            
            var center = coordinateManager.CreateAxial(0, 0);
            
            // 获取边邻居
            var neighbors = coordinateManager.GetNeighbors(center);
            Debug.Log($"坐标 {center} 的边邻居数量: {neighbors.Count}");
            foreach (var neighbor in neighbors)
            {
                Debug.Log($"  边邻居: {neighbor}");
            }
            
            // 获取顶点邻居
            var vertexNeighbors = coordinateManager.GetVertexNeighbors(center);
            Debug.Log($"坐标 {center} 的顶点邻居数量: {vertexNeighbors.Count}");
            foreach (var neighbor in vertexNeighbors)
            {
                Debug.Log($"  顶点邻居: {neighbor}");
            }
        }
        
        /// <summary>
        /// 距离计算示例
        /// </summary>
        private void DistanceExample()
        {
            Debug.Log("--- 距离计算示例 ---");
            
            var coord1 = coordinateManager.CreateAxial(0, 0);
            var coord2 = coordinateManager.CreateAxial(3, -2);
            var coord3 = coordinateManager.CreateAxial(-1, 2);
            
            int distance12 = coordinateManager.GetDistance(coord1, coord2);
            int distance13 = coordinateManager.GetDistance(coord1, coord3);
            int distance23 = coordinateManager.GetDistance(coord2, coord3);
            
            Debug.Log($"距离 {coord1} -> {coord2}: {distance12}");
            Debug.Log($"距离 {coord1} -> {coord3}: {distance13}");
            Debug.Log($"距离 {coord2} -> {coord3}: {distance23}");
        }
        
        /// <summary>
        /// 路径查找示例
        /// </summary>
        private void PathExample()
        {
            Debug.Log("--- 路径查找示例 ---");
            
            var start = coordinateManager.CreateAxial(0, 0);
            var end = coordinateManager.CreateAxial(3, -2);
            
            var path = coordinateManager.GetPath(start, end);
            currentPath = path;
            
            Debug.Log($"路径 {start} -> {end}:");
            for (int i = 0; i < path.Count; i++)
            {
                Debug.Log($"  步骤 {i}: {path[i]}");
            }
            
            if (showPath)
            {
                coordinateManager.DrawPath(path, pathColor);
            }
        }
        
        /// <summary>
        /// 范围查询示例
        /// </summary>
        private void RangeExample()
        {
            Debug.Log("--- 范围查询示例 ---");
            
            var center = coordinateManager.CreateAxial(0, 0);
            int radius = 2;
            
            // 获取范围内所有坐标
            var range = coordinateManager.GetCoordinatesInRange(center, radius);
            currentRange = range;
            
            Debug.Log($"中心 {center}，半径 {radius} 的范围内坐标数量: {range.Count}");
            foreach (var coord in range)
            {
                Debug.Log($"  范围内: {coord}");
            }
            
            // 获取环形坐标
            var ring = coordinateManager.GetRing(center, radius);
            Debug.Log($"中心 {center}，半径 {radius} 的环形坐标数量: {ring.Count}");
            foreach (var coord in ring)
            {
                Debug.Log($"  环形: {coord}");
            }
            
            if (showRange)
            {
                coordinateManager.DrawRange(center, radius, rangeColor);
            }
        }
        
        /// <summary>
        /// 性能优化示例
        /// </summary>
        private void PerformanceExample()
        {
            Debug.Log("--- 性能优化示例 ---");
            
            // 启用缓存
            coordinateManager.CacheEnabled = true;
            
            // 预热缓存
            var center = coordinateManager.CreateAxial(0, 0);
            coordinateManager.WarmupCache(center, 3);
            
            // 执行一些计算来测试缓存
            var testCoords = coordinateManager.GetCoordinatesInRange(center, 3);
            foreach (var coord in testCoords)
            {
                var worldPos = coordinateManager.CoordinateToWorld(coord);
                var neighbors = coordinateManager.GetNeighbors(coord);
                var distance = coordinateManager.GetDistance(center, coord);
            }
            
            // 获取缓存统计
            string cacheStats = coordinateManager.GetCacheStats();
            Debug.Log($"缓存统计信息:\n{cacheStats}");
        }
        
        /// <summary>
        /// 处理坐标点击事件
        /// </summary>
        /// <param name="coordinate">被点击的坐标</param>
        private void OnCoordinateClicked(ITriangleCoordinate coordinate)
        {
            if (!enableClickInteraction) return;
            
            Debug.Log($"点击了坐标: {coordinate}");
            
            if (coordinate is TriangleAxialCoordinate axial)
            {
                lastClickedCoordinate = axial;
                
                // 显示点击的三角形
                if (showClickedTriangle)
                {
                    coordinateManager.DrawTriangle(coordinate);
                }
                
                // 显示邻居连接
                coordinateManager.DrawNeighborConnections(coordinate, Color.red);
                
                // 输出坐标信息
                coordinateManager.LogCoordinateInfo(coordinate);
            }
        }
        
        /// <summary>
        /// 处理坐标悬停事件
        /// </summary>
        /// <param name="coordinate">被悬停的坐标</param>
        private void OnCoordinateHovered(ITriangleCoordinate coordinate)
        {
            // 可以在这里添加悬停效果
        }
        
        /// <summary>
        /// 更新路径起点
        /// </summary>
        public void UpdatePathStart()
        {
            if (lastClickedCoordinate != null)
            {
                pathStart = lastClickedCoordinate;
                RefreshPath();
            }
        }
        
        /// <summary>
        /// 更新路径终点
        /// </summary>
        public void UpdatePathEnd()
        {
            if (lastClickedCoordinate != null)
            {
                pathEnd = lastClickedCoordinate;
                RefreshPath();
            }
        }
        
        /// <summary>
        /// 刷新路径显示
        /// </summary>
        private void RefreshPath()
        {
            if (showPath)
            {
                currentPath = coordinateManager.GetPath(pathStart, pathEnd);
                coordinateManager.DrawPath(currentPath, pathColor);
            }
        }
        
        /// <summary>
        /// 更新范围中心
        /// </summary>
        public void UpdateRangeCenter()
        {
            if (lastClickedCoordinate != null)
            {
                rangeCenter = lastClickedCoordinate;
                RefreshRange();
            }
        }
        
        /// <summary>
        /// 刷新范围显示
        /// </summary>
        private void RefreshRange()
        {
            if (showRange)
            {
                coordinateManager.DrawRange(rangeCenter, rangeRadius, rangeColor);
            }
        }
        
        /// <summary>
        /// 清理所有缓存
        /// </summary>
        public void ClearCaches()
        {
            coordinateManager.ClearCaches();
            Debug.Log("已清理所有缓存");
        }
        
        /// <summary>
        /// 显示缓存统计
        /// </summary>
        public void ShowCacheStats()
        {
            string stats = coordinateManager.GetCacheStats();
            Debug.Log($"缓存统计信息:\n{stats}");
        }
        
        private void OnDrawGizmos()
        {
            if (coordinateManager == null) return;
            
            // 绘制路径
            if (showPath && currentPath != null && currentPath.Count > 0)
            {
                Gizmos.color = pathColor;
                for (int i = 0; i < currentPath.Count - 1; i++)
                {
                    Vector3 start = coordinateManager.CoordinateToWorld(currentPath[i]);
                    Vector3 end = coordinateManager.CoordinateToWorld(currentPath[i + 1]);
                    Gizmos.DrawLine(start, end);
                }
            }
            
            // 绘制范围
            if (showRange && currentRange != null)
            {
                Gizmos.color = rangeColor;
                foreach (var coord in currentRange)
                {
                    Vector3 worldPos = coordinateManager.CoordinateToWorld(coord);
                    Gizmos.DrawWireSphere(worldPos, 0.1f);
                }
            }
            
            // 绘制最后点击的坐标
            if (showClickedTriangle && lastClickedCoordinate != null)
            {
                Gizmos.color = clickedTriangleColor;
                Vector3 worldPos = coordinateManager.CoordinateToWorld(lastClickedCoordinate);
                Gizmos.DrawWireSphere(worldPos, 0.2f);
            }
        }
    }
}