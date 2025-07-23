using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoordinateSystem.Core;

namespace GridCoordinateSystem.Coordinates
{
    /// <summary>
    /// 直角坐标系统
    /// 最基础的坐标系统，使用(x, y)表示网格位置
    /// 适用于大多数四边形网格应用场景
    /// </summary>
    [Serializable]
    public struct CartesianCoordinate : IGridCoordinate, IEquatable<CartesianCoordinate>
    {
        #region 字段
        
        [SerializeField] private int x;
        [SerializeField] private int y;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// X坐标
        /// </summary>
        public int X => x;
        
        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y => y;
        
        #endregion
        
        #region 构造函数
        
        /// <summary>
        /// 构造直角坐标
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        public CartesianCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        /// <summary>
        /// 从Vector2Int构造
        /// </summary>
        /// <param name="vector">Vector2Int坐标</param>
        public CartesianCoordinate(Vector2Int vector)
        {
            this.x = vector.x;
            this.y = vector.y;
        }
        
        #endregion
        
        #region IGridCoordinate实现
        
        /// <summary>
        /// 转换为世界坐标位置
        /// </summary>
        /// <param name="cellSize">网格单元大小</param>
        /// <returns>世界坐标位置</returns>
        public Vector3 ToWorldPosition(float cellSize = 1.0f)
        {
            return new Vector3(x * cellSize, 0, y * cellSize);
        }
        
