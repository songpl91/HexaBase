using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;
using HexCoordinateSystem.Coordinates;

namespace HexCoordinateSystem.Utils
{
    /// <summary>
    /// 六边形坐标系统调试工具
    /// 提供调试信息输出和可视化功能
    /// </summary>
    public static class HexDebugger
    {
        #region 调试信息输出
        
        /// <summary>
        /// 输出坐标详细信息
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <param name="label">标签</param>
        public static void LogCoordinateInfo(IHexCoordinate coordinate, string label = "坐标")
        {
            if (coordinate == null)
            {
                Debug.Log($"{label}: null");
                return;
            }
            
            var axial = coordinate.ToAxial();
            var cube = coordinate.ToCube();
            var world = coordinate.ToWorldPosition();
            
            Debug.Log($"{label} 详细信息:\n" +
                     $"  类型: {coordinate.GetType().Name}\n" +
                     $"  轴向: {axial}\n" +
                     $"  立方: {cube}\n" +
                     $"  世界: {world}\n" +
                     $"  有效: {coordinate.IsValid()}\n" +
                     $"  哈希: {coordinate.GetHashCode()}");
        }
        
        /// <summary>
        /// 输出坐标列表信息
        /// </summary>
        /// <param name="coordinates">坐标列表</param>
        /// <param name="label">标签</param>
        public static void LogCoordinateList(IEnumerable<IHexCoordinate> coordinates, string label = "坐标列表")
        {
            if (coordinates == null)
            {
                Debug.Log($"{label}: null");
                return;
            }
            
            var list = new List<IHexCoordinate>(coordinates);
            Debug.Log($"{label} (共 {list.Count} 个):");
            
            for (int i = 0; i < list.Count; i++)
            {
                var coord = list[i];
                if (coord != null)
                {
                    Debug.Log($"  [{i}] {coord.ToAxial()} -> {coord.ToWorldPosition()}");
                }
                else
                {
                    Debug.Log($"  [{i}] null");
                }
            }
        }
        
        /// <summary>
        /// 输出距离计算信息
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        public static void LogDistanceInfo(IHexCoordinate from, IHexCoordinate to)
        {
            if (from == null || to == null)
            {
                Debug.Log("距离计算: 坐标为null");
                return;
            }
            
            int distance = from.DistanceTo(to);
            var fromAxial = from.ToAxial();
            var toAxial = to.ToAxial();
            
            Debug.Log($"距离计算:\n" +
                     $"  从: {fromAxial}\n" +
                     $"  到: {toAxial}\n" +
                     $"  距离: {distance}");
        }
        
        /// <summary>
        /// 输出路径信息
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="label">标签</param>
        public static void LogPathInfo(IEnumerable<IHexCoordinate> path, string label = "路径")
        {
            if (path == null)
            {
                Debug.Log($"{label}: null");
                return;
            }
            
            var pathList = new List<IHexCoordinate>(path);
            Debug.Log($"{label} (共 {pathList.Count} 步):");
            
            for (int i = 0; i < pathList.Count; i++)
            {
                var coord = pathList[i];
                if (coord != null)
                {
                    Debug.Log($"  步骤 {i}: {coord.ToAxial()}");
                }
            }
        }
        
        #endregion
        
        #region 性能调试
        
        /// <summary>
        /// 测试坐标转换性能
        /// </summary>
        /// <param name="testCount">测试次数</param>
        public static void BenchmarkConversions(int testCount = 10000)
        {
            var testCoords = new List<AxialCoordinate>();
            for (int i = 0; i < testCount; i++)
            {
                testCoords.Add(new AxialCoordinate(Random.Range(-50, 51), Random.Range(-50, 51)));
            }
            
            // 测试轴向到立方转换
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < testCount; i++)
            {
                var cube = testCoords[i].ToCube();
            }
            stopwatch.Stop();
            Debug.Log($"轴向到立方转换 {testCount} 次耗时: {stopwatch.ElapsedMilliseconds}ms");
            
            // 测试轴向到世界坐标转换
            stopwatch.Restart();
            for (int i = 0; i < testCount; i++)
            {
                var world = testCoords[i].ToWorldPosition();
            }
            stopwatch.Stop();
            Debug.Log($"轴向到世界坐标转换 {testCount} 次耗时: {stopwatch.ElapsedMilliseconds}ms");
            
