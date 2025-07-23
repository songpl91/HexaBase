using System;
using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;

namespace HexCoordinateSystem.Coordinates
{
    /// <summary>
    /// 立方坐标系统实现
    /// 使用三个坐标 (x, y, z) 表示六边形位置，满足约束条件 x + y + z = 0
    /// 优点：距离计算最简单，旋转操作直观，数学运算对称性好
    /// 适用场景：需要频繁计算距离的算法（如A*路径查找），需要旋转操作的游戏
    /// </summary>
    [Serializable]
    public struct CubeCoordinate : IHexCoordinate, IEquatable<CubeCoordinate>
    {
        #region 字段和属性
        
        /// <summary>
        /// X坐标
        /// </summary>
        public readonly int x;
        
        /// <summary>
        /// Y坐标
        /// </summary>
        public readonly int y;
        
        /// <summary>
        /// Z坐标
        /// </summary>
        public readonly int z;
        
        /// <summary>
        /// 零坐标常量
        /// </summary>
        public static readonly CubeCoordinate Zero = new CubeCoordinate(0, 0, 0);
        
        #endregion
        
        #region 构造函数
        
        /// <summary>
        /// 构造立方坐标
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="z">Z坐标</param>
        public CubeCoordinate(int x, int y, int z)
        {
            if (x + y + z != 0)
            {
                throw new ArgumentException($"立方坐标必须满足 x + y + z = 0，当前值：{x} + {y} + {z} = {x + y + z}");
            }
            
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        /// <summary>
        /// 从Vector3Int构造立方坐标
        /// </summary>
        /// <param name="vector">包含x、y、z的向量</param>
        public CubeCoordinate(Vector3Int vector) : this(vector.x, vector.y, vector.z)
        {
        }
        
        /// <summary>
        /// 从轴向坐标构造立方坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        public CubeCoordinate(AxialCoordinate axial) : this(axial.q, axial.r, -axial.q - axial.r)
        {
        }
        
        #endregion
        
        #region IHexCoordinate接口实现
        
        /// <summary>
        /// 转换为世界坐标位置
        /// </summary>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>世界坐标位置</returns>
        public Vector3 ToWorldPosition(float hexSize = 1.0f)
        {
            return ToAxial().ToWorldPosition(hexSize);
        }
        
        /// <summary>
        /// 获取所有相邻的六边形坐标
        /// </summary>
        /// <returns>相邻坐标列表</returns>
        public List<IHexCoordinate> GetNeighbors()
        {
            var neighbors = new List<IHexCoordinate>(6);
            
            for (int i = 0; i < HexConstants.CUBE_DIRECTIONS.Length; i++)
            {
                var direction = HexConstants.CUBE_DIRECTIONS[i];
                neighbors.Add(new CubeCoordinate(x + direction.x, y + direction.y, z + direction.z));
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 获取指定方向的邻居坐标
        /// </summary>
        /// <param name="direction">方向索引（0-5）</param>
        /// <returns>邻居坐标</returns>
        public CubeCoordinate GetNeighbor(int direction)
        {
            if (direction < 0 || direction >= HexConstants.CUBE_DIRECTIONS.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), "方向索引必须在0-5之间");
            }
            
            var dir = HexConstants.CUBE_DIRECTIONS[direction];
            return new CubeCoordinate(x + dir.x, y + dir.y, z + dir.z);
        }
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        public int DistanceTo(IHexCoordinate other)
        {
            var otherCube = other.ToCube();
            return Distance(this, otherCube);
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        /// <returns>轴向坐标</returns>
        public AxialCoordinate ToAxial()
        {
            return new AxialCoordinate(x, z);
        }
        
        /// <summary>
        /// 转换为立方坐标（返回自身）
        /// </summary>
        /// <returns>立方坐标</returns>
        public CubeCoordinate ToCube()
        {
            return this;
        }
        
        /// <summary>
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            return x + y + z == 0 &&
                   x >= HexConstants.MIN_COORDINATE_VALUE && 
                   x <= HexConstants.MAX_COORDINATE_VALUE &&
                   y >= HexConstants.MIN_COORDINATE_VALUE && 
                   y <= HexConstants.MAX_COORDINATE_VALUE &&
                   z >= HexConstants.MIN_COORDINATE_VALUE && 
                   z <= HexConstants.MAX_COORDINATE_VALUE;
        }
        
        #endregion
        
        #region 静态方法
        
        /// <summary>
        /// 计算两个立方坐标之间的距离（曼哈顿距离除以2）
        /// </summary>
        /// <param name="a">坐标A</param>
        /// <param name="b">坐标B</param>
        /// <returns>距离值</returns>
        public static int Distance(CubeCoordinate a, CubeCoordinate b)
        {
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
        }
        
        /// <summary>
        /// 将浮点坐标四舍五入为整数坐标
        /// </summary>
        /// <param name="x">浮点X坐标</param>
        /// <param name="y">浮点Y坐标</param>
        /// <param name="z">浮点Z坐标</param>
        /// <returns>四舍五入后的立方坐标</returns>
        public static CubeCoordinate Round(float x, float y, float z)
        {
            int roundedX = Mathf.RoundToInt(x);
            int roundedY = Mathf.RoundToInt(y);
            int roundedZ = Mathf.RoundToInt(z);
            
            float xDiff = Mathf.Abs(roundedX - x);
            float yDiff = Mathf.Abs(roundedY - y);
            float zDiff = Mathf.Abs(roundedZ - z);
            
            if (xDiff > yDiff && xDiff > zDiff)
            {
                roundedX = -roundedY - roundedZ;
            }
            else if (yDiff > zDiff)
            {
                roundedY = -roundedX - roundedZ;
            }
            else
            {
                roundedZ = -roundedX - roundedY;
            }
            
            return new CubeCoordinate(roundedX, roundedY, roundedZ);
        }
        
        /// <summary>
        /// 获取从起点到终点的直线路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>路径上的坐标列表</returns>
        public static List<CubeCoordinate> GetLinePath(CubeCoordinate start, CubeCoordinate end)
        {
            int distance = Distance(start, end);
            var path = new List<CubeCoordinate>(distance + 1);
            
            for (int i = 0; i <= distance; i++)
            {
                float t = distance == 0 ? 0.0f : (float)i / distance;
                float x = start.x + (end.x - start.x) * t;
                float y = start.y + (end.y - start.y) * t;
                float z = start.z + (end.z - start.z) * t;
                path.Add(Round(x, y, z));
            }
            
            return path;
        }
        
        /// <summary>
        /// 获取指定半径范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>范围内的坐标列表</returns>
        public static List<CubeCoordinate> GetCoordinatesInRange(CubeCoordinate center, int radius)
        {
            var coordinates = new List<CubeCoordinate>();
            
            for (int x = -radius; x <= radius; x++)
            {
                int y1 = Mathf.Max(-radius, -x - radius);
                int y2 = Mathf.Min(radius, -x + radius);
                
                for (int y = y1; y <= y2; y++)
                {
                    int z = -x - y;
                    coordinates.Add(new CubeCoordinate(center.x + x, center.y + y, center.z + z));
                }
            }
            
            return coordinates;
        }
        
        /// <summary>
        /// 旋转坐标（顺时针60度的倍数）
        /// </summary>
        /// <param name="coord">要旋转的坐标</param>
        /// <param name="steps">旋转步数（1步 = 60度）</param>
        /// <returns>旋转后的坐标</returns>
        public static CubeCoordinate Rotate(CubeCoordinate coord, int steps)
        {
            // 确保步数在0-5范围内
            steps = ((steps % 6) + 6) % 6;
            
            var current = coord;
            for (int i = 0; i < steps; i++)
            {
                current = new CubeCoordinate(-current.z, -current.x, -current.y);
            }
            
            return current;
        }
        
        /// <summary>
        /// 围绕中心点旋转坐标
        /// </summary>
        /// <param name="coord">要旋转的坐标</param>
        /// <param name="center">旋转中心</param>
        /// <param name="steps">旋转步数（1步 = 60度）</param>
        /// <returns>旋转后的坐标</returns>
        public static CubeCoordinate RotateAround(CubeCoordinate coord, CubeCoordinate center, int steps)
        {
            var relative = coord - center;
            var rotated = Rotate(relative, steps);
            return rotated + center;
        }
        
        /// <summary>
        /// 反射坐标（沿指定轴）
        /// </summary>
        /// <param name="coord">要反射的坐标</param>
        /// <param name="axis">反射轴（0-2，分别对应x、y、z轴）</param>
        /// <returns>反射后的坐标</returns>
        public static CubeCoordinate Reflect(CubeCoordinate coord, int axis)
        {
            switch (axis)
            {
                case 0: // 沿x轴反射
                    return new CubeCoordinate(coord.x, coord.z, coord.y);
                case 1: // 沿y轴反射
                    return new CubeCoordinate(coord.z, coord.y, coord.x);
                case 2: // 沿z轴反射
                    return new CubeCoordinate(coord.y, coord.x, coord.z);
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), "轴索引必须在0-2之间");
            }
        }
        
