using System;
using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;
using HexCoordinateSystem.Coordinates;
using HexCoordinateSystem.Utils;

namespace HexCoordinateSystem
{
    /// <summary>
    /// 六边形坐标系统管理器
    /// 提供统一的接口来管理和使用各种六边形坐标系统
    /// </summary>
    public class HexCoordinateManager : MonoBehaviour
    {
        #region 配置参数
        
        [Header("六边形配置")]
        [SerializeField] private float hexSize = 1.0f;
        [SerializeField] private bool usePointyTop = true;
        [SerializeField] private bool enableCaching = true;
        [SerializeField] private bool enableObjectPool = true;
        
        [Header("性能配置")]
        [SerializeField] private int maxCacheSize = 1000;
        [SerializeField] private bool enableDebugMode = false;
        
        [Header("可视化配置")]
        [SerializeField] private bool showGrid = false;
        [SerializeField] private int gridRadius = 5;
        [SerializeField] private Color gridColor = Color.white;
        [SerializeField] private bool showCoordinateLabels = false;
        
        #endregion
        
        #region 私有字段
        
        private HexCoordinateCache _cache;
        private static HexCoordinateManager _instance;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 单例实例
        /// </summary>
        public static HexCoordinateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<HexCoordinateManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("HexCoordinateManager");
                        _instance = go.AddComponent<HexCoordinateManager>();
                    }
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// 六边形大小
        /// </summary>
        public float HexSize
        {
            get => hexSize;
            set => hexSize = Mathf.Max(0.1f, value);
        }
        
        /// <summary>
        /// 是否使用尖顶六边形
        /// </summary>
        public bool UsePointyTop
        {
            get => usePointyTop;
            set => usePointyTop = value;
        }
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCaching
        {
            get => enableCaching;
            set => enableCaching = value;
        }
        
        /// <summary>
        /// 是否启用对象池
        /// </summary>
        public bool EnableObjectPool
        {
            get => enableObjectPool;
            set => enableObjectPool = value;
        }
        
