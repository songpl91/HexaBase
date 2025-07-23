using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 六边形坐标系统详解与实现
/// 包含所有主要的六边形坐标系统及其转换方法
/// </summary>
public class HexCoordinateSystemsGuide : MonoBehaviour
{
    #region 1. 立方坐标系统 (Cube Coordinates)
    
    /// <summary>
    /// 立方坐标系统 - 最数学化的表示方法
    /// 使用三个坐标 (x, y, z)，满足约束条件：x + y + z = 0
    /// 优点：距离计算简单，旋转操作直观
    /// 缺点：需要三个坐标存储，有冗余信息
    /// </summary>
    [System.Serializable]
    public struct CubeCoordinate
    {
        public int x, y, z;
        
        public CubeCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            
            // 验证约束条件
            if (x + y + z != 0)
            {
                Debug.LogError($"立方坐标约束违反: {x} + {y} + {z} = {x + y + z} ≠ 0");
            }
        }
        
        /// <summary>
        /// 从两个坐标构造第三个坐标
        /// </summary>
        public static CubeCoordinate FromXY(int x, int y)
        {
            return new CubeCoordinate(x, y, -x - y);
        }
        
        /// <summary>
        /// 计算两点之间的距离（曼哈顿距离）
        /// </summary>
        public static int Distance(CubeCoordinate a, CubeCoordinate b)
        {
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
        }
        
        /// <summary>
        /// 获取六个方向的单位向量
        /// </summary>
        public static readonly CubeCoordinate[] Directions = new CubeCoordinate[]
        {
            new CubeCoordinate(1, -1, 0),   // 右
            new CubeCoordinate(1, 0, -1),   // 右上
            new CubeCoordinate(0, 1, -1),   // 左上
            new CubeCoordinate(-1, 1, 0),   // 左
            new CubeCoordinate(-1, 0, 1),   // 左下
            new CubeCoordinate(0, -1, 1)    // 右下
        };
        
        /// <summary>
        /// 获取相邻坐标
        /// </summary>
        public List<CubeCoordinate> GetNeighbors()
        {
            List<CubeCoordinate> neighbors = new List<CubeCoordinate>();
            for (int i = 0; i < Directions.Length; i++)
            {
                CubeCoordinate dir = Directions[i];
                neighbors.Add(new CubeCoordinate(x + dir.x, y + dir.y, z + dir.z));
            }
            return neighbors;
        }
        
        /// <summary>
        /// 顺时针旋转60度
        /// </summary>
        public CubeCoordinate RotateRight()
        {
            return new CubeCoordinate(-z, -x, -y);
        }
        
        /// <summary>
        /// 逆时针旋转60度
        /// </summary>
        public CubeCoordinate RotateLeft()
        {
            return new CubeCoordinate(-y, -z, -x);
        }
        
