using UnityEngine;

namespace GridCoordinateSystem.Core
{
    /// <summary>
    /// 四边形网格系统常量定义
    /// 包含网格计算中使用的各种常量和配置参数
    /// </summary>
    public static class GridConstants
    {
        #region 距离计算常量
        
        /// <summary>
        /// 浮点数比较精度
        /// </summary>
        public const float EPSILON = 0.0001f;
        
        /// <summary>
        /// 对角线距离系数（√2）
        /// </summary>
        public const float DIAGONAL_DISTANCE = 1.414213562f;
        
        #endregion
        
        #region 方向常量
        
        /// <summary>
        /// 4方向偏移量（上、右、下、左）
        /// </summary>
        public static readonly Vector2Int[] DIRECTIONS_4 = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // 上
            new Vector2Int(1, 0),   // 右
            new Vector2Int(0, -1),  // 下
            new Vector2Int(-1, 0)   // 左
        };
        
        /// <summary>
        /// 8方向偏移量（包含对角线）
        /// </summary>
        public static readonly Vector2Int[] DIRECTIONS_8 = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // 上
            new Vector2Int(1, 1),   // 右上
            new Vector2Int(1, 0),   // 右
            new Vector2Int(1, -1),  // 右下
            new Vector2Int(0, -1),  // 下
            new Vector2Int(-1, -1), // 左下
            new Vector2Int(-1, 0),  // 左
            new Vector2Int(-1, 1)   // 左上
        };
        
        #endregion
        
        #region 方向枚举
        
        /// <summary>
        /// 4方向枚举
        /// </summary>
        public enum Direction4
        {
            Up = 0,     // 上
            Right = 1,  // 右
            Down = 2,   // 下
            Left = 3    // 左
        }
        
        /// <summary>
        /// 8方向枚举
        /// </summary>
        public enum Direction8
        {
            Up = 0,         // 上
            UpRight = 1,    // 右上
            Right = 2,      // 右
            DownRight = 3,  // 右下
            Down = 4,       // 下
            DownLeft = 5,   // 左下
            Left = 6,       // 左
            UpLeft = 7      // 左上
        }
        
        #endregion
        
        #region 网格类型
        
        /// <summary>
        /// 网格对齐方式
        /// </summary>
        public enum GridAlignment
        {
            Center,     // 中心对齐
            Corner      // 角落对齐
        }
        
        /// <summary>
        /// 距离计算类型
        /// </summary>
        public enum DistanceType
        {
            Manhattan,   // 曼哈顿距离
            Euclidean,   // 欧几里得距离
            Chebyshev    // 切比雪夫距离
        }
        
        #endregion
        
        #region 性能配置
        
        /// <summary>
        /// 默认对象池大小
        /// </summary>
        public const int DEFAULT_POOL_SIZE = 100;
        
        /// <summary>
        /// 默认缓存大小
        /// </summary>
        public const int DEFAULT_CACHE_SIZE = 1000;
        
        /// <summary>
        /// 最大批量操作数量
        /// </summary>
        public const int MAX_BATCH_SIZE = 10000;
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 获取4方向的偏移量
        /// </summary>
        /// <param name="direction">方向</param>
        /// <returns>偏移量</returns>
        public static Vector2Int GetDirection4(Direction4 direction)
        {
            return DIRECTIONS_4[(int)direction];
        }
        
        /// <summary>
        /// 获取8方向的偏移量
        /// </summary>
        /// <param name="direction">方向</param>
        /// <returns>偏移量</returns>
        public static Vector2Int GetDirection8(Direction8 direction)
        {
            return DIRECTIONS_8[(int)direction];
        }
        
        /// <summary>
        /// 获取相反方向（4方向）
        /// </summary>
        /// <param name="direction">原方向</param>
        /// <returns>相反方向</returns>
        public static Direction4 GetOppositeDirection4(Direction4 direction)
        {
            return (Direction4)(((int)direction + 2) % 4);
        }
        
        /// <summary>
        /// 获取相反方向（8方向）
        /// </summary>
        /// <param name="direction">原方向</param>
        /// <returns>相反方向</returns>
        public static Direction8 GetOppositeDirection8(Direction8 direction)
        {
            return (Direction8)(((int)direction + 4) % 8);
        }
        
        /// <summary>
        /// 检查浮点数是否近似相等
        /// </summary>
        /// <param name="a">数值a</param>
        /// <param name="b">数值b</param>
        /// <returns>是否近似相等</returns>
        public static bool Approximately(float a, float b)
        {
            return Mathf.Abs(a - b) < EPSILON;
        }
        
        #endregion
    }
}