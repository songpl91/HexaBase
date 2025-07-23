using System.Collections.Generic;
using UnityEngine;
using GridCoordinateSystem.Coordinates;
using GridCoordinateSystem.Core;

namespace GridCoordinateSystem.Examples
{
    /// <summary>
    /// 四边形网格系统基础示例
    /// 展示网格系统的基本功能和用法
    /// </summary>
    public class GridBasicExample : MonoBehaviour
    {
        [Header("示例设置")]
        [SerializeField] private bool _runOnStart = true;
        [SerializeField] private bool _enableDebugOutput = true;
        
        [Header("测试坐标")]
        [SerializeField] private Vector2Int _testCoordinate1 = new Vector2Int(0, 0);
        [SerializeField] private Vector2Int _testCoordinate2 = new Vector2Int(5, 3);
        
        private GridCoordinateManager _gridManager;
        
        private void Start()
        {
            _gridManager = GridCoordinateManager.Instance;
            
            if (_runOnStart)
            {
                RunBasicExamples();
            }
        }
        
        /// <summary>
        /// 运行基础示例
        /// </summary>
        [ContextMenu("运行基础示例")]
        public void RunBasicExamples()
        {
            if (_gridManager == null)
            {
                Debug.LogError("GridCoordinateManager 未找到！");
                return;
            }
            
            DebugLog("=== 四边形网格系统基础示例 ===");
            
            // 1. 坐标转换示例
            CoordinateConversionExample();
            
            // 2. 邻居查找示例
            NeighborExample();
            
            // 3. 距离计算示例
            DistanceExample();
            
            // 4. 路径查找示例
            PathfindingExample();
            
            // 5. 范围查询示例
            RangeQueryExample();
            
            // 6. 边界检测示例
            BoundaryExample();
            
            DebugLog("=== 示例完成 ===");
        }
        
        /// <summary>
        /// 坐标转换示例
        /// </summary>
        private void CoordinateConversionExample()
        {
            DebugLog("\n--- 坐标转换示例 ---");
            
            var cartesian = new CartesianCoordinate(_testCoordinate1.x, _testCoordinate1.y);
            DebugLog($"直角坐标: {cartesian}");
            
            // 转换为世界坐标
            Vector3 worldPos = _gridManager.CartesianToWorld(cartesian);
            DebugLog($"世界坐标: {worldPos}");
            
            // 转换为索引坐标
            var index = _gridManager.CartesianToIndex(cartesian);
            DebugLog($"索引坐标: {index}");
            
            // 反向转换
            var backToCartesian = _gridManager.WorldToCartesian(worldPos);
            DebugLog($"世界坐标转回直角坐标: {backToCartesian}");
            
            var backFromIndex = _gridManager.IndexToCartesian(index);
            DebugLog($"索引坐标转回直角坐标: {backFromIndex}");
        }
        
        /// <summary>
        /// 邻居查找示例
        /// </summary>
        private void NeighborExample()
        {
            DebugLog("\n--- 邻居查找示例 ---");
            
            var center = new CartesianCoordinate(_testCoordinate1.x, _testCoordinate1.y);
            DebugLog($"中心坐标: {center}");
            
            // 4邻居
            var neighbors4 = center.GetNeighbors4();
            DebugLog($"4邻居: {string.Join(", ", neighbors4)}");
            
            // 8邻居
            var neighbors8 = center.GetNeighbors8();
            DebugLog($"8邻居: {string.Join(", ", neighbors8)}");
            
            // 使用管理器获取邻居
            var managedNeighbors4 = _gridManager.GetNeighbors4(center);
            DebugLog($"管理器4邻居: {string.Join(", ", managedNeighbors4)}");
        }
        
