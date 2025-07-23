using UnityEngine;
using UnityEditor;
using GridCoordinateSystem.Setup;
using GridCoordinateSystem.Visualization;
using GridCoordinateSystem.Testing;

namespace GridCoordinateSystem.Editor
{
    /// <summary>
    /// 网格系统编辑器工具
    /// </summary>
    public class GridSystemEditorTools
    {
        [MenuItem("Grid System/Create Visualization Scene")]
        public static void CreateVisualizationScene()
        {
            // 创建新场景
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, 
                UnityEditor.SceneManagement.NewSceneMode.Single);
            
            // 创建摄像机
            GameObject cameraObj = new GameObject("Main Camera");
            Camera camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
            cameraObj.tag = "MainCamera";
            
            camera.transform.position = new Vector3(5, 5, -10);
            camera.orthographic = true;
            camera.orthographicSize = 8;
            camera.backgroundColor = Color.black;
            camera.clearFlags = CameraClearFlags.SolidColor;
            
            // 创建可视化设置器
            GameObject setupObj = new GameObject("Grid Visualization Setup");
            GridVisualizationSetup setup = setupObj.AddComponent<GridVisualizationSetup>();
            
            // 创建测试器
            GameObject testerObj = new GameObject("Grid System Tester");
            GridSystemTester tester = testerObj.AddComponent<GridSystemTester>();
            
            Debug.Log("网格可视化场景已创建！点击播放按钮开始验证。");
        }
        
        [MenuItem("Grid System/Quick Setup Current Scene")]
        public static void QuickSetupCurrentScene()
        {
            // 在当前场景中快速设置
            GridVisualizationSetup setup = Object.FindObjectOfType<GridVisualizationSetup>();
            if (setup == null)
            {
                GameObject setupObj = new GameObject("Grid Visualization Setup");
                setup = setupObj.AddComponent<GridVisualizationSetup>();
            }
            
            setup.SetupVisualizationScene();
            
            Debug.Log("当前场景已设置网格可视化！");
        }
        
        [MenuItem("Grid System/Run Tests")]
        public static void RunTests()
        {
            GridSystemTester tester = Object.FindObjectOfType<GridSystemTester>();
            if (tester == null)
            {
                GameObject testerObj = new GameObject("Grid System Tester");
                tester = testerObj.AddComponent<GridSystemTester>();
            }
            
            tester.RunTestsManually();
            
            Debug.Log("网格系统测试已启动，查看控制台输出结果。");
        }
        
        [MenuItem("Grid System/Performance Test")]
        public static void RunPerformanceTest()
        {
            GridSystemTester tester = Object.FindObjectOfType<GridSystemTester>();
            if (tester == null)
            {
                GameObject testerObj = new GameObject("Grid System Tester");
                tester = testerObj.AddComponent<GridSystemTester>();
            }
            
            tester.RunPerformanceTest();
            
            Debug.Log("性能测试已启动，查看控制台输出结果。");
        }
        
        [MenuItem("Grid System/Open Demo Scene")]
        public static void OpenDemoScene()
        {
            string scenePath = "Assets/Scenes/GridVisualizationDemo.unity";
            
            if (System.IO.File.Exists(scenePath))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
                Debug.Log("已打开网格可视化演示场景！");
            }
            else
            {
                Debug.LogWarning($"演示场景不存在: {scenePath}");
                Debug.Log("使用 'Grid System/Create Visualization Scene' 创建新场景。");
            }
        }
        
        [MenuItem("Grid System/Documentation")]
        public static void OpenDocumentation()
        {
            string readmePath = "Assets/Scripts/GridCoordinateSystem/README_Visualization.md";
            
            if (System.IO.File.Exists(readmePath))
            {
                Application.OpenURL("file://" + System.IO.Path.GetFullPath(readmePath));
                Debug.Log("已打开可视化文档！");
            }
            else
            {
                Debug.LogWarning("文档文件不存在！");
            }
        }
        
        [MenuItem("Grid System/Validate Installation")]
        public static void ValidateInstallation()
        {
            Debug.Log("=== 验证网格系统安装 ===");
            
            // 检查核心文件
            string[] requiredFiles = {
                "Assets/Scripts/GridCoordinateSystem/GridCoordinateManager.cs",
                "Assets/Scripts/GridCoordinateSystem/Coordinates/CartesianCoordinate.cs",
                "Assets/Scripts/GridCoordinateSystem/Coordinates/IndexCoordinate.cs",
                "Assets/Scripts/GridCoordinateSystem/Core/GridConstants.cs",
                "Assets/Scripts/GridCoordinateSystem/Core/IGridCoordinate.cs",
                "Assets/Scripts/GridCoordinateSystem/Visualization/GridVisualizer.cs",
                "Assets/Scripts/GridCoordinateSystem/Setup/GridVisualizationSetup.cs",
                "Assets/Scripts/GridCoordinateSystem/Testing/GridSystemTester.cs"
            };
            
            bool allFilesExist = true;
            foreach (string file in requiredFiles)
            {
                if (System.IO.File.Exists(file))
                {
                    Debug.Log($"✓ {file}");
                }
                else
                {
                    Debug.LogError($"✗ 缺少文件: {file}");
                    allFilesExist = false;
                }
            }
            
            if (allFilesExist)
            {
                Debug.Log("✓ 所有核心文件都存在！");
                Debug.Log("网格系统安装完整，可以开始使用。");
                Debug.Log("使用菜单 'Grid System/Create Visualization Scene' 开始验证。");
            }
            else
            {
                Debug.LogError("✗ 安装不完整，请检查缺少的文件。");
            }
        }
    }
}