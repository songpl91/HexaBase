using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoordinateSystem.Core;
using GridCoordinateSystem.Coordinates;

namespace GridCoordinateSystem.Utils
{
    /// <summary>
    /// 四边形网格坐标转换工具类
    /// 提供各种坐标系统之间的转换方法和实用工具
    /// </summary>
    public static class GridConverter
    {
        #region 坐标转换
        
        /// <summary>
        /// 世界坐标转直角坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <param name="alignment">对齐方式</param>
        /// <returns>直角坐标</returns>
        public static CartesianCoordinate WorldToCartesian(Vector3 worldPosition, float cellSize = 1.0f, 
            GridConstants.GridAlignment alignment = GridConstants.GridAlignment.Center)
        {
            return CartesianCoordinate.FromWorldPosition(worldPosition, cellSize, alignment);
        }
        
        /// <summary>
        /// 直角坐标转世界坐标
        /// </summary>
        /// <param name="cartesian">直角坐标</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <returns>世界坐标</returns>
        public static Vector3 CartesianToWorld(CartesianCoordinate cartesian, float cellSize = 1.0f)
        {
            return cartesian.ToWorldPosition(cellSize);
        }
        
        /// <summary>
        /// 直角坐标转索引坐标
        /// </summary>
        /// <param name="cartesian">直角坐标</param>
        /// <param name="width">网格宽度</param>
        /// <returns>索引坐标</returns>
        public static IndexCoordinate CartesianToIndex(CartesianCoordinate cartesian, int width)
        {
            return cartesian.ToIndex(width);
        }
        
        /// <summary>
        /// 索引坐标转直角坐标
        /// </summary>
        /// <param name="index">索引坐标</param>
        /// <param name="width">网格宽度</param>
        /// <returns>直角坐标</returns>
        public static CartesianCoordinate IndexToCartesian(IndexCoordinate index, int width)
        {
            return index.ToCartesian(width);
        }
        
        /// <summary>
        /// 世界坐标转索引坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="width">网格宽度</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <param name="alignment">对齐方式</param>
        /// <returns>索引坐标</returns>
        public static IndexCoordinate WorldToIndex(Vector3 worldPosition, int width, float cellSize = 1.0f, 
            GridConstants.GridAlignment alignment = GridConstants.GridAlignment.Center)
        {
            var cartesian = WorldToCartesian(worldPosition, cellSize, alignment);
            return CartesianToIndex(cartesian, width);
        }
        
        /// <summary>
        /// 索引坐标转世界坐标
        /// </summary>
        /// <param name="index">索引坐标</param>
        /// <param name="width">网格宽度</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <returns>世界坐标</returns>
        public static Vector3 IndexToWorld(IndexCoordinate index, int width, float cellSize = 1.0f)
        {
            var cartesian = IndexToCartesian(index, width);
            return CartesianToWorld(cartesian, cellSize);
        }
        
        #endregion
        
        #region 批量转换
        
        /// <summary>
        /// 批量世界坐标转直角坐标
        /// </summary>
        /// <param name="worldPositions">世界坐标列表</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <param name="alignment">对齐方式</param>
        /// <returns>直角坐标列表</returns>
        public static List<CartesianCoordinate> BatchWorldToCartesian(IEnumerable<Vector3> worldPositions, 
            float cellSize = 1.0f, GridConstants.GridAlignment alignment = GridConstants.GridAlignment.Center)
        {
            var result = new List<CartesianCoordinate>();
            foreach (var worldPos in worldPositions)
            {
                result.Add(WorldToCartesian(worldPos, cellSize, alignment));
            }
            return result;
        }
        
        /// <summary>
        /// 批量直角坐标转世界坐标
        /// </summary>
        /// <param name="cartesianCoords">直角坐标列表</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <returns>世界坐标列表</returns>
        public static List<Vector3> BatchCartesianToWorld(IEnumerable<CartesianCoordinate> cartesianCoords, 
            float cellSize = 1.0f)
        {
            var result = new List<Vector3>();
            foreach (var coord in cartesianCoords)
            {
                result.Add(CartesianToWorld(coord, cellSize));
            }
            return result;
        }
        
        /// <summary>
        /// 批量直角坐标转索引坐标
        /// </summary>
        /// <param name="cartesianCoords">直角坐标列表</param>
        /// <param name="width">网格宽度</param>
        /// <returns>索引坐标列表</returns>
        public static List<IndexCoordinate> BatchCartesianToIndex(IEnumerable<CartesianCoordinate> cartesianCoords, 
            int width)
        {
            var result = new List<IndexCoordinate>();
            foreach (var coord in cartesianCoords)
            {
                result.Add(CartesianToIndex(coord, width));
            }
            return result;
        }
        