        public override string ToString()
        {
            return $"Cube({x}, {y}, {z})";
        }
    }
    
    #endregion
    
    #region 2. 轴向坐标系统 (Axial Coordinates)
    
    /// <summary>
    /// 轴向坐标系统 - 立方坐标的简化版本
    /// 使用两个坐标 (q, r)，第三个坐标可以通过 s = -q - r 计算
    /// 优点：存储效率高，计算相对简单
    /// 缺点：某些操作需要转换为立方坐标
    /// </summary>
    [System.Serializable]
    public struct AxialCoordinate
    {
        public int q, r;
        
        public AxialCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
        
        /// <summary>
        /// 转换为立方坐标
        /// </summary>
        public CubeCoordinate ToCube()
        {
            return new CubeCoordinate(q, r, -q - r);
        }
        
        /// <summary>
        /// 从立方坐标转换
        /// </summary>
        public static AxialCoordinate FromCube(CubeCoordinate cube)
        {
            return new AxialCoordinate(cube.x, cube.y);
        }
        
        /// <summary>
        /// 计算距离
        /// </summary>
        public static int Distance(AxialCoordinate a, AxialCoordinate b)
        {
            return CubeCoordinate.Distance(a.ToCube(), b.ToCube());
        }
        
        /// <summary>
        /// 轴向坐标的六个方向
        /// </summary>
        public static readonly AxialCoordinate[] Directions = new AxialCoordinate[]
        {
            new AxialCoordinate(1, 0),   // 右
            new AxialCoordinate(1, -1),  // 右上
            new AxialCoordinate(0, -1),  // 左上
            new AxialCoordinate(-1, 0),  // 左
            new AxialCoordinate(-1, 1),  // 左下
            new AxialCoordinate(0, 1)    // 右下
        };
        
        /// <summary>
        /// 获取相邻坐标
        /// </summary>
        public List<AxialCoordinate> GetNeighbors()
        {
            List<AxialCoordinate> neighbors = new List<AxialCoordinate>();
            for (int i = 0; i < Directions.Length; i++)
            {
                AxialCoordinate dir = Directions[i];
                neighbors.Add(new AxialCoordinate(q + dir.q, r + dir.r));
            }
            return neighbors;
        }
        
        /// <summary>
        /// 转换为世界坐标
        /// </summary>
        public Vector3 ToWorldPosition(float size = 1.0f)
        {
            float x = size * (3.0f / 2.0f * q);
            float y = size * (Mathf.Sqrt(3.0f) / 2.0f * q + Mathf.Sqrt(3.0f) * r);
            return new Vector3(x, y, 0);
        }
        
        public override string ToString()
        {
            return $"Axial({q}, {r})";
        }
    }
    
    #endregion
    
    #region 3. 偏移坐标系统 (Offset Coordinates)
    
    /// <summary>
    /// 偏移坐标系统 - 类似传统的二维数组索引
    /// 分为奇数行偏移(Odd-R)和偶数行偏移(Even-R)
    /// 以及奇数列偏移(Odd-Q)和偶数列偏移(Even-Q)
    /// 优点：直观易懂，类似传统网格
    /// 缺点：邻居计算复杂，需要区分奇偶行/列
    /// </summary>
    
    /// <summary>
    /// 奇数行偏移坐标 (Odd-R Offset)
    /// 奇数行向右偏移半个六边形
    /// </summary>
    [System.Serializable]
    public struct OffsetCoordinateOddR
    {
        public int col, row;
        
        public OffsetCoordinateOddR(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        public AxialCoordinate ToAxial()
        {
            int q = col - (row - (row & 1)) / 2;
            int r = row;
            return new AxialCoordinate(q, r);
        }
        
        /// <summary>
        /// 从轴向坐标转换
        /// </summary>
        public static OffsetCoordinateOddR FromAxial(AxialCoordinate axial)
        {
            int col = axial.q + (axial.r - (axial.r & 1)) / 2;
            int row = axial.r;
            return new OffsetCoordinateOddR(col, row);
        }
        
        /// <summary>
        /// 获取相邻坐标（奇数行偏移）
        /// </summary>
        public List<OffsetCoordinateOddR> GetNeighbors()
        {
            List<OffsetCoordinateOddR> neighbors = new List<OffsetCoordinateOddR>();
            
            // 奇数行和偶数行的邻居偏移不同
            int[,] evenRowOffsets = { {1, 0}, {0, -1}, {-1, -1}, {-1, 0}, {-1, 1}, {0, 1} };
            int[,] oddRowOffsets = { {1, 0}, {1, -1}, {0, -1}, {-1, 0}, {0, 1}, {1, 1} };
            
            int[,] offsets = (row % 2 == 0) ? evenRowOffsets : oddRowOffsets;
            
            for (int i = 0; i < 6; i++)
            {
                int newCol = col + offsets[i, 0];
                int newRow = row + offsets[i, 1];
                neighbors.Add(new OffsetCoordinateOddR(newCol, newRow));
            }
            
            return neighbors;
        }
        
        public override string ToString()
        {
            return $"OffsetOddR({col}, {row})";
        }
    }
    
    /// <summary>
    /// 奇数列偏移坐标 (Odd-Q Offset) - 原项目使用的系统
    /// 奇数列向上偏移半个六边形
    /// </summary>
    [System.Serializable]
    public struct OffsetCoordinateOddQ
    {
        public int q, r;
        
        public OffsetCoordinateOddQ(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        public AxialCoordinate ToAxial()
        {
            int axialQ = q;
            int axialR = r - (q - (q & 1)) / 2;
            return new AxialCoordinate(axialQ, axialR);
        }
        
        /// <summary>
        /// 从轴向坐标转换
        /// </summary>
        public static OffsetCoordinateOddQ FromAxial(AxialCoordinate axial)
        {
            int q = axial.q;
            int r = axial.r + (axial.q - (axial.q & 1)) / 2;
            return new OffsetCoordinateOddQ(q, r);
        }
        
        /// <summary>
        /// 获取相邻坐标（奇数列偏移）- 与原项目一致的实现
        /// </summary>
        public List<OffsetCoordinateOddQ> GetNeighbors()
        {
            List<OffsetCoordinateOddQ> neighbors = new List<OffsetCoordinateOddQ>();
            
            // 基础四个方向：上、下、右、左
            neighbors.Add(new OffsetCoordinateOddQ(q, r + 1));     // 上
            neighbors.Add(new OffsetCoordinateOddQ(q, r - 1));     // 下
            neighbors.Add(new OffsetCoordinateOddQ(q + 1, r));     // 右
            neighbors.Add(new OffsetCoordinateOddQ(q - 1, r));     // 左
            
            // 对角方向（奇偶列不同）
            if (q % 2 != 0)
            {
                // 奇数列
                neighbors.Add(new OffsetCoordinateOddQ(q - 1, r + 1)); // 左上
                neighbors.Add(new OffsetCoordinateOddQ(q + 1, r + 1)); // 右上
            }
            else
            {
                // 偶数列
                neighbors.Add(new OffsetCoordinateOddQ(q - 1, r - 1)); // 左下
                neighbors.Add(new OffsetCoordinateOddQ(q + 1, r - 1)); // 右下
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// 转换为世界坐标（与原项目一致的实现）
        /// </summary>
        public Vector3 ToWorldPosition(float hexSize = 1.0f)
        {
            float hexHeight = Mathf.Sqrt(3.0f) * hexSize;
            
            // 调整R坐标，处理奇偶列偏移
            int adjustedR = r - q / 2;
            float x = 1.5f * hexSize * q;
            float y = hexHeight * (adjustedR + q / 2.0f);
            
            // 处理负数奇数列的特殊情况
            if (q < 0 && q % 2 != 0)
            {
                y += hexHeight;
            }
            
            return new Vector3(x, y, 0);
        }
        
        public override string ToString()
        {
            return $"OffsetOddQ({q}, {r})";
        }
    }
    
    #endregion
    
    #region 4. 双宽坐标系统 (Doubled Coordinates)
    
    /// <summary>
    /// 双宽坐标系统 - 将坐标放大2倍以避免分数
    /// 优点：避免浮点运算，整数计算
    /// 缺点：坐标值较大，某些位置无效
    /// </summary>
    [System.Serializable]
    public struct DoubledCoordinate
    {
        public int col, row;
        
        public DoubledCoordinate(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        
        /// <summary>
        /// 转换为轴向坐标
        /// </summary>
        public AxialCoordinate ToAxial()
        {
            int q = col;
            int r = (row - col) / 2;
            return new AxialCoordinate(q, r);
        }
        
        /// <summary>
        /// 从轴向坐标转换
        /// </summary>
        public static DoubledCoordinate FromAxial(AxialCoordinate axial)
        {
            int col = axial.q;
            int row = 2 * axial.r + axial.q;
            return new DoubledCoordinate(col, row);
        }
        
        public override string ToString()
        {
            return $"Doubled({col}, {row})";
        }
    }
    
    #endregion
    
    #region 坐标系统转换工具
    
    /// <summary>
    /// 坐标系统转换工具类
    /// 提供各种坐标系统之间的转换方法
    /// </summary>
    public static class CoordinateConverter
    {
        /// <summary>
        /// 轴向坐标转立方坐标
        /// </summary>
        public static CubeCoordinate AxialToCube(AxialCoordinate axial)
        {
            return new CubeCoordinate(axial.q, axial.r, -axial.q - axial.r);
        }
        
        /// <summary>
        /// 立方坐标转轴向坐标
        /// </summary>
        public static AxialCoordinate CubeToAxial(CubeCoordinate cube)
        {
            return new AxialCoordinate(cube.x, cube.y);
        }
        
        /// <summary>
        /// 轴向坐标转奇数列偏移坐标
        /// </summary>
        public static OffsetCoordinateOddQ AxialToOffsetOddQ(AxialCoordinate axial)
        {
            return OffsetCoordinateOddQ.FromAxial(axial);
        }
        
        /// <summary>
        /// 奇数列偏移坐标转轴向坐标
        /// </summary>
        public static AxialCoordinate OffsetOddQToAxial(OffsetCoordinateOddQ offset)
        {
            return offset.ToAxial();
        }
        
        /// <summary>
        /// 世界坐标转轴向坐标（浮点到整数的转换）
        /// </summary>
        public static AxialCoordinate WorldToAxial(Vector3 worldPos, float hexSize = 1.0f)
        {
            float q = (2.0f / 3.0f * worldPos.x) / hexSize;
            float r = (-1.0f / 3.0f * worldPos.x + Mathf.Sqrt(3.0f) / 3.0f * worldPos.y) / hexSize;
            
            return AxialRound(q, r);
        }
        
        /// <summary>
        /// 轴向坐标的四舍五入
        /// </summary>
        private static AxialCoordinate AxialRound(float q, float r)
        {
            return CubeToAxial(CubeRound(q, r, -q - r));
        }
        
        /// <summary>
        /// 立方坐标的四舍五入
        /// </summary>
        private static CubeCoordinate CubeRound(float x, float y, float z)
        {
            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);
            
            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);
            
            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }
            
            return new CubeCoordinate(rx, ry, rz);
        }
    }
    
    #endregion
    
    #region 使用示例和测试
    
    [Header("坐标系统测试")]
    public bool runTests = false;
    
    void Start()
    {
        if (runTests)
        {
            RunCoordinateSystemTests();
        }
    }
    
    /// <summary>
    /// 运行坐标系统测试
    /// </summary>
    private void RunCoordinateSystemTests()
    {
        Debug.Log("=== 六边形坐标系统测试 ===");
        
        // 测试坐标转换
        TestCoordinateConversions();
        
        // 测试距离计算
        TestDistanceCalculations();
        
        // 测试邻居查找
        TestNeighborFinding();
        
        // 测试旋转操作
        TestRotationOperations();
    }
    
    /// <summary>
    /// 测试坐标转换
    /// </summary>
    private void TestCoordinateConversions()
    {
        Debug.Log("--- 坐标转换测试 ---");
        
        // 创建测试坐标
        AxialCoordinate axial = new AxialCoordinate(2, 1);
        Debug.Log($"原始轴向坐标: {axial}");
        
        // 转换为其他坐标系统
        CubeCoordinate cube = axial.ToCube();
        OffsetCoordinateOddQ offsetOddQ = OffsetCoordinateOddQ.FromAxial(axial);
        DoubledCoordinate doubled = DoubledCoordinate.FromAxial(axial);
        
        Debug.Log($"立方坐标: {cube}");
        Debug.Log($"奇数列偏移坐标: {offsetOddQ}");
        Debug.Log($"双宽坐标: {doubled}");
        
        // 转换回轴向坐标验证
        AxialCoordinate backFromCube = AxialCoordinate.FromCube(cube);
        AxialCoordinate backFromOffset = offsetOddQ.ToAxial();
        AxialCoordinate backFromDoubled = doubled.ToAxial();
        
        Debug.Log($"从立方坐标转换回: {backFromCube}");
        Debug.Log($"从偏移坐标转换回: {backFromOffset}");
        Debug.Log($"从双宽坐标转换回: {backFromDoubled}");
    }
    
    /// <summary>
    /// 测试距离计算
    /// </summary>
    private void TestDistanceCalculations()
    {
        Debug.Log("--- 距离计算测试 ---");
        
        CubeCoordinate a = new CubeCoordinate(0, 0, 0);
        CubeCoordinate b = new CubeCoordinate(3, -1, -2);
        
        int distance = CubeCoordinate.Distance(a, b);
        Debug.Log($"坐标 {a} 到 {b} 的距离: {distance}");
        
        // 验证轴向坐标的距离计算
        AxialCoordinate axialA = AxialCoordinate.FromCube(a);
        AxialCoordinate axialB = AxialCoordinate.FromCube(b);
        int axialDistance = AxialCoordinate.Distance(axialA, axialB);
        Debug.Log($"轴向坐标距离验证: {axialDistance} (应该等于 {distance})");
    }
    
    /// <summary>
    /// 测试邻居查找
    /// </summary>
    private void TestNeighborFinding()
    {
        Debug.Log("--- 邻居查找测试 ---");
        
        CubeCoordinate center = new CubeCoordinate(0, 0, 0);
        List<CubeCoordinate> cubeNeighbors = center.GetNeighbors();
        
        Debug.Log($"立方坐标 {center} 的邻居:");
        for (int i = 0; i < cubeNeighbors.Count; i++)
        {
            Debug.Log($"  方向 {i}: {cubeNeighbors[i]}");
        }
        
        // 测试偏移坐标的邻居查找
        OffsetCoordinateOddQ offsetCenter = new OffsetCoordinateOddQ(0, 0);
        List<OffsetCoordinateOddQ> offsetNeighbors = offsetCenter.GetNeighbors();
        
        Debug.Log($"偏移坐标 {offsetCenter} 的邻居:");
        for (int i = 0; i < offsetNeighbors.Count; i++)
        {
            Debug.Log($"  方向 {i}: {offsetNeighbors[i]}");
        }
    }
    
    /// <summary>
    /// 测试旋转操作
    /// </summary>
    private void TestRotationOperations()
    {
        Debug.Log("--- 旋转操作测试 ---");
        
        CubeCoordinate original = new CubeCoordinate(1, -1, 0);
        Debug.Log($"原始坐标: {original}");
        
        CubeCoordinate rotated = original;
        for (int i = 0; i < 6; i++)
        {
            rotated = rotated.RotateRight();
            Debug.Log($"顺时针旋转 {(i + 1) * 60}°: {rotated}");
        }
    }
    
    #endregion
    
    #region 可视化调试
    
    /// <summary>
    /// 在Scene视图中绘制坐标系统示例
    /// </summary>
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        // 绘制不同坐标系统的示例
        DrawCoordinateSystemExample();
    }
    
    /// <summary>
    /// 绘制坐标系统示例
    /// </summary>
    private void DrawCoordinateSystemExample()
    {
        float hexSize = 1.0f;
        int radius = 3;
        
        // 绘制轴向坐标系统
        Gizmos.color = Color.blue;
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Mathf.Abs(q + r) <= radius)
                {
                    AxialCoordinate axial = new AxialCoordinate(q, r);
                    Vector3 worldPos = axial.ToWorldPosition(hexSize);
                    Gizmos.DrawWireSphere(worldPos, 0.1f);
                    
                    // 绘制坐标标签
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(worldPos + Vector3.up * 0.3f, axial.ToString());
                    #endif
                }
            }
        }
        
        // 绘制偏移坐标系统对比
        Gizmos.color = Color.red;
        Vector3 offset = new Vector3(10, 0, 0);
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                OffsetCoordinateOddQ offsetCoord = new OffsetCoordinateOddQ(q, r);
                Vector3 worldPos = offsetCoord.ToWorldPosition(hexSize) + offset;
                Gizmos.DrawWireCube(worldPos, Vector3.one * 0.2f);
                
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(worldPos + Vector3.up * 0.3f, offsetCoord.ToString());
                #endif
            }
        }
    }
    
    #endregion
}