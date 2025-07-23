using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem;
using HexCoordinateSystem.Core;
using HexCoordinateSystem.Coordinates;
using HexCoordinateSystem.Utils;

namespace HexCoordinateSystem.Examples
{
    /// <summary>
    /// 六边形坐标系统使用示例
    /// 展示如何使用各种坐标系统和功能
    /// </summary>
    public class HexCoordinateExample : MonoBehaviour
    {
        [Header("示例配置")]
        [SerializeField] private bool runExamplesOnStart = true;
        [SerializeField] private bool enableVisualization = true;
        [SerializeField] private Transform targetTransform;
        
        [Header("测试参数")]
        [SerializeField] private int testRange = 3;
        [SerializeField] private AxialCoordinate startCoordinate = new AxialCoordinate(0, 0);
        [SerializeField] private AxialCoordinate endCoordinate = new AxialCoordinate(3, 2);
        
        private HexCoordinateManager hexManager;
        private List<AxialCoordinate> currentPath;
        
        private void Start()
        {
            hexManager = HexCoordinateManager.Instance;
            
            if (runExamplesOnStart)
            {
                RunAllExamples();
            }
        }
        
        /// <summary>
        /// 运行所有示例
        /// </summary>
        public void RunAllExamples()
        {
            Debug.Log("=== 六边形坐标系统使用示例 ===");
            
            BasicCoordinateExample();
            CoordinateConversionExample();
            DistanceAndPathExample();
            NeighborExample();
            RangeExample();
            PerformanceExample();
            
            Debug.Log("=== 示例运行完成 ===");
        }
        
        /// <summary>
        /// 基础坐标使用示例
        /// </summary>
        private void BasicCoordinateExample()
        {
            Debug.Log("--- 基础坐标使用示例 ---");
            
            // 创建不同类型的坐标
            var axial = hexManager.CreateAxial(2, -1);
            var cube = hexManager.CreateCube(2, -1, -1);
            var offsetOddQ = hexManager.CreateOffsetOddQ(2, 0);
            var offsetEvenQ = hexManager.CreateOffsetEvenQ(2, -1);
            var doubled = hexManager.CreateDoubled(3, -1);
            
            Debug.Log($"轴向坐标: {axial}");
            Debug.Log($"立方坐标: {cube}");
            Debug.Log($"奇数列偏移坐标: {offsetOddQ}");
            Debug.Log($"偶数列偏移坐标: {offsetEvenQ}");
            Debug.Log($"双宽坐标: {doubled}");
            
            // 验证坐标有效性
            Debug.Log($"轴向坐标有效: {axial.IsValid()}");
            Debug.Log($"立方坐标有效: {cube.IsValid()}");
            
            // 获取世界坐标
            var worldPos = hexManager.AxialToWorld(axial);
            Debug.Log($"轴向坐标 {axial} 的世界坐标: {worldPos}");
        }
        
        /// <summary>
        /// 坐标转换示例
        /// </summary>
        private void CoordinateConversionExample()
        {
            Debug.Log("--- 坐标转换示例 ---");
            
            var originalAxial = hexManager.CreateAxial(3, -2);
            Debug.Log($"原始轴向坐标: {originalAxial}");
            
            // 转换为其他坐标系统
            var cube = originalAxial.ToCube();
            var offsetOddQ = OffsetCoordinateOddQ.FromAxial(originalAxial);
            var offsetEvenQ = OffsetCoordinateEvenQ.FromAxial(originalAxial);
            var doubled = originalAxial.ToDoubled();
            
            Debug.Log($"转换为立方坐标: {cube}");
            Debug.Log($"转换为奇数列偏移坐标: {offsetOddQ}");
            Debug.Log($"转换为偶数列偏移坐标: {offsetEvenQ}");
            Debug.Log($"转换为双宽坐标: {doubled}");
            
            // 验证转换的正确性（转换回轴向坐标）
            var backToAxial1 = cube.ToAxial();
            var backToAxial2 = offsetOddQ.ToAxial();
            var backToAxial3 = offsetEvenQ.ToAxial();
            var backToAxial4 = doubled.ToAxial();
            
            Debug.Log($"立方->轴向: {backToAxial1}, 正确: {originalAxial.Equals(backToAxial1)}");
            Debug.Log($"奇数列偏移->轴向: {backToAxial2}, 正确: {originalAxial.Equals(backToAxial2)}");
            Debug.Log($"偶数列偏移->轴向: {backToAxial3}, 正确: {originalAxial.Equals(backToAxial3)}");
            Debug.Log($"双宽->轴向: {backToAxial4}, 正确: {originalAxial.Equals(backToAxial4)}");
        }
        
