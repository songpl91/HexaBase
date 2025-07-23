using System;
using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;

namespace HexCoordinateSystem.Coordinates
{
    /// <summary>
    /// 双宽坐标系统实现
    /// 将坐标放大2倍以避免分数运算，适用于需要避免浮点运算的场景
    /// 优点：避免浮点运算，整数计算精度高，某些算法实现简单
    /// 适用场景：需要避免浮点运算的嵌入式系统，高精度要求的计算
    /// </summary>
    [Serializable]
    public struct DoubledCoordinate : IHexCoordinate, IEquatable<DoubledCoordinate>
    {
        #region 字段和属性
        
        /// <summary>
        /// 双宽列坐标
        /// </summary>
        public readonly int col;
        
        /// <summary>
        /// 双宽行坐标
        /// </summary>
        public readonly int row;
        
        /// <summary>
        /// 零坐标常量
        /// </summary>
        public static readonly DoubledCoordinate Zero = new DoubledCoordinate(0, 0);
        
        #endregion
        
        #region 构造函数
        
        /// <summary>
        /// 构造双宽坐标
        /// </summary>
        /// <param name="col">双宽列坐标</param>
        /// <param name="row">双宽行坐标</param>
        public DoubledCoordinate(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        
        /// <summary>
        /// 从Vector2Int构造双宽坐标
        /// </summary>
        /// <param name="vector">包含col和row的向量</param>
        public DoubledCoordinate(Vector2Int vector) : this(vector.x, vector.y)
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
            
            // 双宽坐标的六个方向
            neighbors.Add(new DoubledCoordinate(col + 2, row));         // 右
            neighbors.Add(new DoubledCoordinate(col + 1, row + 1));     // 右上
            neighbors.Add(new DoubledCoordinate(col - 1, row + 1));     // 左上
            neighbors.Add(new DoubledCoordinate(col - 2, row));         // 左
            neighbors.Add(new DoubledCoordinate(col - 1, row - 1));     // 左下
            neighbors.Add(new DoubledCoordinate(col + 1, row - 1));     // 右下
            
            return neighbors;
        }
        
