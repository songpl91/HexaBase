using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexagonSystem
{
    /// <summary>
    /// 六边形坐标系统转换工具类
    /// 提供所有主要坐标系统之间的转换和常用操作
    /// </summary>
    public static class HexCoordinateConverter
    {
        #region 坐标结构定义

        /// <summary>
        /// 立方坐标 (x, y, z) 其中 x + y + z = 0
        /// 最适合距离计算和旋转操作
        /// </summary>
        [Serializable]
        public struct CubeCoordinate : IEquatable<CubeCoordinate>
        {
            public int x, y, z;

            public CubeCoordinate(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                
                // 调试模式下验证约束条件
                #if UNITY_EDITOR
                if (x + y + z != 0)
                {
                    Debug.LogWarning($"立方坐标约束违反: {x} + {y} + {z} = {x + y + z} ≠ 0");
                }
                #endif
            }

            public bool Equals(CubeCoordinate other) => x == other.x && y == other.y && z == other.z;
            public override bool Equals(object obj) => obj is CubeCoordinate other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(x, y, z);
            public override string ToString() => $"Cube({x}, {y}, {z})";

            public static bool operator ==(CubeCoordinate a, CubeCoordinate b) => a.Equals(b);
            public static bool operator !=(CubeCoordinate a, CubeCoordinate b) => !a.Equals(b);
        }

        /// <summary>
        /// 轴向坐标 (q, r)，隐含 s = -q - r
        /// 存储效率最高，是立方坐标的简化版
        /// </summary>
        [Serializable]
        public struct AxialCoordinate : IEquatable<AxialCoordinate>
        {
            public int q, r;

            public AxialCoordinate(int q, int r)
            {
                this.q = q;
                this.r = r;
            }

            public bool Equals(AxialCoordinate other) => q == other.q && r == other.r;
            public override bool Equals(object obj) => obj is AxialCoordinate other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(q, r);
            public override string ToString() => $"Axial({q}, {r})";

            public static bool operator ==(AxialCoordinate a, AxialCoordinate b) => a.Equals(b);
            public static bool operator !=(AxialCoordinate a, AxialCoordinate b) => !a.Equals(b);
        }

        /// <summary>
        /// 奇数列偏移坐标 (q, r)
        /// 最直观，适合用户界面和关卡编辑
        /// </summary>
        [Serializable]
        public struct OffsetCoordinateOddQ : IEquatable<OffsetCoordinateOddQ>
        {
            public int q, r;

            public OffsetCoordinateOddQ(int q, int r)
            {
                this.q = q;
                this.r = r;
            }

            public bool Equals(OffsetCoordinateOddQ other) => q == other.q && r == other.r;
            public override bool Equals(object obj) => obj is OffsetCoordinateOddQ other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(q, r);
            public override string ToString() => $"OffsetOddQ({q}, {r})";

            public static bool operator ==(OffsetCoordinateOddQ a, OffsetCoordinateOddQ b) => a.Equals(b);
            public static bool operator !=(OffsetCoordinateOddQ a, OffsetCoordinateOddQ b) => !a.Equals(b);
        }

        /// <summary>
        /// 偶数列偏移坐标 (q, r)
        /// </summary>
        [Serializable]
        public struct OffsetCoordinateEvenQ : IEquatable<OffsetCoordinateEvenQ>
        {
            public int q, r;

            public OffsetCoordinateEvenQ(int q, int r)
            {
                this.q = q;
                this.r = r;
            }

            public bool Equals(OffsetCoordinateEvenQ other) => q == other.q && r == other.r;
            public override bool Equals(object obj) => obj is OffsetCoordinateEvenQ other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(q, r);
            public override string ToString() => $"OffsetEvenQ({q}, {r})";

            public static bool operator ==(OffsetCoordinateEvenQ a, OffsetCoordinateEvenQ b) => a.Equals(b);
            public static bool operator !=(OffsetCoordinateEvenQ a, OffsetCoordinateEvenQ b) => !a.Equals(b);
        }

        /// <summary>
        /// 双宽坐标 (q, r)
        /// 避免浮点运算，适合高精度计算
        /// </summary>
        [Serializable]
        public struct DoubledCoordinate : IEquatable<DoubledCoordinate>
        {
            public int q, r;

            public DoubledCoordinate(int q, int r)
            {
                this.q = q;
                this.r = r;
            }

            public bool Equals(DoubledCoordinate other) => q == other.q && r == other.r;
            public override bool Equals(object obj) => obj is DoubledCoordinate other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(q, r);
            public override string ToString() => $"Doubled({q}, {r})";

            public static bool operator ==(DoubledCoordinate a, DoubledCoordinate b) => a.Equals(b);
            public static bool operator !=(DoubledCoordinate a, DoubledCoordinate b) => !a.Equals(b);
        }

        #endregion

        #region 坐标转换方法

        // 立方坐标 ↔ 轴向坐标
        public static AxialCoordinate CubeToAxial(CubeCoordinate cube)
        {
            return new AxialCoordinate(cube.x, cube.z);
        }

        public static CubeCoordinate AxialToCube(AxialCoordinate axial)
        {
            return new CubeCoordinate(axial.q, -axial.q - axial.r, axial.r);
        }

        // 轴向坐标 ↔ 奇数列偏移坐标
        public static OffsetCoordinateOddQ AxialToOffsetOddQ(AxialCoordinate axial)
        {
            int q = axial.q;
            int r = axial.r + (axial.q - (axial.q & 1)) / 2;
            return new OffsetCoordinateOddQ(q, r);
        }

        public static AxialCoordinate OffsetOddQToAxial(OffsetCoordinateOddQ offset)
        {
            int q = offset.q;
            int r = offset.r - (offset.q - (offset.q & 1)) / 2;
            return new AxialCoordinate(q, r);
        }

        // 轴向坐标 ↔ 偶数列偏移坐标
        public static OffsetCoordinateEvenQ AxialToOffsetEvenQ(AxialCoordinate axial)
        {
            int q = axial.q;
            int r = axial.r + (axial.q + (axial.q & 1)) / 2;
            return new OffsetCoordinateEvenQ(q, r);
        }

        public static AxialCoordinate OffsetEvenQToAxial(OffsetCoordinateEvenQ offset)
        {
            int q = offset.q;
            int r = offset.r - (offset.q + (offset.q & 1)) / 2;
            return new AxialCoordinate(q, r);
        }

        // 轴向坐标 ↔ 双宽坐标
        public static DoubledCoordinate AxialToDoubled(AxialCoordinate axial)
        {
            return new DoubledCoordinate(axial.q, 2 * axial.r + axial.q);
        }

        public static AxialCoordinate DoubledToAxial(DoubledCoordinate doubled)
        {
            int q = doubled.q;
            int r = (doubled.r - doubled.q) / 2;
            return new AxialCoordinate(q, r);
        }

        #endregion

        #region 距离计算

        /// <summary>
        /// 计算两个六边形之间的距离（立方坐标版本 - 最快）
        /// </summary>
        public static int Distance(CubeCoordinate a, CubeCoordinate b)
        {
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
        }

        /// <summary>
        /// 计算两个六边形之间的距离（轴向坐标版本）
        /// </summary>
        public static int Distance(AxialCoordinate a, AxialCoordinate b)
        {
            return Distance(AxialToCube(a), AxialToCube(b));
        }

        /// <summary>
        /// 计算两个六边形之间的距离（偏移坐标版本）
        /// </summary>
        public static int Distance(OffsetCoordinateOddQ a, OffsetCoordinateOddQ b)
        {
            return Distance(OffsetOddQToAxial(a), OffsetOddQToAxial(b));
        }

        #endregion

        #region 邻居查找

        // 立方坐标的六个方向
        private static readonly CubeCoordinate[] CubeDirections = new CubeCoordinate[]
        {
            new CubeCoordinate(1, -1, 0),   // 右
            new CubeCoordinate(1, 0, -1),   // 右上
            new CubeCoordinate(0, 1, -1),   // 左上
            new CubeCoordinate(-1, 1, 0),   // 左
            new CubeCoordinate(-1, 0, 1),   // 左下
            new CubeCoordinate(0, -1, 1)    // 右下
        };

        // 轴向坐标的六个方向
        private static readonly AxialCoordinate[] AxialDirections = new AxialCoordinate[]
        {
            new AxialCoordinate(1, 0),      // 右
            new AxialCoordinate(1, -1),     // 右上
            new AxialCoordinate(0, -1),     // 左上
            new AxialCoordinate(-1, 0),     // 左
            new AxialCoordinate(-1, 1),     // 左下
            new AxialCoordinate(0, 1)       // 右下
        };

        // 奇数列偏移坐标的方向（分奇偶列）
        private static readonly OffsetCoordinateOddQ[] OffsetOddQDirectionsOdd = new OffsetCoordinateOddQ[]
        {
            new OffsetCoordinateOddQ(1, 0),     // 右
            new OffsetCoordinateOddQ(1, -1),    // 右上
            new OffsetCoordinateOddQ(0, -1),    // 左上
            new OffsetCoordinateOddQ(-1, 0),    // 左
            new OffsetCoordinateOddQ(-1, -1),   // 左下
            new OffsetCoordinateOddQ(0, 0)      // 右下
        };

        private static readonly OffsetCoordinateOddQ[] OffsetOddQDirectionsEven = new OffsetCoordinateOddQ[]
        {
            new OffsetCoordinateOddQ(1, 0),     // 右
            new OffsetCoordinateOddQ(1, 1),     // 右上
            new OffsetCoordinateOddQ(0, 1),     // 左上
            new OffsetCoordinateOddQ(-1, 0),    // 左
            new OffsetCoordinateOddQ(-1, 1),    // 左下
            new OffsetCoordinateOddQ(0, 0)      // 右下
        };

        /// <summary>
        /// 获取立方坐标的所有邻居
        /// </summary>
        public static List<CubeCoordinate> GetNeighbors(CubeCoordinate coord)
        {
            var neighbors = new List<CubeCoordinate>(6);
            for (int i = 0; i < 6; i++)
            {
                var dir = CubeDirections[i];
                neighbors.Add(new CubeCoordinate(coord.x + dir.x, coord.y + dir.y, coord.z + dir.z));
            }
            return neighbors;
        }

        /// <summary>
        /// 获取轴向坐标的所有邻居
        /// </summary>
        public static List<AxialCoordinate> GetNeighbors(AxialCoordinate coord)
        {
            var neighbors = new List<AxialCoordinate>(6);
            for (int i = 0; i < 6; i++)
            {
                var dir = AxialDirections[i];
                neighbors.Add(new AxialCoordinate(coord.q + dir.q, coord.r + dir.r));
            }
            return neighbors;
        }

        /// <summary>
        /// 获取奇数列偏移坐标的所有邻居
        /// </summary>
        public static List<OffsetCoordinateOddQ> GetNeighbors(OffsetCoordinateOddQ coord)
        {
            var neighbors = new List<OffsetCoordinateOddQ>(6);
            var directions = (coord.q & 1) != 0 ? OffsetOddQDirectionsOdd : OffsetOddQDirectionsEven;
            
            for (int i = 0; i < 6; i++)
            {
                var dir = directions[i];
                neighbors.Add(new OffsetCoordinateOddQ(coord.q + dir.q, coord.r + dir.r));
            }
            return neighbors;
        }

        /// <summary>
        /// 获取指定方向的邻居（轴向坐标）
        /// </summary>
        public static AxialCoordinate GetNeighbor(AxialCoordinate coord, int direction)
        {
            if (direction < 0 || direction >= 6)
                throw new ArgumentOutOfRangeException(nameof(direction), "方向必须在0-5之间");
            
            var dir = AxialDirections[direction];
            return new AxialCoordinate(coord.q + dir.q, coord.r + dir.r);
        }

        #endregion

        #region 旋转操作

        /// <summary>
        /// 围绕原点旋转立方坐标（60度的倍数）
        /// </summary>
        /// <param name="coord">要旋转的坐标</param>
        /// <param name="steps">旋转步数（1步 = 60度）</param>
        public static CubeCoordinate RotateCube(CubeCoordinate coord, int steps)
        {
            steps = ((steps % 6) + 6) % 6; // 确保在0-5范围内
            
            for (int i = 0; i < steps; i++)
            {
                coord = new CubeCoordinate(-coord.z, -coord.x, -coord.y);
            }
            return coord;
        }

        /// <summary>
        /// 围绕原点旋转轴向坐标
        /// </summary>
        public static AxialCoordinate RotateAxial(AxialCoordinate coord, int steps)
        {
            return CubeToAxial(RotateCube(AxialToCube(coord), steps));
        }

        #endregion

        #region 世界坐标转换

        /// <summary>
        /// 六边形几何常量
        /// </summary>
        public static class HexGeometry
        {
            public const float SQRT3 = 1.7320508075688772f;
            public const float SQRT3_DIV_2 = 0.8660254037844386f;
            public const float SQRT3_DIV_3 = 0.5773502691896257f;
        }

        /// <summary>
        /// 轴向坐标转世界坐标（平顶六边形）
        /// </summary>
        public static Vector3 AxialToWorld(AxialCoordinate coord, float hexSize = 1.0f)
        {
            float x = hexSize * (3.0f / 2.0f * coord.q);
            float y = hexSize * (HexGeometry.SQRT3_DIV_2 * coord.q + HexGeometry.SQRT3 * coord.r);
            return new Vector3(x, y, 0);
        }

        /// <summary>
        /// 世界坐标转轴向坐标（平顶六边形）
        /// </summary>
        public static AxialCoordinate WorldToAxial(Vector3 worldPos, float hexSize = 1.0f)
        {
            float q = (2.0f / 3.0f * worldPos.x) / hexSize;
            float r = (-1.0f / 3.0f * worldPos.x + HexGeometry.SQRT3_DIV_3 * worldPos.y) / hexSize;
            
            return RoundAxial(new Vector2(q, r));
        }

        /// <summary>
        /// 奇数列偏移坐标转世界坐标（兼容原项目）
        /// </summary>
        public static Vector3 OffsetOddQToWorld(OffsetCoordinateOddQ coord, float hexSize = 1.0f)
        {
            // 转换为轴向坐标再转世界坐标
            return AxialToWorld(OffsetOddQToAxial(coord), hexSize);
        }

        /// <summary>
        /// 原项目兼容的世界坐标转换（使用原项目的常量）
        /// </summary>
        public static Vector3 OffsetOddQToWorldOriginal(OffsetCoordinateOddQ coord)
        {
            int q = coord.q;
            int r = coord.r;
            
            // 使用原项目的逻辑
            int adjustedR = r - q / 2;
            float x = 1.5f * q;
            float y = HexGeometry.SQRT3 * (adjustedR + q / 2.0f);
            
            if (q < 0 && q % 2 != 0)
            {
                y += HexGeometry.SQRT3;
            }
            
            return new Vector3(x, y, 0);
        }

        /// <summary>
        /// 浮点坐标四舍五入到最近的六边形
        /// </summary>
        private static AxialCoordinate RoundAxial(Vector2 fractionalAxial)
        {
            return CubeToAxial(RoundCube(new Vector3(
                fractionalAxial.x,
                -fractionalAxial.x - fractionalAxial.y,
                fractionalAxial.y
            )));
        }

        private static CubeCoordinate RoundCube(Vector3 fractionalCube)
        {
            int rx = Mathf.RoundToInt(fractionalCube.x);
            int ry = Mathf.RoundToInt(fractionalCube.y);
            int rz = Mathf.RoundToInt(fractionalCube.z);

            float xDiff = Mathf.Abs(rx - fractionalCube.x);
            float yDiff = Mathf.Abs(ry - fractionalCube.y);
            float zDiff = Mathf.Abs(rz - fractionalCube.z);

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

        #endregion

        #region 范围和区域操作

        /// <summary>
        /// 获取指定半径内的所有六边形坐标
        /// </summary>
        public static List<AxialCoordinate> GetHexesInRange(AxialCoordinate center, int radius)
        {
            var results = new List<AxialCoordinate>();
            
            for (int q = -radius; q <= radius; q++)
            {
                int r1 = Mathf.Max(-radius, -q - radius);
                int r2 = Mathf.Min(radius, -q + radius);
                
                for (int r = r1; r <= r2; r++)
                {
                    results.Add(new AxialCoordinate(center.q + q, center.r + r));
                }
            }
            
            return results;
        }

        /// <summary>
        /// 获取指定半径的环形六边形坐标
        /// </summary>
        public static List<AxialCoordinate> GetHexRing(AxialCoordinate center, int radius)
        {
            if (radius == 0)
            {
                return new List<AxialCoordinate> { center };
            }

            var results = new List<AxialCoordinate>();
            var current = new AxialCoordinate(center.q + AxialDirections[4].q * radius, 
                                            center.r + AxialDirections[4].r * radius);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    results.Add(current);
                    current = GetNeighbor(current, i);
                }
            }

            return results;
        }

        /// <summary>
        /// 获取从起点到终点的直线路径
        /// </summary>
        public static List<AxialCoordinate> GetLinePath(AxialCoordinate start, AxialCoordinate end)
        {
            int distance = Distance(start, end);
            var results = new List<AxialCoordinate>(distance + 1);

            for (int i = 0; i <= distance; i++)
            {
                float t = distance == 0 ? 0.0f : (float)i / distance;
                var lerped = LerpAxial(start, end, t);
                results.Add(lerped);
            }

            return results;
        }

        private static AxialCoordinate LerpAxial(AxialCoordinate a, AxialCoordinate b, float t)
        {
            return CubeToAxial(LerpCube(AxialToCube(a), AxialToCube(b), t));
        }

        private static CubeCoordinate LerpCube(CubeCoordinate a, CubeCoordinate b, float t)
        {
            return RoundCube(new Vector3(
                Mathf.Lerp(a.x, b.x, t),
                Mathf.Lerp(a.y, b.y, t),
                Mathf.Lerp(a.z, b.z, t)
            ));
        }

        #endregion

        #region 性能优化工具

        /// <summary>
        /// 邻居列表对象池
        /// </summary>
        private static readonly Queue<List<AxialCoordinate>> s_AxialNeighborPool = new Queue<List<AxialCoordinate>>();
        private static readonly Queue<List<CubeCoordinate>> s_CubeNeighborPool = new Queue<List<CubeCoordinate>>();

        /// <summary>
        /// 从对象池获取邻居列表
        /// </summary>
        public static List<AxialCoordinate> GetPooledAxialNeighborList()
        {
            if (s_AxialNeighborPool.Count > 0)
            {
                var list = s_AxialNeighborPool.Dequeue();
                list.Clear();
                return list;
            }
            return new List<AxialCoordinate>(6);
        }

        /// <summary>
        /// 归还邻居列表到对象池
        /// </summary>
        public static void ReturnPooledAxialNeighborList(List<AxialCoordinate> list)
        {
            if (list != null && s_AxialNeighborPool.Count < 10) // 限制池大小
            {
                s_AxialNeighborPool.Enqueue(list);
            }
        }

        #endregion

        #region 调试和验证工具

        /// <summary>
        /// 验证坐标转换的正确性
        /// </summary>
        public static bool ValidateCoordinateConversions()
        {
            var testCoords = new AxialCoordinate[]
            {
                new AxialCoordinate(0, 0),
                new AxialCoordinate(1, 0),
                new AxialCoordinate(0, 1),
                new AxialCoordinate(-1, 1),
                new AxialCoordinate(-1, 0),
                new AxialCoordinate(0, -1),
                new AxialCoordinate(1, -1),
                new AxialCoordinate(2, -1),
                new AxialCoordinate(-2, 1)
            };

            foreach (var axial in testCoords)
            {
                // 测试轴向 ↔ 立方转换
                var cube = AxialToCube(axial);
                var backToAxial = CubeToAxial(cube);
                if (axial != backToAxial)
                {
                    Debug.LogError($"轴向↔立方转换失败: {axial} → {cube} → {backToAxial}");
                    return false;
                }

                // 测试轴向 ↔ 偏移转换
                var offset = AxialToOffsetOddQ(axial);
                var backToAxial2 = OffsetOddQToAxial(offset);
                if (axial != backToAxial2)
                {
                    Debug.LogError($"轴向↔偏移转换失败: {axial} → {offset} → {backToAxial2}");
                    return false;
                }

                // 验证立方坐标约束
                if (cube.x + cube.y + cube.z != 0)
                {
                    Debug.LogError($"立方坐标约束违反: {cube}");
                    return false;
                }
            }

            Debug.Log("所有坐标转换验证通过！");
            return true;
        }

        /// <summary>
        /// 性能测试
        /// </summary>
        public static void PerformanceTest(int iterations = 10000)
        {
            var testCoords = new AxialCoordinate[]
            {
                new AxialCoordinate(0, 0),
                new AxialCoordinate(5, -3),
                new AxialCoordinate(-2, 7),
                new AxialCoordinate(10, -5)
            };

            // 距离计算性能测试
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < testCoords.Length - 1; j++)
                {
                    Distance(testCoords[j], testCoords[j + 1]);
                }
            }
            stopwatch.Stop();
            Debug.Log($"距离计算 {iterations * (testCoords.Length - 1)} 次耗时: {stopwatch.ElapsedMilliseconds}ms");

            // 邻居查找性能测试
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                foreach (var coord in testCoords)
                {
                    GetNeighbors(coord);
                }
            }
            stopwatch.Stop();
            Debug.Log($"邻居查找 {iterations * testCoords.Length} 次耗时: {stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion
    }

    #region Unity 编辑器扩展

    #if UNITY_EDITOR
    /// <summary>
    /// Unity 编辑器中的六边形坐标可视化工具
    /// </summary>
    public static class HexCoordinateVisualizer
    {
        /// <summary>
        /// 在Scene视图中绘制六边形网格
        /// </summary>
        public static void DrawHexGrid(Vector3 center, int radius, float hexSize = 1.0f)
        {
            var centerAxial = HexCoordinateConverter.WorldToAxial(center, hexSize);
            var hexes = HexCoordinateConverter.GetHexesInRange(centerAxial, radius);

            foreach (var hex in hexes)
            {
                DrawHexagon(HexCoordinateConverter.AxialToWorld(hex, hexSize), hexSize);
                
                // 绘制坐标标签
                var worldPos = HexCoordinateConverter.AxialToWorld(hex, hexSize);
                var cube = HexCoordinateConverter.AxialToCube(hex);
                var offset = HexCoordinateConverter.AxialToOffsetOddQ(hex);
                
                UnityEditor.Handles.Label(worldPos, 
                    $"A:({hex.q},{hex.r})\nC:({cube.x},{cube.y},{cube.z})\nO:({offset.q},{offset.r})");
            }
        }

        /// <summary>
        /// 绘制单个六边形（边对齐/平顶六边形）
        /// </summary>
        public static void DrawHexagon(Vector3 center, float size)
        {
            var vertices = new Vector3[7]; // 6个顶点 + 回到起点
            
            // 边对齐六边形：第一个顶点在30度角（右上方）
            // 这样顶部和底部的边是水平的
            for (int i = 0; i < 6; i++)
            {
                float angle = (i * 60f + 30f) * Mathf.Deg2Rad; // 加30度偏移实现边对齐
                vertices[i] = center + new Vector3(
                    size * Mathf.Cos(angle),
                    size * Mathf.Sin(angle),
                    0
                );
            }
            vertices[6] = vertices[0]; // 闭合

            UnityEditor.Handles.DrawPolyLine(vertices);
        }

        /// <summary>
        /// 绘制六边形之间的连接线
        /// </summary>
        public static void DrawHexConnections(List<HexCoordinateConverter.AxialCoordinate> path, float hexSize = 1.0f)
        {
            if (path.Count < 2) return;

            for (int i = 0; i < path.Count - 1; i++)
            {
                var start = HexCoordinateConverter.AxialToWorld(path[i], hexSize);
                var end = HexCoordinateConverter.AxialToWorld(path[i + 1], hexSize);
                UnityEditor.Handles.DrawLine(start, end);
            }
        }
    }
    #endif

    #endregion
}