        /// <summary>
        /// 距离和路径示例
        /// </summary>
        private void DistanceAndPathExample()
        {
            Debug.Log("--- 距离和路径示例 ---");
            
            var start = startCoordinate;
            var end = endCoordinate;
            
            // 计算距离
            int distance = hexManager.GetDistance(start, end);
            Debug.Log($"从 {start} 到 {end} 的距离: {distance}");
            
            // 获取路径
            currentPath = hexManager.GetLinePath(start, end);
            Debug.Log($"路径 (共 {currentPath.Count} 步):");
            for (int i = 0; i < currentPath.Count; i++)
            {
                Debug.Log($"  步骤 {i}: {currentPath[i]}");
            }
            
            // 验证路径长度
            Debug.Log($"路径长度验证: 期望 {distance + 1}, 实际 {currentPath.Count}, 正确: {currentPath.Count == distance + 1}");
        }
        
        /// <summary>
        /// 邻居坐标示例
        /// </summary>
        private void NeighborExample()
        {
            Debug.Log("--- 邻居坐标示例 ---");
            
            var center = hexManager.CreateAxial(0, 0);
            var neighbors = hexManager.GetNeighbors(center);
            
            Debug.Log($"坐标 {center} 的邻居 (共 {neighbors.Count} 个):");
            for (int i = 0; i < neighbors.Count; i++)
            {
                var neighbor = neighbors[i];
                var distance = hexManager.GetDistance(center, neighbor);
                Debug.Log($"  方向 {i}: {neighbor}, 距离: {distance}");
            }
            
            // 归还列表到对象池
            hexManager.ReturnAxialList(neighbors);
        }
        
        /// <summary>
        /// 范围坐标示例
        /// </summary>
        private void RangeExample()
        {
            Debug.Log("--- 范围坐标示例 ---");
            
            var center = hexManager.CreateAxial(0, 0);
            var coordinatesInRange = hexManager.GetCoordinatesInRange(center, testRange);
            
            Debug.Log($"以 {center} 为中心，半径 {testRange} 范围内的坐标 (共 {coordinatesInRange.Count} 个):");
            
            // 按距离分组显示
            for (int r = 0; r <= testRange; r++)
            {
                var coordsAtDistance = new List<AxialCoordinate>();
                foreach (var coord in coordinatesInRange)
                {
                    if (hexManager.GetDistance(center, coord) == r)
                    {
                        coordsAtDistance.Add(coord);
                    }
                }
                
                if (coordsAtDistance.Count > 0)
                {
                    Debug.Log($"  距离 {r}: {coordsAtDistance.Count} 个坐标");
                    foreach (var coord in coordsAtDistance)
                    {
                        Debug.Log($"    {coord}");
                    }
                }
            }
            
            // 归还列表到对象池
            hexManager.ReturnAxialList(coordinatesInRange);
        }
        
        /// <summary>
        /// 性能示例
        /// </summary>
        private void PerformanceExample()
        {
            Debug.Log("--- 性能示例 ---");
            
            // 输出性能统计
            hexManager.LogPerformanceStats();
            
            // 运行验证测试
            hexManager.RunValidationTests();
            
            // 运行基准测试
            hexManager.RunBenchmarkTests();
        }
        