        /// <summary>
        /// 距离计算示例
        /// </summary>
        private void DistanceExample()
        {
            DebugLog("\n--- 距离计算示例 ---");
            
            var coord1 = new CartesianCoordinate(_testCoordinate1.x, _testCoordinate1.y);
            var coord2 = new CartesianCoordinate(_testCoordinate2.x, _testCoordinate2.y);
            
            DebugLog($"坐标1: {coord1}");
            DebugLog($"坐标2: {coord2}");
            
            // 曼哈顿距离
            int manhattanDist = coord1.ManhattanDistance(coord2);
            DebugLog($"曼哈顿距离: {manhattanDist}");
            
            // 欧几里得距离
            float euclideanDist = coord1.EuclideanDistance(coord2);
            DebugLog($"欧几里得距离: {euclideanDist:F2}");
            
            // 切比雪夫距离
            int chebyshevDist = coord1.ChebyshevDistance(coord2);
            DebugLog($"切比雪夫距离: {chebyshevDist}");
            
            // 使用管理器计算距离
            float managedDist = _gridManager.GetDistance(coord1, coord2, GridConstants.DistanceType.Euclidean);
            DebugLog($"管理器欧几里得距离: {managedDist:F2}");
        }
        
        /// <summary>
        /// 路径查找示例
        /// </summary>
        private void PathfindingExample()
        {
            DebugLog("\n--- 路径查找示例 ---");
            
            var start = new CartesianCoordinate(_testCoordinate1.x, _testCoordinate1.y);
            var end = new CartesianCoordinate(_testCoordinate2.x, _testCoordinate2.y);
            
            DebugLog($"起点: {start}");
            DebugLog($"终点: {end}");
            
            // 使用转换器计算路径
            var path = GridCoordinateSystem.Utils.GridConverter.GetLinePath(start, end);
            DebugLog($"直线路径 ({path.Count} 步): {string.Join(" -> ", path)}");
            
            // 使用管理器计算路径
            var managedPath = _gridManager.GetLinePath(start, end);
            DebugLog($"管理器路径 ({managedPath.Count} 步): {string.Join(" -> ", managedPath)}");
        }
        
        /// <summary>
        /// 范围查询示例
        /// </summary>
        private void RangeQueryExample()
        {
            DebugLog("\n--- 范围查询示例 ---");
            
            var center = new CartesianCoordinate(_testCoordinate1.x, _testCoordinate1.y);
            int range = 2;
            
            DebugLog($"中心: {center}, 范围: {range}");
            
            // 矩形范围
            var rectRange = _gridManager.GetRectangleRange(center, range, range);
            DebugLog($"矩形范围 ({rectRange.Count} 个): {string.Join(", ", rectRange)}");
            
            // 圆形范围
            var circleRange = _gridManager.GetCircleRange(center, range);
            DebugLog($"圆形范围 ({circleRange.Count} 个): {string.Join(", ", circleRange)}");
            
            // 曼哈顿距离范围
            var manhattanRange = _gridManager.GetManhattanRange(center, range);
            DebugLog($"曼哈顿范围 ({manhattanRange.Count} 个): {string.Join(", ", manhattanRange)}");
        }
        
        /// <summary>
        /// 边界检测示例
        /// </summary>
        private void BoundaryExample()
        {
            DebugLog("\n--- 边界检测示例 ---");
            
            // 测试不同的坐标
            var testCoords = new CartesianCoordinate[]
            {
                new CartesianCoordinate(0, 0),
                new CartesianCoordinate(5, 5),
                new CartesianCoordinate(-1, 0),
                new CartesianCoordinate(0, -1),
                new CartesianCoordinate(_gridManager.GridWidth, 0),
                new CartesianCoordinate(0, _gridManager.GridHeight)
            };
            
            foreach (var coord in testCoords)
            {
                bool inBounds = _gridManager.IsInBounds(coord);
                bool valid = coord.IsValid();
                DebugLog($"坐标 {coord}: 边界内={inBounds}, 有效={valid}");
            }
        }
        
        /// <summary>
        /// 调试输出
        /// </summary>
        /// <param name="message">消息</param>
        private void DebugLog(string message)
        {
            if (_enableDebugOutput)
            {
                Debug.Log(message);
            }
        }
    }
}