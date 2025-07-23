using UnityEngine;

namespace HexCoordinateSystem.Core
{
    /// <summary>
    /// 六边形坐标系统常量定义
    /// 包含预计算的数学常量、方向向量等，提高运算性能
    /// </summary>
    public static class HexConstants
    {
        #region 数学常量
        
        /// <summary>
        /// 根号3的值
        /// </summary>
        public static readonly float SQRT3 = Mathf.Sqrt(3.0f);
        
        /// <summary>
        /// 根号3除以2的值
        /// </summary>
        public static readonly float SQRT3_DIV_2 = SQRT3 / 2.0f;
        
        /// <summary>
        /// 3除以2的值
        /// </summary>
        public static readonly float THREE_DIV_2 = 3.0f / 2.0f;
        
        /// <summary>
        /// 2除以3的值
        /// </summary>
        public static readonly float TWO_DIV_3 = 2.0f / 3.0f;
        
        /// <summary>
        /// 1除以3的值
        /// </summary>
        public static readonly float ONE_DIV_3 = 1.0f / 3.0f;
        
        #endregion
        
        #region 方向常量
        
        /// <summary>
        /// 六边形的六个方向（轴向坐标系统）
        /// 顺序：右、右上、左上、左、左下、右下
        /// </summary>
        public static readonly Vector2Int[] AXIAL_DIRECTIONS = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // 右
            new Vector2Int(1, -1),  // 右上
            new Vector2Int(0, -1),  // 左上
            new Vector2Int(-1, 0),  // 左
            new Vector2Int(-1, 1),  // 左下
            new Vector2Int(0, 1)    // 右下
        };
        
        /// <summary>
        /// 六边形的六个方向（立方坐标系统）
        /// 顺序：右、右上、左上、左、左下、右下
        /// </summary>
        public static readonly Vector3Int[] CUBE_DIRECTIONS = new Vector3Int[]
        {
            new Vector3Int(1, -1, 0),   // 右
            new Vector3Int(1, 0, -1),   // 右上
            new Vector3Int(0, 1, -1),   // 左上
            new Vector3Int(-1, 1, 0),   // 左
            new Vector3Int(-1, 0, 1),   // 左下
            new Vector3Int(0, -1, 1)    // 右下
        };
        
        /// <summary>
        /// 奇数列偏移坐标的方向向量
        /// [0]为偶数列方向，[1]为奇数列方向
        /// </summary>
        public static readonly Vector2Int[,] OFFSET_ODD_Q_DIRECTIONS = new Vector2Int[2, 6]
        {
            // 偶数列方向
            {
                new Vector2Int(1, 0),   // 右
                new Vector2Int(1, -1),  // 右上
                new Vector2Int(0, -1),  // 左上
                new Vector2Int(-1, 0),  // 左
                new Vector2Int(-1, -1), // 左下
                new Vector2Int(0, 0)    // 右下（相对当前位置）
            },
            // 奇数列方向
            {
                new Vector2Int(1, 0),   // 右
                new Vector2Int(1, 1),   // 右上
                new Vector2Int(0, 1),   // 左上
                new Vector2Int(-1, 0),  // 左
                new Vector2Int(-1, 1),  // 左下
                new Vector2Int(0, 0)    // 右下（相对当前位置）
            }
        };
        
        /// <summary>
        /// 偶数列偏移坐标的方向向量
        /// [0]为偶数列方向，[1]为奇数列方向
        /// </summary>
        public static readonly Vector2Int[,] OFFSET_EVEN_Q_DIRECTIONS = new Vector2Int[2, 6]
        {
            // 偶数列方向
            {
                new Vector2Int(1, 0),   // 右
                new Vector2Int(1, 1),   // 右上
                new Vector2Int(0, 1),   // 左上
                new Vector2Int(-1, 0),  // 左
                new Vector2Int(-1, 1),  // 左下
                new Vector2Int(0, 0)    // 右下（相对当前位置）
            },
            // 奇数列方向
            {
                new Vector2Int(1, 0),   // 右
                new Vector2Int(1, -1),  // 右上
                new Vector2Int(0, -1),  // 左上
                new Vector2Int(-1, 0),  // 左
                new Vector2Int(-1, -1), // 左下
                new Vector2Int(0, 0)    // 右下（相对当前位置）
            }
        };
        