        /// <summary>
        /// 批量索引坐标转直角坐标
        /// </summary>
        /// <param name="indexCoords">索引坐标列表</param>
        /// <param name="width">网格宽度</param>
        /// <returns>直角坐标列表</returns>
        public static List<CartesianCoordinate> BatchIndexToCartesian(IEnumerable<IndexCoordinate> indexCoords, 
            int width)
        {
            var result = new List<CartesianCoordinate>();
            foreach (var coord in indexCoords)
            {
                result.Add(IndexToCartesian(coord, width));
            }
            return result;
        }
        
        #endregion
        
        #region 距离计算
        
        /// <summary>
        /// 计算两个坐标之间的距离
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <param name="distanceType">距离类型</param>
        /// <returns>距离值</returns>
        public static float CalculateDistance(IGridCoordinate from, IGridCoordinate to, 
            GridConstants.DistanceType distanceType)
        {
            switch (distanceType)
            {
                case GridConstants.DistanceType.Manhattan:
                    return from.ManhattanDistanceTo(to);
                case GridConstants.DistanceType.Euclidean:
                    return from.EuclideanDistanceTo(to);
                case GridConstants.DistanceType.Chebyshev:
                    return from.ChebyshevDistanceTo(to);
                default:
                    return from.ManhattanDistanceTo(to);
            }
        }
        
        /// <summary>
        /// 计算两个世界坐标之间的网格距离
        /// </summary>
        /// <param name="worldPosA">世界坐标A</param>
        /// <param name="worldPosB">世界坐标B</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <param name="distanceType">距离类型</param>
        /// <param name="alignment">对齐方式</param>
        /// <returns>网格距离</returns>
        public static float CalculateWorldDistance(Vector3 worldPosA, Vector3 worldPosB, float cellSize = 1.0f,
            GridConstants.DistanceType distanceType = GridConstants.DistanceType.Manhattan,
            GridConstants.GridAlignment alignment = GridConstants.GridAlignment.Center)
        {
            var coordA = WorldToCartesian(worldPosA, cellSize, alignment);
            var coordB = WorldToCartesian(worldPosB, cellSize, alignment);
            return CalculateDistance(coordA, coordB, distanceType);
        }
        
        #endregion
        
        #region 路径计算
        
        /// <summary>
        /// 计算两点之间的直线路径（布雷森汉姆算法）
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <returns>路径坐标列表</returns>
        public static List<CartesianCoordinate> GetLinePath(CartesianCoordinate from, CartesianCoordinate to)
        {
            var path = new List<CartesianCoordinate>();
            
            int x0 = from.X, y0 = from.Y;
            int x1 = to.X, y1 = to.Y;
            
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            
            while (true)
            {
                path.Add(new CartesianCoordinate(x0, y0));
                
                if (x0 == x1 && y0 == y1) break;
                
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
            
            return path;
        }
        
        /// <summary>
        /// 计算两点之间的直线路径（索引坐标版本）
        /// </summary>
        /// <param name="from">起始索引坐标</param>
        /// <param name="to">目标索引坐标</param>
        /// <param name="width">网格宽度</param>
        /// <returns>路径索引坐标列表</returns>
        public static List<IndexCoordinate> GetLinePath(IndexCoordinate from, IndexCoordinate to, int width)
        {
            var fromCartesian = IndexToCartesian(from, width);
            var toCartesian = IndexToCartesian(to, width);
            var cartesianPath = GetLinePath(fromCartesian, toCartesian);
            
            var indexPath = new List<IndexCoordinate>();
            foreach (var coord in cartesianPath)
            {
                indexPath.Add(CartesianToIndex(coord, width));
            }
            
            return indexPath;
        }
        
        #endregion
        
        #region 范围查询
        
        /// <summary>
        /// 获取矩形范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        /// <returns>范围内坐标列表</returns>
        public static List<CartesianCoordinate> GetCoordinatesInRectangle(CartesianCoordinate center, 
            int width, int height)
        {
            var coordinates = new List<CartesianCoordinate>();
            
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            
            for (int x = center.X - halfWidth; x <= center.X + halfWidth; x++)
            {
                for (int y = center.Y - halfHeight; y <= center.Y + halfHeight; y++)
                {
                    coordinates.Add(new CartesianCoordinate(x, y));
                }
            }
            
            return coordinates;
        }
        
        /// <summary>
        /// 获取圆形范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <param name="distanceType">距离计算类型</param>
        /// <returns>范围内坐标列表</returns>
        public static List<CartesianCoordinate> GetCoordinatesInCircle(CartesianCoordinate center, 
            int radius, GridConstants.DistanceType distanceType = GridConstants.DistanceType.Euclidean)
        {
            var coordinates = new List<CartesianCoordinate>();
            
            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                for (int y = center.Y - radius; y <= center.Y + radius; y++)
                {
                    var coord = new CartesianCoordinate(x, y);
                    float distance = CalculateDistance(center, coord, distanceType);
                    
                    if (distance <= radius)
                    {
                        coordinates.Add(coord);
                    }
                }
            }
            
            return coordinates;
        }
        
