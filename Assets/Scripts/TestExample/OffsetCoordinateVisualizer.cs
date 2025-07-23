using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 偏移坐标计算公式可视化演示工具
/// 用于直观地展示偏移坐标的计算过程和原理
/// </summary>
public class OffsetCoordinateVisualizer : MonoBehaviour
{
    [Header("网格设置")]
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float hexSize = 1.0f;
    
    [Header("演示坐标")]
    [SerializeField] private int demoQ = 3;  // Q = 列坐标 (相当于X轴，向右)
    [SerializeField] private int demoR = 2;  // R = 行坐标 (相当于Y轴，向上)
    
    [Header("显示选项")]
    [SerializeField] private bool showGrid = true;
    [SerializeField] private bool showCoordinates = true;
    [SerializeField] private bool showCalculation = true;
    [SerializeField] private bool showWorldPosition = true;
    
    [Header("颜色设置")]
    [SerializeField] private Color evenColumnColor = Color.white;
    [SerializeField] private Color oddColumnColor = Color.yellow;
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private Color textColor = Color.black;
    
    /// <summary>
    /// 偏移坐标结构（简化版，用于演示）
    /// </summary>
    [System.Serializable]
    public struct OffsetCoordinate
    {
        public int q, r;
        
        public OffsetCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
        
        /// <summary>
        /// 解释q和r的含义（教学用）
        /// </summary>
        public static void ExplainQRMeaning()
        {
            Debug.Log("=== Q和R的含义解释 ===");
            Debug.Log("Q = 列坐标 (Column)");
            Debug.Log("  - 在偏移坐标中，Q相当于X轴");
            Debug.Log("  - Q增加 = 向右移动");
            Debug.Log("  - Q减少 = 向左移动");
            Debug.Log("");
            Debug.Log("R = 行坐标 (Row)");
            Debug.Log("  - 在偏移坐标中，R相当于Y轴");
            Debug.Log("  - R增加 = 向上移动");
            Debug.Log("  - R减少 = 向下移动");
            Debug.Log("");
            Debug.Log("为什么不直接用X,Y？");
            Debug.Log("  - Q,R是六边形坐标系统的国际标准命名");
            Debug.Log("  - 避免与传统方形坐标系统混淆");
            Debug.Log("  - 在轴向坐标中，Q,R表示六边形的特殊方向");
            Debug.Log("");
            Debug.Log("记忆方法：");
            Debug.Log("  - Q = Quest(探索) = 向右探索");
            Debug.Log("  - R = Row(行) = 行数向上");
            Debug.Log("=== 解释结束 ===");
        }
        
        /// <summary>
        /// 转换为轴向坐标（详细步骤演示）
        /// </summary>
        public AxialCoordinate ToAxialWithSteps(out string[] calculationSteps)
        {
            List<string> steps = new List<string>();
            
            steps.Add($"输入偏移坐标: ({q}, {r})");
            steps.Add($"步骤1: 判断列的奇偶性");
            
            int qMod = q & 1;  // 等价于 q % 2
            steps.Add($"  q & 1 = {q} & 1 = {qMod} ({(qMod == 0 ? "偶数列" : "奇数列")})");
            
            steps.Add($"步骤2: 计算偏移量");
            int offset = (q - qMod) / 2;
            steps.Add($"  偏移量 = (q - (q & 1)) / 2 = ({q} - {qMod}) / 2 = {offset}");
            
            steps.Add($"步骤3: 计算轴向坐标");
            int axialQ = q;
            int axialR = r - offset;
            steps.Add($"  轴向Q = q = {axialQ}");
            steps.Add($"  轴向R = r - 偏移量 = {r} - {offset} = {axialR}");
            
            steps.Add($"结果: 轴向坐标({axialQ}, {axialR})");
            
            calculationSteps = steps.ToArray();
            return new AxialCoordinate(axialQ, axialR);
        }
        
