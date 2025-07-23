using System;
using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;

namespace HexCoordinateSystem.Coordinates
{
    /// <summary>
    /// 奇数列偏移坐标系统实现
    /// 类似传统二维数组，奇数列向上偏移半个单位
    /// 优点：直观易懂，类似传统网格，适合显示和用户界面
    /// 适用场景：用户界面显示，关卡编辑器，需要与传统网格兼容的系统
    /// </summary>
    [Serializable]
    public struct OffsetCoordinateOddQ : IHexCoordinate, IEquatable<OffsetCoordinateOddQ>
    {
        #region 字段和属性
        
        /// <summary>
        /// 列坐标
        /// </summary>
        public readonly int col;
        
        /// <summary>
        /// 行坐标
        /// </summary>
        public readonly int row;
        
        /// <summary>
        /// 零坐标常量
        /// </summary>
        public static readonly OffsetCoordinateOddQ Zero = new OffsetCoordinateOddQ(0, 0);
        
        #endregion
        
        #region 构造函数
        
        /// <summary>
        /// 构造奇数列偏移坐标
        /// </summary>
        /// <param name="col">列坐标</param>
        /// <param name="row">行坐标</param>
        public OffsetCoordinateOddQ(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        
        /// <summary>
        /// 从Vector2Int构造偏移坐标
        /// </summary>
        /// <param name="vector">包含col和row的向量</param>
        public OffsetCoordinateOddQ(Vector2Int vector) : this(vector.x, vector.y)
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
            
            // 基础四个方向（上下左右）
            neighbors.Add(new OffsetCoordinateOddQ(col, row + 1));     // 上
            neighbors.Add(new OffsetCoordinateOddQ(col, row - 1));     // 下
            neighbors.Add(new OffsetCoordinateOddQ(col + 1, row));     // 右
            neighbors.Add(new OffsetCoordinateOddQ(col - 1, row));     // 左
            
            // 对角方向（奇偶列不同）
            if (col % 2 != 0)
            {
                // 奇数列
                neighbors.Add(new OffsetCoordinateOddQ(col - 1, row + 1)); // 左上
                neighbors.Add(new OffsetCoordinateOddQ(col + 1, row + 1)); // 右上
            }
            else
            {
                // 偶数列
                neighbors.Add(new OffsetCoordinateOddQ(col - 1, row - 1)); // 左下
                neighbors.Add(new OffsetCoordinateOddQ(col + 1, row - 1)); // 右下
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 获取指定方向的邻居坐标
        /// </summary>
        /// <param name="direction">方向索引（0-5）</param>
        /// <returns>邻居坐标</returns>
        public OffsetCoordinateOddQ GetNeighbor(int direction)
        {
            if (direction < 0 || direction >= 6)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), "方向索引必须在0-5之间");
            }
            
            int parity = col % 2;
            var dir = HexConstants.OFFSET_ODD_Q_DIRECTIONS[parity, direction];
            
            // 特殊处理右下方向
            if (direction == 5)
            {
                if (parity == 0) // 偶数列
                {
                    return new OffsetCoordinateOddQ(col, row - 1);
                }
                else // 奇数列
                {
                    return new OffsetCoordinateOddQ(col, row + 1);
                }
            }
            
