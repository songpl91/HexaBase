using System.Collections.Generic;
using UnityEngine;
using GridCoordinateSystem.Coordinates;
using GridCoordinateSystem.Shapes;

namespace GridCoordinateSystem.Examples
{
    /// <summary>
    /// 形状演示器
    /// 用于可视化展示各种网格形状的生成效果
    /// </summary>
    public class ShapePatternDemo : MonoBehaviour
    {
        [Header("网格配置")]
        [SerializeField] private float cellSize = 1.0f;
        [SerializeField] private int gridWidth = 50;
        [SerializeField] private int gridHeight = 50;
        
        [Header("可视化配置")]
        [SerializeField] private bool showGrid = true;
        [SerializeField] private bool showCoordinates = false;
        [SerializeField] private Color gridColor = Color.gray;
        [SerializeField] private Color shapeColor = Color.red;
        [SerializeField] private Color centerColor = Color.yellow;
        
        [Header("形状参数")]
        [SerializeField] private ShapeParameters shapeParameters = new ShapeParameters();
        
        [Header("演示控制")]
        [SerializeField] private bool autoDemo = false;
        [SerializeField] private float demoInterval = 2.0f;
        
        private List<CartesianCoordinate> currentShapeCoordinates = new List<CartesianCoordinate>();
        private Camera mainCamera;
        private float lastDemoTime;
        private int currentDemoIndex = 0;
        
        // 预定义的演示形状
        private ShapeParameters[] demoShapes;
        
        void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindObjectOfType<Camera>();
                
            SetupCamera();
            InitializeDemoShapes();
            GenerateCurrentShape();
        }
        