        /// <summary>
        /// 获取指定距离的所有坐标（环形）
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="distance">距离</param>
        /// <param name="distanceType">距离计算类型</param>
        /// <returns>指定距离的坐标列表</returns>
        public static List<CartesianCoordinate> GetCoordinatesAtDistance(CartesianCoordinate center, 
            int distance, GridConstants.DistanceType distanceType = GridConstants.DistanceType.Manhattan)
        {
            var coordinates = new List<CartesianCoordinate>();
            
            for (int x = center.X - distance; x <= center.X + distance; x++)
            {
                for (int y = center.Y - distance; y <= center.Y + distance; y++)
                {
                    var coord = new CartesianCoordinate(x, y);
                    float coordDistance = CalculateDistance(center, coord, distanceType);
                    
                    if (Mathf.Approximately(coordDistance, distance))
                    {
                        coordinates.Add(coord);
                    }
                }
            }
            
            return coordinates;
        }
        
        #endregion
        
        #region 边界检查
        
        /// <summary>
        /// 检查坐标是否在网格边界内
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="gridWidth">网格宽度</param>
        /// <param name="gridHeight">网格高度</param>
        /// <returns>是否在边界内</returns>
        public static bool IsWithinGridBounds(CartesianCoordinate coord, int gridWidth, int gridHeight)
        {
            return coord.IsWithinBounds(0, 0, gridWidth - 1, gridHeight - 1);
        }
        
        /// <summary>
        /// 检查索引坐标是否在网格边界内
        /// </summary>
        /// <param name="index">索引坐标</param>
        /// <param name="gridWidth">网格宽度</param>
        /// <param name="gridHeight">网格高度</param>
        /// <returns>是否在边界内</returns>
        public static bool IsWithinGridBounds(IndexCoordinate index, int gridWidth, int gridHeight)
        {
            return index.IsWithinBounds(gridWidth, gridHeight);
        }
        
        /// <summary>
        /// 将坐标限制在网格边界内
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="gridWidth">网格宽度</param>
        /// <param name="gridHeight">网格高度</param>
        /// <returns>限制后的坐标</returns>
        public static CartesianCoordinate ClampToGridBounds(CartesianCoordinate coord, int gridWidth, int gridHeight)
        {
            int clampedX = Mathf.Clamp(coord.X, 0, gridWidth - 1);
            int clampedY = Mathf.Clamp(coord.Y, 0, gridHeight - 1);
            return new CartesianCoordinate(clampedX, clampedY);
        }
        
        #endregion
        
        #region 实用工具
        
        /// <summary>
        /// 获取两个坐标之间的中点
        /// </summary>
        /// <param name="a">坐标A</param>
        /// <param name="b">坐标B</param>
        /// <returns>中点坐标</returns>
        public static CartesianCoordinate GetMidpoint(CartesianCoordinate a, CartesianCoordinate b)
        {
            int midX = (a.X + b.X) / 2;
            int midY = (a.Y + b.Y) / 2;
            return new CartesianCoordinate(midX, midY);
        }
        
        /// <summary>
        /// 获取坐标的方向向量
        /// </summary>
        /// <param name="from">起始坐标</param>
        /// <param name="to">目标坐标</param>
        /// <returns>方向向量</returns>
        public static Vector2Int GetDirection(CartesianCoordinate from, CartesianCoordinate to)
        {
            return new Vector2Int(
                Mathf.Clamp(to.X - from.X, -1, 1),
                Mathf.Clamp(to.Y - from.Y, -1, 1)
            );
        }
        
        /// <summary>
        /// 旋转坐标（以原点为中心）
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="rotations">旋转次数（90度为单位）</param>
        /// <returns>旋转后的坐标</returns>
        public static CartesianCoordinate RotateCoordinate(CartesianCoordinate coord, int rotations)
        {
            rotations = rotations % 4;
            if (rotations < 0) rotations += 4;
            
            int x = coord.X, y = coord.Y;
            
            for (int i = 0; i < rotations; i++)
            {
                int temp = x;
                x = -y;
                y = temp;
            }
            
            return new CartesianCoordinate(x, y);
        }
        
        /// <summary>
        /// 镜像坐标（水平镜像）
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="centerX">镜像中心X坐标</param>
        /// <returns>镜像后的坐标</returns>
        public static CartesianCoordinate MirrorHorizontal(CartesianCoordinate coord, int centerX = 0)
        {
            return new CartesianCoordinate(2 * centerX - coord.X, coord.Y);
        }
        
        /// <summary>
        /// 镜像坐标（垂直镜像）
        /// </summary>
        /// <param name="coord">坐标</param>
        /// <param name="centerY">镜像中心Y坐标</param>
        /// <returns>镜像后的坐标</returns>
        public static CartesianCoordinate MirrorVertical(CartesianCoordinate coord, int centerY = 0)
        {
            return new CartesianCoordinate(coord.X, 2 * centerY - coord.Y);
        }
        
        #endregion
    }
}