            return new OffsetCoordinateOddQ(col + dir.x, row + dir.y);
        }
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        public int DistanceTo(IHexCoordinate other)
        {
            return ToAxial().DistanceTo(other);
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        /// <returns>轴向坐标</returns>
        public AxialCoordinate ToAxial()
        {
            int q = col;
            int r = row - (col - (col & 1)) / 2;
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
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            return col >= HexConstants.MIN_COORDINATE_VALUE && 
                   col <= HexConstants.MAX_COORDINATE_VALUE &&
                   row >= HexConstants.MIN_COORDINATE_VALUE && 
                   row <= HexConstants.MAX_COORDINATE_VALUE;
        }
        
        #endregion
        
        #region 静态方法
        
        /// <summary>
        /// 从轴向坐标转换为奇数列偏移坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>奇数列偏移坐标</returns>
        public static OffsetCoordinateOddQ FromAxial(AxialCoordinate axial)
        {
            int col = axial.q;
            int row = axial.r + (axial.q - (axial.q & 1)) / 2;
            return new OffsetCoordinateOddQ(col, row);
        }
        
        /// <summary>
        /// 从立方坐标转换为奇数列偏移坐标
        /// </summary>
        /// <param name="cube">立方坐标</param>
        /// <returns>奇数列偏移坐标</returns>
        public static OffsetCoordinateOddQ FromCube(CubeCoordinate cube)
        {
            return FromAxial(cube.ToAxial());
        }
        
        /// <summary>
        /// 从世界坐标转换为奇数列偏移坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>奇数列偏移坐标</returns>
        public static OffsetCoordinateOddQ FromWorldPosition(Vector3 worldPosition, float hexSize = 1.0f)
        {
            var axial = AxialCoordinate.FromWorldPosition(worldPosition, hexSize);
            return FromAxial(axial);
        }
        
        /// <summary>
        /// 计算两个偏移坐标之间的距离
        /// </summary>
        /// <param name="a">坐标A</param>
        /// <param name="b">坐标B</param>
        /// <returns>距离值</returns>
        public static int Distance(OffsetCoordinateOddQ a, OffsetCoordinateOddQ b)
        {
            return AxialCoordinate.Distance(a.ToAxial(), b.ToAxial());
        }
        
        /// <summary>
        /// 获取指定半径范围内的所有坐标
        /// </summary>
        /// <param name="center">中心坐标</param>
        /// <param name="radius">半径</param>
        /// <returns>范围内的坐标列表</returns>
        public static List<OffsetCoordinateOddQ> GetCoordinatesInRange(OffsetCoordinateOddQ center, int radius)
        {
            var axialCoords = AxialCoordinate.GetCoordinatesInRange(center.ToAxial(), radius);
            var offsetCoords = new List<OffsetCoordinateOddQ>(axialCoords.Count);
            
            foreach (var axial in axialCoords)
            {
                offsetCoords.Add(FromAxial(axial));
            }
            
            return offsetCoords;
        }
        
        /// <summary>
        /// 检查坐标是否在矩形区域内
        /// </summary>
        /// <param name="coord">要检查的坐标</param>
        /// <param name="minCol">最小列</param>
        /// <param name="maxCol">最大列</param>
        /// <param name="minRow">最小行</param>
        /// <param name="maxRow">最大行</param>
        /// <returns>是否在区域内</returns>
        public static bool IsInRectangle(OffsetCoordinateOddQ coord, int minCol, int maxCol, int minRow, int maxRow)
        {
            return coord.col >= minCol && coord.col <= maxCol && 
                   coord.row >= minRow && coord.row <= maxRow;
        }
        
        /// <summary>
        /// 获取矩形区域内的所有坐标
        /// </summary>
        /// <param name="minCol">最小列</param>
        /// <param name="maxCol">最大列</param>
        /// <param name="minRow">最小行</param>
        /// <param name="maxRow">最大行</param>
        /// <returns>区域内的坐标列表</returns>
        public static List<OffsetCoordinateOddQ> GetCoordinatesInRectangle(int minCol, int maxCol, int minRow, int maxRow)
        {
            var coordinates = new List<OffsetCoordinateOddQ>();
            
            for (int col = minCol; col <= maxCol; col++)
            {
                for (int row = minRow; row <= maxRow; row++)
                {
                    coordinates.Add(new OffsetCoordinateOddQ(col, row));
                }
            }
            
            return coordinates;
        }
        
        #endregion
        
        #region 运算符重载
        
        /// <summary>
        /// 加法运算符
        /// </summary>
        public static OffsetCoordinateOddQ operator +(OffsetCoordinateOddQ a, OffsetCoordinateOddQ b)
        {
            return new OffsetCoordinateOddQ(a.col + b.col, a.row + b.row);
        }
        
        /// <summary>
        /// 减法运算符
        /// </summary>
        public static OffsetCoordinateOddQ operator -(OffsetCoordinateOddQ a, OffsetCoordinateOddQ b)
        {
            return new OffsetCoordinateOddQ(a.col - b.col, a.row - b.row);
        }
        
        /// <summary>
        /// 标量乘法运算符
        /// </summary>
        public static OffsetCoordinateOddQ operator *(OffsetCoordinateOddQ a, int scalar)
        {
            return new OffsetCoordinateOddQ(a.col * scalar, a.row * scalar);
        }
        
        /// <summary>
        /// 相等运算符
        /// </summary>
        public static bool operator ==(OffsetCoordinateOddQ a, OffsetCoordinateOddQ b)
        {
            return a.col == b.col && a.row == b.row;
        }
        
        /// <summary>
        /// 不等运算符
        /// </summary>
        public static bool operator !=(OffsetCoordinateOddQ a, OffsetCoordinateOddQ b)
        {
            return !(a == b);
        }
        
        #endregion
        
        #region Object重写方法
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        public bool Equals(OffsetCoordinateOddQ other)
        {
            return col == other.col && row == other.row;
        }
        
        /// <summary>
        /// 检查与对象是否相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is OffsetCoordinateOddQ other && Equals(other);
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
            return $"OffsetOddQ({col}, {row})";
        }
        
