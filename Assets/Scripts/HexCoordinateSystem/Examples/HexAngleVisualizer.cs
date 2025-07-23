using UnityEngine;
using HexCoordinateSystem.Coordinates;
using HexCoordinateSystem.Utils;

namespace HexCoordinateSystem.Examples
{
    /// <summary>
    /// 六边形角度计算可视化脚本
    /// 在Scene视图中显示坐标点、连接线、角度信息和六边形网格
    /// </summary>
    public class HexAngleVisualizer : MonoBehaviour
    {
        [Header("坐标设置")]
        [SerializeField] private int fromCol = 1;
        [SerializeField] private int fromRow = 1;
        [SerializeField] private int toCol = -1;
        [SerializeField] private int toRow = -1;
        [SerializeField] private float hexSize = 1.0f;
        
        [Header("可视化设置")]
        [SerializeField] private bool showGrid = true;
        [SerializeField] private int gridRadius = 3;
        [SerializeField] private bool showCoordinateLabels = true;
        [SerializeField] private bool showAngleArc = true;
        [SerializeField] private bool showDirectionArrow = true;
        
        [Header("颜色设置")]
        [SerializeField] private Color gridColor = Color.gray;
        [SerializeField] private Color startPointColor = Color.green;
        [SerializeField] private Color endPointColor = Color.red;
        [SerializeField] private Color connectionLineColor = Color.blue;
        [SerializeField] private Color directionArrowColor = Color.yellow;
        [SerializeField] private Color angleArcColor = Color.magenta;
        
        [Header("计算结果 (只读)")]
        [SerializeField] private float angleDegrees;
        [SerializeField] private float angleRadians;
        [SerializeField] private string directionName;
        [SerializeField] private float worldDistance;
        [SerializeField] private Vector3 fromWorldPos;
        [SerializeField] private Vector3 toWorldPos;
        [SerializeField] private Vector3 directionVector;
        
        private OffsetCoordinateOddQ coordA;
        private OffsetCoordinateOddQ coordB;
        
        void Start()
        {
            UpdateCalculations();
        }
        
        void OnValidate()
        {
            UpdateCalculations();
        }
        
        /// <summary>
        /// 更新所有计算结果
        /// </summary>
        private void UpdateCalculations()
        {
            // 创建偏移坐标
            coordA = new OffsetCoordinateOddQ(fromCol, fromRow);
            coordB = new OffsetCoordinateOddQ(toCol, toRow);
            
            // 计算角度和相关信息
            angleDegrees = HexAngleCalculator.CalculateAngle(coordA, coordB, hexSize);
            angleRadians = HexAngleCalculator.CalculateAngleRadians(coordA, coordB, hexSize);
            directionName = HexAngleCalculator.GetDirectionName(angleDegrees);
            worldDistance = HexAngleCalculator.CalculateWorldDistance(coordA, coordB, hexSize);
            
            // 获取世界坐标位置
            fromWorldPos = coordA.ToWorldPosition(hexSize);
            toWorldPos = coordB.ToWorldPosition(hexSize);
            
            // 获取方向向量
            directionVector = HexAngleCalculator.CalculateDirection(coordA, coordB, hexSize);
            
            // 输出计算结果
            Debug.Log($"=== 角度计算结果 ===");
            Debug.Log($"偏移坐标: A({fromCol},{fromRow}) -> B({toCol},{toRow})");
            Debug.Log($"世界坐标: A{fromWorldPos} -> B{toWorldPos}");
            Debug.Log($"角度: {angleDegrees:F2}° ({angleRadians:F4} 弧度)");
            Debug.Log($"方向: {directionName}");
            Debug.Log($"距离: {worldDistance:F4}");
        }
        
