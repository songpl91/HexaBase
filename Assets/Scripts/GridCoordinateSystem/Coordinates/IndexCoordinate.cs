using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoordinateSystem.Core;

namespace GridCoordinateSystem.Coordinates
{
    /// <summary>
    /// 索引坐标系统
    /// 使用单一索引值表示网格位置，适用于一维数组存储的二维网格
    /// 常用于内存优化和数组访问场景
    /// </summary>
    [Serializable]
    public struct IndexCoordinate : IGridCoordinate, IEquatable<IndexCoordinate>
    {
        #region 字段
        
        [SerializeField] private int index;
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 索引值
        /// </summary>
        public int Index => index;
        
        #endregion
        
        #region 构造函数
        
        /// <summary>
        /// 构造索引坐标
        /// </summary>
        /// <param name="index">索引值</param>
        public IndexCoordinate(int index)
        {
            this.index = index;
        }
        
        #endregion
        
        #region IGridCoordinate实现
        
        /// <summary>
        /// 转换为世界坐标位置
        /// 注意：需要提供网格宽度才能正确转换
        /// </summary>
        /// <param name="cellSize">网格单元大小</param>
        /// <returns>世界坐标位置</returns>
        public Vector3 ToWorldPosition(float cellSize = 1.0f)
        {
            // 索引坐标无法直接转换为世界坐标，需要先转换为直角坐标
            // 这里假设网格宽度为1，实际使用时应该通过ToCartesian(width)方法
            Debug.LogWarning("IndexCoordinate.ToWorldPosition: 需要网格宽度信息，建议先转换为CartesianCoordinate");
            return Vector3.zero;
        }
        
        /// <summary>
        /// 获取4邻居坐标（上下左右）
        /// 注意：需要提供网格宽度才能正确计算邻居
        /// </summary>
        /// <returns>4邻居坐标列表</returns>
        public List<IGridCoordinate> GetNeighbors4()
        {
            Debug.LogWarning("IndexCoordinate.GetNeighbors4: 需要网格宽度信息，建议先转换为CartesianCoordinate");
            return new List<IGridCoordinate>();
        }
        
        /// <summary>
        /// 获取8邻居坐标（包含对角线）
        /// 注意：需要提供网格宽度才能正确计算邻居
        /// </summary>
        /// <returns>8邻居坐标列表</returns>
        public List<IGridCoordinate> GetNeighbors8()
        {
            Debug.LogWarning("IndexCoordinate.GetNeighbors8: 需要网格宽度信息，建议先转换为CartesianCoordinate");
            return new List<IGridCoordinate>();
        }
        
        /// <summary>
        /// 计算到另一个坐标的曼哈顿距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>曼哈顿距离</returns>
        public int ManhattanDistanceTo(IGridCoordinate other)
        {
            Debug.LogWarning("IndexCoordinate.ManhattanDistanceTo: 需要网格宽度信息，建议先转换为CartesianCoordinate");
            return 0;
        }
        
        /// <summary>
        /// 计算到另一个坐标的欧几里得距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>欧几里得距离</returns>
        public float EuclideanDistanceTo(IGridCoordinate other)
        {
            Debug.LogWarning("IndexCoordinate.EuclideanDistanceTo: 需要网格宽度信息，建议先转换为CartesianCoordinate");
            return 0f;
        }
        
        /// <summary>
        /// 计算到另一个坐标的切比雪夫距离（棋盘距离）
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>切比雪夫距离</returns>
        public int ChebyshevDistanceTo(IGridCoordinate other)
        {
            Debug.LogWarning("IndexCoordinate.ChebyshevDistanceTo: 需要网格宽度信息，建议先转换为CartesianCoordinate");
            return 0;
        }
        
        /// <summary>
        /// 转换为直角坐标
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <returns>直角坐标</returns>
        public CartesianCoordinate ToCartesian()
        {
            Debug.LogWarning("IndexCoordinate.ToCartesian: 需要网格宽度信息，请使用ToCartesian(width)方法");
            return CartesianCoordinate.Zero;
        }
        
        /// <summary>
        /// 转换为索引坐标（返回自身）
        /// </summary>
        /// <param name="width">网格宽度（此处忽略）</param>
        /// <returns>索引坐标</returns>
        public IndexCoordinate ToIndex(int width)
        {
            return this;
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
            Debug.LogWarning("IndexCoordinate.IsWithinBounds: 需要网格宽度信息，建议先转换为CartesianCoordinate");
            return false;
        }
        
        /// <summary>
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            return index >= 0;
        }
        
        #endregion
        
        #region 扩展方法（需要网格宽度）
        
        /// <summary>
        /// 转换为直角坐标（需要网格宽度）
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <returns>直角坐标</returns>
        public CartesianCoordinate ToCartesian(int width)
        {
            if (width <= 0)
            {
                Debug.LogError("网格宽度必须大于0");
                return CartesianCoordinate.Zero;
            }
            
            int x = index % width;
            int y = index / width;
            return new CartesianCoordinate(x, y);
        }
        