        /// <summary>
        /// 坐标缓存
        /// </summary>
        public HexCoordinateCache Cache => _cache;
        
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
                DrawHexGrid();
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
                _cache = new HexCoordinateCache
                {
                    MaxCacheSize = maxCacheSize
                };
            }
            
            if (enableDebugMode)
            {
                Debug.Log("六边形坐标系统管理器已初始化");
                Debug.Log($"配置: 大小={hexSize}, 尖顶={usePointyTop}, 缓存={enableCaching}, 对象池={enableObjectPool}");
            }
        }
        
        /// <summary>
        /// 清理管理器
        /// </summary>
        private void CleanupManager()
        {
            _cache?.ClearCache();
            
            if (enableObjectPool)
            {
                HexObjectPool.ClearAllPools();
            }
            
            if (enableDebugMode)
            {
                Debug.Log("六边形坐标系统管理器已清理");
            }
        }
        
        #endregion
        
        #region 坐标转换方法
        
        /// <summary>
        /// 世界坐标转轴向坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>轴向坐标</returns>
        public AxialCoordinate WorldToAxial(Vector3 worldPosition)
        {
            if (usePointyTop)
            {
                return HexConverter.WorldToAxialPointy(worldPosition, hexSize);
            }
            else
            {
                return HexConverter.WorldToAxialFlat(worldPosition, hexSize);
            }
        }
        
        /// <summary>
        /// 轴向坐标转世界坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>世界坐标</returns>
        public Vector3 AxialToWorld(AxialCoordinate axial)
        {
            if (enableCaching && _cache != null)
            {
                return _cache.GetOrComputeWorldPosition(axial, hexSize);
            }
            
            if (usePointyTop)
            {
                return HexConverter.AxialToWorldPointy(axial, hexSize);
            }
            else
            {
                return HexConverter.AxialToWorldFlat(axial, hexSize);
            }
        }
        
        /// <summary>
        /// 获取坐标的邻居
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <returns>邻居坐标列表</returns>
        public List<AxialCoordinate> GetNeighbors(AxialCoordinate coordinate)
        {
            if (enableCaching && _cache != null)
            {
                return _cache.GetOrComputeNeighbors(coordinate);
            }
            
            var neighbors = coordinate.GetNeighbors();
            var result = new List<AxialCoordinate>(neighbors.Count);
            for (int i = 0; i < neighbors.Count; i++)
            {
                result.Add(neighbors[i].ToAxial());
            }
            return result;
        }
        
        /// <summary>
        /// 计算两个坐标之间的距离
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <returns>距离</returns>
        public int GetDistance(AxialCoordinate from, AxialCoordinate to)
        {
            if (enableCaching && _cache != null)
            {
                return _cache.GetOrComputeDistance(from, to);
            }
            
            return AxialCoordinate.Distance(from, to);
        }
        
        /// <summary>
        /// 获取指定范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="range">范围</param>
        /// <returns>坐标列表</returns>
        public List<AxialCoordinate> GetCoordinatesInRange(AxialCoordinate center, int range)
        {
            if (enableObjectPool)
            {
                var result = HexObjectPool.GetAxialList(range * range * 3);
                var coordinates = AxialCoordinate.GetCoordinatesInRange(center, range);
                result.AddRange(coordinates);
                return result;
            }
            
            return AxialCoordinate.GetCoordinatesInRange(center, range);
        }
        
        /// <summary>
        /// 获取两个坐标之间的路径
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <returns>路径坐标列表</returns>
        public List<AxialCoordinate> GetLinePath(AxialCoordinate from, AxialCoordinate to)
        {
            if (enableObjectPool)
            {
                var distance = GetDistance(from, to);
                var result = HexObjectPool.GetAxialList(distance + 1);
                var path = AxialCoordinate.GetLinePath(from, to);
                result.AddRange(path);
                return result;
            }
            
            return AxialCoordinate.GetLinePath(from, to);
        }
        
        #endregion
        
        #region 坐标系统创建方法
        
        /// <summary>
        /// 创建轴向坐标
        /// </summary>
        /// <param name="q">q坐标</param>
        /// <param name="r">r坐标</param>
        /// <returns>轴向坐标</returns>
        public AxialCoordinate CreateAxial(int q, int r)
        {
            return new AxialCoordinate(q, r);
        }
        
        /// <summary>
        /// 创建立方坐标
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="z">z坐标</param>
        /// <returns>立方坐标</returns>
        public CubeCoordinate CreateCube(int x, int y, int z)
        {
            return new CubeCoordinate(x, y, z);
        }
        
        /// <summary>
        /// 创建奇数列偏移坐标
        /// </summary>
        /// <param name="col">列</param>
        /// <param name="row">行</param>
        /// <returns>奇数列偏移坐标</returns>
        public OffsetCoordinateOddQ CreateOffsetOddQ(int col, int row)
        {
            return new OffsetCoordinateOddQ(col, row);
        }
        
        /// <summary>
        /// 创建偶数列偏移坐标
        /// </summary>
        /// <param name="col">列</param>
        /// <param name="row">行</param>
        /// <returns>偶数列偏移坐标</returns>
        public OffsetCoordinateEvenQ CreateOffsetEvenQ(int col, int row)
        {
            return new OffsetCoordinateEvenQ(col, row);
        }
        
        /// <summary>
        /// 创建双宽坐标
        /// </summary>
        /// <param name="col">列</param>
        /// <param name="row">行</param>
        /// <returns>双宽坐标</returns>
        public DoubledCoordinate CreateDoubled(int col, int row)
        {
            return new DoubledCoordinate(col, row);
        }
        
        #endregion
        
        #region 对象池管理
        
        /// <summary>
        /// 归还轴向坐标列表到对象池
        /// </summary>
        /// <param name="list">列表</param>
        public void ReturnAxialList(List<AxialCoordinate> list)
        {
            if (enableObjectPool)
            {
                HexObjectPool.ReturnAxialList(list);
            }
        }
        
        /// <summary>
        /// 归还立方坐标列表到对象池
        /// </summary>
        /// <param name="list">列表</param>
        public void ReturnCubeList(List<CubeCoordinate> list)
        {
            if (enableObjectPool)
            {
                HexObjectPool.ReturnCubeList(list);
            }
        }
        
        /// <summary>
        /// 归还偏移坐标列表到对象池
        /// </summary>
        /// <param name="list">列表</param>
        public void ReturnOffsetList(List<OffsetCoordinateOddQ> list)
        {
            if (enableObjectPool)
            {
                HexObjectPool.ReturnOffsetList(list);
            }
        }
        
        #endregion
        
        #region 调试和可视化
        
        /// <summary>
        /// 绘制六边形网格
        /// </summary>
        private void DrawHexGrid()
        {
            var center = WorldToAxial(transform.position);
            HexDebugger.DrawHexGrid(center, gridRadius, hexSize, gridColor);
            
            if (showCoordinateLabels)
            {
                var coordinates = GetCoordinatesInRange(center, gridRadius);
                foreach (var coord in coordinates)
                {
                    HexDebugger.DrawCoordinateLabel(coord, hexSize, true, false);
                }
                
                if (enableObjectPool)
                {
                    ReturnAxialList(coordinates);
                }
            }
        }
        
        /// <summary>
        /// 输出性能统计信息
        /// </summary>
        public void LogPerformanceStats()
        {
            if (enableDebugMode)
            {
                Debug.Log("=== 六边形坐标系统性能统计 ===");
                
                if (enableCaching && _cache != null)
                {
                    Debug.Log(_cache.GetCacheStatus());
                    Debug.Log($"缓存使用率: {_cache.GetCacheUsage():P2}");
                }
                
                if (enableObjectPool)
                {
                    Debug.Log(HexObjectPool.GetPoolStatus());
                }
            }
        }
        
        /// <summary>
        /// 运行坐标系统验证测试
        /// </summary>
        public void RunValidationTests()
        {
            if (enableDebugMode)
            {
                Debug.Log("=== 开始坐标系统验证测试 ===");
                HexDebugger.ValidateConversions(100);
                HexDebugger.ValidateDistances(100);
                Debug.Log("=== 验证测试完成 ===");
            }
        }
        
        /// <summary>
        /// 运行性能基准测试
        /// </summary>
        public void RunBenchmarkTests()
        {
            if (enableDebugMode)
            {
                Debug.Log("=== 开始性能基准测试 ===");
                HexDebugger.BenchmarkConversions(10000);
                if (enableObjectPool)
                {
                    HexDebugger.BenchmarkObjectPool(1000);
                }
                Debug.Log("=== 基准测试完成 ===");
            }
        }
        
        #endregion
        
        #region 配置管理
        
        /// <summary>
        /// 重置缓存
        /// </summary>
        public void ResetCache()
        {
            _cache?.ClearCache();
            
            if (enableDebugMode)
            {
                Debug.Log("坐标缓存已重置");
            }
        }
        
        /// <summary>
        /// 重置对象池
        /// </summary>
        public void ResetObjectPool()
        {
            if (enableObjectPool)
            {
                HexObjectPool.ClearAllPools();
                
                if (enableDebugMode)
                {
                    Debug.Log("对象池已重置");
                }
            }
        }
        
        /// <summary>
        /// 应用新的配置
        /// </summary>
        public void ApplyConfiguration()
        {
            if (enableCaching && _cache == null)
            {
                _cache = new HexCoordinateCache
                {
                    MaxCacheSize = maxCacheSize
                };
            }
            else if (!enableCaching && _cache != null)
            {
                _cache.ClearCache();
                _cache = null;
            }
            
            if (_cache != null)
            {
                _cache.MaxCacheSize = maxCacheSize;
            }
            
            if (enableDebugMode)
            {
                Debug.Log("配置已应用");
                LogPerformanceStats();
            }
        }
        
        #endregion
    }
}