            // 测试距离计算
            stopwatch.Restart();
            for (int i = 0; i < testCount - 1; i++)
            {
                var distance = testCoords[i].DistanceTo(testCoords[i + 1]);
            }
            stopwatch.Stop();
            Debug.Log($"距离计算 {testCount - 1} 次耗时: {stopwatch.ElapsedMilliseconds}ms");
        }
        
        /// <summary>
        /// 测试对象池性能
        /// </summary>
        /// <param name="testCount">测试次数</param>
        public static void BenchmarkObjectPool(int testCount = 1000)
        {
            // 测试不使用对象池
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < testCount; i++)
            {
                var list = new List<AxialCoordinate>(6);
                // 模拟使用
                for (int j = 0; j < 6; j++)
                {
                    list.Add(new AxialCoordinate(j, j));
                }
            }
            stopwatch.Stop();
            Debug.Log($"不使用对象池创建 {testCount} 个列表耗时: {stopwatch.ElapsedMilliseconds}ms");
            
            // 测试使用对象池
            stopwatch.Restart();
            for (int i = 0; i < testCount; i++)
            {
                var list = HexObjectPool.GetAxialList(6);
                // 模拟使用
                for (int j = 0; j < 6; j++)
                {
                    list.Add(new AxialCoordinate(j, j));
                }
                HexObjectPool.ReturnAxialList(list);
            }
            stopwatch.Stop();
            Debug.Log($"使用对象池创建 {testCount} 个列表耗时: {stopwatch.ElapsedMilliseconds}ms");
            