        void Update()
        {
            HandleInput();
            
            if (autoDemo && Time.time - lastDemoTime > demoInterval)
            {
                NextDemoShape();
                lastDemoTime = Time.time;
            }
        }
        
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            DrawGrid();
            DrawShape();
            DrawCenter();
        }
        
        /// <summary>
        /// 设置摄像机位置
        /// </summary>
        private void SetupCamera()
        {
            if (mainCamera != null)
            {
                mainCamera.transform.position = new Vector3(gridWidth * cellSize / 2, gridHeight * cellSize / 2, -10);
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = Mathf.Max(gridWidth, gridHeight) * cellSize / 2 + 5;
            }
        }
        
        /// <summary>
        /// 初始化演示形状
        /// </summary>
        private void InitializeDemoShapes()
        {
            var center = new CartesianCoordinate(gridWidth / 2, gridHeight / 2);
            
            demoShapes = new ShapeParameters[]
            {
                // 圆形系列
                ShapeParameters.CreateCircle(center, 5, FillMode.Filled),
                ShapeParameters.CreateCircle(center, 5, FillMode.Outline),
                ShapeParameters.CreateCircle(center, 5, FillMode.Hollow),
                
                // 矩形系列
                ShapeParameters.CreateRectangle(center, 10, 6, FillMode.Filled),
                ShapeParameters.CreateRectangle(center, 10, 6, FillMode.Outline),
                
                // 十字形
                ShapeParameters.CreateCross(center, 6, 1),
                ShapeParameters.CreateCross(center, 6, 3),
                
                // 菱形
                new ShapeParameters { shapeType = ShapeType.Diamond, center = center, radius = 6, fillMode = FillMode.Filled },
                new ShapeParameters { shapeType = ShapeType.Diamond, center = center, radius = 6, fillMode = FillMode.Outline },
                
                // 椭圆形
                new ShapeParameters { shapeType = ShapeType.Ellipse, center = center, width = 12, height = 8, fillMode = FillMode.Filled },
                new ShapeParameters { shapeType = ShapeType.Ellipse, center = center, width = 12, height = 8, fillMode = FillMode.Outline },
                
                // 扇形
                ShapeParameters.CreateSector(center, 6, 0, 90, FillMode.Filled),
                ShapeParameters.CreateSector(center, 6, 45, 225, FillMode.Filled),
                ShapeParameters.CreateSector(center, 6, 0, 270, FillMode.Outline),
                
                // 三角形
                new ShapeParameters { shapeType = ShapeType.Triangle, center = center, radius = 6, fillMode = FillMode.Filled },
                new ShapeParameters { shapeType = ShapeType.Triangle, center = center, radius = 6, fillMode = FillMode.Outline },
                
                // 六边形
                new ShapeParameters { shapeType = ShapeType.Hexagon, center = center, radius = 5, fillMode = FillMode.Filled },
                new ShapeParameters { shapeType = ShapeType.Hexagon, center = center, radius = 5, fillMode = FillMode.Outline },
                
                // 星形
                new ShapeParameters { shapeType = ShapeType.Star, center = center, radius = 6, points = 5, fillMode = FillMode.Filled },
                new ShapeParameters { shapeType = ShapeType.Star, center = center, radius = 6, points = 8, fillMode = FillMode.Filled },
                
                // 直线
                new ShapeParameters { shapeType = ShapeType.Line, center = center, radius = 8, startAngle = 0 },
                new ShapeParameters { shapeType = ShapeType.Line, center = center, radius = 8, startAngle = 45 },
            };
        }
        
        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleInput()
        {
            // 鼠标点击更新中心点
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
                
                int gridX = Mathf.RoundToInt(worldPos.x / cellSize);
                int gridY = Mathf.RoundToInt(worldPos.y / cellSize);
                
                shapeParameters.center = new CartesianCoordinate(gridX, gridY);
                GenerateCurrentShape();
            }
            
            // 键盘控制
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextDemoShape();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                GenerateCurrentShape();
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                showGrid = !showGrid;
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                showCoordinates = !showCoordinates;
            }
            
            // 数字键切换形状类型
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeShapeType(ShapeType.Circle);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeShapeType(ShapeType.Rectangle);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeShapeType(ShapeType.Diamond);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeShapeType(ShapeType.Cross);
            if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeShapeType(ShapeType.Ellipse);
            if (Input.GetKeyDown(KeyCode.Alpha6)) ChangeShapeType(ShapeType.Sector);
            if (Input.GetKeyDown(KeyCode.Alpha7)) ChangeShapeType(ShapeType.Triangle);
            if (Input.GetKeyDown(KeyCode.Alpha8)) ChangeShapeType(ShapeType.Hexagon);
            if (Input.GetKeyDown(KeyCode.Alpha9)) ChangeShapeType(ShapeType.Star);
            if (Input.GetKeyDown(KeyCode.Alpha0)) ChangeShapeType(ShapeType.Line);
            
            // 填充模式切换
            if (Input.GetKeyDown(KeyCode.F))
            {
                shapeParameters.fillMode = (FillMode)(((int)shapeParameters.fillMode + 1) % 3);
                GenerateCurrentShape();
            }
            
            // 大小调整
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                shapeParameters.radius = Mathf.Min(shapeParameters.radius + 1, 15);
                GenerateCurrentShape();
            }
            
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                shapeParameters.radius = Mathf.Max(shapeParameters.radius - 1, 1);
                GenerateCurrentShape();
            }
        }
        
        /// <summary>
        /// 切换形状类型
        /// </summary>
        private void ChangeShapeType(ShapeType newType)
        {
            shapeParameters.shapeType = newType;
            GenerateCurrentShape();
        }
        
        /// <summary>
        /// 下一个演示形状
        /// </summary>
        private void NextDemoShape()
        {
            if (demoShapes != null && demoShapes.Length > 0)
            {
                currentDemoIndex = (currentDemoIndex + 1) % demoShapes.Length;
                shapeParameters = demoShapes[currentDemoIndex];
                GenerateCurrentShape();
            }
        }
        
        /// <summary>
        /// 生成当前形状
        /// </summary>
        private void GenerateCurrentShape()
        {
            currentShapeCoordinates = GridShapeGenerator.GenerateShape(shapeParameters);
            Debug.Log($"Generated {shapeParameters.shapeType} with {currentShapeCoordinates.Count} coordinates");
        }
        
        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGrid()
        {
            if (!showGrid) return;
            
            Gizmos.color = gridColor;
            
            // 绘制垂直线
            for (int x = 0; x <= gridWidth; x++)
            {
                Vector3 start = new Vector3(x * cellSize, 0, 0);
                Vector3 end = new Vector3(x * cellSize, gridHeight * cellSize, 0);
                Gizmos.DrawLine(start, end);
            }
            
            // 绘制水平线
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector3 start = new Vector3(0, y * cellSize, 0);
                Vector3 end = new Vector3(gridWidth * cellSize, y * cellSize, 0);
                Gizmos.DrawLine(start, end);
            }
        }
        
        /// <summary>
        /// 绘制形状
        /// </summary>
        private void DrawShape()
        {
            Gizmos.color = shapeColor;
            
            foreach (var coord in currentShapeCoordinates)
            {
                Vector3 worldPos = new Vector3(coord.X * cellSize, coord.Y * cellSize, 0);
                Gizmos.DrawCube(worldPos, Vector3.one * cellSize * 0.8f);
            }
        }
        
        /// <summary>
        /// 绘制中心点
        /// </summary>
        private void DrawCenter()
        {
            Gizmos.color = centerColor;
            Vector3 centerPos = new Vector3(shapeParameters.center.X * cellSize, shapeParameters.center.Y * cellSize, 0);
            Gizmos.DrawSphere(centerPos, cellSize * 0.3f);
        }
        
        /// <summary>
        /// GUI显示
        /// </summary>
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            
            GUILayout.Label("形状演示控制", new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold });
            GUILayout.Space(10);
            
            GUILayout.Label($"当前形状: {shapeParameters.shapeType}");
            GUILayout.Label($"填充模式: {shapeParameters.fillMode}");
            GUILayout.Label($"中心点: ({shapeParameters.center.X}, {shapeParameters.center.Y})");
            GUILayout.Label($"半径/大小: {shapeParameters.radius}");
            GUILayout.Label($"坐标数量: {currentShapeCoordinates.Count}");
            
            GUILayout.Space(10);
            GUILayout.Label("控制说明:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUILayout.Label("• 鼠标点击: 设置中心点");
            GUILayout.Label("• 空格键: 下一个演示形状");
            GUILayout.Label("• R键: 重新生成");
            GUILayout.Label("• G键: 切换网格显示");
            GUILayout.Label("• F键: 切换填充模式");
            GUILayout.Label("• +/-键: 调整大小");
            GUILayout.Label("• 数字键1-9,0: 切换形状类型");
            
            GUILayout.Space(10);
            if (GUILayout.Button("自动演示: " + (autoDemo ? "开启" : "关闭")))
            {
                autoDemo = !autoDemo;
                lastDemoTime = Time.time;
            }
            
            if (GUILayout.Button("导出坐标数据"))
            {
                ExportCoordinateData();
            }
            
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// 导出坐标数据
        /// </summary>
        private void ExportCoordinateData()
        {
            string data = $"Shape: {shapeParameters.shapeType}\n";
            data += $"Center: ({shapeParameters.center.X}, {shapeParameters.center.Y})\n";
            data += $"Parameters: Radius={shapeParameters.radius}, Width={shapeParameters.width}, Height={shapeParameters.height}\n";
            data += $"Fill Mode: {shapeParameters.fillMode}\n";
            data += $"Coordinate Count: {currentShapeCoordinates.Count}\n\n";
            data += "Coordinates:\n";
            
            foreach (var coord in currentShapeCoordinates)
            {
                data += $"({coord.X}, {coord.Y})\n";
            }
            
            Debug.Log(data);
            
            // 也可以保存到文件
            string fileName = $"ShapeData_{shapeParameters.shapeType}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            System.IO.File.WriteAllText(filePath, data);
            Debug.Log($"坐标数据已保存到: {filePath}");
        }
    }
}