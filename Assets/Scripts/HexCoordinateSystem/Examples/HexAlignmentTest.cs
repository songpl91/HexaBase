using UnityEngine;
using HexCoordinateSystem.Core;
using HexCoordinateSystem.Utils;
using HexCoordinateSystem.Coordinates;

namespace HexCoordinateSystem.Examples
{
    /// <summary>
    /// 六边形对齐方式测试脚本
    /// 用于验证六边形是否正确显示为边对齐（平顶）而不是定点对齐（尖顶）
    /// </summary>
    public class HexAlignmentTest : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private float hexSize = 1.0f;
        [SerializeField] private bool showGrid = true;
        [SerializeField] private bool showSingleHex = true;
        [SerializeField] private bool showLabels = true;
        
        [Header("颜色设置")]
        [SerializeField] private Color gridColor = Color.white;
        [SerializeField] private Color singleHexColor = Color.red;
        
        /// <summary>
        /// 在Scene视图中绘制测试内容
        /// </summary>
        private void OnDrawGizmos()
        {
            if (showSingleHex)
            {
                DrawSingleHexTest();
            }
            
            if (showGrid)
            {
                DrawGridTest();
            }
            
            if (showLabels)
            {
                DrawAlignmentLabels();
            }
        }
        
        /// <summary>
        /// 绘制单个六边形测试
        /// </summary>
        private void DrawSingleHexTest()
        {
            Vector3 center = transform.position;
            
            // 使用 HexDebugger 绘制六边形
            HexDebugger.DrawHexagon(center, hexSize, singleHexColor);
            
            // 绘制中心点
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(center, 0.1f);
            
            // 绘制水平和垂直参考线
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(center + Vector3.left * hexSize * 1.5f, center + Vector3.right * hexSize * 1.5f);
            Gizmos.DrawLine(center + Vector3.down * hexSize * 1.5f, center + Vector3.up * hexSize * 1.5f);
        }
        
        /// <summary>
        /// 绘制网格测试
        /// </summary>
        private void DrawGridTest()
        {
            Vector3 center = transform.position + Vector3.right * hexSize * 3;
            var centerCoord = new OffsetCoordinateOddQ(0, 0);
            
            // 绘制3x3网格
            for (int col = -1; col <= 1; col++)
            {
                for (int row = -1; row <= 1; row++)
                {
                    var coord = new OffsetCoordinateOddQ(col, row);
                    var worldPos = coord.ToWorldPosition(hexSize) + center;
                    
                    HexDebugger.DrawHexagon(worldPos, hexSize * 0.9f, gridColor);
                }
            }
        }
        
        /// <summary>
        /// 绘制对齐方式说明标签
        /// </summary>
        private void DrawAlignmentLabels()
        {
#if UNITY_EDITOR
            Vector3 labelPos = transform.position + Vector3.up * hexSize * 2;
            
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(labelPos, 
                "边对齐六边形测试\n" +
                "• 顶部和底部应该是水平边\n" +
                "• 左右两侧应该是斜边\n" +
                "• 如果看到尖顶朝上，说明还是定点对齐");
            
            // 绘制对齐方式示意图
            Vector3 diagramCenter = transform.position + Vector3.down * hexSize * 2;
            
            // 绘制正确的边对齐示意
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.Label(diagramCenter + Vector3.left * hexSize * 2, "✓ 正确：边对齐（平顶）");
            DrawAlignmentDiagram(diagramCenter + Vector3.left * hexSize * 2 + Vector3.down * 0.5f, true);
            
            // 绘制错误的定点对齐示意
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.Label(diagramCenter + Vector3.right * hexSize * 2, "✗ 错误：定点对齐（尖顶）");
            DrawAlignmentDiagram(diagramCenter + Vector3.right * hexSize * 2 + Vector3.down * 0.5f, false);
#endif
        }
        
