using UnityEngine;
using System.Collections.Generic;
using GridCoordinateSystem.Core;
using GridCoordinateSystem.Coordinates;
using GridCoordinateSystem.Utils;

namespace GridCoordinateSystem.Visualization
{
    /// <summary>
    /// 网格可视化器 - 用于验证和演示四边形网格系统
    /// </summary>
    public class GridVisualizer : MonoBehaviour
    {
        [Header("网格配置")]
        [SerializeField] private float _cellSize = 1.0f;
        [SerializeField] private int _gridWidth = 10;
        [SerializeField] private int _gridHeight = 10;
        [SerializeField] private bool _showGrid = true;
        [SerializeField] private bool _showCoordinates = true;
        [SerializeField] private bool _showCellNumbers = false;
        
        [Header("可视化样式")]
        [SerializeField] private Color _gridColor = Color.white;
        [SerializeField] private Color _coordinateTextColor = Color.yellow;
        [SerializeField] private Color _hoverColor = Color.green;
        [SerializeField] private Color _selectedColor = Color.red;
        [SerializeField] private Color _neighborColor = Color.blue;
        [SerializeField] private Color _pathColor = Color.magenta;
        
        [Header("交互设置")]
        [SerializeField] private bool _enableMouseInteraction = true;
        [SerializeField] private KeyCode _selectKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode _showNeighborsKey = KeyCode.N;
        [SerializeField] private KeyCode _showPathKey = KeyCode.P;
        [SerializeField] private KeyCode _clearKey = KeyCode.C;
        
        [Header("演示功能")]
        [SerializeField] private bool _showDistanceCalculation = true;
        [SerializeField] private bool _showRangeQuery = false;
        [SerializeField] private int _rangeQueryRadius = 2;
        
        // 私有变量
        private GridCoordinateManager _gridManager;
        private Camera _mainCamera;
        private CartesianCoordinate _currentHoverPosition;
        private CartesianCoordinate? _selectedPosition;
        private CartesianCoordinate? _secondSelectedPosition;
        private List<CartesianCoordinate> _currentNeighbors = new List<CartesianCoordinate>();
        private List<CartesianCoordinate> _currentPath = new List<CartesianCoordinate>();
        private List<CartesianCoordinate> _rangeQueryResults = new List<CartesianCoordinate>();
        
        // GUI样式
        private GUIStyle _textStyle;
        private GUIStyle _buttonStyle;
        private bool _stylesInitialized = false;
        
        #region Unity生命周期
        
        private void Start()
        {
            InitializeVisualizer();
        }
        
        private void Update()
        {
            if (_enableMouseInteraction)
            {
                HandleMouseInput();
                HandleKeyboardInput();
            }
        }
        
        private void OnDrawGizmos()
        {
            if (_showGrid)
            {
                DrawGrid();
            }
            
            DrawHighlights();
            DrawNeighbors();
            DrawPath();
            DrawRangeQuery();
        }
        
        private void OnGUI()
        {
            InitializeGUIStyles();
            DrawUI();
        }
        
        #endregion
        
        #region 初始化
        
        /// <summary>
        /// 初始化可视化器
        /// </summary>
        private void InitializeVisualizer()
        {
            // 获取或创建网格管理器
            _gridManager = FindObjectOfType<GridCoordinateManager>();
            if (_gridManager == null)
            {
                GameObject managerObj = new GameObject("GridCoordinateManager");
                _gridManager = managerObj.AddComponent<GridCoordinateManager>();
            }
            
            // 配置网格管理器
            _gridManager.CellSize = _cellSize;
            _gridManager.GridWidth = _gridWidth;
            _gridManager.GridHeight = _gridHeight;
            
            // 获取主摄像机
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                _mainCamera = FindObjectOfType<Camera>();
            }
            
            Debug.Log("网格可视化器已初始化");
            Debug.Log($"网格大小: {_gridWidth}x{_gridHeight}, 单元大小: {_cellSize}");
        }
        
