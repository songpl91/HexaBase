# 四边形网格坐标系统 (GridCoordinateSystem)

一个功能完整、高性能的四边形网格坐标系统，专为Unity游戏开发设计，提供了基于格子游戏的基础底板功能。

## 🌟 主要特性

### 🎯 核心功能
- **多种坐标系统支持**: 直角坐标、索引坐标、世界坐标
- **统一接口设计**: 通过 `IGridCoordinate` 接口提供一致的操作体验
- **高效坐标转换**: 世界坐标 ↔ 直角坐标 ↔ 索引坐标
- **邻居查找**: 支持4邻居和8邻居查找
- **距离计算**: 曼哈顿距离、欧几里得距离、切比雪夫距离
- **路径查找**: 基于布雷森汉姆算法的直线路径
- **范围查询**: 矩形范围、圆形范围、曼哈顿距离范围
- **边界检测**: 灵活的边界处理和环绕模式

### ⚡ 性能优化
- **缓存系统**: LRU缓存机制，减少重复计算
- **对象池**: 减少GC压力，提高运行时性能
- **批量操作**: 支持批量坐标转换和查询
- **内存优化**: 结构体设计，避免不必要的内存分配

### 🎨 可视化调试
- **实时网格显示**: 可视化网格线和坐标标签
- **交互式调试**: 鼠标悬停、点击选择、路径显示
- **性能监控**: 缓存命中率、对象池状态
- **自定义样式**: 可配置的颜色、字体、显示范围

### 🔧 易于使用
- **单例管理器**: `GridCoordinateManager` 统一管理所有功能
- **Inspector配置**: 可视化配置网格参数
- **示例代码**: 完整的使用示例和最佳实践
- **事件系统**: 坐标访问和配置变更事件

## 📁 项目结构

```
GridCoordinateSystem/
├── Core/                          # 核心接口和常量
│   ├── IGridCoordinate.cs         # 网格坐标统一接口
│   └── GridConstants.cs           # 系统常量和枚举
├── Coordinates/                   # 坐标系统实现
│   ├── CartesianCoordinate.cs     # 直角坐标系统
│   └── IndexCoordinate.cs         # 索引坐标系统
├── Utils/                         # 工具类
│   ├── GridConverter.cs           # 坐标转换和计算工具
│   ├── GridOptimization.cs        # 缓存和对象池
│   └── GridDebugger.cs           # 可视化调试工具
├── Examples/                      # 示例代码
│   ├── GridBasicExample.cs        # 基础功能示例
│   └── GridInteractiveExample.cs  # 交互式示例
├── GridCoordinateManager.cs       # 主管理器
└── README.md                      # 说明文档
```

## 🚀 快速开始

### 1. 基础设置

在场景中创建一个空的GameObject，添加 `GridCoordinateManager` 组件：

```csharp
// 获取管理器实例
var gridManager = GridCoordinateManager.Instance;

// 配置网格参数
gridManager.CellSize = new Vector2(1f, 1f);
gridManager.GridWidth = 20;
gridManager.GridHeight = 20;
```

### 2. 坐标转换

```csharp
// 创建直角坐标
var cartesian = new CartesianCoordinate(5, 3);

// 转换为世界坐标
Vector3 worldPos = gridManager.CartesianToWorld(cartesian);

// 转换为索引坐标
var index = gridManager.CartesianToIndex(cartesian);

// 从世界坐标转回
var backToCartesian = gridManager.WorldToCartesian(worldPos);
```

### 3. 邻居查找

```csharp
var center = new CartesianCoordinate(5, 5);

// 获取4邻居
var neighbors4 = gridManager.GetNeighbors4(center);

// 获取8邻居
var neighbors8 = gridManager.GetNeighbors8(center);
```

### 4. 距离计算

```csharp
var coord1 = new CartesianCoordinate(0, 0);
var coord2 = new CartesianCoordinate(3, 4);

// 曼哈顿距离
int manhattan = coord1.ManhattanDistance(coord2); // 7

// 欧几里得距离
float euclidean = coord1.EuclideanDistance(coord2); // 5.0

// 切比雪夫距离
int chebyshev = coord1.ChebyshevDistance(coord2); // 4
```

