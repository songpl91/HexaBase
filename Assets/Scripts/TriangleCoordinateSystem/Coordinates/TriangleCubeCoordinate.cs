using System;
using System.Collections.Generic;
using UnityEngine;
using TriangleCoordinateSystem.Core;

namespace TriangleCoordinateSystem.Coordinates
{
    /// <summary>
    /// 三角形立方坐标系统
    /// 使用(x, y, z)坐标表示三角形网格中的位置，其中x + y + z = 0
    /// 立方坐标系统便于进行距离计算和几何运算
    /// </summary>
    [Serializable]
    public struct TriangleCubeCoordinate : ITriangleCoordinate, IEquatable<TriangleCubeCoordinate>
    {
        [SerializeField] private int x;
        [SerializeField] private int y;
        [SerializeField] private int z;
        
        /// <summary>
        /// X坐标
        /// </summary>
        public int X => x;
        
        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y => y;
        
        /// <summary>
        /// Z坐标
        /// </summary>
        public int Z => z;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="z">Z坐标</param>
        public TriangleCubeCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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
            var otherCube = other.ToCube();
            
            // 立方坐标距离计算：max(|dx|, |dy|, |dz|)
            int dx = Mathf.Abs(x - otherCube.x);
            int dy = Mathf.Abs(y - otherCube.y);
            int dz = Mathf.Abs(z - otherCube.z);
            
            return Mathf.Max(dx, Mathf.Max(dy, dz));
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        /// <returns>轴向坐标</returns>
        public TriangleAxialCoordinate ToAxial()
        {
            int q = x;
            int r = z;
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
            return this;
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
            return ToAxial().ToOffsetCoordinate();
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
            // 立方坐标必须满足 x + y + z = 0
            return x + y + z == 0;
        }
        
        /// <summary>
        /// 获取坐标的哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj is TriangleCubeCoordinate other)
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
        public bool Equals(TriangleCubeCoordinate other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        
        /// <summary>
        /// 获取坐标的字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"TriangleCube({x}, {y}, {z}) [{(IsUpward() ? "Up" : "Down")}]";
        }
        
        /// <summary>
        /// 相等运算符
        /// </summary>
        public static bool operator ==(TriangleCubeCoordinate left, TriangleCubeCoordinate right)
        {
            return left.Equals(right);
        }
        
        /// <summary>
        /// 不等运算符
        /// </summary>
        public static bool operator !=(TriangleCubeCoordinate left, TriangleCubeCoordinate right)
        {
            return !left.Equals(right);
        }
        
        /// <summary>
        /// 加法运算符
        /// </summary>
        public static TriangleCubeCoordinate operator +(TriangleCubeCoordinate left, TriangleCubeCoordinate right)
        {
            return new TriangleCubeCoordinate(left.x + right.x, left.y + right.y, left.z + right.z);
        }
        
        /// <summary>
        /// 减法运算符
        /// </summary>
        public static TriangleCubeCoordinate operator -(TriangleCubeCoordinate left, TriangleCubeCoordinate right)
        {
            return new TriangleCubeCoordinate(left.x - right.x, left.y - right.y, left.z - right.z);
        }
        
        /// <summary>
        /// 标量乘法运算符
        /// </summary>
        public static TriangleCubeCoordinate operator *(TriangleCubeCoordinate coord, int scalar)
        {
            return new TriangleCubeCoordinate(coord.x * scalar, coord.y * scalar, coord.z * scalar);
        }
        
        /// <summary>
        /// 标量乘法运算符
        /// </summary>
        public static TriangleCubeCoordinate operator *(int scalar, TriangleCubeCoordinate coord)
        {
            return coord * scalar;
        }
    }
}