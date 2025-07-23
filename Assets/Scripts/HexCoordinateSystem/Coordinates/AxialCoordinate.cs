using System;
using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;

namespace HexCoordinateSystem.Coordinates
{
    /// <summary>
    /// 轴向坐标系统实现
    /// 使用两个坐标 (q, r) 表示六边形位置，第三个坐标 s = -q - r
    /// 优点：存储效率高，保持立方坐标的大部分优点
    /// 适用场景：内存敏感的应用，作为其他坐标系统的中间转换格式
    /// </summary>
    [Serializable]
    public struct AxialCoordinate : IHexCoordinate, IEquatable<AxialCoordinate>
    {
        #region 字段和属性
        
        /// <summary>
        /// Q坐标（列坐标）
        /// </summary>
        public readonly int q;
        
        /// <summary>
        /// R坐标（行坐标）
        /// </summary>
        public readonly int r;
        
        /// <summary>
        /// S坐标（计算得出，满足 q + r + s = 0）
        /// </summary>
        public int s => -q - r;
        
        /// <summary>
        /// 零坐标常量
        /// </summary>
        public static readonly AxialCoordinate Zero = new AxialCoordinate(0, 0);
        
        #endregion
        
        #region 构造函数
        
        /// <summary>
        /// 构造轴向坐标
        /// </summary>
        /// <param name="q">Q坐标</param>
        /// <param name="r">R坐标</param>
        public AxialCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
        
        /// <summary>
        /// 从Vector2Int构造轴向坐标
        /// </summary>
        /// <param name="vector">包含q和r的向量</param>
        public AxialCoordinate(Vector2Int vector) : this(vector.x, vector.y)
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
            return ToWorldPosition(HexConstants.POINTY_TOP_LAYOUT, hexSize);
        }
        
        /// <summary>
        /// 使用指定布局转换为世界坐标位置
        /// </summary>
        /// <param name="layout">六边形布局</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>世界坐标位置</returns>
        public Vector3 ToWorldPosition(HexLayout layout, float hexSize = 1.0f)
        {
            float x = (layout.ForwardMatrix1.x * q + layout.ForwardMatrix1.y * r) * hexSize;
            float y = (layout.ForwardMatrix2.x * q + layout.ForwardMatrix2.y * r) * hexSize;
            return new Vector3(x, y, 0);
        }
        
        /// <summary>
        /// 获取所有相邻的六边形坐标
        /// </summary>
        /// <returns>相邻坐标列表</returns>
        public List<IHexCoordinate> GetNeighbors()
        {
            var neighbors = new List<IHexCoordinate>(6);
            
            for (int i = 0; i < HexConstants.AXIAL_DIRECTIONS.Length; i++)
            {
                var direction = HexConstants.AXIAL_DIRECTIONS[i];
                neighbors.Add(new AxialCoordinate(q + direction.x, r + direction.y));
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 获取指定方向的邻居坐标
        /// </summary>
        /// <param name="direction">方向索引（0-5）</param>
        /// <returns>邻居坐标</returns>
        public AxialCoordinate GetNeighbor(int direction)
        {
            if (direction < 0 || direction >= HexConstants.AXIAL_DIRECTIONS.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), "方向索引必须在0-5之间");
            }
            
            var dir = HexConstants.AXIAL_DIRECTIONS[direction];
            return new AxialCoordinate(q + dir.x, r + dir.y);
        }
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        public int DistanceTo(IHexCoordinate other)
        {
            var otherAxial = other.ToAxial();
            return Distance(this, otherAxial);
        }
        
        /// <summary>
        /// 转换为轴向坐标（返回自身）
        /// </summary>
        /// <returns>轴向坐标</returns>
        public AxialCoordinate ToAxial()
        {
            return this;
        }
        
        /// <summary>
        /// 转换为立方坐标
        /// </summary>
        /// <returns>立方坐标</returns>
        public CubeCoordinate ToCube()
        {
            return new CubeCoordinate(q, r, s);
        }
        
