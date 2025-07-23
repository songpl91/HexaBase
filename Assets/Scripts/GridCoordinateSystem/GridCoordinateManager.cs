using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoordinateSystem.Core;
using GridCoordinateSystem.Coordinates;
using GridCoordinateSystem.Utils;

namespace GridCoordinateSystem
{
    /// <summary>
    /// 四边形网格坐标系统管理器
    /// 提供统一的接口来管理和使用各种四边形网格坐标系统
    /// 作为所有格子游戏的基础底板功能
    /// </summary>
    public class GridCoordinateManager : MonoBehaviour
    {
        #region 配置参数
        
        [Header("网格配置")]
        [SerializeField] private float cellSize = 1.0f;
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 10;
        [SerializeField] private GridConstants.GridAlignment alignment = GridConstants.GridAlignment.Center;
        
        [Header("性能配置")]
        [SerializeField] private bool enableCaching = true;
        [SerializeField] private bool enableObjectPool = true;
        [SerializeField] private int maxCacheSize = 1000;
        [SerializeField] private bool enableDebugMode = false;
        
        [Header("可视化配置")]
        [SerializeField] private bool showGrid = false;
        [SerializeField] private bool showCoordinateLabels = false;
        [SerializeField] private Color gridColor = Color.white;
        [SerializeField] private Color labelColor = Color.yellow;
        
        [Header("边界配置")]
        [SerializeField] private bool enforceBounds = true;
        [SerializeField] private bool wrapAroundX = false;
        [SerializeField] private bool wrapAroundY = false;
        
        #endregion
        
        #region 私有字段
        
        private GridCoordinateCache _cache;
        private static GridCoordinateManager _instance;
        private Dictionary<int, List<CartesianCoordinate>> _neighborCache;
        private Dictionary<string, List<CartesianCoordinate>> _rangeCache;
        
        #endregion
        
        #region 事件
        
        /// <summary>
        /// 网格配置改变事件
        /// </summary>
        public event Action<GridCoordinateManager> OnGridConfigChanged;
        
        /// <summary>
        /// 坐标访问事件
        /// </summary>
        public event Action<IGridCoordinate> OnCoordinateAccessed;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 单例实例
        /// </summary>
        public static GridCoordinateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GridCoordinateManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("GridCoordinateManager");
                        _instance = go.AddComponent<GridCoordinateManager>();
                    }
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// 网格单元大小
        /// </summary>
        public float CellSize
        {
            get => cellSize;
            set
            {
                if (!GridConstants.Approximately(cellSize, value))
                {
                    cellSize = Mathf.Max(0.1f, value);
                    OnGridConfigChanged?.Invoke(this);
                }
            }
        }
        
        /// <summary>
        /// 网格宽度
        /// </summary>
        public int GridWidth
        {
            get => gridWidth;
            set
            {
                if (gridWidth != value)
                {
                    gridWidth = Mathf.Max(1, value);
                    ClearCache();
                    OnGridConfigChanged?.Invoke(this);
                }
            }
        }
        
        /// <summary>
        /// 网格高度
        /// </summary>
        public int GridHeight
        {
            get => gridHeight;
            set
            {
                if (gridHeight != value)
                {
                    gridHeight = Mathf.Max(1, value);
                    ClearCache();
                    OnGridConfigChanged?.Invoke(this);
                }
            }
        }
        
        /// <summary>
        /// 网格对齐方式
        /// </summary>
        public GridConstants.GridAlignment Alignment
        {
            get => alignment;
            set
            {
                if (alignment != value)
                {
                    alignment = value;
                    OnGridConfigChanged?.Invoke(this);
                }
            }
        }
        
        /// <summary>
        /// 是否启用边界限制
        /// </summary>
        public bool EnforceBounds
        {
            get => enforceBounds;
            set => enforceBounds = value;
        }
        
        /// <summary>
        /// 网格总单元数
        /// </summary>
        public int TotalCells => gridWidth * gridHeight;
        