        /// <summary>
        /// 转换为世界坐标位置（需要网格宽度）
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <returns>世界坐标位置</returns>
        public Vector3 ToWorldPosition(int width, float cellSize = 1.0f)
        {
            var cartesian = ToCartesian(width);
            return cartesian.ToWorldPosition(cellSize);
        }
        
        /// <summary>
        /// 获取4邻居坐标（需要网格宽度）
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <param name="height">网格高度（用于边界检查）</param>
        /// <returns>4邻居坐标列表</returns>
        public List<IndexCoordinate> GetNeighbors4(int width, int height)
        {
            var neighbors = new List<IndexCoordinate>();
            var cartesian = ToCartesian(width);
            
            foreach (var direction in GridConstants.DIRECTIONS_4)
            {
                var neighborCartesian = new CartesianCoordinate(
                    cartesian.X + direction.x, 
                    cartesian.Y + direction.y
                );
                
                // 检查边界
                if (neighborCartesian.IsWithinBounds(0, 0, width - 1, height - 1))
                {
                    neighbors.Add(neighborCartesian.ToIndex(width));
                }
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 获取8邻居坐标（需要网格宽度）
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <param name="height">网格高度（用于边界检查）</param>
        /// <returns>8邻居坐标列表</returns>
        public List<IndexCoordinate> GetNeighbors8(int width, int height)
        {
            var neighbors = new List<IndexCoordinate>();
            var cartesian = ToCartesian(width);
            
            foreach (var direction in GridConstants.DIRECTIONS_8)
            {
                var neighborCartesian = new CartesianCoordinate(
                    cartesian.X + direction.x, 
                    cartesian.Y + direction.y
                );
                
                // 检查边界
                if (neighborCartesian.IsWithinBounds(0, 0, width - 1, height - 1))
                {
                    neighbors.Add(neighborCartesian.ToIndex(width));
                }
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 检查坐标是否在指定边界内（需要网格宽度）
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <param name="height">网格高度</param>
        /// <returns>是否在边界内</returns>
        public bool IsWithinBounds(int width, int height)
        {
            return index >= 0 && index < width * height;
        }
        
        #endregion
        
        #region 运算符重载
        
        /// <summary>
        /// 索引加法
        /// </summary>
        public static IndexCoordinate operator +(IndexCoordinate a, int offset)
        {
            return new IndexCoordinate(a.index + offset);
        }
        
        /// <summary>
        /// 索引减法
        /// </summary>
        public static IndexCoordinate operator -(IndexCoordinate a, int offset)
        {
            return new IndexCoordinate(a.index - offset);
        }
        
        /// <summary>
        /// 索引相等比较
        /// </summary>
        public static bool operator ==(IndexCoordinate a, IndexCoordinate b)
        {
            return a.index == b.index;
        }
        
        /// <summary>
        /// 索引不等比较
        /// </summary>
        public static bool operator !=(IndexCoordinate a, IndexCoordinate b)
        {
            return a.index != b.index;
        }
        
        /// <summary>
        /// 隐式转换为int
        /// </summary>
        public static implicit operator int(IndexCoordinate coord)
        {
            return coord.index;
        }
        
        /// <summary>
        /// 隐式转换从int
        /// </summary>
        public static implicit operator IndexCoordinate(int index)
        {
            return new IndexCoordinate(index);
        }
        
        #endregion
        
        #region 静态方法
        
        /// <summary>
        /// 零索引
        /// </summary>
        public static IndexCoordinate Zero => new IndexCoordinate(0);
        
        /// <summary>
        /// 从直角坐标创建索引坐标
        /// </summary>
        /// <param name="cartesian">直角坐标</param>
        /// <param name="width">网格宽度</param>
        /// <returns>索引坐标</returns>
        public static IndexCoordinate FromCartesian(CartesianCoordinate cartesian, int width)
        {
            return new IndexCoordinate(cartesian.Y * width + cartesian.X);
        }
        
        /// <summary>
        /// 从世界坐标创建索引坐标
        /// </summary>
        /// <param name="worldPosition">世界坐标</param>
        /// <param name="width">网格宽度</param>
        /// <param name="cellSize">网格单元大小</param>
        /// <param name="alignment">对齐方式</param>
        /// <returns>索引坐标</returns>
        public static IndexCoordinate FromWorldPosition(Vector3 worldPosition, int width, 
            float cellSize = 1.0f, GridConstants.GridAlignment alignment = GridConstants.GridAlignment.Center)
        {
            var cartesian = CartesianCoordinate.FromWorldPosition(worldPosition, cellSize, alignment);
            return FromCartesian(cartesian, width);
        }
        
        #endregion
        
        #region Object重写
        
        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return index.GetHashCode();
        }
        
        /// <summary>
        /// 检查相等性
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is IndexCoordinate other)
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
        public bool Equals(IndexCoordinate other)
        {
            return index == other.index;
        }
        
        /// <summary>
        /// 获取字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"IndexCoordinate({index})";
        }
        
        #endregion
    }
}