        /// <summary>
        /// 转换为世界坐标（详细步骤演示）
        /// </summary>
        public Vector3 ToWorldPositionWithSteps(float hexSize, out string[] calculationSteps)
        {
            List<string> steps = new List<string>();
            
            steps.Add($"输入偏移坐标: ({q}, {r})");
            steps.Add($"六边形尺寸: {hexSize}");
            
            float hexHeight = Mathf.Sqrt(3.0f) * hexSize;
            steps.Add($"步骤1: 计算六边形高度");
            steps.Add($"  hexHeight = √3 × hexSize = √3 × {hexSize} = {hexHeight:F3}");
            
            steps.Add($"步骤2: 调整R坐标");
            int adjustedR = r - q / 2;
            steps.Add($"  adjustedR = r - q/2 = {r} - {q}/2 = {adjustedR}");
            
            steps.Add($"步骤3: 计算X坐标");
            float x = 1.5f * hexSize * q;
            steps.Add($"  x = 1.5 × hexSize × q = 1.5 × {hexSize} × {q} = {x}");
            
            steps.Add($"步骤4: 计算Y坐标");
            float y = hexHeight * (adjustedR + q / 2.0f);
            steps.Add($"  y = hexHeight × (adjustedR + q/2) = {hexHeight:F3} × ({adjustedR} + {q}/2) = {y:F3}");
            
            // 处理负数奇数列的特殊情况
            if (q < 0 && q % 2 != 0)
            {
                y += hexHeight;
                steps.Add($"步骤5: 负数奇数列特殊处理");
                steps.Add($"  y += hexHeight = {y - hexHeight:F3} + {hexHeight:F3} = {y:F3}");
            }
            
            steps.Add($"结果: 世界坐标({x}, {y:F3}, 0)");
            
            calculationSteps = steps.ToArray();
            return new Vector3(x, y, 0);
        }
    }
    
    /// <summary>
    /// 轴向坐标结构（简化版）
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
        
        public override string ToString()
        {
            return $"({q}, {r})";
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 在Scene视图中绘制可视化内容
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showGrid) return;
        
        // 绘制网格
        DrawHexGrid();
        
        // 高亮显示演示坐标
        DrawHighlightedHex();
        
        // 显示坐标标签
        if (showCoordinates)
        {
            DrawCoordinateLabels();
        }
    }
    
    /// <summary>
    /// 绘制六边形网格
    /// </summary>
    private void DrawHexGrid()
    {
        for (int q = 0; q < gridWidth; q++)
        {
            for (int r = 0; r < gridHeight; r++)
            {
                OffsetCoordinate offset = new OffsetCoordinate(q, r);
                Vector3 worldPos = offset.ToWorldPositionWithSteps(hexSize, out _);
                
                // 根据奇偶列设置颜色
                Gizmos.color = (q % 2 == 0) ? evenColumnColor : oddColumnColor;
                
                // 绘制六边形
                DrawHexagon(worldPos, hexSize);
            }
        }
    }
    
    /// <summary>
    /// 绘制高亮的演示六边形
    /// </summary>
    private void DrawHighlightedHex()
    {
        if (demoQ >= 0 && demoQ < gridWidth && demoR >= 0 && demoR < gridHeight)
        {
            OffsetCoordinate demoOffset = new OffsetCoordinate(demoQ, demoR);
            Vector3 worldPos = demoOffset.ToWorldPositionWithSteps(hexSize, out _);
            
            Gizmos.color = highlightColor;
            DrawHexagon(worldPos, hexSize * 1.1f); // 稍微大一点
        }
    }
    
    /// <summary>
    /// 绘制坐标标签
    /// </summary>
    private void DrawCoordinateLabels()
    {
        for (int q = 0; q < gridWidth; q++)
        {
            for (int r = 0; r < gridHeight; r++)
            {
                OffsetCoordinate offset = new OffsetCoordinate(q, r);
                Vector3 worldPos = offset.ToWorldPositionWithSteps(hexSize, out _);
                
                // 显示偏移坐标
                Handles.Label(worldPos + Vector3.up * 0.2f, $"({q},{r})", 
                    new GUIStyle() { normal = { textColor = textColor }, fontSize = 10 });
                
                // 显示轴向坐标
                AxialCoordinate axial = offset.ToAxialWithSteps(out _);
                Handles.Label(worldPos - Vector3.up * 0.2f, $"A{axial}", 
                    new GUIStyle() { normal = { textColor = Color.blue }, fontSize = 8 });
            }
        }
    }
    
    /// <summary>
    /// 绘制六边形
    /// </summary>
    private void DrawHexagon(Vector3 center, float size)
    {
        Vector3[] vertices = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            vertices[i] = center + new Vector3(
                Mathf.Cos(angle) * size,
                Mathf.Sin(angle) * size,
                0
            );
        }
        
        // 绘制六边形边框
        for (int i = 0; i < 6; i++)
        {
            Gizmos.DrawLine(vertices[i], vertices[(i + 1) % 6]);
        }
    }
