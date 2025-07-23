using UnityEngine;
using HexCoordinateSystem.Coordinates;
using HexCoordinateSystem.Utils;

namespace HexCoordinateSystem.Examples
{
    /// <summary>
    /// 简单的角度计算测试（增强版）
    /// 提供详细的计算验证和可视化输出
    /// </summary>
    public class SimpleAngleTest : MonoBehaviour
    {
        [Header("测试配置")]
        [SerializeField] private bool runOnStart = true;
        [SerializeField] private bool showDetailedOutput = true;
        [SerializeField] private bool runContinuousTest = false;
        [SerializeField] private float testInterval = 2.0f;
        
        [Header("当前测试结果")]
        [SerializeField] private float currentAngleDegrees;
        [SerializeField] private string currentDirection;
        [SerializeField] private float currentDistance;
        [SerializeField] private Vector3 currentFromWorld;
        [SerializeField] private Vector3 currentToWorld;
        
        private float lastTestTime;
        
        void Start()
        {
            if (runOnStart)
            {
                TestUserExample();
            }
        }
        
        void Update()
        {
            if (runContinuousTest && Time.time - lastTestTime >= testInterval)
            {
                TestUserExample();
                lastTestTime = Time.time;
            }
        }
        
        /// <summary>
        /// 测试用户提供的示例：A(1,1) 到 B(-1,-1)
        /// </summary>
        [ContextMenu("执行用户示例测试")]
        public void TestUserExample()
        {
            Debug.Log("=== 用户示例：偏移坐标 A(1,1) 到 B(-1,-1) 的角度计算 ===");
            
            // 创建偏移坐标
            var coordA = new OffsetCoordinateOddQ(1, 1);
            var coordB = new OffsetCoordinateOddQ(-1, -1);
            
            // 使用默认六边形大小 1.0
            float hexSize = 1.0f;
            
            // 执行详细测试
            PerformDetailedTest(coordA, coordB, hexSize, "用户示例");
        }
        
        /// <summary>
        /// 执行详细的角度计算测试
        /// </summary>
        /// <param name="coordA">起始坐标</param>
        /// <param name="coordB">目标坐标</param>
        /// <param name="hexSize">六边形大小</param>
        /// <param name="testName">测试名称</param>
        public void PerformDetailedTest(OffsetCoordinateOddQ coordA, OffsetCoordinateOddQ coordB, float hexSize, string testName)
        {
            // 获取世界坐标
            Vector3 worldA = coordA.ToWorldPosition(hexSize);
            Vector3 worldB = coordB.ToWorldPosition(hexSize);
            
            // 更新当前测试结果
            currentFromWorld = worldA;
            currentToWorld = worldB;
            
            if (showDetailedOutput)
            {
                Debug.Log($"--- {testName} 详细信息 ---");
                Debug.Log($"偏移坐标: A({coordA.col},{coordA.row}) → B({coordB.col},{coordB.row})");
                Debug.Log($"世界坐标: A{worldA:F3} → B{worldB:F3}");
            }
            
            // 计算角度
            float angleDegrees = HexAngleCalculator.CalculateAngle(coordA, coordB, hexSize);
            float angleRadians = HexAngleCalculator.CalculateAngleRadians(coordA, coordB, hexSize);
            currentAngleDegrees = angleDegrees;
            
            // 获取方向名称
            string directionName = HexAngleCalculator.GetDirectionName(angleDegrees);
            currentDirection = directionName;
            
            // 计算方向向量
            Vector3 direction = HexAngleCalculator.CalculateDirection(coordA, coordB, hexSize);
            
            // 计算世界距离
            float worldDistance = HexAngleCalculator.CalculateWorldDistance(coordA, coordB, hexSize);
            currentDistance = worldDistance;
            
            // 输出主要结果
            Debug.Log($"🎯 {testName} 结果: {angleDegrees:F2}° ({directionName}) 距离: {worldDistance:F4}");
            
            if (showDetailedOutput)
            {
                Debug.Log($"角度详情: {angleDegrees:F2}° ({angleRadians:F4} 弧度)");
                Debug.Log($"方向向量: {direction:F3}");
                
                // 手动验证计算
                Vector3 manualDirection = worldB - worldA;
                float manualAngle = Mathf.Atan2(manualDirection.y, manualDirection.x) * Mathf.Rad2Deg;
                if (manualAngle < 0) manualAngle += 360f;
                
                Debug.Log($"手动验证: 方向向量{manualDirection:F3} 角度{manualAngle:F2}°");
                
                // 验证计算一致性
                float angleDifference = Mathf.Abs(angleDegrees - manualAngle);
                if (angleDifference < 0.01f)
                {
                    Debug.Log("✅ 计算验证通过");
                }
                else
                {
                    Debug.LogWarning($"⚠️ 计算差异: {angleDifference:F4}°");
                }
            }
        }
        
        /// <summary>
        /// 运行多个测试案例
        /// </summary>
        [ContextMenu("运行多个测试案例")]
        public void RunMultipleTests()
        {
            Debug.Log("=== 开始多个测试案例 ===");
            
            var testCases = new (int fromCol, int fromRow, int toCol, int toRow, string name)[]
            {
                (1, 1, -1, -1, "用户示例"),
                (0, 0, 1, 0, "正东"),
                (0, 0, 0, 1, "正北"),
                (0, 0, -1, 0, "正西"),
                (0, 0, 0, -1, "正南"),
                (0, 0, 1, 1, "东北"),
                (0, 0, -1, 1, "西北"),
                (0, 0, -1, -1, "西南"),
                (0, 0, 1, -1, "东南"),
                (2, 3, -1, -2, "随机测试1"),
                (-1, 2, 3, -1, "随机测试2")
            };
            
            foreach (var testCase in testCases)
            {
                var coordA = new OffsetCoordinateOddQ(testCase.fromCol, testCase.fromRow);
                var coordB = new OffsetCoordinateOddQ(testCase.toCol, testCase.toRow);
                PerformDetailedTest(coordA, coordB, 1.0f, testCase.name);
            }
            
            Debug.Log("=== 多个测试案例完成 ===");
        }
        
        /// <summary>
        /// 测试不同六边形大小的影响
        /// </summary>
        [ContextMenu("测试不同六边形大小")]
        public void TestDifferentHexSizes()
        {
            Debug.Log("=== 测试不同六边形大小对角度的影响 ===");
            
            var coordA = new OffsetCoordinateOddQ(1, 1);
            var coordB = new OffsetCoordinateOddQ(-1, -1);
            
            float[] hexSizes = { 0.5f, 1.0f, 2.0f, 5.0f };
            
            foreach (float size in hexSizes)
            {
                float angle = HexAngleCalculator.CalculateAngle(coordA, coordB, size);
                float distance = HexAngleCalculator.CalculateWorldDistance(coordA, coordB, size);
                Debug.Log($"六边形大小 {size}: 角度 {angle:F2}° 距离 {distance:F4}");
            }
            
            Debug.Log("注意: 角度不受六边形大小影响，但距离会按比例缩放");
        }
        
        /// <summary>
        /// 开始/停止连续测试
        /// </summary>
        [ContextMenu("切换连续测试")]
        public void ToggleContinuousTest()
        {
            runContinuousTest = !runContinuousTest;
            Debug.Log($"连续测试: {(runContinuousTest ? "开启" : "关闭")}");
            
            if (runContinuousTest)
            {
                lastTestTime = Time.time;
            }
        }
    }
}