            Debug.Log(HexObjectPool.GetPoolStatus());
        }
        
        #endregion
        
        #region 坐标验证
        
        /// <summary>
        /// 验证坐标系统转换的正确性
        /// </summary>
        /// <param name="testCount">测试次数</param>
        public static void ValidateConversions(int testCount = 100)
        {
            int errorCount = 0;
            
            for (int i = 0; i < testCount; i++)
            {
                var originalAxial = new AxialCoordinate(Random.Range(-20, 21), Random.Range(-20, 21));
                
                // 测试轴向 -> 立方 -> 轴向
                var cube = originalAxial.ToCube();
                var backToAxial = cube.ToAxial();
                if (!originalAxial.Equals(backToAxial))
                {
                    Debug.LogError($"轴向<->立方转换错误: {originalAxial} -> {cube} -> {backToAxial}");
                    errorCount++;
                }
                
                // 测试轴向 -> 偏移 -> 轴向
                var offset = OffsetCoordinateOddQ.FromAxial(originalAxial);
                var backToAxialFromOffset = offset.ToAxial();
                if (!originalAxial.Equals(backToAxialFromOffset))
                {
                    Debug.LogError($"轴向<->偏移转换错误: {originalAxial} -> {offset} -> {backToAxialFromOffset}");
                    errorCount++;
                }
                
                // 测试轴向 -> 双宽 -> 轴向
                var doubled = originalAxial.ToDoubled();
                var backToAxialFromDoubled = doubled.ToAxial();
                if (!originalAxial.Equals(backToAxialFromDoubled))
                {
                    Debug.LogError($"轴向<->双宽转换错误: {originalAxial} -> {doubled} -> {backToAxialFromDoubled}");
                    errorCount++;
                }
            }
            
            if (errorCount == 0)
            {
                Debug.Log($"坐标转换验证通过! 测试了 {testCount} 个坐标");
            }
            else
            {
                Debug.LogError($"坐标转换验证失败! {testCount} 个测试中有 {errorCount} 个错误");
            }
        }
        
        /// <summary>
        /// 验证距离计算的正确性
        /// </summary>
        /// <param name="testCount">测试次数</param>
        public static void ValidateDistances(int testCount = 100)
        {
            int errorCount = 0;
            
            for (int i = 0; i < testCount; i++)
            {
                var coord1 = new AxialCoordinate(Random.Range(-10, 11), Random.Range(-10, 11));
                var coord2 = new AxialCoordinate(Random.Range(-10, 11), Random.Range(-10, 11));
                
                // 测试距离对称性
                int distance1 = coord1.DistanceTo(coord2);
                int distance2 = coord2.DistanceTo(coord1);
                if (distance1 != distance2)
                {
                    Debug.LogError($"距离计算不对称: {coord1} 到 {coord2} = {distance1}, 反向 = {distance2}");
                    errorCount++;
                }
                
                // 测试自身距离为0
                if (coord1.DistanceTo(coord1) != 0)
                {
                    Debug.LogError($"自身距离不为0: {coord1}");
                    errorCount++;
                }
                
                // 测试三角不等式
                var coord3 = new AxialCoordinate(Random.Range(-10, 11), Random.Range(-10, 11));
                int dist12 = coord1.DistanceTo(coord2);
                int dist23 = coord2.DistanceTo(coord3);
                int dist13 = coord1.DistanceTo(coord3);
                if (dist13 > dist12 + dist23)
                {
                    Debug.LogError($"违反三角不等式: {coord1}->{coord2}->{coord3}, {dist13} > {dist12} + {dist23}");
                    errorCount++;
                }
            }
            
            if (errorCount == 0)
            {
                Debug.Log($"距离计算验证通过! 测试了 {testCount} 个坐标对");
            }
            else
            {
                Debug.LogError($"距离计算验证失败! {testCount} 个测试中有 {errorCount} 个错误");
            }
        }
        
        #endregion
        
        #region 可视化辅助
        
        /// <summary>
        /// 在Scene视图中绘制六边形网格
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <param name="hexSize">六边形大小</param>
        /// <param name="color">颜色</param>
        public static void DrawHexGrid(AxialCoordinate center, int radius, float hexSize = 1.0f, Color color = default)
        {
            if (color == default)
                color = Color.white;
            
            var coordinates = AxialCoordinate.GetCoordinatesInRange(center, radius);
            
            foreach (var coord in coordinates)
            {
                DrawHexagon(coord.ToWorldPosition(hexSize), hexSize, color);
            }
        }
        
        /// <summary>
        /// 在Scene视图中绘制单个六边形（边对齐/平顶六边形）
        /// </summary>
        /// <param name="center">中心世界坐标</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        public static void DrawHexagon(Vector3 center, float size, Color color)
        {
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            
            Vector3[] vertices = new Vector3[6];
            // 边对齐六边形：第一个顶点在30度角（右上方）
            // 这样顶部和底部的边是水平的
            for (int i = 0; i < 6; i++)
            {
                float angle = (60 * i + 30) * Mathf.Deg2Rad; // 加30度偏移实现边对齐
                vertices[i] = center + new Vector3(
                    size * Mathf.Cos(angle),
                    0,
                    size * Mathf.Sin(angle)
                );
            }
            
            // 绘制六边形边框
            for (int i = 0; i < 6; i++)
            {
                Gizmos.DrawLine(vertices[i], vertices[(i + 1) % 6]);
            }
            
            Gizmos.color = oldColor;
        }
        
        /// <summary>
        /// 在Scene视图中绘制坐标路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="hexSize">六边形大小</param>
        /// <param name="color">颜色</param>
        public static void DrawPath(IEnumerable<IHexCoordinate> path, float hexSize = 1.0f, Color color = default)
        {
            if (color == default)
                color = Color.red;
            
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            
            var pathList = new List<IHexCoordinate>(path);
            for (int i = 0; i < pathList.Count - 1; i++)
            {
                var from = pathList[i].ToWorldPosition(hexSize);
                var to = pathList[i + 1].ToWorldPosition(hexSize);
                Gizmos.DrawLine(from, to);
                
                // 绘制方向箭头
                var direction = (to - from).normalized;
                var arrowHead1 = to - direction * 0.3f + Vector3.Cross(direction, Vector3.up) * 0.1f;
                var arrowHead2 = to - direction * 0.3f - Vector3.Cross(direction, Vector3.up) * 0.1f;
                Gizmos.DrawLine(to, arrowHead1);
                Gizmos.DrawLine(to, arrowHead2);
            }
            
            Gizmos.color = oldColor;
        }
        
        /// <summary>
        /// 在Scene视图中绘制坐标标签
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <param name="showAxial">显示轴向坐标</param>
        /// <param name="showCube">显示立方坐标</param>
        public static void DrawCoordinateLabel(IHexCoordinate coordinate, float hexSize = 1.0f, 
                                             bool showAxial = true, bool showCube = false)
        {
            if (coordinate == null) return;
            
            var worldPos = coordinate.ToWorldPosition(hexSize);
            var axial = coordinate.ToAxial();
            var cube = coordinate.ToCube();
            
            string label = "";
            if (showAxial)
            {
                label += $"({axial.q},{axial.r})";
            }
            if (showCube)
            {
                if (label.Length > 0) label += "\n";
                label += $"({cube.x},{cube.y},{cube.z})";
            }
            
#if UNITY_EDITOR
            UnityEditor.Handles.Label(worldPos, label);
#endif
        }
        
        #endregion
    }
}