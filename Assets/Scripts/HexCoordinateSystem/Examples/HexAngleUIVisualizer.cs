using UnityEngine;
using UnityEngine.UI;
using HexCoordinateSystem.Coordinates;
using HexCoordinateSystem.Utils;

namespace HexCoordinateSystem.Examples
{
    /// <summary>
    /// 六边形角度计算UI可视化脚本
    /// 在Game视图中显示计算结果和可视化图形
    /// </summary>
    public class HexAngleUIVisualizer : MonoBehaviour
    {
        [Header("坐标设置")]
        [SerializeField] private int fromCol = 1;
        [SerializeField] private int fromRow = 1;
        [SerializeField] private int toCol = -1;
        [SerializeField] private int toRow = -1;
        [SerializeField] private float hexSize = 1.0f;
        
        [Header("UI组件")]
        [SerializeField] private Text resultText;
        [SerializeField] private LineRenderer connectionLine;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform centerPoint;
        
        [Header("可视化设置")]
        [SerializeField] private float visualScale = 100f; // UI缩放比例
        [SerializeField] private Color startColor = Color.green;
        [SerializeField] private Color endColor = Color.red;
        [SerializeField] private Color lineColor = Color.blue;
        
        private OffsetCoordinateOddQ coordA;
        private OffsetCoordinateOddQ coordB;
        private Vector3 fromWorldPos;
        private Vector3 toWorldPos;
        
        void Start()
        {
            InitializeUI();
            UpdateVisualization();
        }
        
