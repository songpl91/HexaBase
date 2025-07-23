using UnityEngine;
using TriangleCoordinateSystem.Core;
using TriangleCoordinateSystem.Coordinates;
using TriangleCoordinateSystem.Utils;

namespace TriangleCoordinateSystem.Utils
{
    /// <summary>
    /// 三角形坐标系统调试和可视化工具
    /// 提供Scene视图中的网格绘制、坐标标签显示等调试功能
    /// </summary>
    public static class TriangleDebugger
    {
        /// <summary>
        /// 是否启用调试模式
        /// </summary>
        public static bool DebugEnabled { get; set; } = true;
        
        /// <summary>
        /// 网格线颜色
        /// </summary>
        public static Color GridColor { get; set; } = Color.gray;
        
        /// <summary>
        /// 向上三角形颜色
        /// </summary>
        public static Color UpwardTriangleColor { get; set; } = Color.blue;
        
        /// <summary>
        /// 向下三角形颜色
        /// </summary>
        public static Color DownwardTriangleColor { get; set; } = Color.red;
        
        /// <summary>
        /// 坐标标签颜色
        /// </summary>
        public static Color LabelColor { get; set; } = Color.white;
        
        /// <summary>
        /// 是否显示坐标标签
        /// </summary>
        public static bool ShowLabels { get; set; } = true;
        
        /// <summary>
        /// 是否显示网格线
        /// </summary>
        public static bool ShowGrid { get; set; } = true;
        
        /// <summary>
        /// 是否显示三角形填充
        /// </summary>
        public static bool ShowTriangleFill { get; set; } = false;
        
        /// <summary>
        /// 绘制三角形网格
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">绘制半径</param>
        /// <param name="triangleSize">三角形大小</param>
        public static void DrawTriangleGrid(TriangleAxialCoordinate center, int radius, float triangleSize = 1.0f)
        {
            if (!DebugEnabled) return;
            
            var coordinates = TriangleConverter.GetCoordinatesInRange(center, radius);
            
            foreach (var coord in coordinates)
            {
                DrawTriangle(coord, triangleSize);
            }
        }
        
        /// <summary>
        /// 绘制单个三角形
        /// </summary>
        /// <param name="coordinate">三角形坐标</param>
        /// <param name="triangleSize">三角形大小</param>
        public static void DrawTriangle(ITriangleCoordinate coordinate, float triangleSize = 1.0f)
        {
            if (!DebugEnabled) return;
            
            Vector3 center = coordinate.ToWorldPosition(triangleSize);
            Vector3[] vertices = GetTriangleVertices(coordinate, triangleSize);
            
            // 绘制三角形边框
            if (ShowGrid)
            {
                Color originalColor = Gizmos.color;
                Gizmos.color = GridColor;
                
                for (int i = 0; i < 3; i++)
                {
                    Gizmos.DrawLine(vertices[i], vertices[(i + 1) % 3]);
                }
                
                Gizmos.color = originalColor;
            }
            
            // 绘制三角形填充
            if (ShowTriangleFill)
            {
                Color fillColor = coordinate.IsUpward() ? UpwardTriangleColor : DownwardTriangleColor;
                fillColor.a = 0.3f;
                
                // 使用Gizmos绘制填充三角形（简化版本）
                Color originalColor = Gizmos.color;
                Gizmos.color = fillColor;
                
                // 绘制三角形的中心点
                Gizmos.DrawSphere(center, triangleSize * 0.1f);
                
                Gizmos.color = originalColor;
            }
            
            // 绘制坐标标签
            if (ShowLabels)
            {
                DrawCoordinateLabel(coordinate, center);
            }
        }
        
        /// <summary>
        /// 绘制坐标路径
        /// </summary>
        /// <param name="path">路径坐标列表</param>
        /// <param name="triangleSize">三角形大小</param>
        /// <param name="pathColor">路径颜色</param>
        public static void DrawPath(System.Collections.Generic.List<TriangleAxialCoordinate> path, float triangleSize = 1.0f, Color? pathColor = null)
        {
            if (!DebugEnabled || path == null || path.Count < 2) return;
            
            Color color = pathColor ?? Color.yellow;
            Color originalColor = Gizmos.color;
            Gizmos.color = color;
            
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = path[i].ToWorldPosition(triangleSize);
                Vector3 end = path[i + 1].ToWorldPosition(triangleSize);
                Gizmos.DrawLine(start, end);
                
                // 绘制路径点
                Gizmos.DrawSphere(start, triangleSize * 0.05f);
            }
            
            // 绘制最后一个点
            if (path.Count > 0)
            {
                Vector3 lastPoint = path[path.Count - 1].ToWorldPosition(triangleSize);
                Gizmos.DrawSphere(lastPoint, triangleSize * 0.05f);
            }
            
            Gizmos.color = originalColor;
        }
        
        /// <summary>
        /// 绘制坐标范围
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <param name="triangleSize">三角形大小</param>
        /// <param name="rangeColor">范围颜色</param>
        public static void DrawRange(TriangleAxialCoordinate center, int radius, float triangleSize = 1.0f, Color? rangeColor = null)
        {
            if (!DebugEnabled) return;
            
            Color color = rangeColor ?? Color.green;
            Color originalColor = Gizmos.color;
            Gizmos.color = color;
            
            var coordinates = TriangleConverter.GetCoordinatesInRange(center, radius);
            
            foreach (var coord in coordinates)
            {
                Vector3 position = coord.ToWorldPosition(triangleSize);
                Gizmos.DrawWireSphere(position, triangleSize * 0.1f);
            }
            
            Gizmos.color = originalColor;
        }
        
