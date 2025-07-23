using System.Collections.Generic;
using UnityEngine;
using TriangleCoordinateSystem.Core;
using TriangleCoordinateSystem.Coordinates;

namespace TriangleCoordinateSystem.Utils
{
    /// <summary>
    /// 三角形坐标转换和计算工具类
    /// 提供各种坐标系统之间的转换和几何计算功能
    /// </summary>
    public static class TriangleConverter
    {
        /// <summary>
        /// 世界坐标转换为轴向坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标位置</param>
        /// <param name="triangleSize">三角形边长</param>
        /// <returns>轴向坐标</returns>
        public static TriangleAxialCoordinate WorldToAxial(Vector3 worldPosition, float triangleSize = 1.0f)
        {
            // 将世界坐标转换为网格坐标
            float x = worldPosition.x / triangleSize;
            float y = worldPosition.y / triangleSize;
            
            // 计算基础网格位置
            float q = x / TriangleConstants.COLUMN_SPACING_RATIO;
            float r = y / TriangleConstants.ROW_SPACING_RATIO;
            
            // 四舍五入到最近的整数坐标
            int roundedQ = Mathf.RoundToInt(q);
            int roundedR = Mathf.RoundToInt(r);
            
            // 检查是否需要调整坐标以匹配正确的三角形
            var candidate = new TriangleAxialCoordinate(roundedQ, roundedR);
            var candidateWorld = candidate.ToWorldPosition(triangleSize);
            
            // 计算到候选位置的距离
            float distance = Vector3.Distance(worldPosition, candidateWorld);
            
            // 检查相邻的三角形，找到最近的一个
            var neighbors = candidate.GetNeighbors();
            TriangleAxialCoordinate closest = candidate;
            float minDistance = distance;
            
            foreach (var neighbor in neighbors)
            {
                var neighborAxial = neighbor.ToAxial();
                var neighborWorld = neighborAxial.ToWorldPosition(triangleSize);
                float neighborDistance = Vector3.Distance(worldPosition, neighborWorld);
                
                if (neighborDistance < minDistance)
                {
                    minDistance = neighborDistance;
                    closest = neighborAxial;
                }
            }
            
            return closest;
        }
        
        /// <summary>
        /// 世界坐标转换为立方坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标位置</param>
        /// <param name="triangleSize">三角形边长</param>
        /// <returns>立方坐标</returns>
        public static TriangleCubeCoordinate WorldToCube(Vector3 worldPosition, float triangleSize = 1.0f)
        {
            return WorldToAxial(worldPosition, triangleSize).ToCube();
        }
        
        /// <summary>
        /// 世界坐标转换为偏移坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标位置</param>
        /// <param name="triangleSize">三角形边长</param>
        /// <returns>偏移坐标</returns>
        public static TriangleOffsetCoordinate WorldToOffset(Vector3 worldPosition, float triangleSize = 1.0f)
        {
            var axial = WorldToAxial(worldPosition, triangleSize);
            return AxialToOffset(axial);
        }
        
        /// <summary>
        /// 轴向坐标转换为偏移坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>偏移坐标</returns>
        public static TriangleOffsetCoordinate AxialToOffset(TriangleAxialCoordinate axial)
        {
            int col = axial.Q + (axial.R - (axial.R & 1)) / 2;
            int row = axial.R;
            return new TriangleOffsetCoordinate(col, row);
        }
        
        /// <summary>
        /// 偏移坐标转换为轴向坐标
        /// </summary>
        /// <param name="offset">偏移坐标</param>
        /// <returns>轴向坐标</returns>
        public static TriangleAxialCoordinate OffsetToAxial(TriangleOffsetCoordinate offset)
        {
            return offset.ToAxial();
        }
        
        /// <summary>
        /// 立方坐标转换为轴向坐标
        /// </summary>
        /// <param name="cube">立方坐标</param>
        /// <returns>轴向坐标</returns>
        public static TriangleAxialCoordinate CubeToAxial(TriangleCubeCoordinate cube)
        {
            return cube.ToAxial();
        }
        
