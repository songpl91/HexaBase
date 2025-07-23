using UnityEngine;
using GridCoordinateSystem.Visualization;

namespace GridCoordinateSystem.Setup
{
    /// <summary>
    /// 网格可视化场景设置器
    /// </summary>
    public class GridVisualizationSetup : MonoBehaviour
    {
        [Header("场景设置")]
        [SerializeField] private bool _autoSetupOnStart = true;
        [SerializeField] private bool _createCamera = true;
        [SerializeField] private bool _createVisualizer = true;
        
        [Header("摄像机配置")]
        [SerializeField] private Vector3 _cameraPosition = new Vector3(5, 5, -10);
        [SerializeField] private float _cameraSize = 8f;
        [SerializeField] private Color _backgroundColor = Color.black;
        
        [Header("网格配置")]
        [SerializeField] private float _cellSize = 1.0f;
        [SerializeField] private int _gridWidth = 10;
        [SerializeField] private int _gridHeight = 10;
        
        private void Start()
        {
            if (_autoSetupOnStart)
            {
                SetupVisualizationScene();
            }
        }
        
        /// <summary>
        /// 设置可视化场景
        /// </summary>
        [ContextMenu("Setup Visualization Scene")]
        public void SetupVisualizationScene()
        {
            Debug.Log("开始设置网格可视化场景...");
            
            // 创建摄像机
            if (_createCamera)
            {
                SetupCamera();
            }
            
            // 创建可视化器
            if (_createVisualizer)
            {
                SetupVisualizer();
            }
            
            Debug.Log("网格可视化场景设置完成！");
            Debug.Log("使用说明:");
            Debug.Log("- 左键点击选择网格位置");
            Debug.Log("- 按 N 键切换邻居显示");
            Debug.Log("- 按 P 键切换路径显示");
            Debug.Log("- 按 C 键清除选择");
        }
        
        /// <summary>
        /// 设置摄像机
        /// </summary>
        private void SetupCamera()
        {
            Camera mainCamera = Camera.main;
            
            if (mainCamera == null)
            {
                // 创建新的摄像机
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
                
                Debug.Log("创建了新的主摄像机");
            }
            
            // 配置摄像机
            mainCamera.transform.position = _cameraPosition;
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = _cameraSize;
            mainCamera.backgroundColor = _backgroundColor;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            
            // 确保摄像机能看到网格中心
            Vector3 gridCenter = new Vector3(_gridWidth * _cellSize / 2f, _gridHeight * _cellSize / 2f, 0);
            Vector3 cameraPos = gridCenter + new Vector3(0, 0, -10);
            mainCamera.transform.position = cameraPos;
            
            Debug.Log($"摄像机已配置，位置: {cameraPos}");
        }
        
        /// <summary>
        /// 设置可视化器
        /// </summary>
        private void SetupVisualizer()
        {
            // 检查是否已存在可视化器
            GridVisualizer existingVisualizer = FindObjectOfType<GridVisualizer>();
            
            if (existingVisualizer == null)
            {
                // 创建新的可视化器
                GameObject visualizerObj = new GameObject("Grid Visualizer");
                GridVisualizer visualizer = visualizerObj.AddComponent<GridVisualizer>();
                
                // 通过反射设置私有字段（因为它们是SerializeField）
                var visualizerType = typeof(GridVisualizer);
                
                SetPrivateField(visualizer, "_cellSize", _cellSize);
                SetPrivateField(visualizer, "_gridWidth", _gridWidth);
                SetPrivateField(visualizer, "_gridHeight", _gridHeight);
                SetPrivateField(visualizer, "_showGrid", true);
                SetPrivateField(visualizer, "_showCoordinates", true);
                SetPrivateField(visualizer, "_enableMouseInteraction", true);
                
                Debug.Log("创建了新的网格可视化器");
            }
            else
            {
                // 更新现有可视化器的配置
                SetPrivateField(existingVisualizer, "_cellSize", _cellSize);
                SetPrivateField(existingVisualizer, "_gridWidth", _gridWidth);
                SetPrivateField(existingVisualizer, "_gridHeight", _gridHeight);
                
                Debug.Log("更新了现有的网格可视化器配置");
            }
        }
        
        /// <summary>
        /// 设置私有字段
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
        
        /// <summary>
        /// 创建演示预设
        /// </summary>
        [ContextMenu("Create Demo Presets")]
        public void CreateDemoPresets()
        {
            // 小网格演示
            CreatePreset("Small Grid Demo", 1.0f, 5, 5);
            
            // 中等网格演示
            CreatePreset("Medium Grid Demo", 1.0f, 10, 10);
            
            // 大网格演示
            CreatePreset("Large Grid Demo", 0.8f, 15, 15);
            
            // 细密网格演示
            CreatePreset("Dense Grid Demo", 0.5f, 20, 20);
        }
        
        /// <summary>
        /// 创建预设
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="cellSize">单元大小</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        private void CreatePreset(string name, float cellSize, int width, int height)
        {
            GameObject presetObj = new GameObject(name);
            GridVisualizationSetup setup = presetObj.AddComponent<GridVisualizationSetup>();
            
            setup._cellSize = cellSize;
            setup._gridWidth = width;
            setup._gridHeight = height;
            setup._autoSetupOnStart = false;
            
            // 调整摄像机位置以适应网格大小
            Vector3 gridCenter = new Vector3(width * cellSize / 2f, height * cellSize / 2f, 0);
            setup._cameraPosition = gridCenter + new Vector3(0, 0, -10);
            setup._cameraSize = Mathf.Max(width * cellSize, height * cellSize) / 2f + 2f;
            
            Debug.Log($"创建了预设: {name} ({width}x{height}, 单元大小: {cellSize})");
        }
    }
}