### 5. 路径查找

```csharp
var start = new CartesianCoordinate(0, 0);
var end = new CartesianCoordinate(5, 3);

// 获取直线路径
var path = gridManager.GetLinePath(start, end);
```

### 6. 范围查询

```csharp
var center = new CartesianCoordinate(5, 5);

// 矩形范围
var rectRange = gridManager.GetRectangleRange(center, 2, 2);

// 圆形范围
var circleRange = gridManager.GetCircleRange(center, 3);

// 曼哈顿距离范围
var manhattanRange = gridManager.GetManhattanRange(center, 2);
```

## 🎮 坐标系统详解

### 直角坐标 (CartesianCoordinate)
- **用途**: 最直观的2D网格坐标表示
- **格式**: (x, y)
- **特点**: 支持负坐标，适合无限网格
- **运算**: 支持加减乘运算和比较操作

```csharp
var coord = new CartesianCoordinate(3, 4);
var neighbor = coord + CartesianCoordinate.Right; // (4, 4)
```

### 索引坐标 (IndexCoordinate)
- **用途**: 一维数组存储的二维网格
- **格式**: index = y * width + x
- **特点**: 内存连续，访问效率高
- **限制**: 需要指定网格宽度

```csharp
var index = new IndexCoordinate(15); // 假设网格宽度为5，对应(0, 3)
var cartesian = gridManager.IndexToCartesian(index); // (0, 3)
```

### 世界坐标 (World Coordinate)
- **用途**: Unity世界空间中的实际位置
- **格式**: Vector3(x, y, z)
- **特点**: 与Unity渲染系统直接对应
- **转换**: 通过网格大小和原点进行转换

## 🔧 高级功能

### 缓存系统

```csharp
// 启用缓存
gridManager.EnableCache = true;
gridManager.CacheSize = 1000;

// 查看缓存统计
Debug.Log(gridManager.GetCacheStats());
```

### 对象池

```csharp
// 从对象池获取列表
var list = GridObjectPool.GetList<CartesianCoordinate>();

// 使用完毕后返回
GridObjectPool.ReturnList(list);
```

### 事件系统

```csharp
// 监听坐标访问事件
gridManager.OnCoordinateAccessed += (coord) => {
    Debug.Log($"访问了坐标: {coord}");
};

// 监听配置变更事件
gridManager.OnGridConfigChanged += () => {
    Debug.Log("网格配置已更改");
};
```

## 🎨 可视化调试

### 启用调试器

1. 在场景中添加 `GridDebugger` 组件
2. 配置调试参数：

```csharp
var debugger = FindObjectOfType<GridDebugger>();
debugger.EnableDebug = true;
debugger.ShowGrid = true;
debugger.ShowCoordinates = true;
```

### 调试控制

- **F1**: 切换调试显示
- **F2**: 切换网格显示
- **F3**: 切换坐标标签
- **鼠标**: 悬停显示坐标信息
- **左键**: 选择坐标并显示邻居

### 自定义调试范围

```csharp
debugger.SetDebugRange(20, 20, new Vector2Int(0, 0));
```

## 📊 性能优化建议

### 1. 合理使用缓存
```csharp
// 对于频繁访问的坐标转换，启用缓存
gridManager.EnableCache = true;
gridManager.CacheSize = 500; // 根据实际需求调整
```

### 2. 批量操作
```csharp
// 批量转换坐标
var cartesianList = new List<CartesianCoordinate> { /* ... */ };
var worldPositions = GridConverter.CartesianToWorldBatch(cartesianList, gridManager.CellSize, gridManager.GridOrigin);
```

### 3. 对象池使用
```csharp
// 使用对象池减少GC压力
var tempList = GridObjectPool.GetList<CartesianCoordinate>();
// ... 使用列表
GridObjectPool.ReturnList(tempList);
```

### 4. 边界检查优化
```csharp
// 在循环中先检查边界
if (gridManager.IsInBounds(coord))
{
    // 执行操作
}
```

## 🎯 使用场景

### 策略游戏
- 单位移动和定位
- 建筑放置
- 地形编辑
- 战斗范围计算

### 益智游戏
- 方块消除
- 拼图游戏
- 数独类游戏
- 迷宫生成

