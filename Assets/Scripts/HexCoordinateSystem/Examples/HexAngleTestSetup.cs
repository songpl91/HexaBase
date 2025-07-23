using UnityEngine;
using HexCoordinateSystem.Coordinates;
using HexCoordinateSystem.Utils;

namespace HexCoordinateSystem.Examples
{
    /// <summary>
    /// 六边形角度计算测试场景设置器
    /// 自动创建测试对象和可视化组件
    /// </summary>
    public class HexAngleTestSetup : MonoBehaviour
    {
        [Header("自动设置选项")]
        [SerializeField] private bool autoCreateOnStart = true;
        [SerializeField] private bool createSimpleTest = true;
        [SerializeField] private bool createVisualizer = true;
        [SerializeField] private bool createUIVisualizer = true;
        
        [Header("测试配置")]
        [SerializeField] private int testFromCol = 1;
        [SerializeField] private int testFromRow = 1;
        [SerializeField] private int testToCol = -1;
        [SerializeField] private int testToRow = -1;
        [SerializeField] private float testHexSize = 1.0f;
        
        void Start()
        {
            if (autoCreateOnStart)
            {
                SetupTestScene();
            }
        }
        
        /// <summary>
        /// 设置测试场景
        /// </summary>
        [ContextMenu("设置测试场景")]
        public void SetupTestScene()
        {
            Debug.Log("=== 开始设置六边形角度计算测试场景 ===");
            
            // 创建简单测试脚本
            if (createSimpleTest)
            {
                CreateSimpleTest();
            }
            
            // 创建Scene视图可视化器
            if (createVisualizer)
            {
                CreateVisualizer();
            }
            
            // 创建UI可视化器
            if (createUIVisualizer)
            {
                CreateUIVisualizer();
            }
            
            // 执行初始测试
            ExecuteInitialTest();
            
            Debug.Log("=== 测试场景设置完成 ===");
        }
        
        /// <summary>
        /// 创建简单测试组件
        /// </summary>
        private void CreateSimpleTest()
        {
            GameObject testObj = GameObject.Find("SimpleAngleTest");
            if (testObj == null)
            {
                testObj = new GameObject("SimpleAngleTest");
                testObj.AddComponent<SimpleAngleTest>();
                Debug.Log("已创建 SimpleAngleTest 组件");
            }
        }
        
