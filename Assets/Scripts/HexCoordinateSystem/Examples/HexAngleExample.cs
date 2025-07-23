using UnityEngine;
using HexCoordinateSystem.Coordinates;
using HexCoordinateSystem.Utils;

namespace HexCoordinateSystem.Examples
{
    /// <summary>
    /// 六边形坐标角度计算示例
    /// </summary>
    public class HexAngleExample : MonoBehaviour
    {
        [Header("坐标设置")]
        [SerializeField] private int fromCol = 1;
        [SerializeField] private int fromRow = 1;
        [SerializeField] private int toCol = -1;
        [SerializeField] private int toRow = -1;
        [SerializeField] private float hexSize = 1.0f;
        
        [Header("计算结果")]
        [SerializeField] private float angleDegrees;
        [SerializeField] private float angleRadians;
        [SerializeField] private string directionName;
        [SerializeField] private float worldDistance;
        [SerializeField] private Vector3 fromWorldPos;
        [SerializeField] private Vector3 toWorldPos;
        [SerializeField] private Vector3 direction;
        
        void Start()
        {
            CalculateAngle();
        }
        
        void OnValidate()
        {
            CalculateAngle();
        }
        
        /// <summary>
        /// 计算角度和相关信息
        /// </summary>
        public void CalculateAngle()
        {
            // 创建偏移坐标
            var from = new OffsetCoordinateOddQ(fromCol, fromRow);
            var to = new OffsetCoordinateOddQ(toCol, toRow);
            
            // 计算角度（度数）
            angleDegrees = HexAngleCalculator.CalculateAngle(from, to, hexSize);
            
            // 计算角度（弧度）
            angleRadians = HexAngleCalculator.CalculateAngleRadians(from, to, hexSize);
            
            // 获取方向名称
            directionName = HexAngleCalculator.GetDirectionName(angleDegrees);
            
            // 计算世界坐标距离
            worldDistance = HexAngleCalculator.CalculateWorldDistance(from, to, hexSize);
            
            // 获取世界坐标位置
            fromWorldPos = from.ToWorldPosition(hexSize);
            toWorldPos = to.ToWorldPosition(hexSize);
            
            // 获取方向向量
            direction = HexAngleCalculator.CalculateDirection(from, to, hexSize);
            
            // 输出结果到控制台
            Debug.Log($"偏移坐标 A({fromCol},{fromRow}) 到 B({toCol},{toRow}) 的角度计算结果:");
            Debug.Log($"  世界坐标: A{fromWorldPos} -> B{toWorldPos}");
            Debug.Log($"  角度: {angleDegrees:F2}° ({angleRadians:F4} 弧度)");
            Debug.Log($"  方向: {directionName}");
            Debug.Log($"  世界距离: {worldDistance:F4}");
            Debug.Log($"  方向向量: {direction}");
        }
        
        /// <summary>
        /// 在Scene视图中绘制可视化信息
        /// </summary>
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            // 绘制起始点
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(fromWorldPos, 0.1f);
            
            // 绘制目标点
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(toWorldPos, 0.1f);
            
            // 绘制连接线
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(fromWorldPos, toWorldPos);
            
            // 绘制方向箭头
            Vector3 arrowStart = fromWorldPos;
            Vector3 arrowEnd = fromWorldPos + direction * 0.5f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(arrowStart, arrowEnd);
            
            // 绘制箭头头部
            Vector3 arrowHead1 = arrowEnd - direction * 0.1f + Vector3.Cross(direction, Vector3.forward) * 0.05f;
            Vector3 arrowHead2 = arrowEnd - direction * 0.1f - Vector3.Cross(direction, Vector3.forward) * 0.05f;
            Gizmos.DrawLine(arrowEnd, arrowHead1);
            Gizmos.DrawLine(arrowEnd, arrowHead2);
        }
        
        /// <summary>
        /// 测试多个坐标对的角度计算
        /// </summary>
        [ContextMenu("测试多个坐标对")]
        public void TestMultipleCoordinates()
        {
            var testCases = new (int fromCol, int fromRow, int toCol, int toRow)[]
            {
                (0, 0, 1, 0),   // 向右
                (0, 0, 0, 1),   // 向上
                (0, 0, -1, 0),  // 向左
                (0, 0, 0, -1),  // 向下
                (1, 1, -1, -1), // 用户提供的示例
                (0, 0, 1, 1),   // 对角线
                (0, 0, -1, -1), // 对角线
            };
            
            Debug.Log("=== 多个坐标对的角度计算测试 ===");
            
            foreach (var testCase in testCases)
            {
                var from = new OffsetCoordinateOddQ(testCase.fromCol, testCase.fromRow);
                var to = new OffsetCoordinateOddQ(testCase.toCol, testCase.toRow);
                
                float angle = HexAngleCalculator.CalculateAngle(from, to, hexSize);
                string dirName = HexAngleCalculator.GetDirectionName(angle);
                
                Debug.Log($"从 ({testCase.fromCol},{testCase.fromRow}) 到 ({testCase.toCol},{testCase.toRow}): " +
                         $"{angle:F2}° ({dirName})");
            }
        }
    }
}