        #endregion
        
        #region 布局常量
        
        /// <summary>
        /// 尖顶六边形布局（Pointy-top hexagon）
        /// </summary>
        public static readonly HexLayout POINTY_TOP_LAYOUT = new HexLayout(
            new Vector2(SQRT3, SQRT3_DIV_2),      // 正向变换矩阵
            new Vector2(0.0f, THREE_DIV_2),
            new Vector2(SQRT3_DIV_3, -ONE_DIV_3), // 逆向变换矩阵
            new Vector2(0.0f, TWO_DIV_3),
            0.5f                                   // 起始角度
        );
        
        /// <summary>
        /// 平顶六边形布局（Flat-top hexagon）
        /// </summary>
        public static readonly HexLayout FLAT_TOP_LAYOUT = new HexLayout(
            new Vector2(THREE_DIV_2, 0.0f),       // 正向变换矩阵
            new Vector2(SQRT3_DIV_2, SQRT3),
            new Vector2(TWO_DIV_3, 0.0f),         // 逆向变换矩阵
            new Vector2(-ONE_DIV_3, SQRT3_DIV_3),
            0.0f                                   // 起始角度
        );
        
        private static readonly float SQRT3_DIV_3 = SQRT3 / 3.0f;
        
        #endregion
        
        #region 验证常量
        
        /// <summary>
        /// 浮点数比较的精度阈值
        /// </summary>
        public const float EPSILON = 1e-6f;
        
        /// <summary>
        /// 最大有效坐标值（防止溢出）
        /// </summary>
        public const int MAX_COORDINATE_VALUE = 10000;
        
        /// <summary>
        /// 最小有效坐标值（防止溢出）
        /// </summary>
        public const int MIN_COORDINATE_VALUE = -10000;
        
        #endregion
    }
    
    /// <summary>
    /// 六边形布局信息
    /// 定义六边形在世界坐标中的排列方式
    /// </summary>
    public struct HexLayout
    {
        /// <summary>
        /// 正向变换矩阵第一行
        /// </summary>
        public readonly Vector2 ForwardMatrix1;
        
        /// <summary>
        /// 正向变换矩阵第二行
        /// </summary>
        public readonly Vector2 ForwardMatrix2;
        
        /// <summary>
        /// 逆向变换矩阵第一行
        /// </summary>
        public readonly Vector2 InverseMatrix1;
        
        /// <summary>
        /// 逆向变换矩阵第二行
        /// </summary>
        public readonly Vector2 InverseMatrix2;
        
        /// <summary>
        /// 起始角度（弧度）
        /// </summary>
        public readonly float StartAngle;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="forwardMatrix1">正向变换矩阵第一行</param>
        /// <param name="forwardMatrix2">正向变换矩阵第二行</param>
        /// <param name="inverseMatrix1">逆向变换矩阵第一行</param>
        /// <param name="inverseMatrix2">逆向变换矩阵第二行</param>
        /// <param name="startAngle">起始角度</param>
        public HexLayout(Vector2 forwardMatrix1, Vector2 forwardMatrix2, 
                        Vector2 inverseMatrix1, Vector2 inverseMatrix2, 
                        float startAngle)
        {
            ForwardMatrix1 = forwardMatrix1;
            ForwardMatrix2 = forwardMatrix2;
            InverseMatrix1 = inverseMatrix1;
            InverseMatrix2 = inverseMatrix2;
            StartAngle = startAngle;
        }
    }
}