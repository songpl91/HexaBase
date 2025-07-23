using System;
using System.Collections.Generic;
using UnityEngine;
using TriangleCoordinateSystem.Core;

namespace TriangleCoordinateSystem.Coordinates
{
    /// <summary>
    /// 三角形偏移坐标系统
    /// 使用(col, row)坐标表示三角形网格中的位置
    /// 类似于传统的二维数组索引，便于理解和存储
    /// </summary>
    [Serializable]
    public struct TriangleOffsetCoordinate : ITriangleCoordinate, IEquatable<TriangleOffsetCoordinate>
    {
        [SerializeField] private int col; // 列坐标
        [SerializeField] private int row; // 行坐标
        
        /// <summary>
        /// 列坐标
        /// </summary>
        public int Col => col;
        
        /// <summary>
        /// 行坐标
        /// </summary>
        public int Row => row;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="col">列坐标</param>
        /// <param name="row">行坐标</param>
        public TriangleOffsetCoordinate(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        
        /// <summary>
        /// 转换为世界坐标位置
        /// </summary>
        /// <param name="triangleSize">三角形边长</param>
        /// <returns>世界坐标位置</returns>
        public Vector3 ToWorldPosition(float triangleSize = 1.0f)
        {
            return ToAxial().ToWorldPosition(triangleSize);
        }
        
        /// <summary>
        /// 获取相邻的三角形坐标
        /// </summary>
        /// <returns>相邻坐标列表</returns>
        public List<ITriangleCoordinate> GetNeighbors()
        {
            return ToAxial().GetNeighbors();
        }
        
        /// <summary>
        /// 获取顶点相邻的三角形坐标
        /// </summary>
        /// <returns>顶点相邻坐标列表</returns>
        public List<ITriangleCoordinate> GetVertexNeighbors()
        {
            return ToAxial().GetVertexNeighbors();
        }
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        public int DistanceTo(ITriangleCoordinate other)
        {
            return ToAxial().DistanceTo(other);
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        /// <returns>轴向坐标</returns>
        public TriangleAxialCoordinate ToAxial()
        {
            // 偏移坐标到轴向坐标的转换
            int q = col - (row - (row & 1)) / 2;
            int r = row;
            return new TriangleAxialCoordinate(q, r);
        }
        
        /// <summary>
        /// 转换为轴向坐标（别名方法）
        /// </summary>
        /// <returns>轴向坐标</returns>
        public TriangleAxialCoordinate ToAxialCoordinate()
        {
            return ToAxial();
        }
        
        /// <summary>
        /// 转换为立方坐标
        /// </summary>
        /// <returns>立方坐标</returns>
        public TriangleCubeCoordinate ToCube()
        {
            return ToAxial().ToCube();
        }
        
        /// <summary>
        /// 转换为立方坐标（别名方法）
        /// </summary>
        /// <returns>立方坐标</returns>
        public TriangleCubeCoordinate ToCubeCoordinate()
        {
            return ToCube();
        }
        
        /// <summary>
        /// 转换为偏移坐标（别名方法）
        /// </summary>
        /// <returns>偏移坐标</returns>
        public TriangleOffsetCoordinate ToOffsetCoordinate()
        {
            return this;
        }
        
        /// <summary>
        /// 获取三角形的朝向
        /// </summary>
        /// <returns>true表示向上，false表示向下</returns>
        public bool IsUpward()
        {
            return ToAxial().IsUpward();
        }
        
        /// <summary>
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            // 偏移坐标总是有效的
            return true;
        }
        
        /// <summary>
        /// 转换为数组索引（用于一维数组存储）
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <returns>数组索引</returns>
        public int ToIndex(int width)
        {
            return row * width + col;
        }
        
        /// <summary>
        /// 从数组索引创建偏移坐标
        /// </summary>
        /// <param name="index">数组索引</param>
        /// <param name="width">网格宽度</param>
        /// <returns>偏移坐标</returns>
        public static TriangleOffsetCoordinate FromIndex(int index, int width)
        {
            int row = index / width;
            int col = index % width;
            return new TriangleOffsetCoordinate(col, row);
        }
        
        /// <summary>
        /// 检查坐标是否在指定边界内
        /// </summary>
        /// <param name="minCol">最小列坐标</param>
        /// <param name="minRow">最小行坐标</param>
        /// <param name="maxCol">最大列坐标</param>
        /// <param name="maxRow">最大行坐标</param>
        /// <returns>是否在边界内</returns>
        public bool IsWithinBounds(int minCol, int minRow, int maxCol, int maxRow)
        {
            return col >= minCol && col <= maxCol && row >= minRow && row <= maxRow;
        }
        
        /// <summary>
        /// 获取坐标的哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return col.GetHashCode() ^ (row.GetHashCode() << 2);
        }
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is TriangleOffsetCoordinate other)
            {
                return Equals(other);
            }
            return false;
        }
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        /// <param name="other">比较坐标</param>
        /// <returns>是否相等</returns>
        public bool Equals(TriangleOffsetCoordinate other)
        {
            return col == other.col && row == other.row;
        }
        
        /// <summary>
        /// 获取坐标的字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"TriangleOffset({col}, {row}) [{(IsUpward() ? "Up" : "Down")}]";
        }
        
        /// <summary>
        /// 相等运算符
        /// </summary>
        public static bool operator ==(TriangleOffsetCoordinate left, TriangleOffsetCoordinate right)
        {
            return left.Equals(right);
        }
        
        /// <summary>
        /// 不等运算符
        /// </summary>
        public static bool operator !=(TriangleOffsetCoordinate left, TriangleOffsetCoordinate right)
        {
            return !left.Equals(right);
        }
        
        /// <summary>
        /// 加法运算符
        /// </summary>
        public static TriangleOffsetCoordinate operator +(TriangleOffsetCoordinate left, TriangleOffsetCoordinate right)
        {
            return new TriangleOffsetCoordinate(left.col + right.col, left.row + right.row);
        }
        
        /// <summary>
        /// 减法运算符
        /// </summary>
        public static TriangleOffsetCoordinate operator -(TriangleOffsetCoordinate left, TriangleOffsetCoordinate right)
        {
            return new TriangleOffsetCoordinate(left.col - right.col, left.row - right.row);
        }
    }
}