#endif
}

#if UNITY_EDITOR
/// <summary>
/// 偏移坐标计算器的编辑器界面
/// </summary>
[CustomEditor(typeof(OffsetCoordinateVisualizer))]
public class OffsetCoordinateVisualizerEditor : Editor
{
    private OffsetCoordinateVisualizer visualizer;
    private Vector2 scrollPosition;
    
    private void OnEnable()
    {
        visualizer = (OffsetCoordinateVisualizer)target;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("计算演示", EditorStyles.boldLabel);
        
        if (GUILayout.Button("解释Q和R的含义", GUILayout.Height(25)))
        {
            OffsetCoordinateVisualizer.OffsetCoordinate.ExplainQRMeaning();
        }
        
        if (GUILayout.Button("演示偏移坐标计算", GUILayout.Height(30)))
        {
            DemonstrateCalculation();
        }
        
        EditorGUILayout.Space();
        
        // 显示计算步骤
        ShowCalculationSteps();
        
        EditorGUILayout.Space();
        
        // 快速测试按钮
        EditorGUILayout.LabelField("快速测试", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("偶数列示例 (2,3)"))
        {
            SetDemoCoordinate(2, 3);
        }
        if (GUILayout.Button("奇数列示例 (3,3)"))
        {
            SetDemoCoordinate(3, 3);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("原点 (0,0)"))
        {
            SetDemoCoordinate(0, 0);
        }
        if (GUILayout.Button("负数列 (-1,2)"))
        {
            SetDemoCoordinate(-1, 2);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// 演示计算过程
    /// </summary>
    private void DemonstrateCalculation()
    {
        var demoOffset = new OffsetCoordinateVisualizer.OffsetCoordinate(
            visualizer.GetType().GetField("demoQ", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(visualizer) as int? ?? 0,
            visualizer.GetType().GetField("demoR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(visualizer) as int? ?? 0
        );
        
        // 获取计算步骤
        var axial = demoOffset.ToAxialWithSteps(out string[] axialSteps);
        var worldPos = demoOffset.ToWorldPositionWithSteps(1.0f, out string[] worldSteps);
        
        // 在控制台输出详细步骤
        Debug.Log("=== 偏移坐标计算演示 ===");
        Debug.Log("轴向坐标转换:");
        foreach (string step in axialSteps)
        {
            Debug.Log("  " + step);
        }
        
        Debug.Log("\n世界坐标转换:");
        foreach (string step in worldSteps)
        {
            Debug.Log("  " + step);
        }
        
        Debug.Log("=== 演示结束 ===");
    }
    
    /// <summary>
    /// 显示计算步骤
    /// </summary>
    private void ShowCalculationSteps()
    {
        var demoQ = (int)visualizer.GetType().GetField("demoQ", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(visualizer);
        var demoR = (int)visualizer.GetType().GetField("demoR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(visualizer);
        
        var demoOffset = new OffsetCoordinateVisualizer.OffsetCoordinate(demoQ, demoR);
        
        // 计算轴向坐标
        var axial = demoOffset.ToAxialWithSteps(out string[] axialSteps);
        
        EditorGUILayout.LabelField($"当前演示坐标: ({demoQ}, {demoR})", EditorStyles.boldLabel);
        
        // 显示轴向坐标转换步骤
        EditorGUILayout.LabelField("轴向坐标转换步骤:", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
        
        foreach (string step in axialSteps)
        {
            EditorGUILayout.LabelField(step, EditorStyles.wordWrappedLabel);
        }
        
        EditorGUILayout.EndScrollView();
        
        // 显示结果
        EditorGUILayout.LabelField($"轴向坐标结果: {axial}", EditorStyles.boldLabel);
        
        // 显示世界坐标
        var worldPos = demoOffset.ToWorldPositionWithSteps(1.0f, out _);
        EditorGUILayout.LabelField($"世界坐标: ({worldPos.x:F2}, {worldPos.y:F2})", EditorStyles.boldLabel);
    }
    
    /// <summary>
    /// 设置演示坐标
    /// </summary>
    private void SetDemoCoordinate(int q, int r)
    {
        var demoQField = visualizer.GetType().GetField("demoQ", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var demoRField = visualizer.GetType().GetField("demoR", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        demoQField.SetValue(visualizer, q);
        demoRField.SetValue(visualizer, r);
        
        EditorUtility.SetDirty(visualizer);
        SceneView.RepaintAll();
    }
}
#endif