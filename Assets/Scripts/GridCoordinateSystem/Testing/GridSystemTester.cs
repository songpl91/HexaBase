using UnityEngine;
using System.Collections;
using GridCoordinateSystem.Core;
using GridCoordinateSystem.Coordinates;
using GridCoordinateSystem.Utils;

namespace GridCoordinateSystem.Testing
{
    /// <summary>
    /// 网格系统功能测试器
    /// </summary>
    public class GridSystemTester : MonoBehaviour
    {
        [Header("测试配置")]
        [SerializeField] private bool _runTestsOnStart = true;
        [SerializeField] private bool _runContinuousTests = false;
        [SerializeField] private float _testInterval = 2.0f;
        
        [Header("网格配置")]
        [SerializeField] private float _cellSize = 1.0f;
        [SerializeField] private int _gridWidth = 10;
        [SerializeField] private int _gridHeight = 10;
        
        private GridCoordinateManager _gridManager;
        private int _testCounter = 0;
        
        private void Start()
        {
            InitializeGridManager();
            
            if (_runTestsOnStart)
            {
                StartCoroutine(RunAllTests());
            }
            
            if (_runContinuousTests)
            {
                StartCoroutine(ContinuousTestLoop());
            }
        }
        
        /// <summary>
        /// 初始化网格管理器
        /// </summary>
        private void InitializeGridManager()
        {
            _gridManager = FindObjectOfType<GridCoordinateManager>();
            if (_gridManager == null)
            {
                GameObject managerObj = new GameObject("GridCoordinateManager");
                _gridManager = managerObj.AddComponent<GridCoordinateManager>();
            }
            
            _gridManager.CellSize = _cellSize;
            _gridManager.GridWidth = _gridWidth;
            _gridManager.GridHeight = _gridHeight;
            
            Debug.Log("=== 网格系统测试器已初始化 ===");
            Debug.Log($"网格大小: {_gridWidth}x{_gridHeight}, 单元大小: {_cellSize}");
        }
        
        /// <summary>
        /// 运行所有测试
        /// </summary>
        private IEnumerator RunAllTests()
        {
            Debug.Log("=== 开始运行所有测试 ===");
            
            yield return StartCoroutine(TestCoordinateConversions());
            yield return new WaitForSeconds(0.5f);
            
            yield return StartCoroutine(TestNeighborFinding());
            yield return new WaitForSeconds(0.5f);
            
            yield return StartCoroutine(TestDistanceCalculations());
            yield return new WaitForSeconds(0.5f);
            
            yield return StartCoroutine(TestPathFinding());
            yield return new WaitForSeconds(0.5f);
            
            yield return StartCoroutine(TestRangeQueries());
            yield return new WaitForSeconds(0.5f);
            
            yield return StartCoroutine(TestBoundaryChecking());
            yield return new WaitForSeconds(0.5f);
            
            Debug.Log("=== 所有测试完成 ===");
        }
        
        /// <summary>
        /// 持续测试循环
        /// </summary>
        private IEnumerator ContinuousTestLoop()
        {
            while (_runContinuousTests)
            {
                yield return new WaitForSeconds(_testInterval);
                
                _testCounter++;
                Debug.Log($"=== 连续测试 #{_testCounter} ===");
                
                // 随机选择一个测试
                int testType = Random.Range(0, 6);
                switch (testType)
                {
                    case 0: yield return StartCoroutine(TestCoordinateConversions()); break;
                    case 1: yield return StartCoroutine(TestNeighborFinding()); break;
                    case 2: yield return StartCoroutine(TestDistanceCalculations()); break;
                    case 3: yield return StartCoroutine(TestPathFinding()); break;
                    case 4: yield return StartCoroutine(TestRangeQueries()); break;
                    case 5: yield return StartCoroutine(TestBoundaryChecking()); break;
                }
            }
        }
        