        /// <summary>
        /// 世界坐标转换示例
        /// </summary>
        public void WorldCoordinateExample()
        {
            Debug.Log("--- 世界坐标转换示例 ---");
            
            if (targetTransform != null)
            {
                // 将目标物体的世界坐标转换为六边形坐标
                var worldPos = targetTransform.position;
                var hexCoord = hexManager.WorldToAxial(worldPos);
                
                Debug.Log($"目标物体世界坐标: {worldPos}");
                Debug.Log($"对应的六边形坐标: {hexCoord}");
                
                // 将六边形坐标转换回世界坐标
                var backToWorld = hexManager.AxialToWorld(hexCoord);
                Debug.Log($"转换回的世界坐标: {backToWorld}");
                
                // 计算误差
                var error = Vector3.Distance(worldPos, backToWorld);
                Debug.Log($"转换误差: {error}");
            }
            else
            {
                Debug.LogWarning("未设置目标Transform，跳过世界坐标转换示例");
            }
        }
        
        /// <summary>
        /// 交互式示例：点击获取坐标
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 将鼠标点击位置转换为世界坐标
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var worldPos = hit.point;
                    var hexCoord = hexManager.WorldToAxial(worldPos);
                    
                    Debug.Log($"点击位置: {worldPos} -> 六边形坐标: {hexCoord}");
                    
                    // 如果有当前路径，显示到点击位置的距离
                    if (currentPath != null && currentPath.Count > 0)
                    {
                        var startCoord = currentPath[0];
                        var distance = hexManager.GetDistance(startCoord, hexCoord);
                        Debug.Log($"从路径起点 {startCoord} 到点击位置的距离: {distance}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 在Scene视图中绘制可视化信息
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!enableVisualization || !Application.isPlaying)
                return;
            
            // 绘制当前路径
            if (currentPath != null && currentPath.Count > 1)
            {
                var worldPath = new List<Vector3>();
                foreach (var coord in currentPath)
                {
                    worldPath.Add(hexManager.AxialToWorld(coord));
                }
                
                // 绘制路径线
                Gizmos.color = Color.red;
                for (int i = 0; i < worldPath.Count - 1; i++)
                {
                    Gizmos.DrawLine(worldPath[i], worldPath[i + 1]);
                }
                
                // 绘制起点和终点
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(worldPath[0], 0.2f);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(worldPath[worldPath.Count - 1], 0.2f);
            }
            
            // 绘制目标物体对应的六边形坐标
            if (targetTransform != null)
            {
                var hexCoord = hexManager.WorldToAxial(targetTransform.position);
                var hexWorldPos = hexManager.AxialToWorld(hexCoord);
                
                Gizmos.color = Color.yellow;
                HexDebugger.DrawHexagon(hexWorldPos, hexManager.HexSize, Color.yellow);
            }
        }
        
        #region 公共接口方法
        
        /// <summary>
        /// 设置起始坐标
        /// </summary>
        /// <param name="q">q坐标</param>
        /// <param name="r">r坐标</param>
        public void SetStartCoordinate(int q, int r)
        {
            startCoordinate = new AxialCoordinate(q, r);
            Debug.Log($"起始坐标设置为: {startCoordinate}");
        }
        
        /// <summary>
        /// 设置结束坐标
        /// </summary>
        /// <param name="q">q坐标</param>
        /// <param name="r">r坐标</param>
        public void SetEndCoordinate(int q, int r)
        {
            endCoordinate = new AxialCoordinate(q, r);
            Debug.Log($"结束坐标设置为: {endCoordinate}");
        }
        
        /// <summary>
        /// 重新计算路径
        /// </summary>
        public void RecalculatePath()
        {
            if (hexManager != null)
            {
                // 归还之前的路径列表
                if (currentPath != null)
                {
                    hexManager.ReturnAxialList(currentPath);
                }
                
                currentPath = hexManager.GetLinePath(startCoordinate, endCoordinate);
                Debug.Log($"路径已重新计算，共 {currentPath.Count} 步");
            }
        }
        
        /// <summary>
        /// 清理资源
        /// </summary>
        private void OnDestroy()
        {
            if (currentPath != null && hexManager != null)
            {
                hexManager.ReturnAxialList(currentPath);
            }
        }
        
        #endregion
    }
}