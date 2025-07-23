using System.Collections.Generic;
using HexCoordinateSystem.Coordinates;
using UnityEngine;

namespace HexCoordinateSystem.Core
{
    /// <summary>
    /// 六边形坐标系统统一接口
    /// 为所有坐标系统提供统一的操作接口，便于不同坐标系统之间的转换和使用
    /// </summary>
    public interface IHexCoordinate
    {
        /// <summary>
        /// 转换为世界坐标位置
        /// </summary>
        /// <param name="hexSize">六边形的大小（半径）</param>
        /// <returns>世界坐标位置</returns>
        Vector3 ToWorldPosition(float hexSize = 1.0f);
        
        /// <summary>
        /// 获取所有相邻的六边形坐标
        /// </summary>
        /// <returns>相邻坐标列表</returns>
        List<IHexCoordinate> GetNeighbors();
        
        /// <summary>
        /// 计算到另一个坐标的距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>距离值</returns>
        int DistanceTo(IHexCoordinate other);
        
        /// <summary>
        /// 转换为轴向坐标（作为标准转换格式）
        /// </summary>
        /// <returns>轴向坐标</returns>
        AxialCoordinate ToAxial();
        
        /// <summary>
        /// 转换为立方坐标（用于高效计算）
        /// </summary>
        /// <returns>立方坐标</returns>
        CubeCoordinate ToCube();
        
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