        /// <summary>
        /// 测试坐标转换
        /// </summary>
        private IEnumerator TestCoordinateConversions()
        {
            Debug.Log("--- 测试坐标转换 ---");
            
            // 测试几个关键点
            var testPoints = new CartesianCoordinate[]
            {
                new CartesianCoordinate(0, 0),
                new CartesianCoordinate(_gridWidth / 2, _gridHeight / 2),
                new CartesianCoordinate(_gridWidth - 1, _gridHeight - 1),
                new CartesianCoordinate(Random.Range(0, _gridWidth), Random.Range(0, _gridHeight))
            };
            
            foreach (var cartesian in testPoints)
            {
                // 直角坐标 -> 世界坐标 -> 直角坐标
                Vector3 world = _gridManager.CartesianToWorld(cartesian);
                CartesianCoordinate backToCartesian = _gridManager.WorldToCartesian(world);
                
                // 直角坐标 -> 索引坐标 -> 直角坐标
                IndexCoordinate index = _gridManager.CartesianToIndex(cartesian);
                CartesianCoordinate backFromIndex = _gridManager.IndexToCartesian(index);
                
                Debug.Log($"直角坐标: {cartesian}");
                Debug.Log($"  -> 世界坐标: {world}");
                Debug.Log($"  -> 回转直角坐标: {backToCartesian}");
                Debug.Log($"  -> 索引坐标: {index}");
                Debug.Log($"  -> 回转直角坐标: {backFromIndex}");
                
                // 验证转换的正确性
                bool worldConversionCorrect = cartesian.Equals(backToCartesian);
                bool indexConversionCorrect = cartesian.Equals(backFromIndex);
                
                Debug.Log($"  世界坐标转换正确: {worldConversionCorrect}");
                Debug.Log($"  索引坐标转换正确: {indexConversionCorrect}");
                
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        /// <summary>
        /// 测试邻居查找
        /// </summary>
        private IEnumerator TestNeighborFinding()
        {
            Debug.Log("--- 测试邻居查找 ---");
            
            var testPositions = new CartesianCoordinate[]
            {
                new CartesianCoordinate(0, 0), // 角落
                new CartesianCoordinate(_gridWidth / 2, _gridHeight / 2), // 中心
                new CartesianCoordinate(0, _gridHeight / 2), // 边缘
                new CartesianCoordinate(Random.Range(1, _gridWidth - 1), Random.Range(1, _gridHeight - 1)) // 随机内部点
            };
            
            foreach (var position in testPositions)
            {
                var neighbors4 = _gridManager.GetNeighbors4(position);
                var neighbors8 = _gridManager.GetNeighbors8(position);
                
                Debug.Log($"位置 {position}:");
                Debug.Log($"  4邻居 ({neighbors4.Count}个): {string.Join(", ", neighbors4)}");
                Debug.Log($"  8邻居 ({neighbors8.Count}个): {string.Join(", ", neighbors8)}");
                
                // 验证邻居都在边界内
                bool allNeighbors4Valid = true;
                bool allNeighbors8Valid = true;
                
                foreach (var neighbor in neighbors4)
                {
                    if (!_gridManager.IsInBounds(neighbor))
                    {
                        allNeighbors4Valid = false;
                        break;
                    }
                }
                
                foreach (var neighbor in neighbors8)
                {
                    if (!_gridManager.IsInBounds(neighbor))
                    {
                        allNeighbors8Valid = false;
                        break;
                    }
                }
                
                Debug.Log($"  4邻居都有效: {allNeighbors4Valid}");
                Debug.Log($"  8邻居都有效: {allNeighbors8Valid}");
                
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        /// <summary>
        /// 测试距离计算
        /// </summary>
        private IEnumerator TestDistanceCalculations()
        {
            Debug.Log("--- 测试距离计算 ---");
            
            var point1 = new CartesianCoordinate(1, 1);
            var point2 = new CartesianCoordinate(4, 5);
            
            float manhattanDist = _gridManager.GetDistance(point1, point2, GridConstants.DistanceType.Manhattan);
            float euclideanDist = _gridManager.GetDistance(point1, point2, GridConstants.DistanceType.Euclidean);
            float chebyshevDist = _gridManager.GetDistance(point1, point2, GridConstants.DistanceType.Chebyshev);
            
            Debug.Log($"从 {point1} 到 {point2} 的距离:");
            Debug.Log($"  曼哈顿距离: {manhattanDist}");
            Debug.Log($"  欧几里得距离: {euclideanDist:F2}");
            Debug.Log($"  切比雪夫距离: {chebyshevDist}");
            
            // 验证距离计算的合理性
            Debug.Log($"  曼哈顿距离 >= 欧几里得距离: {manhattanDist >= euclideanDist}");
            Debug.Log($"  欧几里得距离 >= 切比雪夫距离: {euclideanDist >= chebyshevDist}");
            
            yield return new WaitForSeconds(0.1f);
        }
        
        /// <summary>
        /// 测试路径查找
        /// </summary>
        private IEnumerator TestPathFinding()
        {
            Debug.Log("--- 测试路径查找 ---");
            
            var start = new CartesianCoordinate(0, 0);
            var end = new CartesianCoordinate(3, 3);
            
            var path = _gridManager.GetLinePath(start, end);
            
            Debug.Log($"从 {start} 到 {end} 的路径:");
            Debug.Log($"  路径长度: {path.Count}");
            Debug.Log($"  路径: {string.Join(" -> ", path)}");
            
            // 验证路径的连续性
            bool pathContinuous = true;
            for (int i = 0; i < path.Count - 1; i++)
            {
                float distance = _gridManager.GetDistance(path[i], path[i + 1], GridConstants.DistanceType.Euclidean);
                if (distance > 1.5f) // 允许对角线移动
                {
                    pathContinuous = false;
                    break;
                }
            }
            
            Debug.Log($"  路径连续性: {pathContinuous}");
            
            yield return new WaitForSeconds(0.1f);
        }
        
        /// <summary>
        /// 测试范围查询
        /// </summary>
        private IEnumerator TestRangeQueries()
        {
            Debug.Log("--- 测试范围查询 ---");
            
            var center = new CartesianCoordinate(_gridWidth / 2, _gridHeight / 2);
            int radius = 2;
            
            var rectangleRange = _gridManager.GetRectangleRange(center, radius, radius);
            var circleRange = _gridManager.GetCircleRange(center, radius);
            var manhattanRange = _gridManager.GetManhattanRange(center, radius);
            var chebyshevRange = _gridManager.GetChebyshevRange(center, radius);
            
            Debug.Log($"以 {center} 为中心，半径 {radius} 的范围查询:");
            Debug.Log($"  矩形范围: {rectangleRange.Count} 个坐标");
            Debug.Log($"  圆形范围: {circleRange.Count} 个坐标");
            Debug.Log($"  曼哈顿范围: {manhattanRange.Count} 个坐标");
            Debug.Log($"  切比雪夫范围: {chebyshevRange.Count} 个坐标");
            
            // 验证所有结果都在边界内
            bool allInBounds = true;
            foreach (var coord in circleRange)
            {
                if (!_gridManager.IsInBounds(coord))
                {
                    allInBounds = false;
                    break;
                }
            }
            
            Debug.Log($"  所有结果都在边界内: {allInBounds}");
            
            yield return new WaitForSeconds(0.1f);
        }
        
        /// <summary>
        /// 测试边界检查
        /// </summary>
        private IEnumerator TestBoundaryChecking()
        {
            Debug.Log("--- 测试边界检查 ---");
            
            var testCoordinates = new CartesianCoordinate[]
            {
                new CartesianCoordinate(-1, -1), // 超出边界
                new CartesianCoordinate(0, 0), // 边界内
                new CartesianCoordinate(_gridWidth, _gridHeight), // 超出边界
                new CartesianCoordinate(_gridWidth - 1, _gridHeight - 1), // 边界内
                new CartesianCoordinate(_gridWidth / 2, -1), // 部分超出
                new CartesianCoordinate(-1, _gridHeight / 2) // 部分超出
            };
            
            foreach (var coord in testCoordinates)
            {
                bool inBounds = _gridManager.IsInBounds(coord);
                Debug.Log($"坐标 {coord}: {(inBounds ? "在边界内" : "超出边界")}");
                
                yield return new WaitForSeconds(0.05f);
            }
            
            // 测试索引坐标边界检查
            Debug.Log("索引坐标边界检查:");
            var indexTests = new IndexCoordinate[]
            {
                new IndexCoordinate(-1),
                new IndexCoordinate(0),
                new IndexCoordinate(_gridWidth * _gridHeight - 1),
                new IndexCoordinate(_gridWidth * _gridHeight)
            };
            
            foreach (var index in indexTests)
            {
                bool inBounds = _gridManager.IsInBounds(index);
                Debug.Log($"索引 {index.Index}: {(inBounds ? "在边界内" : "超出边界")}");
                
                yield return new WaitForSeconds(0.05f);
            }
        }
        
        /// <summary>
        /// 手动运行测试
        /// </summary>
        [ContextMenu("Run All Tests")]
        public void RunTestsManually()
        {
            StartCoroutine(RunAllTests());
        }
        
        /// <summary>
        /// 开始连续测试
        /// </summary>
        [ContextMenu("Start Continuous Tests")]
        public void StartContinuousTests()
        {
            _runContinuousTests = true;
            StartCoroutine(ContinuousTestLoop());
        }
        
        /// <summary>
        /// 停止连续测试
        /// </summary>
        [ContextMenu("Stop Continuous Tests")]
        public void StopContinuousTests()
        {
            _runContinuousTests = false;
        }
        
        /// <summary>
        /// 性能测试
        /// </summary>
        [ContextMenu("Run Performance Test")]
        public void RunPerformanceTest()
        {
            StartCoroutine(PerformanceTest());
        }
        
        /// <summary>
        /// 性能测试协程
        /// </summary>
        private IEnumerator PerformanceTest()
        {
            Debug.Log("=== 开始性能测试 ===");
            
            int iterations = 1000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // 测试坐标转换性能
            for (int i = 0; i < iterations; i++)
            {
                var cartesian = new CartesianCoordinate(Random.Range(0, _gridWidth), Random.Range(0, _gridHeight));
                var world = _gridManager.CartesianToWorld(cartesian);
                var backToCartesian = _gridManager.WorldToCartesian(world);
            }
            
            stopwatch.Stop();
            Debug.Log($"坐标转换性能: {iterations} 次转换耗时 {stopwatch.ElapsedMilliseconds} ms");
            
            yield return new WaitForSeconds(0.1f);
            
            // 测试邻居查找性能
            stopwatch.Restart();
            
            for (int i = 0; i < iterations; i++)
            {
                var position = new CartesianCoordinate(Random.Range(0, _gridWidth), Random.Range(0, _gridHeight));
                var neighbors = _gridManager.GetNeighbors8(position);
            }
            
            stopwatch.Stop();
            Debug.Log($"邻居查找性能: {iterations} 次查找耗时 {stopwatch.ElapsedMilliseconds} ms");
            
            Debug.Log("=== 性能测试完成 ===");
        }
    }
}