        /// <summary>
        /// 在Scene视图中绘制可视化内容
        /// </summary>
        void OnDrawGizmos()
        {
            if (coordA == null || coordB == null)
            {
                UpdateCalculations();
            }
            
            // 绘制六边形网格
            if (showGrid)
            {
                DrawHexGrid();
            }
            
            // 绘制起始点
            Gizmos.color = startPointColor;
            DrawHexagon(fromWorldPos, hexSize * 0.8f);
            Gizmos.DrawSphere(fromWorldPos, hexSize * 0.1f);
            
            // 绘制目标点
            Gizmos.color = endPointColor;
            DrawHexagon(toWorldPos, hexSize * 0.8f);
            Gizmos.DrawSphere(toWorldPos, hexSize * 0.1f);
            
            // 绘制连接线
            Gizmos.color = connectionLineColor;
            Gizmos.DrawLine(fromWorldPos, toWorldPos);
            
            // 绘制方向箭头
            if (showDirectionArrow)
            {
                DrawDirectionArrow();
            }
            
            // 绘制角度弧线
            if (showAngleArc)
            {
                DrawAngleArc();
            }
            
            // 绘制坐标标签
            if (showCoordinateLabels)
            {
                DrawCoordinateLabels();
            }
        }
        
        /// <summary>
        /// 绘制六边形网格
        /// </summary>
        private void DrawHexGrid()
        {
            Gizmos.color = gridColor;
            
            // 以原点为中心绘制网格
            var centerCoord = new OffsetCoordinateOddQ(0, 0);
            var centerWorld = centerCoord.ToWorldPosition(hexSize);
            
            for (int col = -gridRadius; col <= gridRadius; col++)
            {
                for (int row = -gridRadius; row <= gridRadius; row++)
                {
                    var coord = new OffsetCoordinateOddQ(col, row);
                    var worldPos = coord.ToWorldPosition(hexSize);
                    
                    // 只绘制在指定半径内的六边形
                    if (Vector3.Distance(worldPos, centerWorld) <= gridRadius * hexSize * 1.5f)
                    {
                        DrawHexagon(worldPos, hexSize * 0.95f);
                    }
                }
            }
        }
        
        /// <summary>
        /// 绘制六边形轮廓（边对齐/平顶六边形）
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="size">大小</param>
        private void DrawHexagon(Vector3 center, float size)
        {
            Vector3[] vertices = new Vector3[6];
            
            // 边对齐六边形：第一个顶点在30度角（右上方）
            // 这样顶部和底部的边是水平的
            for (int i = 0; i < 6; i++)
            {
                float angle = (i * 60f + 30) * Mathf.Deg2Rad; // 加30度偏移实现边对齐
                vertices[i] = center + new Vector3(
                    Mathf.Cos(angle) * size,
                    Mathf.Sin(angle) * size,
                    0
                );
            }
            
            // 绘制六边形边线
            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                Gizmos.DrawLine(vertices[i], vertices[next]);
            }
        }
        
        /// <summary>
        /// 绘制方向箭头
        /// </summary>
        private void DrawDirectionArrow()
        {
            Gizmos.color = directionArrowColor;
            
            // 计算箭头位置（从起点向目标点延伸一部分距离）
            Vector3 arrowStart = fromWorldPos;
            Vector3 arrowEnd = fromWorldPos + directionVector * (worldDistance * 0.7f);
            
            // 绘制箭头主线
            Gizmos.DrawLine(arrowStart, arrowEnd);
            
            // 绘制箭头头部
            float arrowHeadSize = hexSize * 0.2f;
            Vector3 perpendicular = Vector3.Cross(directionVector, Vector3.forward).normalized;
            
            Vector3 arrowHead1 = arrowEnd - directionVector * arrowHeadSize + perpendicular * arrowHeadSize * 0.5f;
            Vector3 arrowHead2 = arrowEnd - directionVector * arrowHeadSize - perpendicular * arrowHeadSize * 0.5f;
            
            Gizmos.DrawLine(arrowEnd, arrowHead1);
            Gizmos.DrawLine(arrowEnd, arrowHead2);
        }
        