### RPG游戏
- 地图编辑器
- 技能范围显示
- 物品摆放
- 战斗系统

### 模拟游戏
- 城市建设
- 农场管理
- 工厂布局
- 资源管理

## 🔄 扩展性

### 自定义坐标系统

实现 `IGridCoordinate` 接口来创建自定义坐标系统：

```csharp
public struct CustomCoordinate : IGridCoordinate
{
    // 实现接口方法
    public Vector3 ToWorldPosition(Vector2 cellSize, Vector2 origin) { /* ... */ }
    public List<IGridCoordinate> GetNeighbors4() { /* ... */ }
    // ... 其他方法
}
```

### 自定义距离算法

```csharp
public static float CustomDistance(CartesianCoordinate a, CartesianCoordinate b)
{
    // 自定义距离计算逻辑
    return Mathf.Sqrt(Mathf.Pow(a.X - b.X, 2) + Mathf.Pow(a.Y - b.Y, 2));
}
```

### 自定义路径算法

```csharp
public static List<CartesianCoordinate> CustomPathfinding(CartesianCoordinate start, CartesianCoordinate end)
{
    // 实现A*、Dijkstra等算法
    return new List<CartesianCoordinate>();
}
```

## ⚠️ 注意事项

### 1. 坐标系统选择
- **直角坐标**: 适合无限网格或需要负坐标的场景
- **索引坐标**: 适合固定大小网格和数组存储
- **世界坐标**: 适合与Unity渲染系统交互

### 2. 性能考虑
- 大量坐标转换时启用缓存
- 避免在Update中进行复杂的网格计算
- 使用对象池减少内存分配

### 3. 边界处理
- 根据游戏需求选择合适的边界模式
- 环绕模式适合无缝地图
- 严格边界适合有限地图

### 4. 调试模式
- 发布版本中禁用调试功能
- 调试范围不要设置过大，影响性能

## 🤝 最佳实践

### 1. 初始化顺序
```csharp
void Start()
{
    // 1. 获取管理器
    var gridManager = GridCoordinateManager.Instance;
    
    // 2. 配置参数
    gridManager.CellSize = new Vector2(1f, 1f);
    gridManager.GridWidth = 20;
    gridManager.GridHeight = 20;
    
    // 3. 启用优化功能
    gridManager.EnableCache = true;
    gridManager.EnableObjectPool = true;
    
    // 4. 初始化游戏逻辑
    InitializeGame();
}
```

### 2. 错误处理
```csharp
public bool TryPlaceObject(CartesianCoordinate coord)
{
    if (!gridManager.IsInBounds(coord))
    {
        Debug.LogWarning($"坐标 {coord} 超出边界");
        return false;
    }
    
    if (IsOccupied(coord))
    {
        Debug.LogWarning($"坐标 {coord} 已被占用");
        return false;
    }
    
    // 放置对象
    PlaceObjectAt(coord);
    return true;
}
```

### 3. 资源管理
```csharp
void OnDestroy()
{
    // 清理缓存
    if (gridManager != null)
    {
        gridManager.ClearCache();
    }
    
    // 清理对象池
    GridObjectPool.ClearAllPools();
}
```

## 📈 版本历史

### v1.0.0 (当前版本)
- ✅ 完整的四边形网格坐标系统
- ✅ 多种坐标系统支持
- ✅ 高性能缓存和对象池
- ✅ 可视化调试工具
- ✅ 完整的示例代码
- ✅ 详细的文档说明

## 🔮 未来计划

### v1.1.0
- [ ] A*路径查找算法
- [ ] 更多距离计算类型
- [ ] 网格序列化支持
- [ ] 多线程优化

### v1.2.0
- [ ] 可视化编辑器工具
- [ ] 更多坐标系统支持
- [ ] 性能分析工具
- [ ] 单元测试覆盖

## 📞 技术支持

如果您在使用过程中遇到问题或有改进建议，请：

1. 查看示例代码和文档
2. 检查控制台错误信息
3. 确认网格配置是否正确
4. 验证坐标是否在有效范围内

---

**四边形网格坐标系统** - 为基于格子的游戏提供强大而灵活的基础底板功能！ 🎮✨