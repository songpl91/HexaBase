using System;
using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;
using HexCoordinateSystem.Coordinates;

namespace HexCoordinateSystem.Utils
{
    /// <summary>
    /// 六边形坐标系统转换工具类
    /// 提供各种坐标系统之间的高效转换方法
    /// </summary>
    public static class HexConverter
    {
        #region 基础转换方法
        
        /// <summary>
        /// 轴向坐标转立方坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>立方坐标</returns>
        public static CubeCoordinate AxialToCube(AxialCoordinate axial)
        {
            return new CubeCoordinate(axial.q, -axial.q - axial.r, axial.r);
        }
        
        /// <summary>
        /// 立方坐标转轴向坐标
        /// </summary>
        /// <param name="cube">立方坐标</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate CubeToAxial(CubeCoordinate cube)
        {
            return new AxialCoordinate(cube.x, cube.z);
        }
        
        /// <summary>
        /// 轴向坐标转奇数列偏移坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>奇数列偏移坐标</returns>
        public static OffsetCoordinateOddQ AxialToOffsetOddQ(AxialCoordinate axial)
        {
            int col = axial.q;
            int row = axial.r + (axial.q - (axial.q & 1)) / 2;
            return new OffsetCoordinateOddQ(col, row);
        }
        
        /// <summary>
        /// 奇数列偏移坐标转轴向坐标
        /// </summary>
        /// <param name="offset">奇数列偏移坐标</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate OffsetOddQToAxial(OffsetCoordinateOddQ offset)
        {
            int q = offset.col;
            int r = offset.row - (offset.col - (offset.col & 1)) / 2;
            return new AxialCoordinate(q, r);
        }
        
        /// <summary>
        /// 轴向坐标转偶数列偏移坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>偶数列偏移坐标</returns>
        public static OffsetCoordinateEvenQ AxialToOffsetEvenQ(AxialCoordinate axial)
        {
            int col = axial.q;
            int row = axial.r + (axial.q + (axial.q & 1)) / 2;
            return new OffsetCoordinateEvenQ(col, row);
        }
        
        /// <summary>
        /// 偶数列偏移坐标转轴向坐标
        /// </summary>
        /// <param name="offset">偶数列偏移坐标</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate OffsetEvenQToAxial(OffsetCoordinateEvenQ offset)
        {
            int q = offset.col;
            int r = offset.row - (offset.col + (offset.col & 1)) / 2;
            return new AxialCoordinate(q, r);
        }
        
        /// <summary>
        /// 轴向坐标转双宽坐标
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>双宽坐标</returns>
        public static DoubledCoordinate AxialToDoubled(AxialCoordinate axial)
        {
            return new DoubledCoordinate(2 * axial.q + axial.r, axial.r);
        }
        
        /// <summary>
        /// 双宽坐标转轴向坐标
        /// </summary>
        /// <param name="doubled">双宽坐标</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate DoubledToAxial(DoubledCoordinate doubled)
        {
            int q = (doubled.col - doubled.row) / 2;
            int r = doubled.row;
            return new AxialCoordinate(q, r);
        }
        
        #endregion
        
        #region 批量转换方法
        
        /// <summary>
        /// 批量转换轴向坐标到立方坐标
        /// </summary>
        /// <param name="axialCoords">轴向坐标列表</param>
        /// <returns>立方坐标列表</returns>
        public static List<CubeCoordinate> BatchAxialToCube(IEnumerable<AxialCoordinate> axialCoords)
        {
            var result = HexObjectPool.GetCubeList();
            
            foreach (var axial in axialCoords)
            {
                result.Add(AxialToCube(axial));
            }
            
            return result;
        }
        
        /// <summary>
        /// 批量转换立方坐标到轴向坐标
        /// </summary>
        /// <param name="cubeCoords">立方坐标列表</param>
        /// <returns>轴向坐标列表</returns>
        public static List<AxialCoordinate> BatchCubeToAxial(IEnumerable<CubeCoordinate> cubeCoords)
        {
            var result = HexObjectPool.GetAxialList();
            
            foreach (var cube in cubeCoords)
            {
                result.Add(CubeToAxial(cube));
            }
            
            return result;
        }
        
        /// <summary>
        /// 批量转换轴向坐标到偏移坐标
        /// </summary>
        /// <param name="axialCoords">轴向坐标列表</param>
        /// <returns>偏移坐标列表</returns>
        public static List<OffsetCoordinateOddQ> BatchAxialToOffsetOddQ(IEnumerable<AxialCoordinate> axialCoords)
        {
            var result = HexObjectPool.GetOffsetList();
            
            foreach (var axial in axialCoords)
            {
                result.Add(AxialToOffsetOddQ(axial));
            }
            
            return result;
        }
        
        /// <summary>
        /// 批量转换偏移坐标到轴向坐标
        /// </summary>
        /// <param name="offsetCoords">偏移坐标列表</param>
        /// <returns>轴向坐标列表</returns>
        public static List<AxialCoordinate> BatchOffsetOddQToAxial(IEnumerable<OffsetCoordinateOddQ> offsetCoords)
        {
            var result = HexObjectPool.GetAxialList();
            
            foreach (var offset in offsetCoords)
            {
                result.Add(OffsetOddQToAxial(offset));
            }
            
            return result;
        }
        
        #endregion
        
        #region 世界坐标转换
        