        #endregion
        
        #region 隐式转换
        
        /// <summary>
        /// 从Vector2Int隐式转换
        /// </summary>
        public static implicit operator OffsetCoordinateOddQ(Vector2Int vector)
        {
            return new OffsetCoordinateOddQ(vector.x, vector.y);
        }
        
        /// <summary>
        /// 转换为Vector2Int
        /// </summary>
        public static implicit operator Vector2Int(OffsetCoordinateOddQ coord)
        {
            return new Vector2Int(coord.col, coord.row);
        }
        
        #endregion
    }
    
    /// <summary>
    /// 偶数列偏移坐标系统实现
    /// 类似传统二维数组，偶数列向上偏移半个单位
    /// </summary>
    [Serializable]
    public struct OffsetCoordinateEvenQ : IHexCoordinate, IEquatable<OffsetCoordinateEvenQ>
    {
        #region 字段和属性
        
        /// <summary>
        /// 列坐标
        /// </summary>
        public readonly int col;
        
        /// <summary>
        /// 行坐标
        /// </summary>
        public readonly int row;
        
        /// <summary>
        /// 零坐标常量
        /// </summary>
        public static readonly OffsetCoordinateEvenQ Zero = new OffsetCoordinateEvenQ(0, 0);
        
        #endregion
        
        #region 构造函数
        
        /// <summary>
        /// 构造偶数列偏移坐标
        /// </summary>
        /// <param name="col">列坐标</param>
        /// <param name="row">行坐标</param>
        public OffsetCoordinateEvenQ(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        
        /// <summary>
        /// 从Vector2Int构造偏移坐标
        /// </summary>
        /// <param name="vector">包含col和row的向量</param>
        public OffsetCoordinateEvenQ(Vector2Int vector) : this(vector.x, vector.y)
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
            
            // 基础四个方向（上下左右）
            neighbors.Add(new OffsetCoordinateEvenQ(col, row + 1));     // 上
            neighbors.Add(new OffsetCoordinateEvenQ(col, row - 1));     // 下
            neighbors.Add(new OffsetCoordinateEvenQ(col + 1, row));     // 右
            neighbors.Add(new OffsetCoordinateEvenQ(col - 1, row));     // 左
            
            // 对角方向（奇偶列不同）
            if (col % 2 == 0)
            {
                // 偶数列
                neighbors.Add(new OffsetCoordinateEvenQ(col - 1, row + 1)); // 左上
                neighbors.Add(new OffsetCoordinateEvenQ(col + 1, row + 1)); // 右上
            }
            else
            {
                // 奇数列
                neighbors.Add(new OffsetCoordinateEvenQ(col - 1, row - 1)); // 左下
                neighbors.Add(new OffsetCoordinateEvenQ(col + 1, row - 1)); // 右下
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        public int DistanceTo(IHexCoordinate other)
        {
            return ToAxial().DistanceTo(other);
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        /// <returns>轴向坐标</returns>
        public AxialCoordinate ToAxial()
        {
            int q = col;
            int r = row - (col + (col & 1)) / 2;
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
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            return col >= HexConstants.MIN_COORDINATE_VALUE && 
                   col <= HexConstants.MAX_COORDINATE_VALUE &&
                   row >= HexConstants.MIN_COORDINATE_VALUE && 
                   row <= HexConstants.MAX_COORDINATE_VALUE;
        }
        
        #endregion
        
        #region 静态方法
        
        /// <summary>
        /// 从轴向坐标转换为偶数列偏移坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>偶数列偏移坐标</returns>
        public static OffsetCoordinateEvenQ FromAxial(AxialCoordinate axial)
        {
            int col = axial.q;
            int row = axial.r + (axial.q + (axial.q & 1)) / 2;
            return new OffsetCoordinateEvenQ(col, row);
        }
        
        /// <summary>
        /// 从立方坐标转换为偶数列偏移坐标
        /// </summary>
        /// <param name="cube">立方坐标</param>
        /// <returns>偶数列偏移坐标</returns>
        public static OffsetCoordinateEvenQ FromCube(CubeCoordinate cube)
        {
            return FromAxial(cube.ToAxial());
        }
        
        #endregion
        
        #region Object重写方法
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        public bool Equals(OffsetCoordinateEvenQ other)
        {
            return col == other.col && row == other.row;
        }
        
        /// <summary>
        /// 检查与对象是否相等
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is OffsetCoordinateEvenQ other && Equals(other);
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
            return $"OffsetEvenQ({col}, {row})";
        }
        
        #endregion
    }
}