        #endregion
        
        #region 运算符重载
        
        /// <summary>
        /// 加法运算符
        /// </summary>
        public static CubeCoordinate operator +(CubeCoordinate a, CubeCoordinate b)
        {
            return new CubeCoordinate(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        
        /// <summary>
        /// 减法运算符
        /// </summary>
        public static CubeCoordinate operator -(CubeCoordinate a, CubeCoordinate b)
        {
            return new CubeCoordinate(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        
        /// <summary>
        /// 标量乘法运算符
        /// </summary>
        public static CubeCoordinate operator *(CubeCoordinate a, int scalar)
        {
            return new CubeCoordinate(a.x * scalar, a.y * scalar, a.z * scalar);
        }
        
        /// <summary>
        /// 取负运算符
        /// </summary>
        public static CubeCoordinate operator -(CubeCoordinate a)
        {
            return new CubeCoordinate(-a.x, -a.y, -a.z);
        }
        
        /// <summary>
        /// 相等运算符
        /// </summary>
        public static bool operator ==(CubeCoordinate a, CubeCoordinate b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        
        /// <summary>
        /// 不等运算符
        /// </summary>
        public static bool operator !=(CubeCoordinate a, CubeCoordinate b)
        {
            return !(a == b);
        }
        
        #endregion
        
        #region Object重写方法
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        public bool Equals(CubeCoordinate other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        
        /// <summary>
        /// 检查与对象是否相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is CubeCoordinate other && Equals(other);
        }
        
        /// <summary>
        /// 获取哈希值
        /// </summary>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"Cube({x}, {y}, {z})";
        }
        
        #endregion
        
        #region 隐式转换
        
        /// <summary>
        /// 从Vector3Int隐式转换
        /// </summary>
        public static implicit operator CubeCoordinate(Vector3Int vector)
        {
            return new CubeCoordinate(vector.x, vector.y, vector.z);
        }
        
        /// <summary>
        /// 转换为Vector3Int
        /// </summary>
        public static implicit operator Vector3Int(CubeCoordinate coord)
        {
            return new Vector3Int(coord.x, coord.y, coord.z);
        }
        
        #endregion
    }
}