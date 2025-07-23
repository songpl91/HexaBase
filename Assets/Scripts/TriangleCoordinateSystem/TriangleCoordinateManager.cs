using System;
using System.Collections.Generic;
using UnityEngine;
using TriangleCoordinateSystem.Core;
using TriangleCoordinateSystem.Coordinates;
using TriangleCoordinateSystem.Utils;

namespace TriangleCoordinateSystem
{
    /// <summary>
    /// 三角形坐标系统管理器
    /// 提供统一的接口来管理和使用各种三角形坐标系统
    /// </summary>
    public class TriangleCoordinateManager : MonoBehaviour
    {
        [Header("三角形网格设置")]
        [SerializeField] private float triangleSize = 1.0f;
        [SerializeField] private Vector3 gridOrigin = Vector3.zero;
        
        [Header("可视化设置")]
        [SerializeField] private bool showGrid = true;
        [SerializeField] private bool showLabels = true;
        [SerializeField] private bool showTriangleFill = false;
        [SerializeField] private int visualRadius = 5;
        [SerializeField] private Color gridColor = Color.gray;
        [SerializeField] private Color upwardTriangleColor = Color.blue;
        [SerializeField] private Color downwardTriangleColor = Color.red;
        
        [Header("性能设置")]
        [SerializeField] private bool enableCache = true;
        [SerializeField] private bool enableObjectPool = true;
        