        /// <summary>
        /// 获取指定方向的邻居坐标
        /// </summary>
        /// <param name="direction">方向索引（0-5）</param>
        /// <returns>邻居坐标</returns>
        public DoubledCoordinate GetNeighbor(int direction)
        {
            if (direction < 0 || direction >= 6)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), "方向索引必须在0-5之间");
            }
            
            switch (direction)
            {
                case 0: return new DoubledCoordinate(col + 2, row);         // 右
                case 1: return new DoubledCoordinate(col + 1, row + 1);     // 右上
                case 2: return new DoubledCoordinate(col - 1, row + 1);     // 左上
                case 3: return new DoubledCoordinate(col - 2, row);         // 左
                case 4: return new DoubledCoordinate(col - 1, row - 1);     // 左下
                case 5: return new DoubledCoordinate(col + 1, row - 1);     // 右下
                default: throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        public int DistanceTo(IHexCoordinate other)
        {
            var otherDoubled = other.ToAxial().ToDoubled();
            return Distance(this, otherDoubled);
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        /// <returns>轴向坐标</returns>
        public AxialCoordinate ToAxial()
        {
            int q = col;
            int r = (row - col) / 2;
            return new AxialCoordinate(q, r);
        }
        
        /// <summary>
        /// 转换为立方坐标
        /// </summary>
        /// <returns>立方坐标</returns>
        public CubeCoordinate ToCube()
        {
            return ToAxial().ToCube();
        }
        
        /// <summary>
        /// 检查坐标是否有效（双宽坐标有特殊的有效性规则）
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            // 双宽坐标必须满足 (col + row) % 2 == 0
            return (col + row) % 2 == 0 &&
                   col >= HexConstants.MIN_COORDINATE_VALUE && 
                   col <= HexConstants.MAX_COORDINATE_VALUE &&
                   row >= HexConstants.MIN_COORDINATE_VALUE && 
                   row <= HexConstants.MAX_COORDINATE_VALUE;
        }
        
        #endregion
        
        #region 静态方法
        
        /// <summary>
        /// 从轴向坐标转换为双宽坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>双宽坐标</returns>
        public static DoubledCoordinate FromAxial(AxialCoordinate axial)
        {
            int col = axial.q;
            int row = 2 * axial.r + axial.q;
            return new DoubledCoordinate(col, row);
        }
        
        /// <summary>
        /// 从立方坐标转换为双宽坐标
        /// </summary>
        /// <param name="cube">立方坐标</param>
        /// <returns>双宽坐标</returns>
        public static DoubledCoordinate FromCube(CubeCoordinate cube)
        {
            return FromAxial(cube.ToAxial());
        }
        
        /// <summary>
        /// 从世界坐标转换为双宽坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>双宽坐标</returns>
        public static DoubledCoordinate FromWorldPosition(Vector3 worldPosition, float hexSize = 1.0f)
        {
            var axial = AxialCoordinate.FromWorldPosition(worldPosition, hexSize);
            return FromAxial(axial);
        }
        
        /// <summary>
        /// 计算两个双宽坐标之间的距离
        /// </summary>
        /// <param name="a">坐标A</param>
        /// <param name="b">坐标B</param>
        /// <returns>距离值</returns>
        public static int Distance(DoubledCoordinate a, DoubledCoordinate b)
        {
            int colDiff = Mathf.Abs(a.col - b.col);
            int rowDiff = Mathf.Abs(a.row - b.row);
            
            return Mathf.Max(colDiff, (colDiff + rowDiff) / 2);
        }
        
        /// <summary>
        /// 获取指定半径范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>范围内的坐标列表</returns>
        public static List<DoubledCoordinate> GetCoordinatesInRange(DoubledCoordinate center, int radius)
        {
            var coordinates = new List<DoubledCoordinate>();
            
            for (int col = center.col - 2 * radius; col <= center.col + 2 * radius; col++)
            {
                for (int row = center.row - 2 * radius; row <= center.row + 2 * radius; row++)
                {
                    var coord = new DoubledCoordinate(col, row);
                    if (coord.IsValid() && Distance(center, coord) <= radius)
                    {
                        coordinates.Add(coord);
                    }
                }
            }
            
            return coordinates;
        }
        
        /// <summary>
        /// 获取从起点到终点的直线路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>路径上的坐标列表</returns>
        public static List<DoubledCoordinate> GetLinePath(DoubledCoordinate start, DoubledCoordinate end)
        {
            // 转换为轴向坐标进行路径计算，然后转换回双宽坐标
            var axialPath = AxialCoordinate.GetLinePath(start.ToAxial(), end.ToAxial());
            var doubledPath = new List<DoubledCoordinate>(axialPath.Count);
            
            foreach (var axial in axialPath)
            {
                doubledPath.Add(FromAxial(axial));
            }
            
            return doubledPath;
        }
        
        /// <summary>
        /// 验证坐标是否符合双宽坐标的规则
        /// </summary>
        /// <param name="col">列坐标</param>
        /// <param name="row">行坐标</param>
        /// <returns>是否为有效的双宽坐标</returns>
        public static bool IsValidDoubledCoordinate(int col, int row)
        {
            return (col + row) % 2 == 0;
        }
        
        /// <summary>
        /// 获取最近的有效双宽坐标
        /// </summary>
        /// <param name="col">列坐标</param>
        /// <param name="row">行坐标</param>
        /// <returns>最近的有效双宽坐标</returns>
        public static DoubledCoordinate GetNearestValid(int col, int row)
        {
            if (IsValidDoubledCoordinate(col, row))
            {
                return new DoubledCoordinate(col, row);
            }
            
            // 调整到最近的有效坐标
            if ((col + row) % 2 != 0)
            {
                // 可以调整col或row，选择调整较小的那个
                if (Mathf.Abs(col) <= Mathf.Abs(row))
                {
                    col += (col >= 0) ? 1 : -1;
                }
                else
                {
                    row += (row >= 0) ? 1 : -1;
                }
            }
            
            return new DoubledCoordinate(col, row);
        }
        
        #endregion
        
        #region 运算符重载
        
        /// <summary>
        /// 加法运算符
        /// </summary>
        public static DoubledCoordinate operator +(DoubledCoordinate a, DoubledCoordinate b)
        {
            return new DoubledCoordinate(a.col + b.col, a.row + b.row);
        }
        
        /// <summary>
        /// 减法运算符
        /// </summary>
        public static DoubledCoordinate operator -(DoubledCoordinate a, DoubledCoordinate b)
        {
            return new DoubledCoordinate(a.col - b.col, a.row - b.row);
        }
        
        /// <summary>
        /// 标量乘法运算符
        /// </summary>
        public static DoubledCoordinate operator *(DoubledCoordinate a, int scalar)
        {
            return new DoubledCoordinate(a.col * scalar, a.row * scalar);
        }
        
        /// <summary>
        /// 相等运算符
        /// </summary>
        public static bool operator ==(DoubledCoordinate a, DoubledCoordinate b)
        {
            return a.col == b.col && a.row == b.row;
        }
        
        /// <summary>
        /// 不等运算符
        /// </summary>
        public static bool operator !=(DoubledCoordinate a, DoubledCoordinate b)
        {
            return !(a == b);
        }
        
        #endregion
        
        #region Object重写方法
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        public bool Equals(DoubledCoordinate other)
        {
            return col == other.col && row == other.row;
        }
        
        /// <summary>
        /// 检查与对象是否相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is DoubledCoordinate other && Equals(other);
        }
        
        /// <summary>
        /// 获取哈希值
        /// </summary>
        public override int GetHashCode()
        {
            return col.GetHashCode() ^ (row.GetHashCode() << 2);
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"Doubled({col}, {row})";
        }
        
        #endregion
        
        #region 隐式转换
        
        /// <summary>
        /// 从Vector2Int隐式转换
        /// </summary>
        public static implicit operator DoubledCoordinate(Vector2Int vector)
        {
            return new DoubledCoordinate(vector.x, vector.y);
        }
        
        /// <summary>
        /// 转换为Vector2Int
        /// </summary>
        public static implicit operator Vector2Int(DoubledCoordinate coord)
        {
            return new Vector2Int(coord.col, coord.row);
        }
        
        #endregion
    }
}

namespace HexCoordinateSystem.Coordinates
{
    /// <summary>
    /// 轴向坐标的扩展方法
    /// </summary>
    public static class AxialCoordinateExtensions
    {
        /// <summary>
        /// 转换为双宽坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>双宽坐标</returns>
        public static DoubledCoordinate ToDoubled(this AxialCoordinate axial)
        {
            return DoubledCoordinate.FromAxial(axial);
        }
    }
}