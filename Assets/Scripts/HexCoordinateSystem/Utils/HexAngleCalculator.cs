using UnityEngine;
using HexCoordinateSystem.Coordinates;

namespace HexCoordinateSystem.Utils
{
    /// <summary>
    /// 六边形坐标系统角度计算工具类
    /// </summary>
    public static class HexAngleCalculator
    {
        /// <summary>
        /// 计算两个偏移坐标在世界坐标中的旋转角度
        /// </summary>
        /// <param name="from">起始偏移坐标</param>
        /// <param name="to">目标偏移坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>从起始点到目标点的角度（度数，0-360度）</returns>
        public static float CalculateAngle(OffsetCoordinateOddQ from, OffsetCoordinateOddQ to, float hexSize = 1.0f)
        {
            // 将偏移坐标转换为世界坐标
            Vector3 fromWorld = from.ToWorldPosition(hexSize);
            Vector3 toWorld = to.ToWorldPosition(hexSize);
            
            // 计算方向向量
            Vector3 direction = toWorld - fromWorld;
            
            // 计算角度（使用Atan2函数，返回弧度）
            float angleRadians = Mathf.Atan2(direction.y, direction.x);
            
            // 转换为度数
            float angleDegrees = angleRadians * Mathf.Rad2Deg;
            
            // 确保角度在0-360度范围内
            if (angleDegrees < 0)
            {
                angleDegrees += 360f;
            }
            
            return angleDegrees;
        }
        
        /// <summary>
        /// 计算两个偏移坐标在世界坐标中的旋转角度（弧度）
        /// </summary>
        /// <param name="from">起始偏移坐标</param>
        /// <param name="to">目标偏移坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>从起始点到目标点的角度（弧度，0-2π）</returns>
        public static float CalculateAngleRadians(OffsetCoordinateOddQ from, OffsetCoordinateOddQ to, float hexSize = 1.0f)
        {
            // 将偏移坐标转换为世界坐标
            Vector3 fromWorld = from.ToWorldPosition(hexSize);
            Vector3 toWorld = to.ToWorldPosition(hexSize);
            
            // 计算方向向量
            Vector3 direction = toWorld - fromWorld;
            
            // 计算角度（使用Atan2函数，返回弧度）
            float angleRadians = Mathf.Atan2(direction.y, direction.x);
            
            // 确保角度在0-2π范围内
            if (angleRadians < 0)
            {
                angleRadians += 2 * Mathf.PI;
            }
            
            return angleRadians;
        }
        
        /// <summary>
        /// 计算两个偏移坐标在世界坐标中的方向向量
        /// </summary>
        /// <param name="from">起始偏移坐标</param>
        /// <param name="to">目标偏移坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>从起始点到目标点的方向向量（已归一化）</returns>
        public static Vector3 CalculateDirection(OffsetCoordinateOddQ from, OffsetCoordinateOddQ to, float hexSize = 1.0f)
        {
            // 将偏移坐标转换为世界坐标
            Vector3 fromWorld = from.ToWorldPosition(hexSize);
            Vector3 toWorld = to.ToWorldPosition(hexSize);
            
            // 计算方向向量并归一化
            Vector3 direction = (toWorld - fromWorld).normalized;
            
            return direction;
        }
        
        /// <summary>
        /// 计算两个偏移坐标在世界坐标中的距离
        /// </summary>
        /// <param name="from">起始偏移坐标</param>
        /// <param name="to">目标偏移坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <returns>世界坐标中的欧几里得距离</returns>
        public static float CalculateWorldDistance(OffsetCoordinateOddQ from, OffsetCoordinateOddQ to, float hexSize = 1.0f)
        {
            // 将偏移坐标转换为世界坐标
            Vector3 fromWorld = from.ToWorldPosition(hexSize);
            Vector3 toWorld = to.ToWorldPosition(hexSize);
            
            // 计算欧几里得距离
            return Vector3.Distance(fromWorld, toWorld);
        }
        
        /// <summary>
        /// 获取角度的描述性方向名称
        /// </summary>
        /// <param name="angleDegrees">角度（度数）</param>
        /// <returns>方向名称</returns>
        public static string GetDirectionName(float angleDegrees)
        {
            // 确保角度在0-360度范围内
            angleDegrees = angleDegrees % 360f;
            if (angleDegrees < 0) angleDegrees += 360f;
            
            if (angleDegrees >= 337.5f || angleDegrees < 22.5f)
                return "东";
            else if (angleDegrees >= 22.5f && angleDegrees < 67.5f)
                return "东北";
            else if (angleDegrees >= 67.5f && angleDegrees < 112.5f)
                return "北";
            else if (angleDegrees >= 112.5f && angleDegrees < 157.5f)
                return "西北";
            else if (angleDegrees >= 157.5f && angleDegrees < 202.5f)
                return "西";
            else if (angleDegrees >= 202.5f && angleDegrees < 247.5f)
                return "西南";
            else if (angleDegrees >= 247.5f && angleDegrees < 292.5f)
                return "南";
            else // 292.5f <= angleDegrees < 337.5f
                return "东南";
        }
    }
}