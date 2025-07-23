using System;
using System.Collections.Generic;
using UnityEngine;
using TriangleCoordinateSystem.Core;

namespace TriangleCoordinateSystem.Coordinates
{
    /// <summary>
    /// 三角形轴向坐标系统
    /// 使用(q, r)坐标表示三角形网格中的位置
    /// q表示列坐标，r表示行坐标
    /// </summary>
    [Serializable]
    public struct TriangleAxialCoordinate : ITriangleCoordinate, IEquatable<TriangleAxialCoordinate>
    {
        [SerializeField] private int q; // 列坐标
        [SerializeField] private int r; // 行坐标
        
        /// <summary>
        /// 列坐标（Q轴）
        /// </summary>
        public int Q => q;
        
        /// <summary>
        /// 行坐标（R轴）
        /// </summary>
        public int R => r;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="q">列坐标</param>
        /// <param name="r">行坐标</param>
        public TriangleAxialCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
        
        /// <summary>
        /// 转换为世界坐标位置
        /// </summary>
        /// <param name="triangleSize">三角形边长</param>
        /// <returns>世界坐标位置</returns>
        public Vector3 ToWorldPosition(float triangleSize = 1.0f)
        {
            // 计算基础位置
            float x = q * triangleSize * TriangleConstants.COLUMN_SPACING_RATIO;
            float y = r * triangleSize * TriangleConstants.ROW_SPACING_RATIO;
            
            // 根据三角形朝向调整位置
            if (!IsUpward())
            {
                x += triangleSize * TriangleConstants.COLUMN_SPACING_RATIO;
            }
            
            return new Vector3(x, y, 0);
        }
        
        /// <summary>
        /// 获取相邻的三角形坐标
        /// </summary>
        /// <returns>相邻坐标列表</returns>
        public List<ITriangleCoordinate> GetNeighbors()
        {
            var neighbors = new List<ITriangleCoordinate>();
            
            if (IsUpward())
            {
                // 向上三角形的邻居
                neighbors.Add(new TriangleAxialCoordinate(q + 1, r));     // 右邻居
                neighbors.Add(new TriangleAxialCoordinate(q, r + 1));     // 左下邻居
                neighbors.Add(new TriangleAxialCoordinate(q + 1, r + 1)); // 右下邻居
            }
            else
            {
                // 向下三角形的邻居
                neighbors.Add(new TriangleAxialCoordinate(q - 1, r));     // 左邻居
                neighbors.Add(new TriangleAxialCoordinate(q, r - 1));     // 左上邻居
                neighbors.Add(new TriangleAxialCoordinate(q - 1, r - 1)); // 右上邻居
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 获取顶点相邻的三角形坐标
        /// </summary>
        /// <returns>顶点相邻坐标列表</returns>
        public List<ITriangleCoordinate> GetVertexNeighbors()
        {
            var vertexNeighbors = new List<ITriangleCoordinate>();
            
            // 添加边邻居
            vertexNeighbors.AddRange(GetNeighbors());
            
            if (IsUpward())
            {
                // 向上三角形的顶点邻居
                vertexNeighbors.Add(new TriangleAxialCoordinate(q - 1, r));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q + 2, r));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q - 1, r + 1));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q + 2, r + 1));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q, r + 2));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q + 1, r + 2));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q - 1, r - 1));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q + 2, r - 1));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q + 1, r - 1));
            }
            else
            {
                // 向下三角形的顶点邻居
                vertexNeighbors.Add(new TriangleAxialCoordinate(q + 1, r));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q - 2, r));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q + 1, r - 1));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q - 2, r - 1));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q, r - 2));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q - 1, r - 2));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q + 1, r + 1));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q - 2, r + 1));
                vertexNeighbors.Add(new TriangleAxialCoordinate(q - 1, r + 1));
            }
            
            return vertexNeighbors;
        }
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        public int DistanceTo(ITriangleCoordinate other)
        {
            var otherAxial = other.ToAxial();
            
            // 转换为立方坐标计算距离
            var thisCube = ToCube();
            var otherCube = otherAxial.ToCube();
            
            return thisCube.DistanceTo(otherCube);
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        /// <returns>轴向坐标</returns>
        public TriangleAxialCoordinate ToAxial()
        {
            return this;
        }
        
        /// <summary>
        /// 转换为立方坐标
        /// </summary>
        /// <returns>立方坐标</returns>
        public TriangleCubeCoordinate ToCube()
        {
            int x = q;
            int z = r;
            int y = -x - z;
            return new TriangleCubeCoordinate(x, y, z);
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
        /// 转换为偏移坐标
        /// </summary>
        /// <returns>偏移坐标</returns>
        public TriangleOffsetCoordinate ToOffsetCoordinate()
        {
            // 轴向坐标转偏移坐标
            // 偏移坐标的列 = q
            // 偏移坐标的行 = r + (q - (q & 1)) / 2
            int col = q;
            int row = r + (q - (q & 1)) / 2;
            return new TriangleOffsetCoordinate(col, row);
        }
        
        /// <summary>
        /// 获取三角形的朝向
        /// </summary>
        /// <returns>true表示向上，false表示向下</returns>
        public bool IsUpward()
        {
            // 根据坐标奇偶性判断朝向
            return (q + r) % 2 == 0;
        }
        
        /// <summary>
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        public bool IsValid()
        {
            // 三角形坐标总是有效的
            return true;
        }
        
        /// <summary>
        /// 获取坐标的哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return q.GetHashCode() ^ (r.GetHashCode() << 2);
        }
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is TriangleAxialCoordinate other)
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
        public bool Equals(TriangleAxialCoordinate other)
        {
            return q == other.q && r == other.r;
        }
        
        /// <summary>
        /// 获取坐标的字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"TriangleAxial({q}, {r}) [{(IsUpward() ? "Up" : "Down")}]";
        }
        
        /// <summary>
        /// 相等运算符
        /// </summary>
        public static bool operator ==(TriangleAxialCoordinate left, TriangleAxialCoordinate right)
        {
            return left.Equals(right);
        }
        
        /// <summary>
        /// 不等运算符
        /// </summary>
        public static bool operator !=(TriangleAxialCoordinate left, TriangleAxialCoordinate right)
        {
            return !left.Equals(right);
        }
        
        /// <summary>
        /// 加法运算符
        /// </summary>
        public static TriangleAxialCoordinate operator +(TriangleAxialCoordinate left, TriangleAxialCoordinate right)
        {
            return new TriangleAxialCoordinate(left.q + right.q, left.r + right.r);
        }
        
        /// <summary>
        /// 减法运算符
        /// </summary>
        public static TriangleAxialCoordinate operator -(TriangleAxialCoordinate left, TriangleAxialCoordinate right)
        {
            return new TriangleAxialCoordinate(left.q - right.q, left.r - right.r);
        }
    }
}