        /// <summary>
        /// 绘制邻居连接
        /// </summary>
        /// <param name="coordinate">中心坐标</param>
        /// <param name="triangleSize">三角形大小</param>
        /// <param name="neighborColor">邻居连接颜色</param>
        public static void DrawNeighborConnections(ITriangleCoordinate coordinate, float triangleSize = 1.0f, Color? neighborColor = null)
        {
            if (!DebugEnabled) return;
            
            Color color = neighborColor ?? Color.cyan;
            Color originalColor = Gizmos.color;
            Gizmos.color = color;
            
            Vector3 center = coordinate.ToWorldPosition(triangleSize);
            var neighbors = coordinate.GetNeighbors();
            
            foreach (var neighbor in neighbors)
            {
                Vector3 neighborPos = neighbor.ToWorldPosition(triangleSize);
                Gizmos.DrawLine(center, neighborPos);
            }
            
            Gizmos.color = originalColor;
        }
        
        /// <summary>
        /// 获取三角形顶点坐标
        /// </summary>
        /// <param name="coordinate">三角形坐标</param>
        /// <param name="triangleSize">三角形大小</param>
        /// <returns>三角形顶点数组</returns>
        private static Vector3[] GetTriangleVertices(ITriangleCoordinate coordinate, float triangleSize)
        {
            Vector3 center = coordinate.ToWorldPosition(triangleSize);
            Vector3[] vertices = new Vector3[3];
            
            float height = triangleSize * TriangleConstants.TRIANGLE_HEIGHT_RATIO;
            
            if (coordinate.IsUpward())
            {
                // 向上的三角形
                vertices[0] = center + new Vector3(0, height * 2f / 3f, 0);                    // 顶点
                vertices[1] = center + new Vector3(-triangleSize * 0.5f, -height / 3f, 0);     // 左下
                vertices[2] = center + new Vector3(triangleSize * 0.5f, -height / 3f, 0);      // 右下
            }
            else
            {
                // 向下的三角形
                vertices[0] = center + new Vector3(0, -height * 2f / 3f, 0);                   // 底点
                vertices[1] = center + new Vector3(-triangleSize * 0.5f, height / 3f, 0);      // 左上
                vertices[2] = center + new Vector3(triangleSize * 0.5f, height / 3f, 0);       // 右上
            }
            
            return vertices;
        }
        
        /// <summary>
        /// 绘制坐标标签
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <param name="position">世界位置</param>
        private static void DrawCoordinateLabel(ITriangleCoordinate coordinate, Vector3 position)
        {
#if UNITY_EDITOR
            var style = new GUIStyle();
            style.normal.textColor = LabelColor;
            style.fontSize = 10;
            style.alignment = TextAnchor.MiddleCenter;
            
            string label = $"{coordinate.ToAxial().Q},{coordinate.ToAxial().R}";
            if (coordinate.IsUpward())
            {
                label += "↑";
            }
            else
            {
                label += "↓";
            }
            
            UnityEditor.Handles.Label(position, label, style);
#endif
        }
        
        /// <summary>
        /// 验证坐标转换的正确性
        /// </summary>
        /// <param name="coordinate">要验证的坐标</param>
        /// <param name="triangleSize">三角形大小</param>
        /// <returns>验证结果</returns>
        public static bool ValidateCoordinate(ITriangleCoordinate coordinate, float triangleSize = 1.0f)
        {
            try
            {
                // 测试坐标转换
                var axial = coordinate.ToAxial();
                var cube = coordinate.ToCube();
                var worldPos = coordinate.ToWorldPosition(triangleSize);
                
                // 验证立方坐标的有效性
                if (!cube.IsValid())
                {
                    Debug.LogError($"无效的立方坐标: {cube}");
                    return false;
                }
                
                // 验证坐标转换的一致性
                var backToAxial = cube.ToAxial();
                if (!axial.Equals(backToAxial))
                {
                    Debug.LogError($"坐标转换不一致: {axial} != {backToAxial}");
                    return false;
                }
                
                // 验证邻居查找
                var neighbors = coordinate.GetNeighbors();
                if (neighbors.Count != TriangleConstants.NEIGHBOR_COUNT)
                {
                    Debug.LogError($"邻居数量错误: 期望{TriangleConstants.NEIGHBOR_COUNT}，实际{neighbors.Count}");
                    return false;
                }
                
                Debug.Log($"坐标验证通过: {coordinate}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"坐标验证失败: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="coordinate">坐标</param>
        public static void LogCoordinateInfo(ITriangleCoordinate coordinate)
        {
            if (!DebugEnabled) return;
            
            var axial = coordinate.ToAxial();
            var cube = coordinate.ToCube();
            var worldPos = coordinate.ToWorldPosition();
            var neighbors = coordinate.GetNeighbors();
            
            Debug.Log($"=== 三角形坐标信息 ===\n" +
                     $"轴向坐标: {axial}\n" +
                     $"立方坐标: {cube}\n" +
                     $"世界坐标: {worldPos}\n" +
                     $"朝向: {(coordinate.IsUpward() ? "向上" : "向下")}\n" +
                     $"邻居数量: {neighbors.Count}\n" +
                     $"有效性: {coordinate.IsValid()}");
        }
    }
}