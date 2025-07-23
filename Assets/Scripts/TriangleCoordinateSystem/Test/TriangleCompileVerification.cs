using UnityEngine;
using TriangleCoordinateSystem;
using TriangleCoordinateSystem.Coordinates;

namespace TriangleCoordinateSystem.Test
{
    /// <summary>
    /// 三角形坐标系统编译验证
    /// 用于快速验证所有API是否正确编译
    /// </summary>
    public class TriangleCompileVerification : MonoBehaviour
    {
        [Header("验证设置")]
        [SerializeField] private bool runVerificationOnStart = true;
        
        private void Start()
        {
            if (runVerificationOnStart)
            {
                VerifyCompilation();
            }
        }
        
        /// <summary>
        /// 验证编译
        /// </summary>
        private void VerifyCompilation()
        {
            Debug.Log("=== 三角形坐标系统编译验证 ===");
            
            try
            {
                // 验证坐标创建
                VerifyCoordinateCreation();
                
                // 验证坐标转换
                VerifyCoordinateConversion();
                
                // 验证管理器
                VerifyManager();
                
                Debug.Log("✅ 编译验证成功 - 所有API都可以正常使用！");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ 编译验证失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 验证坐标创建
        /// </summary>
        private void VerifyCoordinateCreation()
        {
            // 创建轴向坐标
            var axial = new TriangleAxialCoordinate(1, 2);
            
            // 创建立方坐标
            var cube = new TriangleCubeCoordinate(1, -3, 2);
            
            // 创建偏移坐标
            var offset = new TriangleOffsetCoordinate(2, 1);
            
            Debug.Log("✓ 坐标创建验证通过");
        }
        
        /// <summary>
        /// 验证坐标转换
        /// </summary>
        private void VerifyCoordinateConversion()
        {
            var axial = new TriangleAxialCoordinate(1, 2);
            
            // 验证轴向坐标的转换方法
            var cubeFromAxial = axial.ToCubeCoordinate();
            var offsetFromAxial = axial.ToOffsetCoordinate();
            
            // 验证立方坐标的转换方法
            var cube = new TriangleCubeCoordinate(1, -3, 2);
            var axialFromCube = cube.ToAxialCoordinate();
            var offsetFromCube = cube.ToOffsetCoordinate();
            
            // 验证偏移坐标的转换方法
            var offset = new TriangleOffsetCoordinate(2, 1);
            var axialFromOffset = offset.ToAxialCoordinate();
            var cubeFromOffset = offset.ToCubeCoordinate();
            
            Debug.Log("✓ 坐标转换验证通过");
        }
        
        /// <summary>
        /// 验证管理器
        /// </summary>
        private void VerifyManager()
        {
            // 创建管理器组件
            var manager = gameObject.AddComponent<TriangleCoordinateManager>();
            
            // 验证属性设置
            manager.TriangleSize = 1.5f;
            manager.GridOrigin = Vector3.one;
            manager.CacheEnabled = true;
            
            // 验证坐标创建方法
            var axial = manager.CreateAxial(1, 2);
            var cube = manager.CreateCube(1, -3, 2);
            var offset = manager.CreateOffset(2, 1);
            
            // 验证坐标转换方法
            var worldPos = manager.CoordinateToWorld(axial);
            var axialFromWorld = manager.WorldToAxial(worldPos);
            
            // 验证几何计算方法
            var distance = manager.GetDistance(axial, cube);
            var neighbors = manager.GetNeighbors(axial);
            
            Debug.Log("✓ 管理器验证通过");
        }
    }
}