        void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateVisualization();
            }
        }
        
        /// <summary>
        /// 初始化UI组件
        /// </summary>
        private void InitializeUI()
        {
            // 如果没有指定UI组件，尝试自动创建
            if (resultText == null)
            {
                CreateUIComponents();
            }
            
            // 设置连接线属性
            if (connectionLine != null)
            {
                connectionLine.material = new Material(Shader.Find("Sprites/Default"));
                connectionLine.startColor = lineColor;
                connectionLine.endColor = lineColor;
                connectionLine.startWidth = 0.1f;
                connectionLine.endWidth = 0.1f;
                connectionLine.positionCount = 2;
                connectionLine.useWorldSpace = false;
            }
        }
        
        /// <summary>
        /// 创建UI组件
        /// </summary>
        private void CreateUIComponents()
        {
            // 创建Canvas（如果不存在）
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            
            // 创建结果文本
            if (resultText == null)
            {
                GameObject textObj = new GameObject("ResultText");
                textObj.transform.SetParent(canvas.transform, false);
                resultText = textObj.AddComponent<Text>();
                resultText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                resultText.fontSize = 16;
                resultText.color = Color.white;
                
                RectTransform rectTransform = textObj.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.anchoredPosition = new Vector2(10, -10);
                rectTransform.sizeDelta = new Vector2(400, 200);
            }
            
            // 创建可视化点
            CreateVisualizationPoints();
        }
        
        /// <summary>
        /// 创建可视化点
        /// </summary>
        private void CreateVisualizationPoints()
        {
            // 创建中心点
            if (centerPoint == null)
            {
                GameObject centerObj = new GameObject("CenterPoint");
                centerPoint = centerObj.transform;
                centerPoint.SetParent(transform);
                centerPoint.localPosition = Vector3.zero;
            }
            
            // 创建起始点
            if (startPoint == null)
            {
                GameObject startObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                startObj.name = "StartPoint";
                startPoint = startObj.transform;
                startPoint.SetParent(centerPoint);
                startObj.GetComponent<Renderer>().material.color = startColor;
                startObj.transform.localScale = Vector3.one * 0.2f;
            }
            
            // 创建结束点
            if (endPoint == null)
            {
                GameObject endObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                endObj.name = "EndPoint";
                endPoint = endObj.transform;
                endPoint.SetParent(centerPoint);
                endObj.GetComponent<Renderer>().material.color = endColor;
                endObj.transform.localScale = Vector3.one * 0.2f;
            }
            
            // 创建连接线
            if (connectionLine == null)
            {
                GameObject lineObj = new GameObject("ConnectionLine");
                lineObj.transform.SetParent(centerPoint);
                connectionLine = lineObj.AddComponent<LineRenderer>();
            }
        }
        
        /// <summary>
        /// 更新可视化显示
        /// </summary>
        public void UpdateVisualization()
        {
            // 创建偏移坐标
            coordA = new OffsetCoordinateOddQ(fromCol, fromRow);
            coordB = new OffsetCoordinateOddQ(toCol, toRow);
            
            // 获取世界坐标
            fromWorldPos = coordA.ToWorldPosition(hexSize);
            toWorldPos = coordB.ToWorldPosition(hexSize);
            
            // 计算角度和相关信息
            float angleDegrees = HexAngleCalculator.CalculateAngle(coordA, coordB, hexSize);
            float angleRadians = HexAngleCalculator.CalculateAngleRadians(coordA, coordB, hexSize);
            string directionName = HexAngleCalculator.GetDirectionName(angleDegrees);
            float worldDistance = HexAngleCalculator.CalculateWorldDistance(coordA, coordB, hexSize);
            Vector3 direction = HexAngleCalculator.CalculateDirection(coordA, coordB, hexSize);
            
            // 更新UI文本
            if (resultText != null)
            {
                resultText.text = $"=== 六边形角度计算结果 ===\n" +
                                 $"偏移坐标: A({fromCol},{fromRow}) → B({toCol},{toRow})\n" +
                                 $"世界坐标: A{fromWorldPos:F2} → B{toWorldPos:F2}\n" +
                                 $"角度: {angleDegrees:F2}° ({angleRadians:F4} 弧度)\n" +
                                 $"方向: {directionName}\n" +
                                 $"世界距离: {worldDistance:F4}\n" +
                                 $"方向向量: {direction:F3}\n" +
                                 $"六边形大小: {hexSize}";
            }
            
            // 更新可视化点位置
            if (startPoint != null && endPoint != null)
            {
                Vector3 scaledFromPos = fromWorldPos * visualScale;
                Vector3 scaledToPos = toWorldPos * visualScale;
                
                startPoint.localPosition = scaledFromPos;
                endPoint.localPosition = scaledToPos;
                
                // 更新连接线
                if (connectionLine != null)
                {
                    connectionLine.SetPosition(0, scaledFromPos);
                    connectionLine.SetPosition(1, scaledToPos);
                }
            }
            
            // 输出到控制台
            Debug.Log($"=== 角度计算更新 ===");
            Debug.Log($"A({fromCol},{fromRow}) → B({toCol},{toRow}) = {angleDegrees:F2}° ({directionName})");
        }
        
        /// <summary>
        /// 设置新的坐标
        /// </summary>
        /// <param name="newFromCol">起始列</param>
        /// <param name="newFromRow">起始行</param>
        /// <param name="newToCol">目标列</param>
        /// <param name="newToRow">目标行</param>
        public void SetCoordinates(int newFromCol, int newFromRow, int newToCol, int newToRow)
        {
            fromCol = newFromCol;
            fromRow = newFromRow;
            toCol = newToCol;
            toRow = newToRow;
            UpdateVisualization();
        }
        
        /// <summary>
        /// 设置六边形大小
        /// </summary>
        /// <param name="newHexSize">新的六边形大小</param>
        public void SetHexSize(float newHexSize)
        {
            hexSize = newHexSize;
            UpdateVisualization();
        }
        
        /// <summary>
        /// 重置为示例坐标
        /// </summary>
        [ContextMenu("重置为示例坐标")]
        public void ResetToExample()
        {
            SetCoordinates(1, 1, -1, -1);
        }
        
        /// <summary>
        /// 交换起始点和目标点
        /// </summary>
        [ContextMenu("交换起始点和目标点")]
        public void SwapPoints()
        {
            SetCoordinates(toCol, toRow, fromCol, fromRow);
        }
        
        /// <summary>
        /// 测试常见方向
        /// </summary>
        [ContextMenu("测试常见方向")]
        public void TestCommonDirections()
        {
            var directions = new (int col, int row, string name)[]
            {
                (1, 0, "右"),
                (0, 1, "上"),
                (-1, 0, "左"),
                (0, -1, "下"),
                (1, 1, "右上"),
                (-1, -1, "左下"),
                (1, -1, "右下"),
                (-1, 1, "左上")
            };
            
            Debug.Log("=== 常见方向测试 ===");
            foreach (var dir in directions)
            {
                var from = new OffsetCoordinateOddQ(0, 0);
                var to = new OffsetCoordinateOddQ(dir.col, dir.row);
                float angle = HexAngleCalculator.CalculateAngle(from, to, hexSize);
                string dirName = HexAngleCalculator.GetDirectionName(angle);
                Debug.Log($"{dir.name}方向 (0,0)→({dir.col},{dir.row}): {angle:F1}° ({dirName})");
            }
        }
    }
}