        [Header("调试设置")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private TriangleAxialCoordinate debugCenter = new TriangleAxialCoordinate(0, 0);
        
        // 事件
        public event Action<ITriangleCoordinate> OnCoordinateClicked;
        public event Action<ITriangleCoordinate> OnCoordinateHovered;
        
        /// <summary>
        /// 三角形大小
        /// </summary>
        public float TriangleSize
        {
            get => triangleSize;
            set => triangleSize = Mathf.Max(0.1f, value);
        }
        
        /// <summary>
        /// 网格原点
        /// </summary>
        public Vector3 GridOrigin
        {
            get => gridOrigin;
            set => gridOrigin = value;
        }
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool CacheEnabled
        {
            get => enableCache;
            set
            {
                enableCache = value;
                TriangleOptimization.CacheEnabled = value;
            }
        }
        
        private void Awake()
        {
            // 初始化设置
            TriangleOptimization.CacheEnabled = enableCache;
            TriangleDebugger.DebugEnabled = debugMode;
            TriangleDebugger.ShowGrid = showGrid;
            TriangleDebugger.ShowLabels = showLabels;
            TriangleDebugger.ShowTriangleFill = showTriangleFill;
            TriangleDebugger.GridColor = gridColor;
            TriangleDebugger.UpwardTriangleColor = upwardTriangleColor;
            TriangleDebugger.DownwardTriangleColor = downwardTriangleColor;
        }
        
        private void Update()
        {
            // 处理鼠标输入
            HandleMouseInput();
        }
        
        private void OnDrawGizmos()
        {
            if (showGrid && Application.isPlaying)
            {
                // 绘制网格
                TriangleDebugger.DrawTriangleGrid(debugCenter, visualRadius, triangleSize);
            }
        }
        
        #region 坐标创建方法
        
        /// <summary>
        /// 创建轴向坐标
        /// </summary>
        /// <param name="q">Q坐标</param>
        /// <param name="r">R坐标</param>
        /// <returns>轴向坐标</returns>
        public TriangleAxialCoordinate CreateAxial(int q, int r)
        {
            return new TriangleAxialCoordinate(q, r);
        }
        
        /// <summary>
        /// 创建立方坐标
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="z">Z坐标</param>
        /// <returns>立方坐标</returns>
        public TriangleCubeCoordinate CreateCube(int x, int y, int z)
        {
            return new TriangleCubeCoordinate(x, y, z);
        }
        
        /// <summary>
        /// 创建偏移坐标
        /// </summary>
        /// <param name="col">列坐标</param>
        /// <param name="row">行坐标</param>
        /// <returns>偏移坐标</returns>
        public TriangleOffsetCoordinate CreateOffset(int col, int row)
        {
            return new TriangleOffsetCoordinate(col, row);
        }
        
        #endregion
        
        #region 坐标转换方法
        
        /// <summary>
        /// 世界坐标转换为轴向坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>轴向坐标</returns>
        public TriangleAxialCoordinate WorldToAxial(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - gridOrigin;
            return TriangleConverter.WorldToAxial(localPosition, triangleSize);
        }
        
        /// <summary>
        /// 世界坐标转换为立方坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>立方坐标</returns>
        public TriangleCubeCoordinate WorldToCube(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - gridOrigin;
            return TriangleConverter.WorldToCube(localPosition, triangleSize);
        }
        
        /// <summary>
        /// 世界坐标转换为偏移坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <returns>偏移坐标</returns>
        public TriangleOffsetCoordinate WorldToOffset(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - gridOrigin;
            return TriangleConverter.WorldToOffset(localPosition, triangleSize);
        }
        
        /// <summary>
        /// 坐标转换为世界坐标
        /// </summary>
        /// <param name="coordinate">三角形坐标</param>
        /// <returns>世界坐标</returns>
        public Vector3 CoordinateToWorld(ITriangleCoordinate coordinate)
        {
            if (enableCache)
            {
                return gridOrigin + TriangleOptimization.GetWorldPositionCached(coordinate, triangleSize);
            }
            else
            {
                return gridOrigin + coordinate.ToWorldPosition(triangleSize);
            }
        }
        
        #endregion
        
        #region 几何计算方法
        
        /// <summary>
        /// 计算两个坐标之间的距离
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <returns>距离值</returns>
        public int GetDistance(ITriangleCoordinate from, ITriangleCoordinate to)
        {
            if (enableCache)
            {
                return TriangleOptimization.GetDistanceCached(from, to);
            }
            else
            {
                return from.DistanceTo(to);
            }
        }
        
        /// <summary>
        /// 获取坐标的邻居
        /// </summary>
        /// <param name="coordinate">中心坐标</param>
        /// <returns>邻居坐标列表</returns>
        public List<ITriangleCoordinate> GetNeighbors(ITriangleCoordinate coordinate)
        {
            if (enableCache)
            {
                return TriangleOptimization.GetNeighborsCached(coordinate);
            }
            else
            {
                return coordinate.GetNeighbors();
            }
        }
        
        /// <summary>
        /// 获取坐标的顶点邻居
        /// </summary>
        /// <param name="coordinate">中心坐标</param>
        /// <returns>顶点邻居坐标列表</returns>
        public List<ITriangleCoordinate> GetVertexNeighbors(ITriangleCoordinate coordinate)
        {
            return coordinate.GetVertexNeighbors();
        }
        
        /// <summary>
        /// 获取两点之间的路径
        /// </summary>
        /// <param name="start">起始坐标</param>
        /// <param name="end">结束坐标</param>
        /// <returns>路径坐标列表</returns>
        public List<TriangleAxialCoordinate> GetPath(TriangleAxialCoordinate start, TriangleAxialCoordinate end)
        {
            return TriangleConverter.GetLinePath(start, end);
        }
        
        /// <summary>
        /// 获取指定范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>范围内的坐标列表</returns>
        public List<TriangleAxialCoordinate> GetCoordinatesInRange(TriangleAxialCoordinate center, int radius)
        {
            return TriangleConverter.GetCoordinatesInRange(center, radius);
        }
        
        /// <summary>
        /// 获取指定距离的环形坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>环形坐标列表</returns>
        public List<TriangleAxialCoordinate> GetRing(TriangleAxialCoordinate center, int radius)
        {
            return TriangleConverter.GetRing(center, radius);
        }
        
        #endregion
        
        #region 可视化方法
        
        /// <summary>
        /// 绘制三角形
        /// </summary>
        /// <param name="coordinate">三角形坐标</param>
        public void DrawTriangle(ITriangleCoordinate coordinate)
        {
            TriangleDebugger.DrawTriangle(coordinate, triangleSize);
        }
        
        /// <summary>
        /// 绘制路径
        /// </summary>
        /// <param name="path">路径坐标列表</param>
        /// <param name="pathColor">路径颜色</param>
        public void DrawPath(List<TriangleAxialCoordinate> path, Color? pathColor = null)
        {
            TriangleDebugger.DrawPath(path, triangleSize, pathColor);
        }
        
        /// <summary>
        /// 绘制范围
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <param name="rangeColor">范围颜色</param>
        public void DrawRange(TriangleAxialCoordinate center, int radius, Color? rangeColor = null)
        {
            TriangleDebugger.DrawRange(center, radius, triangleSize, rangeColor);
        }
        
        /// <summary>
        /// 绘制邻居连接
        /// </summary>
        /// <param name="coordinate">中心坐标</param>
        /// <param name="neighborColor">邻居连接颜色</param>
        public void DrawNeighborConnections(ITriangleCoordinate coordinate, Color? neighborColor = null)
        {
            TriangleDebugger.DrawNeighborConnections(coordinate, triangleSize, neighborColor);
        }
        
        #endregion
        
        #region 性能和调试方法
        
        /// <summary>
        /// 清理所有缓存
        /// </summary>
        public void ClearCaches()
        {
            TriangleOptimization.ClearAllCaches();
        }
        
        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存统计信息</returns>
        public string GetCacheStats()
        {
            return TriangleOptimization.GetCacheStats();
        }
        
        /// <summary>
        /// 预热缓存
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">预热半径</param>
        public void WarmupCache(TriangleAxialCoordinate center, int radius)
        {
            TriangleOptimization.WarmupCache(center, radius, triangleSize);
        }
        
        /// <summary>
        /// 验证坐标
        /// </summary>
        /// <param name="coordinate">要验证的坐标</param>
        /// <returns>验证结果</returns>
        public bool ValidateCoordinate(ITriangleCoordinate coordinate)
        {
            return TriangleDebugger.ValidateCoordinate(coordinate, triangleSize);
        }
        
        /// <summary>
        /// 输出坐标信息
        /// </summary>
        /// <param name="coordinate">坐标</param>
        public void LogCoordinateInfo(ITriangleCoordinate coordinate)
        {
            TriangleDebugger.LogCoordinateInfo(coordinate);
        }
        
        #endregion
        
        #region 鼠标交互
        
        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        private void HandleMouseInput()
        {
            if (Camera.main == null) return;
            
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            
            var hoveredCoordinate = WorldToAxial(mouseWorldPos);
            OnCoordinateHovered?.Invoke(hoveredCoordinate);
            
            if (Input.GetMouseButtonDown(0))
            {
                var clickedCoordinate = WorldToAxial(mouseWorldPos);
                OnCoordinateClicked?.Invoke(clickedCoordinate);
                
                if (debugMode)
                {
                    LogCoordinateInfo(clickedCoordinate);
                }
            }
        }
        
        #endregion
        
        #region Unity编辑器方法
        
        private void OnValidate()
        {
            // 确保三角形大小不为负数
            triangleSize = Mathf.Max(0.1f, triangleSize);
            
            // 更新调试器设置
            if (Application.isPlaying)
            {
                TriangleDebugger.ShowGrid = showGrid;
                TriangleDebugger.ShowLabels = showLabels;
                TriangleDebugger.ShowTriangleFill = showTriangleFill;
                TriangleDebugger.GridColor = gridColor;
                TriangleDebugger.UpwardTriangleColor = upwardTriangleColor;
                TriangleDebugger.DownwardTriangleColor = downwardTriangleColor;
                TriangleDebugger.DebugEnabled = debugMode;
                TriangleOptimization.CacheEnabled = enableCache;
            }
        }
        
        #endregion
    }
}