        /// <summary>
        /// 轴向坐标转换为立方坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>立方坐标</returns>
        public static TriangleCubeCoordinate AxialToCube(TriangleAxialCoordinate axial)
        {
            return axial.ToCube();
        }
        
        /// <summary>
        /// 计算两点之间的直线路径
        /// </summary>
        /// <param name="start">起始坐标</param>
        /// <param name="end">结束坐标</param>
        /// <returns>路径上的坐标列表</returns>
        public static List<TriangleAxialCoordinate> GetLinePath(TriangleAxialCoordinate start, TriangleAxialCoordinate end)
        {
            var path = new List<TriangleAxialCoordinate>();
            int distance = start.DistanceTo(end);
            
            if (distance == 0)
            {
                path.Add(start);
                return path;
            }
            
            var startCube = start.ToCube();
            var endCube = end.ToCube();
            
            for (int i = 0; i <= distance; i++)
            {
                float t = (float)i / distance;
                
                // 线性插值
                float x = Mathf.Lerp(startCube.X, endCube.X, t);
                float y = Mathf.Lerp(startCube.Y, endCube.Y, t);
                float z = Mathf.Lerp(startCube.Z, endCube.Z, t);
                
                // 四舍五入到最近的有效立方坐标
                var roundedCube = RoundToCube(x, y, z);
                path.Add(roundedCube.ToAxial());
            }
            
            return path;
        }
        
        /// <summary>
        /// 获取指定范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>范围内的坐标列表</returns>
        public static List<TriangleAxialCoordinate> GetCoordinatesInRange(TriangleAxialCoordinate center, int radius)
        {
            var coordinates = new List<TriangleAxialCoordinate>();
            var centerCube = center.ToCube();
            
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(radius, -x + radius); y++)
                {
                    int z = -x - y;
                    var cube = new TriangleCubeCoordinate(centerCube.X + x, centerCube.Y + y, centerCube.Z + z);
                    
                    if (cube.IsValid())
                    {
                        coordinates.Add(cube.ToAxial());
                    }
                }
            }
            
            return coordinates;
        }
        
        /// <summary>
        /// 获取指定距离的环形坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>环形坐标列表</returns>
        public static List<TriangleAxialCoordinate> GetRing(TriangleAxialCoordinate center, int radius)
        {
            var ring = new List<TriangleAxialCoordinate>();
            
            if (radius == 0)
            {
                ring.Add(center);
                return ring;
            }
            
            var centerCube = center.ToCube();
            
            // 三角形网格的环形遍历
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    // 计算环上的坐标
                    int x = centerCube.X + radius * GetDirectionVector(i).X + j * GetDirectionVector((i + 2) % 6).X;
                    int y = centerCube.Y + radius * GetDirectionVector(i).Y + j * GetDirectionVector((i + 2) % 6).Y;
                    int z = centerCube.Z + radius * GetDirectionVector(i).Z + j * GetDirectionVector((i + 2) % 6).Z;
                    
                    var cube = new TriangleCubeCoordinate(x, y, z);
                    if (cube.IsValid())
                    {
                        ring.Add(cube.ToAxial());
                    }
                }
            }
            
            return ring;
        }
        
        /// <summary>
        /// 四舍五入到最近的有效立方坐标
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="z">Z坐标</param>
        /// <returns>有效的立方坐标</returns>
        private static TriangleCubeCoordinate RoundToCube(float x, float y, float z)
        {
            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);
            
            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);
            
            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }
            
            return new TriangleCubeCoordinate(rx, ry, rz);
        }
        
        /// <summary>
        /// 获取方向向量
        /// </summary>
        /// <param name="direction">方向索引</param>
        /// <returns>方向向量</returns>
        private static TriangleCubeCoordinate GetDirectionVector(int direction)
        {
            var directions = new TriangleCubeCoordinate[]
            {
                new TriangleCubeCoordinate(1, -1, 0),
                new TriangleCubeCoordinate(1, 0, -1),
                new TriangleCubeCoordinate(0, 1, -1),
                new TriangleCubeCoordinate(-1, 1, 0),
                new TriangleCubeCoordinate(-1, 0, 1),
                new TriangleCubeCoordinate(0, -1, 1)
            };
            
            return directions[direction % 6];
        }
    }
}