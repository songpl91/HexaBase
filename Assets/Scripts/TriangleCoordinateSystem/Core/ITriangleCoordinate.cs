using System.Collections.Generic;
using TriangleCoordinateSystem.Coordinates;
using UnityEngine;

namespace TriangleCoordinateSystem.Core
{
    /// <summary>
    /// 三角形网格坐标系统统一接口
    /// 为所有坐标系统提供统一的操作接口，便于不同坐标系统之间的转换和使用
    /// </summary>
    public interface ITriangleCoordinate
    {
        /// <summary>
        /// 转换为世界坐标位置
        /// </summary>
        /// <param name="triangleSize">三角形的边长</param>
        /// <returns>世界坐标位置</returns>
        Vector3 ToWorldPosition(float triangleSize = 1.0f);
        
        /// <summary>
        /// 获取所有相邻的三角形坐标（每个三角形有3个邻居）
        /// </summary>
        /// <returns>相邻坐标列表</returns>
        List<ITriangleCoordinate> GetNeighbors();
        
        /// <summary>
        /// 获取顶点相邻的三角形坐标（每个顶点连接12个三角形）
        /// </summary>
        /// <returns>顶点相邻坐标列表</returns>
        List<ITriangleCoordinate> GetVertexNeighbors();
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        int DistanceTo(ITriangleCoordinate other);
        
        /// <summary>
        /// 转换为轴向坐标（作为标准转换格式）
        /// </summary>
        /// <returns>轴向坐标</returns>
        TriangleAxialCoordinate ToAxial();
        
        /// <summary>
        /// 转换为立方坐标（用于高效计算）
        /// </summary>
        /// <returns>立方坐标</returns>
        TriangleCubeCoordinate ToCube();
        
        /// <summary>
        /// 获取三角形的朝向（向上或向下）
        /// </summary>
        /// <returns>true表示向上，false表示向下</returns>
        bool IsUpward();
        
        /// <summary>
        /// 检查坐标是否有效
        /// </summary>
        /// <returns>是否为有效坐标</returns>
        bool IsValid();
        
        /// <summary>
        /// 获取坐标的哈希值（用于字典等集合）
        /// </summary>
        /// <returns>哈希值</returns>
        int GetHashCode();
        
        /// <summary>
        /// 检查两个坐标是否相等
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>是否相等</returns>
        bool Equals(object obj);
        
        /// <summary>
        /// 获取坐标的字符串表示
        /// </summary>
        /// <returns>字符串表示</returns>
        string ToString();
    }
}