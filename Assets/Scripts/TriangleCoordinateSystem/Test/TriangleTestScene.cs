using UnityEngine;
using TriangleCoordinateSystem;
using TriangleCoordinateSystem.Coordinates;
using TriangleCoordinateSystem.Utils;

namespace TriangleCoordinateSystem.Test
{
    /// <summary>
    /// 三角形坐标系统测试场景
    /// 用于验证系统是否正常工作
    /// </summary>
    public class TriangleTestScene : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private bool runTestOnStart = true;
        [SerializeField] private bool enableVisualDebug = true;
        
        private TriangleCoordinateManager manager;
        
        private void Start()
        {
            if (runTestOnStart)
            {
                InitializeTriangleSystem();
                RunBasicTests();
            }
        }
        
        /// <summary>
        /// 初始化三角形坐标系统
        /// </summary>
        private void InitializeTriangleSystem()
        {
            Debug.Log("=== 初始化三角形坐标系统 ===");
            
            // 创建管理器组件
            if (manager == null)
            {
                manager = gameObject.AddComponent<TriangleCoordinateManager>();
            }
            
            // 设置网格参数
            manager.TriangleSize = 1.0f;
            manager.GridOrigin = Vector3.zero;
            
            // 启用缓存
            manager.CacheEnabled = true;
            
            Debug.Log("三角形坐标系统初始化完成");
        }
        
        /// <summary>
        /// 运行基础测试
        /// </summary>
        private void RunBasicTests()
        {
            Debug.Log("=== 运行基础测试 ===");
            
            // 测试坐标创建
            TestCoordinateCreation();
            
            // 测试坐标转换
            TestCoordinateConversion();
            
            // 测试邻居查找
            TestNeighborFinding();
            
            // 测试距离计算
            TestDistanceCalculation();
            
            Debug.Log("基础测试完成！");
        }
        
        /// <summary>
        /// 测试坐标创建
        /// </summary>
        private void TestCoordinateCreation()
        {
            Debug.Log("--- 测试坐标创建 ---");
            
            // 创建不同类型的坐标
            var axial = manager.CreateAxial(2, -1);
            var cube = manager.CreateCube(2, -1, -1);
            var offset = manager.CreateOffset(3, 1);
            
            Debug.Log($"轴向坐标: {axial}");
            Debug.Log($"立方坐标: {cube}");
            Debug.Log($"偏移坐标: {offset}");
        }
        
        /// <summary>
        /// 测试坐标转换
        /// </summary>
        private void TestCoordinateConversion()
        {
            Debug.Log("--- 测试坐标转换 ---");
            
            var axial = new TriangleAxialCoordinate(2, -1);
            
            // 测试所有转换方法
            var cube = axial.ToCubeCoordinate();
            var offset = axial.ToOffsetCoordinate();
            var worldPos = manager.CoordinateToWorld(axial);
            
            Debug.Log($"轴向 {axial} -> 立方 {cube}");
            Debug.Log($"轴向 {axial} -> 偏移 {offset}");
            Debug.Log($"轴向 {axial} -> 世界坐标 {worldPos}");
            
            // 验证转换的一致性
            var backToAxial = cube.ToAxialCoordinate();
            bool conversionCorrect = axial.Equals(backToAxial);
            Debug.Log($"转换一致性检查: {conversionCorrect}");
        }
        
        /// <summary>
        /// 测试邻居查找
        /// </summary>
        private void TestNeighborFinding()
        {
            Debug.Log("--- 测试邻居查找 ---");
            
            var center = new TriangleAxialCoordinate(0, 0);
            var neighbors = center.GetNeighbors();
            
            Debug.Log($"中心坐标 {center} 的邻居:");
            for (int i = 0; i < neighbors.Count; i++)
            {
                Debug.Log($"  邻居 {i}: {neighbors[i]}");
            }
        }
        
        /// <summary>
        /// 测试距离计算
        /// </summary>
        private void TestDistanceCalculation()
        {
            Debug.Log("--- 测试距离计算 ---");
            
            var coord1 = new TriangleAxialCoordinate(0, 0);
            var coord2 = new TriangleAxialCoordinate(2, -1);
            
            int distance = coord1.DistanceTo(coord2);
            Debug.Log($"坐标 {coord1} 到 {coord2} 的距离: {distance}");
        }
        
        /// <summary>
        /// 在Scene视图中绘制调试信息
        /// </summary>
        private void OnDrawGizmos()
        {
            if (manager != null && enableVisualDebug)
            {
                // 绘制一个小的三角形网格用于测试
                var center = new TriangleAxialCoordinate(0, 0);
                TriangleDebugger.DrawTriangleGrid(center, 3, manager.TriangleSize);
            }
        }
    }
}