        /// <summary>
        /// 获取4邻居坐标（上下左右）
        /// </summary>
        /// <returns>4邻居坐标列表</returns>
        public List<IGridCoordinate> GetNeighbors4()
        {
            var neighbors = new List<IGridCoordinate>(4);
            
            foreach (var direction in GridConstants.DIRECTIONS_4)
            {
                neighbors.Add(new CartesianCoordinate(x + direction.x, y + direction.y));
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 获取8邻居坐标（包含对角线）
        /// </summary>
        /// <returns>8邻居坐标列表</returns>
        public List<IGridCoordinate> GetNeighbors8()
        {
            var neighbors = new List<IGridCoordinate>(8);
            
            foreach (var direction in GridConstants.DIRECTIONS_8)
            {
                neighbors.Add(new CartesianCoordinate(x + direction.x, y + direction.y));
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 计算到另一个坐标的曼哈顿距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>曼哈顿距离</returns>
        public int ManhattanDistance(CartesianCoordinate other)
        {
            return Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y);
        }
        
        /// <summary>
        /// 计算到另一个坐标的欧几里得距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>欧几里得距离</returns>
        public float EuclideanDistance(CartesianCoordinate other)
        {
            int dx = x - other.x;
            int dy = y - other.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
        
        /// <summary>
        /// 计算到另一个坐标的切比雪夫距离（棋盘距离）
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>切比雪夫距离</returns>
        public int ChebyshevDistance(CartesianCoordinate other)
        {
            return Mathf.Max(Mathf.Abs(x - other.x), Mathf.Abs(y - other.y));
        }
        
        /// <summary>
        /// 计算到另一个坐标的曼哈顿距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>曼哈顿距离</returns>
        public int ManhattanDistanceTo(IGridCoordinate other)
        {
            var otherCartesian = other.ToCartesian();
            return Mathf.Abs(x - otherCartesian.X) + Mathf.Abs(y - otherCartesian.Y);
        }
        
        /// <summary>
        /// 计算到另一个坐标的欧几里得距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>欧几里得距离</returns>
        public float EuclideanDistanceTo(IGridCoordinate other)
        {
            var otherCartesian = other.ToCartesian();
            int dx = x - otherCartesian.X;
            int dy = y - otherCartesian.Y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
        
        /// <summary>
        /// 计算到另一个坐标的切比雪夫距离（棋盘距离）
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>切比雪夫距离</returns>
        public int ChebyshevDistanceTo(IGridCoordinate other)
        {
            var otherCartesian = other.ToCartesian();
            return Mathf.Max(Mathf.Abs(x - otherCartesian.X), Mathf.Abs(y - otherCartesian.Y));
        }
        
        /// <summary>
        /// 转换为直角坐标（返回自身）
        /// </summary>
        /// <returns>直角坐标</returns>
        public CartesianCoordinate ToCartesian()
        {
            return this;
        }
        
        /// <summary>
        /// 转换为索引坐标
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <returns>索引坐标</returns>
        public IndexCoordinate ToIndex(int width)
        {
            return new IndexCoordinate(y * width + x);
        }
        
        /// <summary>
        /// 检查坐标是否在指定边界内
        /// </summary>
        /// <param name="minX">最小X坐标</param>
        /// <param name="minY">最小Y坐标</param>
        /// <param name="maxX">最大X坐标</param>
        /// <param name="maxY">最大Y坐标</param>
        /// <returns>是否在边界内</returns>
        public bool IsWithinBounds(int minX, int minY, int maxX, int maxY)
        {
            return x >= minX && x <= maxX && y >= minY && y <= maxY;
        }
        
        /// <summary>
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            return true; // 直角坐标总是有效的
        }
        
        #endregion
        
        #region 运算符重载
        
        /// <summary>
        /// 坐标加法
        /// </summary>
        public static CartesianCoordinate operator +(CartesianCoordinate a, CartesianCoordinate b)
        {
            return new CartesianCoordinate(a.x + b.x, a.y + b.y);
        }
        
        /// <summary>
        /// 坐标减法
        /// </summary>
        public static CartesianCoordinate operator -(CartesianCoordinate a, CartesianCoordinate b)
        {
            return new CartesianCoordinate(a.x - b.x, a.y - b.y);
        }
        
        /// <summary>
        /// 坐标乘法（标量）
        /// </summary>
        public static CartesianCoordinate operator *(CartesianCoordinate coord, int scalar)
        {
            return new CartesianCoordinate(coord.x * scalar, coord.y * scalar);
        }
        
        /// <summary>
        /// 坐标相等比较
        /// </summary>
        public static bool operator ==(CartesianCoordinate a, CartesianCoordinate b)
        {
            return a.x == b.x && a.y == b.y;
        }
        
        /// <summary>
        /// 坐标不等比较
        /// </summary>
        public static bool operator !=(CartesianCoordinate a, CartesianCoordinate b)
        {
            return !(a == b);
        }
        
        /// <summary>
        /// 隐式转换为Vector2Int
        /// </summary>
        public static implicit operator Vector2Int(CartesianCoordinate coord)
        {
            return new Vector2Int(coord.x, coord.y);
        }
        
        /// <summary>
        /// 隐式转换从Vector2Int
        /// </summary>
        public static implicit operator CartesianCoordinate(Vector2Int vector)
        {
            return new CartesianCoordinate(vector.x, vector.y);
        }
        
        #endregion
        
        #region 静态方法
        
        /// <summary>
        /// 零坐标
        /// </summary>
        public static CartesianCoordinate Zero => new CartesianCoordinate(0, 0);
        
        /// <summary>
        /// 单位X坐标
        /// </summary>
        public static CartesianCoordinate UnitX => new CartesianCoordinate(1, 0);
        
        /// <summary>
        /// 单位Y坐标
        /// </summary>
        public static CartesianCoordinate UnitY => new CartesianCoordinate(0, 1);
        
        /// <summary>
        /// 从世界坐标创建网格坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <param name="alignment">对齐方式</param>
        /// <returns>网格坐标</returns>
        public static CartesianCoordinate FromWorldPosition(Vector3 worldPosition, float cellSize = 1.0f, 
            GridConstants.GridAlignment alignment = GridConstants.GridAlignment.Center)
        {
            float offsetX = alignment == GridConstants.GridAlignment.Center ? 0.5f : 0f;
            float offsetY = alignment == GridConstants.GridAlignment.Center ? 0.5f : 0f;
            
            int x = Mathf.FloorToInt((worldPosition.x / cellSize) + offsetX);
            int y = Mathf.FloorToInt((worldPosition.z / cellSize) + offsetY);
            
            return new CartesianCoordinate(x, y);
        }
        
        #endregion
        
        #region Object重写
        
        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }
        
        /// <summary>
        /// 检查相等性
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is CartesianCoordinate other)
            {
                return Equals(other);
            }
            return false;
        }
        
        /// <summary>
        /// 检查相等性（强类型）
        /// </summary>
        /// <param name="other">比较坐标</param>
        /// <returns>是否相等</returns>
        public bool Equals(CartesianCoordinate other)
        {
            return x == other.x && y == other.y;
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"CartesianCoordinate({x}, {y})";
        }
        
        #endregion
    }
}