        /// <summary>
        /// 初始化GUI样式
        /// </summary>
        private void InitializeGUIStyles()
        {
            if (_stylesInitialized) return;
            
            _textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };
            
            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12
            };
            
            _stylesInitialized = true;
        }
        
        #endregion
        
        #region 输入处理
        
        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        private void HandleMouseInput()
        {
            if (_mainCamera == null) return;
            
            // 获取鼠标世界坐标
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            
            // 转换为网格坐标
            _currentHoverPosition = _gridManager.WorldToCartesian(mouseWorldPos);
            
            // 处理点击
            if (Input.GetKeyDown(_selectKey))
            {
                SelectPosition(_currentHoverPosition);
            }
        }
        
        /// <summary>
        /// 处理键盘输入
        /// </summary>
        private void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(_showNeighborsKey))
            {
                ToggleNeighbors();
            }
            
            if (Input.GetKeyDown(_showPathKey))
            {
                TogglePath();
            }
            
            if (Input.GetKeyDown(_clearKey))
            {
                ClearSelection();
            }
        }
        
        #endregion
        
        #region 选择和交互
        
        /// <summary>
        /// 选择位置
        /// </summary>
        /// <param name="position">位置</param>
        private void SelectPosition(CartesianCoordinate position)
        {
            if (!_gridManager.IsInBounds(position))
            {
                Debug.Log($"位置 {position} 超出网格边界");
                return;
            }
            
            if (_selectedPosition == null)
            {
                // 第一次选择
                _selectedPosition = position;
                Debug.Log($"选择第一个位置: {position}");
                
                // 显示邻居
                ShowNeighbors(position);
                
                // 显示范围查询
                if (_showRangeQuery)
                {
                    ShowRangeQuery(position);
                }
            }
            else if (_secondSelectedPosition == null)
            {
                // 第二次选择
                _secondSelectedPosition = position;
                Debug.Log($"选择第二个位置: {position}");
                
                // 显示路径和距离
                ShowPathAndDistance(_selectedPosition.Value, position);
            }
            else
            {
                // 重新开始选择
                ClearSelection();
                SelectPosition(position);
            }
        }
        
        /// <summary>
        /// 显示邻居
        /// </summary>
        /// <param name="position">位置</param>
        private void ShowNeighbors(CartesianCoordinate position)
        {
            _currentNeighbors.Clear();
            
            var neighbors4 = _gridManager.GetNeighbors4(position);
            var neighbors8 = _gridManager.GetNeighbors8(position);
            
            _currentNeighbors.AddRange(neighbors4);
            
            Debug.Log($"位置 {position} 的4邻居: {string.Join(", ", neighbors4)}");
            Debug.Log($"位置 {position} 的8邻居: {string.Join(", ", neighbors8)}");
        }
        
        /// <summary>
        /// 显示路径和距离
        /// </summary>
        /// <param name="from">起始位置</param>
        /// <param name="to">目标位置</param>
        private void ShowPathAndDistance(CartesianCoordinate from, CartesianCoordinate to)
        {
            // 计算距离
            float manhattanDist = _gridManager.GetDistance(from, to, GridConstants.DistanceType.Manhattan);
            float euclideanDist = _gridManager.GetDistance(from, to, GridConstants.DistanceType.Euclidean);
            float chebyshevDist = _gridManager.GetDistance(from, to, GridConstants.DistanceType.Chebyshev);
            
            Debug.Log($"从 {from} 到 {to} 的距离:");
            Debug.Log($"  曼哈顿距离: {manhattanDist}");
            Debug.Log($"  欧几里得距离: {euclideanDist:F2}");
            Debug.Log($"  切比雪夫距离: {chebyshevDist}");
            
            // 计算路径
            _currentPath = _gridManager.GetLinePath(from, to);
            Debug.Log($"路径 ({_currentPath.Count} 步): {string.Join(" -> ", _currentPath)}");
        }
        
        /// <summary>
        /// 显示范围查询
        /// </summary>
        /// <param name="center">中心位置</param>
        private void ShowRangeQuery(CartesianCoordinate center)
        {
            _rangeQueryResults = _gridManager.GetCircleRange(center, _rangeQueryRadius);
            Debug.Log($"以 {center} 为中心，半径 {_rangeQueryRadius} 的范围内有 {_rangeQueryResults.Count} 个坐标");
        }
        
        /// <summary>
        /// 切换邻居显示
        /// </summary>
        private void ToggleNeighbors()
        {
            if (_selectedPosition.HasValue)
            {
                if (_currentNeighbors.Count > 0)
                {
                    _currentNeighbors.Clear();
                }
                else
                {
                    ShowNeighbors(_selectedPosition.Value);
                }
            }
        }
        
        /// <summary>
        /// 切换路径显示
        /// </summary>
        private void TogglePath()
        {
            if (_selectedPosition.HasValue && _secondSelectedPosition.HasValue)
            {
                if (_currentPath.Count > 0)
                {
                    _currentPath.Clear();
                }
                else
                {
                    ShowPathAndDistance(_selectedPosition.Value, _secondSelectedPosition.Value);
                }
            }
        }
        
        /// <summary>
        /// 清除选择
        /// </summary>
        private void ClearSelection()
        {
            _selectedPosition = null;
            _secondSelectedPosition = null;
            _currentNeighbors.Clear();
            _currentPath.Clear();
            _rangeQueryResults.Clear();
            
            Debug.Log("清除所有选择");
        }
        
        #endregion
        
        #region 绘制方法
        
        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGrid()
        {
            Gizmos.color = _gridColor;
            
            Vector3 origin = Vector3.zero;
            
            // 绘制垂直线
            for (int x = 0; x <= _gridWidth; x++)
            {
                Vector3 start = origin + new Vector3(x * _cellSize, 0, 0);
                Vector3 end = origin + new Vector3(x * _cellSize, _gridHeight * _cellSize, 0);
                Gizmos.DrawLine(start, end);
            }
            
            // 绘制水平线
            for (int y = 0; y <= _gridHeight; y++)
            {
                Vector3 start = origin + new Vector3(0, y * _cellSize, 0);
                Vector3 end = origin + new Vector3(_gridWidth * _cellSize, y * _cellSize, 0);
                Gizmos.DrawLine(start, end);
            }
        }
        
        /// <summary>
        /// 绘制高亮
        /// </summary>
        private void DrawHighlights()
        {
            // 绘制悬停高亮
            if (_gridManager.IsInBounds(_currentHoverPosition))
            {
                Gizmos.color = _hoverColor;
                Vector3 hoverWorldPos = _gridManager.CartesianToWorld(_currentHoverPosition);
                Gizmos.DrawWireCube(hoverWorldPos, new Vector3(_cellSize * 0.9f, _cellSize * 0.9f, 0.1f));
            }
            
            // 绘制第一个选中位置
            if (_selectedPosition.HasValue)
            {
                Gizmos.color = _selectedColor;
                Vector3 selectedWorldPos = _gridManager.CartesianToWorld(_selectedPosition.Value);
                Gizmos.DrawWireCube(selectedWorldPos, new Vector3(_cellSize * 0.8f, _cellSize * 0.8f, 0.1f));
            }
            
            // 绘制第二个选中位置
            if (_secondSelectedPosition.HasValue)
            {
                Gizmos.color = Color.cyan;
                Vector3 secondSelectedWorldPos = _gridManager.CartesianToWorld(_secondSelectedPosition.Value);
                Gizmos.DrawWireCube(secondSelectedWorldPos, new Vector3(_cellSize * 0.8f, _cellSize * 0.8f, 0.1f));
            }
        }
        
        /// <summary>
        /// 绘制邻居
        /// </summary>
        private void DrawNeighbors()
        {
            if (_currentNeighbors.Count == 0) return;
            
            Gizmos.color = _neighborColor;
            
            foreach (var neighbor in _currentNeighbors)
            {
                if (_gridManager.IsInBounds(neighbor))
                {
                    Vector3 neighborWorldPos = _gridManager.CartesianToWorld(neighbor);
                    Gizmos.DrawWireCube(neighborWorldPos, new Vector3(_cellSize * 0.6f, _cellSize * 0.6f, 0.1f));
                }
            }
        }
        
        /// <summary>
        /// 绘制路径
        /// </summary>
        private void DrawPath()
        {
            if (_currentPath.Count < 2) return;
            
            Gizmos.color = _pathColor;
            
            for (int i = 0; i < _currentPath.Count - 1; i++)
            {
                Vector3 start = _gridManager.CartesianToWorld(_currentPath[i]);
                Vector3 end = _gridManager.CartesianToWorld(_currentPath[i + 1]);
                
                Gizmos.DrawLine(start, end);
                
                // 绘制箭头
                Vector3 direction = (end - start).normalized;
                Vector3 arrowHead1 = end - direction * (_cellSize * 0.2f) + Vector3.Cross(direction, Vector3.forward) * (_cellSize * 0.1f);
                Vector3 arrowHead2 = end - direction * (_cellSize * 0.2f) - Vector3.Cross(direction, Vector3.forward) * (_cellSize * 0.1f);
                
                Gizmos.DrawLine(end, arrowHead1);
                Gizmos.DrawLine(end, arrowHead2);
            }
        }
        
        /// <summary>
        /// 绘制范围查询结果
        /// </summary>
        private void DrawRangeQuery()
        {
            if (!_showRangeQuery || _rangeQueryResults.Count == 0) return;
            
            Gizmos.color = Color.yellow;
            
            foreach (var coord in _rangeQueryResults)
            {
                if (_gridManager.IsInBounds(coord))
                {
                    Vector3 worldPos = _gridManager.CartesianToWorld(coord);
                    Gizmos.DrawWireCube(worldPos, new Vector3(_cellSize * 0.3f, _cellSize * 0.3f, 0.1f));
                }
            }
        }
        
        #endregion
        
        #region UI绘制
        
        /// <summary>
        /// 绘制UI
        /// </summary>
        private void DrawUI()
        {
            DrawInfoPanel();
            DrawControlPanel();
            DrawCoordinateLabels();
        }
        
        /// <summary>
        /// 绘制信息面板
        /// </summary>
        private void DrawInfoPanel()
        {
            string info = $"四边形网格可视化器\n";
            info += $"网格大小: {_gridWidth}x{_gridHeight}\n";
            info += $"单元大小: {_cellSize}\n";
            info += $"当前悬停: {_currentHoverPosition}\n";
            
            if (_selectedPosition.HasValue)
            {
                info += $"选中位置1: {_selectedPosition.Value}\n";
                info += $"世界坐标: {_gridManager.CartesianToWorld(_selectedPosition.Value)}\n";
                info += $"索引坐标: {_gridManager.CartesianToIndex(_selectedPosition.Value)}\n";
            }
            
            if (_secondSelectedPosition.HasValue)
            {
                info += $"选中位置2: {_secondSelectedPosition.Value}\n";
                
                if (_showDistanceCalculation)
                {
                    float manhattanDist = _gridManager.GetDistance(_selectedPosition.Value, _secondSelectedPosition.Value, GridConstants.DistanceType.Manhattan);
                    float euclideanDist = _gridManager.GetDistance(_selectedPosition.Value, _secondSelectedPosition.Value, GridConstants.DistanceType.Euclidean);
                    
                    info += $"曼哈顿距离: {manhattanDist}\n";
                    info += $"欧几里得距离: {euclideanDist:F2}\n";
                }
            }
            
            GUI.Box(new Rect(10, 10, 300, 200), info, _textStyle);
        }
        
        /// <summary>
        /// 绘制控制面板
        /// </summary>
        private void DrawControlPanel()
        {
            string controls = "控制说明:\n";
            controls += $"左键: 选择位置\n";
            controls += $"{_showNeighborsKey}: 切换邻居显示\n";
            controls += $"{_showPathKey}: 切换路径显示\n";
            controls += $"{_clearKey}: 清除选择\n";
            
            GUI.Box(new Rect(Screen.width - 200, 10, 190, 120), controls, _textStyle);
            
            // 功能开关
            Rect toggleRect = new Rect(Screen.width - 200, 140, 190, 150);
            GUILayout.BeginArea(toggleRect);
            
            _showGrid = GUILayout.Toggle(_showGrid, "显示网格");
            _showCoordinates = GUILayout.Toggle(_showCoordinates, "显示坐标");
            _showCellNumbers = GUILayout.Toggle(_showCellNumbers, "显示单元编号");
            _showDistanceCalculation = GUILayout.Toggle(_showDistanceCalculation, "显示距离计算");
            _showRangeQuery = GUILayout.Toggle(_showRangeQuery, "显示范围查询");
            
            if (_showRangeQuery)
            {
                GUILayout.Label($"查询半径: {_rangeQueryRadius}");
                _rangeQueryRadius = (int)GUILayout.HorizontalSlider(_rangeQueryRadius, 1, 5);
            }
            
            if (GUILayout.Button("清除所有", _buttonStyle))
            {
                ClearSelection();
            }
            
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// 绘制坐标标签
        /// </summary>
        private void DrawCoordinateLabels()
        {
            if (!_showCoordinates || _mainCamera == null) return;
            
            GUI.color = _coordinateTextColor;
            
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    var coord = new CartesianCoordinate(x, y);
                    Vector3 worldPos = _gridManager.CartesianToWorld(coord);
                    Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);
                    
                    if (screenPos.z > 0)
                    {
                        screenPos.y = Screen.height - screenPos.y;
                        
                        string label;
                        if (_showCellNumbers)
                        {
                            var indexCoord = _gridManager.CartesianToIndex(coord);
                            label = $"({x},{y})\n[{indexCoord.Index}]";
                        }
                        else
                        {
                            label = $"({x},{y})";
                        }
                        
                        Vector2 labelSize = _textStyle.CalcSize(new GUIContent(label));
                        Rect labelRect = new Rect(screenPos.x - labelSize.x / 2, screenPos.y - labelSize.y / 2, labelSize.x, labelSize.y);
                        
                        GUI.Label(labelRect, label, _textStyle);
                    }
                }
            }
            
            GUI.color = Color.white;
        }
        
        #endregion
        
        #region 公共方法
        
        /// <summary>
        /// 设置网格大小
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetGridSize(int width, int height)
        {
            _gridWidth = width;
            _gridHeight = height;
            
            if (_gridManager != null)
            {
                _gridManager.GridWidth = width;
                _gridManager.GridHeight = height;
            }
            
            ClearSelection();
        }
        
        /// <summary>
        /// 设置单元大小
        /// </summary>
        /// <param name="cellSize">单元大小</param>
        public void SetCellSize(float cellSize)
        {
            _cellSize = cellSize;
            
            if (_gridManager != null)
            {
                _gridManager.CellSize = cellSize;
            }
        }
        
        #endregion
    }
}