        /// <summary>
        /// 绘制角度弧线
        /// </summary>
        private void DrawAngleArc()
        {
            Gizmos.color = angleArcColor;
            
            float arcRadius = hexSize * 0.5f;
            int arcSegments = 20;
            
            // 计算起始角度（正东方向为0度）
            float startAngle = 0f;
            float endAngle = angleDegrees;
            
            Vector3 lastPoint = fromWorldPos + new Vector3(arcRadius, 0, 0);
            
            for (int i = 1; i <= arcSegments; i++)
            {
                float t = (float)i / arcSegments;
                float currentAngle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
                
                Vector3 currentPoint = fromWorldPos + new Vector3(
                    Mathf.Cos(currentAngle) * arcRadius,
                    Mathf.Sin(currentAngle) * arcRadius,
                    0
                );
                
                Gizmos.DrawLine(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
            
            // 绘制角度标记线
            Gizmos.DrawLine(fromWorldPos, fromWorldPos + new Vector3(arcRadius, 0, 0));
            Gizmos.DrawLine(fromWorldPos, fromWorldPos + directionVector * arcRadius);
        }
        
        /// <summary>
        /// 绘制坐标标签（在Scene视图中显示文本）
        /// </summary>
        private void DrawCoordinateLabels()
        {
#if UNITY_EDITOR
            // 使用Unity编辑器的Handles来绘制文本
            UnityEditor.Handles.color = Color.white;
            
            // 绘制起始点标签
            Vector3 labelOffset = Vector3.up * hexSize * 0.3f;
            UnityEditor.Handles.Label(fromWorldPos + labelOffset, 
                $"A({fromCol},{fromRow})\n{fromWorldPos:F2}");
            
            // 绘制目标点标签
            UnityEditor.Handles.Label(toWorldPos + labelOffset, 
                $"B({toCol},{toRow})\n{toWorldPos:F2}");
            
            // 绘制角度信息
            Vector3 midPoint = (fromWorldPos + toWorldPos) * 0.5f;
            UnityEditor.Handles.Label(midPoint + labelOffset * 2, 
                $"角度: {angleDegrees:F1}°\n方向: {directionName}\n距离: {worldDistance:F2}");
#endif
        }
        
        /// <summary>
        /// 重置为默认示例坐标
        /// </summary>
        [ContextMenu("重置为示例坐标 A(1,1) B(-1,-1)")]
        public void ResetToExample()
        {
            fromCol = 1;
            fromRow = 1;
            toCol = -1;
            toRow = -1;
            UpdateCalculations();
        }
        
        /// <summary>
        /// 交换起始点和目标点
        /// </summary>
        [ContextMenu("交换起始点和目标点")]
        public void SwapPoints()
        {
            int tempCol = fromCol;
            int tempRow = fromRow;
            
            fromCol = toCol;
            fromRow = toRow;
            toCol = tempCol;
            toRow = tempRow;
            
            UpdateCalculations();
        }
        
        /// <summary>
        /// 测试多个预设坐标对
        /// </summary>
        [ContextMenu("测试预设坐标对")]
        public void TestPresetCoordinates()
        {
            var presets = new (int fromCol, int fromRow, int toCol, int toRow, string description)[]
            {
                (0, 0, 1, 0, "向右"),
                (0, 0, 0, 1, "向上"),
                (0, 0, -1, 0, "向左"),
                (0, 0, 0, -1, "向下"),
                (1, 1, -1, -1, "用户示例"),
                (0, 0, 1, 1, "右上对角"),
                (0, 0, -1, -1, "左下对角")
            };
            
            Debug.Log("=== 预设坐标对测试结果 ===");
            
            foreach (var preset in presets)
            {
                var coordFrom = new OffsetCoordinateOddQ(preset.fromCol, preset.fromRow);
                var coordTo = new OffsetCoordinateOddQ(preset.toCol, preset.toRow);
                
                float angle = HexAngleCalculator.CalculateAngle(coordFrom, coordTo, hexSize);
                string direction = HexAngleCalculator.GetDirectionName(angle);
                
                Debug.Log($"{preset.description}: ({preset.fromCol},{preset.fromRow}) -> ({preset.toCol},{preset.toRow}) " +
                         $"= {angle:F1}° ({direction})");
            }
        }
    }
}