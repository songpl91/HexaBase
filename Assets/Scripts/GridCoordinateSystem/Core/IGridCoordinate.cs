using System.Collections.Generic;
using GridCoordinateSystem.Coordinates;
using UnityEngine;

namespace GridCoordinateSystem.Core
{
    /// <summary>
    /// 四边形网格坐标系统统一接口
    /// 为所有坐标系统提供统一的操作接口，便于不同坐标系统之间的转换和使用
    /// </summary>
    public interface IGridCoordinate
    {
        /// <summary>
        /// 转换为世界坐标位置
        /// </summary>
        /// <param name="cellSize">网格单元大小</param>
        /// <returns>世界坐标位置</returns>
        Vector3 ToWorldPosition(float cellSize = 1.0f);
        
        /// <summary>
        /// 获取4邻居坐标（上下左右）
        /// </summary>
        /// <returns>4邻居坐标列表</returns>
        List<IGridCoordinate> GetNeighbors4();
        
        /// <summary>
        /// 获取8邻居坐标（包含对角线）
        /// </summary>
        /// <returns>8邻居坐标列表</returns>
        List<IGridCoordinate> GetNeighbors8();
        
        /// <summary>
        /// 计算到另一个坐标的曼哈顿距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>曼哈顿距离</returns>
        int ManhattanDistanceTo(IGridCoordinate other);
        
        /// <summary>
        /// 计算到另一个坐标的欧几里得距离
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>欧几里得距离</returns>
        float EuclideanDistanceTo(IGridCoordinate other);
        
        /// <summary>
        /// 计算到另一个坐标的切比雪夫距离（棋盘距离）
        /// </summary>
        /// <param name="other">目标坐标</param>
        /// <returns>切比雪夫距离</returns>
        int ChebyshevDistanceTo(IGridCoordinate other);
        
        /// <summary>
        /// 转换为直角坐标（作为标准转换格式）
        /// </summary>
        /// <returns>直角坐标</returns>
        CartesianCoordinate ToCartesian();
        
        /// <summary>
        /// 转换为索引坐标（用于数组访问）
        /// </summary>
        /// <param name="width">网格宽度</param>
        /// <returns>索引坐标</returns>
        IndexCoordinate ToIndex(int width);
        
        /// <summary>
        /// 检查坐标是否在指定边界内
        /// </summary>
        /// <param name="minX">最小X坐标</param>
        /// <param name="minY">最小Y坐标</param>
        /// <param name="maxX">最大X坐标</param>
        /// <param name="maxY">最大Y坐标</param>
        /// <returns>是否在边界内</returns>
        bool IsWithinBounds(int minX, int minY, int maxX, int maxY);
        
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