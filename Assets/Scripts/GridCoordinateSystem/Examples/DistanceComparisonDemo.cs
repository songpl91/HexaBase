using UnityEngine;
using System.Collections.Generic;
using GridCoordinateSystem.Coordinates;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GridCoordinateSystem.Examples
{
    /// <summary>
    /// 距离计算对比演示
    /// 可视化展示曼哈顿距离、欧几里得距离和切比雪夫距离的区别
    /// </summary>
    public class DistanceComparisonDemo : MonoBehaviour
    {
        [Header("演示配置")]
        [SerializeField] private int gridWidth = 15;
        [SerializeField] private int gridHeight = 15;
        [SerializeField] private float cellSize = 1.0f;
        [SerializeField] private CartesianCoordinate centerPoint = new CartesianCoordinate(7, 7);
        [SerializeField] private int maxDistance = 5;
        
        [Header("可视化样式")]
        [SerializeField] private Color manhattanColor = Color.red;
        [SerializeField] private Color euclideanColor = Color.green;
        [SerializeField] private Color chebyshevColor = Color.blue;
        [SerializeField] private Color centerColor = Color.yellow;
        [SerializeField] private Color gridLineColor = Color.gray;
        
        [Header("显示控制")]
        [SerializeField] private bool showManhattan = true;
        [SerializeField] private bool showEuclidean = true;
        [SerializeField] private bool showChebyshev = true;
        [SerializeField] private bool showGrid = true;
        [SerializeField] private bool showDistanceText = true;
        
        private GridCoordinateManager _gridManager;
        private Camera _camera;
        
        /// <summary>
        /// 初始化演示
        /// </summary>
        private void Start()
        {
            InitializeDemo();
            SetupCamera();
        }
        
        /// <summary>
        /// 初始化网格管理器和演示参数
        /// </summary>
        private void InitializeDemo()
        {
            // 获取或创建GridCoordinateManager实例
            _gridManager = GridCoordinateManager.Instance;
            
            // 配置网格参数
            _gridManager.GridWidth = gridWidth;
            _gridManager.GridHeight = gridHeight;
            _gridManager.CellSize = cellSize;
            
            // 确保中心点在网格范围内
            centerPoint = new CartesianCoordinate(
                Mathf.Clamp(centerPoint.X, 0, gridWidth - 1),
                Mathf.Clamp(centerPoint.Y, 0, gridHeight - 1)
            );
            
            Debug.Log($"距离对比演示初始化完成");
            Debug.Log($"网格大小: {gridWidth}x{gridHeight}");
            Debug.Log($"中心点: ({centerPoint.X}, {centerPoint.Y})");
            Debug.Log($"最大距离: {maxDistance}");
        }
        
        /// <summary>
        /// 设置摄像机位置
        /// </summary>
        private void SetupCamera()
        {
            _camera = Camera.main;
            if (_camera != null)
            {
                // 计算合适的摄像机位置
                float gridCenterX = (gridWidth - 1) * cellSize * 0.5f;
                float gridCenterZ = (gridHeight - 1) * cellSize * 0.5f;
                float cameraHeight = Mathf.Max(gridWidth, gridHeight) * cellSize * 0.8f;
                
                _camera.transform.position = new Vector3(gridCenterX, cameraHeight, gridCenterZ - 2);
                _camera.transform.LookAt(new Vector3(gridCenterX, 0, gridCenterZ));
            }
        }
        
        /// <summary>
        /// 绘制网格和距离可视化
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_gridManager == null) return;
            
            // 绘制网格线
            if (showGrid)
            {
                DrawGrid();
            }
            
            // 绘制距离区域
            DrawDistanceAreas();
            
            // 绘制中心点
            DrawCenterPoint();
        }
        
        /// <summary>
        /// 绘制网格线
        /// </summary>
        private void DrawGrid()
        {
            Gizmos.color = gridLineColor;
            
            // 垂直线
            for (int x = 0; x <= gridWidth; x++)
            {
                Vector3 start = new Vector3(x * cellSize, 0, 0);
                Vector3 end = new Vector3(x * cellSize, 0, gridHeight * cellSize);
                Gizmos.DrawLine(start, end);
            }
            
            // 水平线
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector3 start = new Vector3(0, 0, y * cellSize);
                Vector3 end = new Vector3(gridWidth * cellSize, 0, y * cellSize);
                Gizmos.DrawLine(start, end);
            }
        }
        
        /// <summary>
        /// 绘制距离区域
        /// </summary>
        private void DrawDistanceAreas()
        {
            for (int distance = 1; distance <= maxDistance; distance++)
            {
                if (showManhattan)
                {
                    DrawManhattanDistance(distance);
                }
                
                if (showEuclidean)
                {
                    DrawEuclideanDistance(distance);
                }
                
                if (showChebyshev)
                {
                    DrawChebyshevDistance(distance);
                }
            }
        }
        
        /// <summary>
        /// 绘制曼哈顿距离区域（菱形）
        /// </summary>
        private void DrawManhattanDistance(int distance)
        {
            Gizmos.color = new Color(manhattanColor.r, manhattanColor.g, manhattanColor.b, 0.3f);
            
            List<Vector3> points = new List<Vector3>();
            
            // 生成菱形的四个顶点
            Vector3 center = GetWorldPosition(centerPoint);
            float offset = distance * cellSize;
            
            points.Add(center + new Vector3(0, 0, offset));      // 上
            points.Add(center + new Vector3(offset, 0, 0));      // 右
            points.Add(center + new Vector3(0, 0, -offset));     // 下
            points.Add(center + new Vector3(-offset, 0, 0));     // 左
            
            // 绘制菱形边框
            Gizmos.color = manhattanColor;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 current = points[i];
                Vector3 next = points[(i + 1) % points.Count];
                Gizmos.DrawLine(current, next);
            }
        }
        
        /// <summary>
        /// 绘制欧几里得距离区域（圆形）
        /// </summary>
        private void DrawEuclideanDistance(int distance)
        {
            Gizmos.color = euclideanColor;
            
            Vector3 center = GetWorldPosition(centerPoint);
            float radius = distance * cellSize;
            
            // 绘制圆形（使用多边形近似）
            int segments = 32;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);
            
            for (int i = 1; i <= segments; i++)
            {
                float angle = (float)i / segments * 2 * Mathf.PI;
                Vector3 currentPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );
                
                Gizmos.DrawLine(prevPoint, currentPoint);
                prevPoint = currentPoint;
            }
        }
        
        /// <summary>
        /// 绘制切比雪夫距离区域（正方形）
        /// </summary>
        private void DrawChebyshevDistance(int distance)
        {
            Gizmos.color = chebyshevColor;
            
            Vector3 center = GetWorldPosition(centerPoint);
            float offset = distance * cellSize;
            
            // 正方形的四个顶点
            Vector3[] corners = new Vector3[]
            {
                center + new Vector3(-offset, 0, -offset), // 左下
                center + new Vector3(offset, 0, -offset),  // 右下
                center + new Vector3(offset, 0, offset),   // 右上
                center + new Vector3(-offset, 0, offset)   // 左上
            };
            
            // 绘制正方形边框
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 current = corners[i];
                Vector3 next = corners[(i + 1) % corners.Length];
                Gizmos.DrawLine(current, next);
            }
        }
        
        /// <summary>
        /// 绘制中心点
        /// </summary>
        private void DrawCenterPoint()
        {
            Gizmos.color = centerColor;
            Vector3 worldPos = GetWorldPosition(centerPoint);
            Gizmos.DrawSphere(worldPos, cellSize * 0.2f);
        }
        
        /// <summary>
        /// 将网格坐标转换为世界坐标
        /// </summary>
        private Vector3 GetWorldPosition(CartesianCoordinate coord)
        {
            return new Vector3(coord.X * cellSize, 0, coord.Y * cellSize);
        }
        
        /// <summary>
        /// 在GUI中显示距离信息
        /// </summary>
        private void OnGUI()
        {
            if (!showDistanceText) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("距离计算对比演示");
            GUILayout.Space(10);
            
            GUILayout.Label($"中心点: ({centerPoint.X}, {centerPoint.Y})");
            GUILayout.Label($"网格大小: {gridWidth} x {gridHeight}");
            GUILayout.Space(10);
            
            // 显示不同距离的示例计算
            var testPoint = new CartesianCoordinate(centerPoint.X + 3, centerPoint.Y + 2);
            
            GUILayout.Label($"示例点: ({testPoint.X}, {testPoint.Y})");
            
            // 创建带颜色的GUI样式
            var manhattanStyle = new GUIStyle(GUI.skin.label);
            manhattanStyle.normal.textColor = manhattanColor;
            var euclideanStyle = new GUIStyle(GUI.skin.label);
            euclideanStyle.normal.textColor = euclideanColor;
            var chebyshevStyle = new GUIStyle(GUI.skin.label);
            chebyshevStyle.normal.textColor = chebyshevColor;
            
            GUILayout.Label($"曼哈顿距离: {centerPoint.ManhattanDistanceTo(testPoint)}", manhattanStyle);
            GUILayout.Label($"欧几里得距离: {centerPoint.EuclideanDistanceTo(testPoint):F2}", euclideanStyle);
            GUILayout.Label($"切比雪夫距离: {centerPoint.ChebyshevDistanceTo(testPoint)}", chebyshevStyle);
            
            GUILayout.Space(10);
            
            // 距离特点说明
            GUILayout.Label("距离特点:");
            GUILayout.Label("• 曼哈顿: 4方向移动 (菱形)", manhattanStyle);
            GUILayout.Label("• 欧几里得: 自由移动 (圆形)", euclideanStyle);
            GUILayout.Label("• 切比雪夫: 8方向移动 (正方形)", chebyshevStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("点击场景更改中心点");
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// 运行时更新中心点（用于交互演示）
        /// </summary>
        private void Update()
        {
            // 鼠标点击更新中心点
            if (Input.GetMouseButtonDown(0))
            {
                UpdateCenterPointFromMouse();
            }
        }
        
        /// <summary>
        /// 根据鼠标位置更新中心点
        /// </summary>
        private void UpdateCenterPointFromMouse()
        {
            if (_camera == null) return;
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 将世界坐标转换为网格坐标
                int gridX = Mathf.RoundToInt(hit.point.x / cellSize);
                int gridY = Mathf.RoundToInt(hit.point.z / cellSize);
                
                // 确保在网格范围内
                gridX = Mathf.Clamp(gridX, 0, gridWidth - 1);
                gridY = Mathf.Clamp(gridY, 0, gridHeight - 1);
                
                centerPoint = new CartesianCoordinate(gridX, gridY);
                
                Debug.Log($"中心点更新为: ({centerPoint.X}, {centerPoint.Y})");
            }
        }
    }
}