        /// <summary>
        /// 创建Scene视图可视化器
        /// </summary>
        private void CreateVisualizer()
        {
            GameObject visualizerObj = GameObject.Find("HexAngleVisualizer");
            if (visualizerObj == null)
            {
                visualizerObj = new GameObject("HexAngleVisualizer");
                var visualizer = visualizerObj.AddComponent<HexAngleVisualizer>();
                
                // 设置测试坐标
                var visualizerType = typeof(HexAngleVisualizer);
                var fromColField = visualizerType.GetField("fromCol", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var fromRowField = visualizerType.GetField("fromRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var toColField = visualizerType.GetField("toCol", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var toRowField = visualizerType.GetField("toRow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var hexSizeField = visualizerType.GetField("hexSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                fromColField?.SetValue(visualizer, testFromCol);
                fromRowField?.SetValue(visualizer, testFromRow);
                toColField?.SetValue(visualizer, testToCol);
                toRowField?.SetValue(visualizer, testToRow);
                hexSizeField?.SetValue(visualizer, testHexSize);
                
                Debug.Log("已创建 HexAngleVisualizer 组件（Scene视图可视化）");
            }
        }
        
        /// <summary>
        /// 创建UI可视化器
        /// </summary>
        private void CreateUIVisualizer()
        {
            GameObject uiVisualizerObj = GameObject.Find("HexAngleUIVisualizer");
            if (uiVisualizerObj == null)
            {
                uiVisualizerObj = new GameObject("HexAngleUIVisualizer");
                var uiVisualizer = uiVisualizerObj.AddComponent<HexAngleUIVisualizer>();
                
                // 设置测试坐标
                uiVisualizer.SetCoordinates(testFromCol, testFromRow, testToCol, testToRow);
                uiVisualizer.SetHexSize(testHexSize);
                
                Debug.Log("已创建 HexAngleUIVisualizer 组件（Game视图可视化）");
            }
        }
        
        /// <summary>
        /// 执行初始测试
        /// </summary>
        private void ExecuteInitialTest()
        {
            Debug.Log("=== 执行初始角度计算测试 ===");
            
            // 创建测试坐标
            var coordA = new OffsetCoordinateOddQ(testFromCol, testFromRow);
            var coordB = new OffsetCoordinateOddQ(testToCol, testToRow);
            
            // 计算结果
            float angle = HexAngleCalculator.CalculateAngle(coordA, coordB, testHexSize);
            string direction = HexAngleCalculator.GetDirectionName(angle);
            float distance = HexAngleCalculator.CalculateWorldDistance(coordA, coordB, testHexSize);
            
            Vector3 worldA = coordA.ToWorldPosition(testHexSize);
            Vector3 worldB = coordB.ToWorldPosition(testHexSize);
            
            Debug.Log($"测试坐标: A({testFromCol},{testFromRow}) → B({testToCol},{testToRow})");
            Debug.Log($"世界坐标: A{worldA} → B{worldB}");
            Debug.Log($"计算结果: {angle:F2}° ({direction}) 距离: {distance:F4}");
        }
        
        /// <summary>
        /// 运行完整测试套件
        /// </summary>
        [ContextMenu("运行完整测试套件")]
        public void RunFullTestSuite()
        {
            Debug.Log("=== 开始完整测试套件 ===");
            
            var testCases = new (int fromCol, int fromRow, int toCol, int toRow, string description)[]
            {
                (0, 0, 1, 0, "正东方向"),
                (0, 0, 0, 1, "正北方向"),
                (0, 0, -1, 0, "正西方向"),
                (0, 0, 0, -1, "正南方向"),
                (0, 0, 1, 1, "东北方向"),
                (0, 0, -1, 1, "西北方向"),
                (0, 0, -1, -1, "西南方向"),
                (0, 0, 1, -1, "东南方向"),
                (1, 1, -1, -1, "用户示例"),
                (2, 2, -2, -2, "用户示例放大"),
            };
            
            foreach (var testCase in testCases)
            {
                var from = new OffsetCoordinateOddQ(testCase.fromCol, testCase.fromRow);
                var to = new OffsetCoordinateOddQ(testCase.toCol, testCase.toRow);
                
                float angle = HexAngleCalculator.CalculateAngle(from, to, testHexSize);
                string direction = HexAngleCalculator.GetDirectionName(angle);
                float distance = HexAngleCalculator.CalculateWorldDistance(from, to, testHexSize);
                
                Debug.Log($"{testCase.description}: ({testCase.fromCol},{testCase.fromRow})→({testCase.toCol},{testCase.toRow}) " +
                         $"= {angle:F1}° ({direction}) 距离:{distance:F2}");
            }
            
            Debug.Log("=== 完整测试套件结束 ===");
        }
        
        /// <summary>
        /// 清理测试对象
        /// </summary>
        [ContextMenu("清理测试对象")]
        public void CleanupTestObjects()
        {
            var testObjects = new string[]
            {
                "SimpleAngleTest",
                "HexAngleVisualizer", 
                "HexAngleUIVisualizer"
            };
            
            foreach (var objName in testObjects)
            {
                GameObject obj = GameObject.Find(objName);
                if (obj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(obj);
                    }
                    else
                    {
                        DestroyImmediate(obj);
                    }
                    Debug.Log($"已清理测试对象: {objName}");
                }
            }
        }
        
        /// <summary>
        /// 设置新的测试坐标
        /// </summary>
        /// <param name="fromCol">起始列</param>
        /// <param name="fromRow">起始行</param>
        /// <param name="toCol">目标列</param>
        /// <param name="toRow">目标行</param>
        public void SetTestCoordinates(int fromCol, int fromRow, int toCol, int toRow)
        {
            testFromCol = fromCol;
            testFromRow = fromRow;
            testToCol = toCol;
            testToRow = toRow;
            
            // 更新现有的可视化器
            var uiVisualizer = FindObjectOfType<HexAngleUIVisualizer>();
            if (uiVisualizer != null)
            {
                uiVisualizer.SetCoordinates(fromCol, fromRow, toCol, toRow);
            }
            
            Debug.Log($"已更新测试坐标: ({fromCol},{fromRow}) → ({toCol},{toRow})");
        }
    }
}