        /// <summary>
        /// 绘制对齐方式示意图
        /// </summary>
        /// <param name="center">中心位置</param>
        /// <param name="flatTop">是否为边对齐（平顶）</param>
        private void DrawAlignmentDiagram(Vector3 center, bool flatTop)
        {
            Vector3[] vertices = new Vector3[6];
            float size = 0.3f;
            
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                if (flatTop)
                {
                    angle += 30f * Mathf.Deg2Rad; // 边对齐：加30度偏移
                }
                
                vertices[i] = center + new Vector3(
                    Mathf.Cos(angle) * size,
                    Mathf.Sin(angle) * size,
                    0
                );
            }
            
            // 绘制六边形
            Gizmos.color = flatTop ? Color.green : Color.red;
            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                Gizmos.DrawLine(vertices[i], vertices[next]);
            }
            
            // 标记顶部边/点
            if (flatTop)
            {
                // 标记顶部水平边
                Gizmos.color = Color.yellow;
                Vector3 topEdgeStart = vertices[1];
                Vector3 topEdgeEnd = vertices[2];
                Gizmos.DrawLine(topEdgeStart, topEdgeEnd);
                Gizmos.DrawSphere((topEdgeStart + topEdgeEnd) * 0.5f, 0.05f);
            }
            else
            {
                // 标记顶部尖点
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(vertices[1], 0.05f);
            }
        }
        
        /// <summary>
        /// 运行对齐测试
        /// </summary>
        [ContextMenu("运行对齐测试")]
        public void RunAlignmentTest()
        {
            Debug.Log("=== 六边形对齐方式测试 ===");
            
            // 测试单个坐标的六边形顶点
            var coord = new OffsetCoordinateOddQ(0, 0);
            var worldPos = coord.ToWorldPosition(hexSize);
            
            Debug.Log($"测试坐标: {coord}");
            Debug.Log($"世界位置: {worldPos}");
            
            // 计算六边形顶点（边对齐）
            Vector3[] vertices = new Vector3[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = (i * 60f + 30f) * Mathf.Deg2Rad; // 边对齐
                vertices[i] = worldPos + new Vector3(
                    Mathf.Cos(angle) * hexSize,
                    Mathf.Sin(angle) * hexSize,
                    0
                );
                Debug.Log($"顶点 {i}: {vertices[i]:F3} (角度: {(i * 60f + 30f):F1}°)");
            }
            
            // 检查顶部和底部边是否水平
            Vector3 topEdge = vertices[2] - vertices[1];
            Vector3 bottomEdge = vertices[5] - vertices[4];
            
            bool topIsHorizontal = Mathf.Abs(topEdge.y) < 0.001f;
            bool bottomIsHorizontal = Mathf.Abs(bottomEdge.y) < 0.001f;
            
            Debug.Log($"顶部边是否水平: {topIsHorizontal} (Y差值: {topEdge.y:F6})");
            Debug.Log($"底部边是否水平: {bottomIsHorizontal} (Y差值: {bottomEdge.y:F6})");
            
            if (topIsHorizontal && bottomIsHorizontal)
            {
                Debug.Log("✓ 测试通过：六边形为边对齐（平顶）");
            }
            else
            {
                Debug.LogWarning("✗ 测试失败：六边形不是边对齐");
            }
        }
        
        /// <summary>
        /// 比较不同对齐方式
        /// </summary>
        [ContextMenu("比较对齐方式")]
        public void CompareAlignments()
        {
            Debug.Log("=== 对齐方式比较 ===");
            
            Vector3 center = Vector3.zero;
            
            Debug.Log("定点对齐（尖顶）顶点：");
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector3 vertex = center + new Vector3(
                    Mathf.Cos(angle) * hexSize,
                    Mathf.Sin(angle) * hexSize,
                    0
                );
                Debug.Log($"  顶点 {i}: {vertex:F3} (角度: {(i * 60f):F1}°)");
            }
            
            Debug.Log("边对齐（平顶）顶点：");
            for (int i = 0; i < 6; i++)
            {
                float angle = (i * 60f + 30f) * Mathf.Deg2Rad;
                Vector3 vertex = center + new Vector3(
                    Mathf.Cos(angle) * hexSize,
                    Mathf.Sin(angle) * hexSize,
                    0
                );
                Debug.Log($"  顶点 {i}: {vertex:F3} (角度: {(i * 60f + 30f):F1}°)");
            }
        }
    }
}