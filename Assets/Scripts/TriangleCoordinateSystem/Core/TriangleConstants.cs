using UnityEngine;

namespace TriangleCoordinateSystem.Core
{
    /// <summary>
    /// 三角形网格系统常量定义
    /// 包含三角形网格计算所需的数学常量和配置参数
    /// </summary>
    public static class TriangleConstants
    {
        /// <summary>
        /// 浮点数比较精度
        /// </summary>
        public const float EPSILON = 0.0001f;
        
        /// <summary>
        /// 等边三角形的高度系数（sqrt(3)/2）
        /// </summary>
        public const float TRIANGLE_HEIGHT_RATIO = 0.866025403784f;
        
        /// <summary>
        /// 三角形网格的行间距系数（3/4 * height）
        /// </summary>
        public const float ROW_SPACING_RATIO = 0.649519052838f;
        
        /// <summary>
        /// 三角形网格的列间距（边长的一半）
        /// </summary>
        public const float COLUMN_SPACING_RATIO = 0.5f;
        
        /// <summary>
        /// 60度角的弧度值
        /// </summary>
        public const float ANGLE_60_RAD = Mathf.PI / 3f;
        
        /// <summary>
        /// 120度角的弧度值
        /// </summary>
        public const float ANGLE_120_RAD = 2f * Mathf.PI / 3f;
        
        /// <summary>
        /// 三角形的边数
        /// </summary>
        public const int TRIANGLE_SIDES = 3;
        
        /// <summary>
        /// 每个三角形的邻居数量
        /// </summary>
        public const int NEIGHBOR_COUNT = 3;
        
        /// <summary>
        /// 每个顶点连接的三角形数量
        /// </summary>
        public const int VERTEX_NEIGHBOR_COUNT = 12;
        
        /// <summary>
        /// 默认三角形大小
        /// </summary>
        public const float DEFAULT_TRIANGLE_SIZE = 1.0f;
        
        /// <summary>
        /// 最大缓存大小
        /// </summary>
        public const int MAX_CACHE_SIZE = 1000;
        
        /// <summary>
        /// 对象池初始大小
        /// </summary>
        public const int INITIAL_POOL_SIZE = 100;
        
        /// <summary>
        /// 对象池最大大小
        /// </summary>
        public const int MAX_POOL_SIZE = 500;
    }
    
    /// <summary>
    /// 三角形朝向枚举
    /// </summary>
    public enum TriangleOrientation
    {
        /// <summary>
        /// 向上的三角形（顶点在上）
        /// </summary>
        Upward = 0,
        
        /// <summary>
        /// 向下的三角形（顶点在下）
        /// </summary>
        Downward = 1
    }
    
    /// <summary>
    /// 三角形邻居方向枚举
    /// </summary>
    public enum TriangleDirection
    {
        /// <summary>
        /// 右侧邻居
        /// </summary>
        Right = 0,
        
        /// <summary>
        /// 左下邻居（对于向上三角形）或左上邻居（对于向下三角形）
        /// </summary>
        LeftDown = 1,
        
        /// <summary>
        /// 右下邻居（对于向上三角形）或右上邻居（对于向下三角形）
        /// </summary>
        RightDown = 2
    }
}