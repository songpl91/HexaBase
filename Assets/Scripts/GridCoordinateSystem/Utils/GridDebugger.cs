using UnityEngine;
using GridCoordinateSystem.Coordinates;
using GridCoordinateSystem.Core;

namespace GridCoordinateSystem.Utils
{
    /// <summary>
    /// 四边形网格调试器
    /// 提供网格系统的可视化调试功能
    /// </summary>
    public class GridDebugger : MonoBehaviour
    {
        #region 序列化字段
        
        [Header("调试设置")]
        [SerializeField] private bool _enableDebug = true;
        [SerializeField] private bool _showGrid = true;
        [SerializeField] private bool _showCoordinates = true;
        [SerializeField] private bool _showMousePosition = true;
        [SerializeField] private bool _showNeighbors = false;
        [SerializeField] private bool _showPath = false;
        
        [Header("可视化样式")]
        [SerializeField] private Color _gridColor = Color.white;
        [SerializeField] private Color _coordinateTextColor = Color.yellow;
        [SerializeField] private Color _mousePositionColor = Color.red;
        [SerializeField] private Color _neighborColor = Color.green;
        [SerializeField] private Color _pathColor = Color.blue;
        [SerializeField] private float _lineWidth = 1f;
        [SerializeField] private int _fontSize = 12;
        
        [Header("调试范围")]
        [SerializeField] private int _debugRangeX = 10;
        [SerializeField] private int _debugRangeY = 10;
        [SerializeField] private Vector2Int _debugOffset = Vector2Int.zero;
        
        [Header("交互设置")]
        [SerializeField] private bool _enableMouseInteraction = true;
        [SerializeField] private KeyCode _toggleDebugKey = KeyCode.F1;
        [SerializeField] private KeyCode _toggleGridKey = KeyCode.F2;
        [SerializeField] private KeyCode _toggleCoordinatesKey = KeyCode.F3;
        
        #endregion
        
        #region 私有字段
        
        private GridCoordinateManager _gridManager;
        private Camera _mainCamera;
        private CartesianCoordinate _mouseGridPosition;
        private CartesianCoordinate _selectedPosition;
        private CartesianCoordinate[] _neighbors;
        private CartesianCoordinate[] _pathCoordinates;
        private GUIStyle _textStyle;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 是否启用调试
        /// </summary>
        public bool EnableDebug
        {
            get => _enableDebug;
            set => _enableDebug = value;
        }
        
        /// <summary>
        /// 是否显示网格
        /// </summary>
        public bool ShowGrid
        {
            get => _showGrid;
            set => _showGrid = value;
        }
        
        /// <summary>
        /// 是否显示坐标
        /// </summary>
        public bool ShowCoordinates
        {
            get => _showCoordinates;
            set => _showCoordinates = value;
        }
        
        /// <summary>
        /// 当前鼠标网格位置
        /// </summary>
        public CartesianCoordinate MouseGridPosition => _mouseGridPosition;
        
        /// <summary>
        /// 当前选中位置
        /// </summary>
        public CartesianCoordinate SelectedPosition => _selectedPosition;
        
        #endregion
        
        #region Unity生命周期
        
        private void Awake()
        {
            _gridManager = GridCoordinateManager.Instance;
            _mainCamera = Camera.main;
            
            if (_mainCamera == null)
                _mainCamera = FindObjectOfType<Camera>();
        }
        
        private void Start()
        {
            InitializeTextStyle();
        }
        
        private void Update()
        {
            if (!_enableDebug) return;
            
            HandleInput();
            UpdateMousePosition();
        }
        
        private void OnDrawGizmos()
        {
            if (!_enableDebug || _gridManager == null) return;
            
            if (_showGrid)
                DrawGrid();
                
            if (_showNeighbors && _selectedPosition != null)
                DrawNeighbors();
                
            if (_showPath && _pathCoordinates != null && _pathCoordinates.Length > 0)
                DrawPath();
        }
        
        private void OnGUI()
        {
            if (!_enableDebug || _gridManager == null) return;
            
            if (_showCoordinates)
                DrawCoordinateLabels();
                
            if (_showMousePosition)
                DrawMousePositionInfo();
                
            DrawDebugInfo();
        }
        