        /// <summary>
        /// 世界坐标转轴向坐标（尖顶六边形）
        /// </summary>
        /// <param name="worldPos">世界坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate WorldToAxialPointy(Vector3 worldPos, float hexSize = 1.0f)
        {
            float q = (HexConstants.SQRT3 / 3.0f * worldPos.x - 1.0f / 3.0f * worldPos.z) / hexSize;
            float r = (2.0f / 3.0f * worldPos.z) / hexSize;
            return AxialCoordinate.Round(q, r);
        }
        
        /// <summary>
        /// 世界坐标转轴向坐标（平顶六边形）
        /// </summary>
        /// <param name="worldPos">世界坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate WorldToAxialFlat(Vector3 worldPos, float hexSize = 1.0f)
        {
            float q = (2.0f / 3.0f * worldPos.x) / hexSize;
            float r = (-1.0f / 3.0f * worldPos.x + HexConstants.SQRT3 / 3.0f * worldPos.z) / hexSize;
            return AxialCoordinate.Round(q, r);
        }
        
        /// <summary>
        /// 轴向坐标转世界坐标（尖顶六边形）
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>世界坐标</returns>
        public static Vector3 AxialToWorldPointy(AxialCoordinate axial, float hexSize = 1.0f)
        {
            float x = hexSize * (HexConstants.SQRT3 * axial.q + HexConstants.SQRT3 / 2.0f * axial.r);
            float z = hexSize * (3.0f / 2.0f * axial.r);
            return new Vector3(x, 0, z);
        }
        
        /// <summary>
        /// 轴向坐标转世界坐标（平顶六边形）
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>世界坐标</returns>
        public static Vector3 AxialToWorldFlat(AxialCoordinate axial, float hexSize = 1.0f)
        {
            float x = hexSize * (3.0f / 2.0f * axial.q);
            float z = hexSize * (HexConstants.SQRT3 / 2.0f * axial.q + HexConstants.SQRT3 * axial.r);
            return new Vector3(x, 0, z);
        }
        
        #endregion
        
        #region 通用转换方法
        
        /// <summary>
        /// 将任意坐标转换为轴向坐标
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <returns>轴向坐标</returns>
        public static AxialCoordinate ToAxial(IHexCoordinate coordinate)
        {
            if (coordinate == null)
                throw new ArgumentNullException(nameof(coordinate));
            
            return coordinate.ToAxial();
        }
        
        /// <summary>
        /// 将任意坐标转换为立方坐标
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <returns>立方坐标</returns>
        public static CubeCoordinate ToCube(IHexCoordinate coordinate)
        {
            if (coordinate == null)
                throw new ArgumentNullException(nameof(coordinate));
            
            return coordinate.ToCube();
        }
        
        /// <summary>
        /// 将任意坐标转换为世界坐标
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>世界坐标</returns>
        public static Vector3 ToWorldPosition(IHexCoordinate coordinate, float hexSize = 1.0f)
        {
            if (coordinate == null)
                throw new ArgumentNullException(nameof(coordinate));
            
            return coordinate.ToWorldPosition(hexSize);
        }
        
        #endregion
        
        #region 坐标验证
        
        /// <summary>
        /// 验证坐标是否有效
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <returns>是否有效</returns>
        public static bool IsValidCoordinate(IHexCoordinate coordinate)
        {
            if (coordinate == null)
                return false;
            
            return coordinate.IsValid();
        }
        
        /// <summary>
        /// 验证轴向坐标是否有效
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <returns>是否有效</returns>
        public static bool IsValidAxial(AxialCoordinate axial)
        {
            return axial.IsValid();
        }
        
        /// <summary>
        /// 验证立方坐标是否有效
        /// </summary>
        /// <param name="cube">立方坐标</param>
        /// <returns>是否有效</returns>
        public static bool IsValidCube(CubeCoordinate cube)
        {
            return cube.IsValid();
        }
        
        #endregion
        
        #region 坐标舍入
        
        /// <summary>
        /// 舍入浮点轴向坐标到最近的整数坐标
        /// </summary>
        /// <param name="q">q坐标</param>
        /// <param name="r">r坐标</param>
        /// <returns>舍入后的轴向坐标</returns>
        public static AxialCoordinate RoundAxial(float q, float r)
        {
            return AxialCoordinate.Round(q, r);
        }
        
        /// <summary>
        /// 舍入浮点立方坐标到最近的整数坐标
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="z">z坐标</param>
        /// <returns>舍入后的立方坐标</returns>
        public static CubeCoordinate RoundCube(float x, float y, float z)
        {
            return CubeCoordinate.Round(x, y, z);
        }
        
        #endregion
        
        #region 坐标范围检查
        
        /// <summary>
        /// 检查坐标是否在指定范围内
        /// </summary>
        /// <param name="coordinate">坐标</param>
        /// <param name="center">中心坐标</param>
        /// <param name="range">范围</param>
        /// <returns>是否在范围内</returns>
        public static bool IsInRange(IHexCoordinate coordinate, IHexCoordinate center, int range)
        {
            if (coordinate == null || center == null)
                return false;
            
            return coordinate.DistanceTo(center) <= range;
        }
        
        /// <summary>
        /// 检查轴向坐标是否在矩形区域内
        /// </summary>
        /// <param name="axial">轴向坐标</param>
        /// <param name="minQ">最小q值</param>
        /// <param name="maxQ">最大q值</param>
        /// <param name="minR">最小r值</param>
        /// <param name="maxR">最大r值</param>
        /// <returns>是否在矩形区域内</returns>
        public static bool IsInRectangle(AxialCoordinate axial, int minQ, int maxQ, int minR, int maxR)
        {
            return axial.q >= minQ && axial.q <= maxQ && axial.r >= minR && axial.r <= maxR;
        }
        
        #endregion
    }
}