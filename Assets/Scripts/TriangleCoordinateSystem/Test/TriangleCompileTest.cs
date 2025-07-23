using UnityEngine;
using TriangleCoordinateSystem;
using TriangleCoordinateSystem.Coordinates;

namespace TriangleCoordinateSystem.Test
{
    /// <summary>
    /// 三角形坐标系统编译测试
    /// 用于验证所有方法是否正确编译
    /// </summary>
    public class TriangleCompileTest : MonoBehaviour
    {
        private void Start()
        {
            TestCoordinateConversions();
        }
        
        /// <summary>
        /// 测试坐标转换方法
        /// </summary>
        private void TestCoordinateConversions()
        {
            Debug.Log("=== 三角形坐标系统编译测试 ===");
            
            // 测试轴向坐标转换
            TestAxialCoordinateConversions();
            
            // 测试立方坐标转换
            TestCubeCoordinateConversions();
            
            // 测试偏移坐标转换
            TestOffsetCoordinateConversions();
            
            Debug.Log("编译测试完成 - 所有方法都可以正常调用！");
        }
        
        /// <summary>
        /// 测试轴向坐标转换
        /// </summary>
        private void TestAxialCoordinateConversions()
        {
            Debug.Log("--- 轴向坐标转换测试 ---");
            
            var axial = new TriangleAxialCoordinate(2, -1);
            Debug.Log($"轴向坐标: {axial}");
            
            // 测试转换方法（这些是之前报错的方法）
            var cube = axial.ToCubeCoordinate();
            var offset = axial.ToOffsetCoordinate();
            
            Debug.Log($"轴向 -> 立方: {cube}");
            Debug.Log($"轴向 -> 偏移: {offset}");
            
            // 测试其他转换方法
            var cubeFromAxial = axial.ToCube();
            Debug.Log($"轴向 -> 立方(ToCube): {cubeFromAxial}");
            
            // 验证转换结果一致性
            bool cubeMethodsMatch = cube.Equals(cubeFromAxial);
            Debug.Log($"ToCubeCoordinate 和 ToCube 结果一致: {cubeMethodsMatch}");
        }
        
        /// <summary>
        /// 测试立方坐标转换
        /// </summary>
        private void TestCubeCoordinateConversions()
        {
            Debug.Log("--- 立方坐标转换测试 ---");
            
            var cube = new TriangleCubeCoordinate(2, -1, -1);
            Debug.Log($"立方坐标: {cube}");
            
            // 测试所有转换方法
            var axial = cube.ToAxialCoordinate();
            var axial2 = cube.ToAxial();
            var cube2 = cube.ToCubeCoordinate();
            var offset = cube.ToOffsetCoordinate();
            
            Debug.Log($"立方 -> 轴向: {axial}");
            Debug.Log($"立方 -> 轴向(ToAxial): {axial2}");
            Debug.Log($"立方 -> 立方: {cube2}");
            Debug.Log($"立方 -> 偏移: {offset}");
            
            // 验证转换结果一致性
            bool axialMethodsMatch = axial.Equals(axial2);
            bool cubeMethodsMatch = cube.Equals(cube2);
            Debug.Log($"ToAxialCoordinate 和 ToAxial 结果一致: {axialMethodsMatch}");
            Debug.Log($"ToCubeCoordinate 和 ToCube 结果一致: {cubeMethodsMatch}");
        }
        
        /// <summary>
        /// 测试偏移坐标转换
        /// </summary>
        private void TestOffsetCoordinateConversions()
        {
            Debug.Log("--- 偏移坐标转换测试 ---");
            
            var offset = new TriangleOffsetCoordinate(3, 1);
            Debug.Log($"偏移坐标: {offset}");
            
            // 测试所有转换方法
            var axial = offset.ToAxialCoordinate();
            var axial2 = offset.ToAxial();
            var cube = offset.ToCubeCoordinate();
            var cube2 = offset.ToCube();
            var offset2 = offset.ToOffsetCoordinate();
            
            Debug.Log($"偏移 -> 轴向: {axial}");
            Debug.Log($"偏移 -> 轴向(ToAxial): {axial2}");
            Debug.Log($"偏移 -> 立方: {cube}");
            Debug.Log($"偏移 -> 立方(ToCube): {cube2}");
            Debug.Log($"偏移 -> 偏移: {offset2}");
            
            // 验证转换结果一致性
            bool axialMethodsMatch = axial.Equals(axial2);
            bool cubeMethodsMatch = cube.Equals(cube2);
            bool offsetMethodsMatch = offset.Equals(offset2);
            Debug.Log($"ToAxialCoordinate 和 ToAxial 结果一致: {axialMethodsMatch}");
            Debug.Log($"ToCubeCoordinate 和 ToCube 结果一致: {cubeMethodsMatch}");
            Debug.Log($"ToOffsetCoordinate 返回自身: {offsetMethodsMatch}");
        }
    }
}