        #endregion
        
        #region 输入处理
        
        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleInput()
        {
            // 切换调试功能
            if (Input.GetKeyDown(_toggleDebugKey))
                _enableDebug = !_enableDebug;
                
            if (Input.GetKeyDown(_toggleGridKey))
                _showGrid = !_showGrid;
                
            if (Input.GetKeyDown(_toggleCoordinatesKey))
                _showCoordinates = !_showCoordinates;
            
            // 鼠标交互
            if (_enableMouseInteraction && Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }
        
        /// <summary>
        /// 更新鼠标位置
        /// </summary>
        private void UpdateMousePosition()
        {
            if (_mainCamera == null) return;
            
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            
            _mouseGridPosition = _gridManager.WorldToCartesian(mouseWorldPos);
        }
        
        /// <summary>
        /// 处理鼠标点击
        /// </summary>
        private void HandleMouseClick()
        {
            _selectedPosition = _mouseGridPosition;
            
            if (_showNeighbors)
            {
                _neighbors = new CartesianCoordinate[8];
                var neighbors8 = _selectedPosition.GetNeighbors8();
                for (int i = 0; i < neighbors8.Count && i < 8; i++)
                {
                    _neighbors[i] = (CartesianCoordinate)neighbors8[i];
                }
            }
        }
        
        #endregion
        
        #region 绘制方法
        
        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGrid()
        {
            Gizmos.color = _gridColor;
            
            Vector2 cellSize = new Vector2(_gridManager.CellSize, _gridManager.CellSize);
            Vector2 gridOrigin = Vector2.zero; // 使用原点作为网格起始位置
            
            // 计算绘制范围
            int startX = _debugOffset.x - _debugRangeX / 2;
            int endX = _debugOffset.x + _debugRangeX / 2;
            int startY = _debugOffset.y - _debugRangeY / 2;
            int endY = _debugOffset.y + _debugRangeY / 2;
            
            // 绘制垂直线
            for (int x = startX; x <= endX; x++)
            {
                Vector3 start = new Vector3(gridOrigin.x + x * cellSize.x, gridOrigin.y + startY * cellSize.y, 0);
                Vector3 end = new Vector3(gridOrigin.x + x * cellSize.x, gridOrigin.y + endY * cellSize.y, 0);
                Gizmos.DrawLine(start, end);
            }
            
            // 绘制水平线
            for (int y = startY; y <= endY; y++)
            {
                Vector3 start = new Vector3(gridOrigin.x + startX * cellSize.x, gridOrigin.y + y * cellSize.y, 0);
                Vector3 end = new Vector3(gridOrigin.x + endX * cellSize.x, gridOrigin.y + y * cellSize.y, 0);
                Gizmos.DrawLine(start, end);
            }
        }
        
        /// <summary>
        /// 绘制邻居
        /// </summary>
        private void DrawNeighbors()
        {
            if (_neighbors == null) return;
            
            Gizmos.color = _neighborColor;
            Vector2 cellSize = new Vector2(_gridManager.CellSize, _gridManager.CellSize);
            
            foreach (var neighbor in _neighbors)
            {
                if (neighbor != null)
                {
                    Vector3 worldPos = _gridManager.CartesianToWorld(neighbor);
                    Gizmos.DrawWireCube(worldPos, new Vector3(cellSize.x * 0.8f, cellSize.y * 0.8f, 0));
                }
            }
        }
        
        /// <summary>
        /// 绘制路径
        /// </summary>
        private void DrawPath()
        {
            if (_pathCoordinates == null || _pathCoordinates.Length < 2) return;
            
            Gizmos.color = _pathColor;
            
            for (int i = 0; i < _pathCoordinates.Length - 1; i++)
            {
                Vector3 start = _gridManager.CartesianToWorld(_pathCoordinates[i]);
                Vector3 end = _gridManager.CartesianToWorld(_pathCoordinates[i + 1]);
                Gizmos.DrawLine(start, end);
                
                // 绘制箭头
                Vector3 direction = (end - start).normalized;
                Vector3 arrowHead1 = end - direction * 0.2f + Vector3.Cross(direction, Vector3.forward) * 0.1f;
                Vector3 arrowHead2 = end - direction * 0.2f - Vector3.Cross(direction, Vector3.forward) * 0.1f;
                
                Gizmos.DrawLine(end, arrowHead1);
                Gizmos.DrawLine(end, arrowHead2);
            }
        }
        
        /// <summary>
        /// 绘制坐标标签
        /// </summary>
        private void DrawCoordinateLabels()
        {
            if (_mainCamera == null) return;
            
            GUI.color = _coordinateTextColor;
            
            int startX = _debugOffset.x - _debugRangeX / 2;
            int endX = _debugOffset.x + _debugRangeX / 2;
            int startY = _debugOffset.y - _debugRangeY / 2;
            int endY = _debugOffset.y + _debugRangeY / 2;
            
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    var coord = new CartesianCoordinate(x, y);
                    Vector3 worldPos = _gridManager.CartesianToWorld(coord);
                    Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);
                    
                    if (screenPos.z > 0)
                    {
                        screenPos.y = Screen.height - screenPos.y;
                        string label = $"({x},{y})";
                        
                        Vector2 labelSize = _textStyle.CalcSize(new GUIContent(label));
                        Rect labelRect = new Rect(screenPos.x - labelSize.x / 2, screenPos.y - labelSize.y / 2, labelSize.x, labelSize.y);
                        
                        GUI.Label(labelRect, label, _textStyle);
                    }
                }
            }
        }
        
        /// <summary>
        /// 绘制鼠标位置信息
        /// </summary>
        private void DrawMousePositionInfo()
        {
            GUI.color = _mousePositionColor;
            
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            string info = $"鼠标位置:\n世界坐标: ({mouseWorldPos.x:F2}, {mouseWorldPos.y:F2})\n网格坐标: {_mouseGridPosition}";
            
            Rect infoRect = new Rect(10, 10, 300, 80);
            GUI.Box(infoRect, info, _textStyle);
        }
        
        /// <summary>
        /// 绘制调试信息
        /// </summary>
        private void DrawDebugInfo()
        {
            GUI.color = Color.white;
            
            string debugInfo = $"调试控制:\n{_toggleDebugKey}: 切换调试\n{_toggleGridKey}: 切换网格\n{_toggleCoordinatesKey}: 切换坐标";
            
            if (_selectedPosition != null)
            {
                debugInfo += $"\n\n选中位置: {_selectedPosition}";
                debugInfo += $"\n世界坐标: {_gridManager.CartesianToWorld(_selectedPosition)}";
            }
            
            Rect debugRect = new Rect(Screen.width - 250, 10, 240, 150);
            GUI.Box(debugRect, debugInfo, _textStyle);
        }
        
        #endregion
        
        #region 公共方法
        
        /// <summary>
        /// 设置路径用于可视化
        /// </summary>
        /// <param name="path">路径坐标数组</param>
        public void SetPath(CartesianCoordinate[] path)
        {
            _pathCoordinates = path;
            _showPath = path != null && path.Length > 0;
        }
        
        /// <summary>
        /// 清除路径
        /// </summary>
        public void ClearPath()
        {
            _pathCoordinates = null;
            _showPath = false;
        }
        
        /// <summary>
        /// 设置调试范围
        /// </summary>
        /// <param name="rangeX">X轴范围</param>
        /// <param name="rangeY">Y轴范围</param>
        /// <param name="offset">偏移量</param>
        public void SetDebugRange(int rangeX, int rangeY, Vector2Int offset = default)
        {
            _debugRangeX = Mathf.Max(1, rangeX);
            _debugRangeY = Mathf.Max(1, rangeY);
            _debugOffset = offset;
        }
        
        #endregion
        
        #region 私有方法
        
        /// <summary>
        /// 初始化文本样式
        /// </summary>
        private void InitializeTextStyle()
        {
            _textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = _fontSize,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = _coordinateTextColor }
            };
        }
        
        #endregion
    }
}