        /// <summary>
        /// 网格中心坐标
        /// </summary>
        public CartesianCoordinate GridCenter => new CartesianCoordinate(gridWidth / 2, gridHeight / 2);
        
        /// <summary>
        /// 坐标缓存
        /// </summary>
        public GridCoordinateCache Cache => _cache;
        
        #endregion
        
        #region Unity生命周期
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeManager();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (showGrid && Application.isPlaying)
            {
                DrawGrid();
            }
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                CleanupManager();
                _instance = null;
            }
        }
        
        #endregion
        
        #region 初始化和清理
        
        /// <summary>
        /// 初始化管理器
        /// </summary>
        private void InitializeManager()
        {
            if (enableCaching)
            {
                _cache = new GridCoordinateCache
                {
                    MaxCacheSize = maxCacheSize
                };
                _neighborCache = new Dictionary<int, List<CartesianCoordinate>>();
                _rangeCache = new Dictionary<string, List<CartesianCoordinate>>();
            }
            
            if (enableDebugMode)
            {
                Debug.Log("四边形网格坐标系统管理器已初始化");
                Debug.Log($"配置: 大小={cellSize}, 宽度={gridWidth}, 高度={gridHeight}, 对齐={alignment}");
            }
        }
        
        /// <summary>
        /// 清理管理器
        /// </summary>
        private void CleanupManager()
        {
            ClearCache();
            
            if (enableObjectPool)
            {
                GridObjectPool.ClearAllPools();
            }
            
            if (enableDebugMode)
            {
                Debug.Log("四边形网格坐标系统管理器已清理");
            }
        }
        
        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            _cache?.ClearCache();
            _neighborCache?.Clear();
            _rangeCache?.Clear();
        }
        
        #endregion
        
        #region 坐标转换方法
        
        /// <summary>
        /// 世界坐标转直角坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>直角坐标</returns>
        public CartesianCoordinate WorldToCartesian(Vector3 worldPosition)
        {
            var coord = GridConverter.WorldToCartesian(worldPosition, cellSize, alignment);
            
            if (enforceBounds)
            {
                coord = ApplyBounds(coord);
            }
            
            OnCoordinateAccessed?.Invoke(coord);
            return coord;
        }
        
        /// <summary>
        /// 直角坐标转世界坐标
        /// </summary>
        /// <param name="cartesian">直角坐标</param>
        /// <returns>世界坐标</returns>
        public Vector3 CartesianToWorld(CartesianCoordinate cartesian)
        {
            return GridConverter.CartesianToWorld(cartesian, cellSize);
        }
        
        /// <summary>
        /// 世界坐标转索引坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>索引坐标</returns>
        public IndexCoordinate WorldToIndex(Vector3 worldPosition)
        {
            var cartesian = WorldToCartesian(worldPosition);
            return CartesianToIndex(cartesian);
        }
        
        /// <summary>
        /// 索引坐标转世界坐标
        /// </summary>
        /// <param name="index">索引坐标</param>
        /// <returns>世界坐标</returns>
        public Vector3 IndexToWorld(IndexCoordinate index)
        {
            var cartesian = IndexToCartesian(index);
            return CartesianToWorld(cartesian);
        }
        
        /// <summary>
        /// 直角坐标转索引坐标
        /// </summary>
        /// <param name="cartesian">直角坐标</param>
        /// <returns>索引坐标</returns>
        public IndexCoordinate CartesianToIndex(CartesianCoordinate cartesian)
        {
            return GridConverter.CartesianToIndex(cartesian, gridWidth);
        }
        
        /// <summary>
        /// 索引坐标转直角坐标
        /// </summary>
        /// <param name="index">索引坐标</param>
        /// <returns>直角坐标</returns>
        public CartesianCoordinate IndexToCartesian(IndexCoordinate index)
        {
            return GridConverter.IndexToCartesian(index, gridWidth);
        }
        
        #endregion
        
        #region 邻居查找
        
        /// <summary>
        /// 获取4邻居坐标
        /// </summary>
        /// <param name="coord">中心坐标</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>邻居坐标列表</returns>
        public List<CartesianCoordinate> GetNeighbors4(CartesianCoordinate coord, bool useCache = true)
        {
            if (useCache && enableCaching && _neighborCache != null)
            {
                int key = coord.GetHashCode();
                if (_neighborCache.TryGetValue(key, out var cachedNeighbors))
                {
                    return new List<CartesianCoordinate>(cachedNeighbors);
                }
            }
            
            var neighbors = new List<CartesianCoordinate>();
            
            foreach (var direction in GridConstants.DIRECTIONS_4)
            {
                var neighbor = new CartesianCoordinate(coord.X + direction.x, coord.Y + direction.y);
                
                if (enforceBounds)
                {
                    neighbor = ApplyBounds(neighbor);
                    if (IsValidCoordinate(neighbor))
                    {
                        neighbors.Add(neighbor);
                    }
                }
                else
                {
                    neighbors.Add(neighbor);
                }
            }
            
            if (useCache && enableCaching && _neighborCache != null)
            {
                int key = coord.GetHashCode();
                _neighborCache[key] = new List<CartesianCoordinate>(neighbors);
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 获取8邻居坐标
        /// </summary>
        /// <param name="coord">中心坐标</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>邻居坐标列表</returns>
        public List<CartesianCoordinate> GetNeighbors8(CartesianCoordinate coord, bool useCache = true)
        {
            var neighbors = new List<CartesianCoordinate>();
            
            foreach (var direction in GridConstants.DIRECTIONS_8)
            {
                var neighbor = new CartesianCoordinate(coord.X + direction.x, coord.Y + direction.y);
                
                if (enforceBounds)
                {
                    neighbor = ApplyBounds(neighbor);
                    if (IsValidCoordinate(neighbor))
                    {
                        neighbors.Add(neighbor);
                    }
                }
                else
                {
                    neighbors.Add(neighbor);
                }
            }
            
            return neighbors;
        }
        
        #endregion
        
        #region 距离和路径
        
        /// <summary>
        /// 计算两个坐标之间的距离
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <param name="distanceType">距离类型</param>
        /// <returns>距离值</returns>
        public float CalculateDistance(IGridCoordinate from, IGridCoordinate to, 
            GridConstants.DistanceType distanceType = GridConstants.DistanceType.Manhattan)
        {
            return GridConverter.CalculateDistance(from, to, distanceType);
        }
        
        /// <summary>
        /// 计算两个坐标之间的距离（简化方法名）
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <param name="distanceType">距离类型</param>
        /// <returns>距离值</returns>
        public float GetDistance(IGridCoordinate from, IGridCoordinate to, 
            GridConstants.DistanceType distanceType = GridConstants.DistanceType.Manhattan)
        {
            return CalculateDistance(from, to, distanceType);
        }
        
        /// <summary>
        /// 计算两个直角坐标之间的距离
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <param name="distanceType">距离类型</param>
        /// <returns>距离值</returns>
        public float GetDistance(CartesianCoordinate from, CartesianCoordinate to, 
            GridConstants.DistanceType distanceType = GridConstants.DistanceType.Manhattan)
        {
            return GridConverter.CalculateDistance(from, to, distanceType);
        }
        
        /// <summary>
        /// 获取两点之间的直线路径
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <returns>路径坐标列表</returns>
        public List<CartesianCoordinate> GetLinePath(CartesianCoordinate from, CartesianCoordinate to)
        {
            var path = GridConverter.GetLinePath(from, to);
            
            if (enforceBounds)
            {
                var validPath = new List<CartesianCoordinate>();
                foreach (var coord in path)
                {
                    if (IsValidCoordinate(coord))
                    {
                        validPath.Add(coord);
                    }
                }
                return validPath;
            }
            
            return path;
        }
        
        #endregion
        
        #region 范围查询
        
        /// <summary>
        /// 获取矩形范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>范围内坐标列表</returns>
        public List<CartesianCoordinate> GetCoordinatesInRectangle(CartesianCoordinate center, 
            int width, int height, bool useCache = true)
        {
            string cacheKey = $"rect_{center.X}_{center.Y}_{width}_{height}";
            
            if (useCache && enableCaching && _rangeCache != null && _rangeCache.TryGetValue(cacheKey, out var cached))
            {
                return new List<CartesianCoordinate>(cached);
            }
            
            var coordinates = GridConverter.GetCoordinatesInRectangle(center, width, height);
            
            if (enforceBounds)
            {
                var validCoordinates = new List<CartesianCoordinate>();
                foreach (var coord in coordinates)
                {
                    if (IsValidCoordinate(coord))
                    {
                        validCoordinates.Add(coord);
                    }
                }
                coordinates = validCoordinates;
            }
            
            if (useCache && enableCaching && _rangeCache != null)
            {
                _rangeCache[cacheKey] = new List<CartesianCoordinate>(coordinates);
            }
            
            return coordinates;
        }
        
        /// <summary>
        /// 获取圆形范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <param name="distanceType">距离计算类型</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>范围内坐标列表</returns>
        public List<CartesianCoordinate> GetCoordinatesInCircle(CartesianCoordinate center, 
            int radius, GridConstants.DistanceType distanceType = GridConstants.DistanceType.Euclidean, 
            bool useCache = true)
        {
            string cacheKey = $"circle_{center.X}_{center.Y}_{radius}_{distanceType}";
            
            if (useCache && enableCaching && _rangeCache != null && _rangeCache.TryGetValue(cacheKey, out var cached))
            {
                return new List<CartesianCoordinate>(cached);
            }
            
            var coordinates = GridConverter.GetCoordinatesInCircle(center, radius, distanceType);
            
            if (enforceBounds)
            {
                var validCoordinates = new List<CartesianCoordinate>();
                foreach (var coord in coordinates)
                {
                    if (IsValidCoordinate(coord))
                    {
                        validCoordinates.Add(coord);
                    }
                }
                coordinates = validCoordinates;
            }
            
            if (useCache && enableCaching && _rangeCache != null)
            {
                _rangeCache[cacheKey] = new List<CartesianCoordinate>(coordinates);
            }
            
            return coordinates;
        }
        
        /// <summary>
        /// 获取矩形范围内的所有坐标（简化方法名）
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>范围内坐标列表</returns>
        public List<CartesianCoordinate> GetRectangleRange(CartesianCoordinate center, 
            int width, int height, bool useCache = true)
        {
            return GetCoordinatesInRectangle(center, width, height, useCache);
        }
        
        /// <summary>
        /// 获取圆形范围内的所有坐标（简化方法名）
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>范围内坐标列表</returns>
        public List<CartesianCoordinate> GetCircleRange(CartesianCoordinate center, 
            int radius, bool useCache = true)
        {
            return GetCoordinatesInCircle(center, radius, GridConstants.DistanceType.Euclidean, useCache);
        }
        
        /// <summary>
        /// 获取曼哈顿距离范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="distance">曼哈顿距离</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>范围内坐标列表</returns>
        public List<CartesianCoordinate> GetManhattanRange(CartesianCoordinate center, 
            int distance, bool useCache = true)
        {
            return GetCoordinatesInCircle(center, distance, GridConstants.DistanceType.Manhattan, useCache);
        }
        
        /// <summary>
        /// 获取切比雪夫距离范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="distance">切比雪夫距离</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <returns>范围内坐标列表</returns>
        public List<CartesianCoordinate> GetChebyshevRange(CartesianCoordinate center, 
            int distance, bool useCache = true)
        {
            return GetCoordinatesInCircle(center, distance, GridConstants.DistanceType.Chebyshev, useCache);
        }
        
        /// <summary>
        /// 获取所有网格坐标
        /// </summary>
        /// <returns>所有网格坐标列表</returns>
        public List<CartesianCoordinate> GetAllCoordinates()
        {
            var coordinates = new List<CartesianCoordinate>();
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    coordinates.Add(new CartesianCoordinate(x, y));
                }
            }
            
            return coordinates;
        }
        
        #endregion
        
        #region 边界处理
        
        /// <summary>
        /// 检查坐标是否有效
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <returns>是否有效</returns>
        public bool IsValidCoordinate(CartesianCoordinate coord)
        {
            return coord.IsWithinBounds(0, 0, gridWidth - 1, gridHeight - 1);
        }
        
        /// <summary>
        /// 检查索引坐标是否有效
        /// </summary>
        /// <param name="index">索引坐标</param>
        /// <returns>是否有效</returns>
        public bool IsValidCoordinate(IndexCoordinate index)
        {
            return index.IsWithinBounds(gridWidth, gridHeight);
        }
        
        /// <summary>
        /// 检查坐标是否在边界内（简化方法名）
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <returns>是否在边界内</returns>
        public bool IsInBounds(CartesianCoordinate coord)
        {
            return IsValidCoordinate(coord);
        }
        
        /// <summary>
        /// 检查索引坐标是否在边界内（简化方法名）
        /// </summary>
        /// <param name="index">索引坐标</param>
        /// <returns>是否在边界内</returns>
        public bool IsInBounds(IndexCoordinate index)
        {
            return IsValidCoordinate(index);
        }
        
        /// <summary>
        /// 检查通用坐标是否在边界内
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <returns>是否在边界内</returns>
        public bool IsInBounds(IGridCoordinate coord)
        {
            var cartesian = coord.ToCartesian();
            return IsValidCoordinate(cartesian);
        }
        
        /// <summary>
        /// 应用边界限制
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <returns>应用边界后的坐标</returns>
        private CartesianCoordinate ApplyBounds(CartesianCoordinate coord)
        {
            int x = coord.X;
            int y = coord.Y;
            
            if (wrapAroundX)
            {
                x = ((x % gridWidth) + gridWidth) % gridWidth;
            }
            else
            {
                x = Mathf.Clamp(x, 0, gridWidth - 1);
            }
            
            if (wrapAroundY)
            {
                y = ((y % gridHeight) + gridHeight) % gridHeight;
            }
            else
            {
                y = Mathf.Clamp(y, 0, gridHeight - 1);
            }
            
            return new CartesianCoordinate(x, y);
        }
        
        #endregion
        
        #region 可视化
        
        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGrid()
        {
            Gizmos.color = gridColor;
            
            // 绘制垂直线
            for (int x = 0; x <= gridWidth; x++)
            {
                Vector3 start = new Vector3(x * cellSize, 0, 0);
                Vector3 end = new Vector3(x * cellSize, 0, gridHeight * cellSize);
                Gizmos.DrawLine(start, end);
            }
            
            // 绘制水平线
            for (int y = 0; y <= gridHeight; y++)
            {
                Vector3 start = new Vector3(0, 0, y * cellSize);
                Vector3 end = new Vector3(gridWidth * cellSize, 0, y * cellSize);
                Gizmos.DrawLine(start, end);
            }
            
            // 绘制坐标标签
            if (showCoordinateLabels)
            {
                DrawCoordinateLabels();
            }
        }
        
        /// <summary>
        /// 绘制坐标标签
        /// </summary>
        private void DrawCoordinateLabels()
        {
            // 这里可以使用GUI或其他方式绘制坐标标签
            // 由于Gizmos不支持文本，这里只是占位
        }
        
        #endregion
        
        #region 调试和统计
        
        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存统计信息</returns>
        public string GetCacheStats()
        {
            if (!enableCaching || _cache == null)
            {
                return "缓存未启用";
            }
            
            int neighborCacheCount = _neighborCache?.Count ?? 0;
            int rangeCacheCount = _rangeCache?.Count ?? 0;
            
            return $"邻居缓存: {neighborCacheCount}, 范围缓存: {rangeCacheCount}";
        }
        
        /// <summary>
        /// 获取网格信息
        /// </summary>
        /// <returns>网格信息</returns>
        public string GetGridInfo()
        {
            return $"网格大小: {gridWidth}x{gridHeight}, 单元大小: {cellSize}, 总单元数: {TotalCells}";
        }
        
        #endregion
    }
}