        /// <summary>
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            return q >= HexConstants.MIN_COORDINATE_VALUE && 
                   q <= HexConstants.MAX_COORDINATE_VALUE &&
                   r >= HexConstants.MIN_COORDINATE_VALUE && 
                   r <= HexConstants.MAX_COORDINATE_VALUE;
        }
        
        #endregion
        
        #region 静态方法
        
        /// <summary>
        /// 计算两个轴向坐标之间的距离
        /// </summary>
        /// <param name="a">坐标A</param>
        /// <param name="b">坐标B</param>
        /// <returns>距离值</returns>
        public static int Distance(AxialCoordinate a, AxialCoordinate b)
        {
            return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.q + a.r - b.q - b.r) + Mathf.Abs(a.r - b.r)) / 2;
        }
        
        /// <summary>
        /// 从世界坐标转换为轴向坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate FromWorldPosition(Vector3 worldPosition, float hexSize = 1.0f)
        {
            return FromWorldPosition(worldPosition, HexConstants.POINTY_TOP_LAYOUT, hexSize);
        }
        
        /// <summary>
        /// 使用指定布局从世界坐标转换为轴向坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="layout">六边形布局</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate FromWorldPosition(Vector3 worldPosition, HexLayout layout, float hexSize = 1.0f)
        {
            Vector2 point = new Vector2(worldPosition.x / hexSize, worldPosition.y / hexSize);
            
            float q = layout.InverseMatrix1.x * point.x + layout.InverseMatrix1.y * point.y;
            float r = layout.InverseMatrix2.x * point.x + layout.InverseMatrix2.y * point.y;
            
            return Round(q, r);
        }
        
        /// <summary>
        /// 将浮点坐标四舍五入为整数坐标
        /// </summary>
        /// <param name="q">浮点Q坐标</param>
        /// <param name="r">浮点R坐标</param>
        /// <returns>四舍五入后的轴向坐标</returns>
        public static AxialCoordinate Round(float q, float r)
        {
            float s = -q - r;
            
            int roundedQ = Mathf.RoundToInt(q);
            int roundedR = Mathf.RoundToInt(r);
            int roundedS = Mathf.RoundToInt(s);
            
            float qDiff = Mathf.Abs(roundedQ - q);
            float rDiff = Mathf.Abs(roundedR - r);
            float sDiff = Mathf.Abs(roundedS - s);
            
            if (qDiff > rDiff && qDiff > sDiff)
            {
                roundedQ = -roundedR - roundedS;
            }
            else if (rDiff > sDiff)
            {
                roundedR = -roundedQ - roundedS;
            }
            
            return new AxialCoordinate(roundedQ, roundedR);
        }
        
        /// <summary>
        /// 获取从起点到终点的直线路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>路径上的坐标列表</returns>
        public static List<AxialCoordinate> GetLinePath(AxialCoordinate start, AxialCoordinate end)
        {
            int distance = Distance(start, end);
            var path = new List<AxialCoordinate>(distance + 1);
            
            for (int i = 0; i <= distance; i++)
            {
                float t = distance == 0 ? 0.0f : (float)i / distance;
                float q = start.q + (end.q - start.q) * t;
                float r = start.r + (end.r - start.r) * t;
                path.Add(Round(q, r));
            }
            
            return path;
        }
        
        /// <summary>
        /// 获取指定半径范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>范围内的坐标列表</returns>
        public static List<AxialCoordinate> GetCoordinatesInRange(AxialCoordinate center, int radius)
        {
            var coordinates = new List<AxialCoordinate>();
            
            for (int q = -radius; q <= radius; q++)
            {
                int r1 = Mathf.Max(-radius, -q - radius);
                int r2 = Mathf.Min(radius, -q + radius);
                
                for (int r = r1; r <= r2; r++)
                {
                    coordinates.Add(new AxialCoordinate(center.q + q, center.r + r));
                }
            }
            
            return coordinates;
        }
        
        /// <summary>
        /// 获取指定半径的环形坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>环形坐标列表</returns>
        public static List<AxialCoordinate> GetRing(AxialCoordinate center, int radius)
        {
            if (radius == 0)
            {
                return new List<AxialCoordinate> { center };
            }
            
            var ring = new List<AxialCoordinate>(radius * 6);
            var current = center + new AxialCoordinate(HexConstants.AXIAL_DIRECTIONS[4].x * radius, 
                                                      HexConstants.AXIAL_DIRECTIONS[4].y * radius);
            
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    ring.Add(current);
                    current = current.GetNeighbor(i);
                }
            }
            
            return ring;
        }
        
        #endregion
        
        #region 运算符重载
        
        /// <summary>
        /// 加法运算符
        /// </summary>
        public static AxialCoordinate operator +(AxialCoordinate a, AxialCoordinate b)
        {
            return new AxialCoordinate(a.q + b.q, a.r + b.r);
        }
        
        /// <summary>
        /// 减法运算符
        /// </summary>
        public static AxialCoordinate operator -(AxialCoordinate a, AxialCoordinate b)
        {
            return new AxialCoordinate(a.q - b.q, a.r - b.r);
        }
        
        /// <summary>
        /// 标量乘法运算符
        /// </summary>
        public static AxialCoordinate operator *(AxialCoordinate a, int scalar)
        {
            return new AxialCoordinate(a.q * scalar, a.r * scalar);
        }
        
        /// <summary>
        /// 相等运算符
        /// </summary>
        public static bool operator ==(AxialCoordinate a, AxialCoordinate b)
        {
            return a.q == b.q && a.r == b.r;
        }
        
        /// <summary>
        /// 不等运算符
        /// </summary>
        public static bool operator !=(AxialCoordinate a, AxialCoordinate b)
        {
            return !(a == b);
        }
        
        #endregion
        
        #region Object重写方法
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        public bool Equals(AxialCoordinate other)
        {
            return q == other.q && r == other.r;
        }
        
        /// <summary>
        /// 检查与对象是否相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is AxialCoordinate other && Equals(other);
        }
        
        /// <summary>
        /// 获取哈希值
        /// </summary>
        public override int GetHashCode()
        {
            return q.GetHashCode() ^ (r.GetHashCode() << 2);
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"Axial({q}, {r})";
        }
        
        #endregion
        
        #region 隐式转换
        
        /// <summary>
        /// 从Vector2Int隐式转换
        /// </summary>
        public static implicit operator AxialCoordinate(Vector2Int vector)
        {
            return new AxialCoordinate(vector.x, vector.y);
        }
        
        /// <summary>
        /// 转换为Vector2Int
        /// </summary>
        public static implicit operator Vector2Int(AxialCoordinate coord)
        {
            return new Vector2Int(coord.q, coord.r);
        }
        
        #endregion
    }
}