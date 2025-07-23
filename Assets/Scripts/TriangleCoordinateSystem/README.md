# 三角形网格坐标系统 (Triangle Grid Coordinate System)

一个完整的、高性能的三角形网格坐标系统实现，专为Unity游戏开发设计。

## 主要特性

### 🔺 多种坐标系统支持
- **轴向坐标系统** (Axial Coordinate System) - 使用 q, r 两个轴
- **立方坐标系统** (Cube Coordinate System) - 使用 x, y, z 三个轴 (x + y + z = 0)
- **偏移坐标系统** (Offset Coordinate System) - 传统的行列坐标系统

### 🎯 统一接口设计
- `ITriangleCoordinate` 统一接口，支持所有坐标系统
- 无缝的坐标系统间转换
- 一致的API设计模式

### ⚡ 高效坐标转换
- 世界坐标 ↔ 三角形坐标的双向转换
- 支持不同大小的三角形网格
- 精确的浮点数处理

### 🔍 邻居查找
- **边邻居** - 共享边的相邻三角形 (3个)
- **顶点邻居** - 共享顶点的相邻三角形 (9个)
- 高效的邻居查找算法

### 📏 距离计算
- 三角形网格距离计算
- 支持不同朝向的三角形
- 优化的距离算法

### 🛣️ 路径查找
- 两点间直线路径计算
- 支持路径可视化
- 路径优化算法

### 📐 范围查询
- 指定半径内的所有坐标
- 环形坐标查询
- 高效的范围计算

### 🔒 边界检测
- 坐标有效性验证
- 网格边界检查
- 安全的坐标操作

## 性能优化

### 🚀 缓存系统
- **世界坐标缓存** - 避免重复的坐标转换计算
- **邻居查找缓存** - 缓存邻居查找结果
- **距离计算缓存** - 缓存距离计算结果
- 智能缓存管理和清理

### 🏊 对象池
- 坐标对象复用
- 减少GC压力
- 提高运行时性能

### 📊 批量操作
- 批量坐标转换
- 批量距离计算
- 批量邻居查找

## 开发工具

### 🐛 调试工具
- 详细的调试信息输出
- 坐标转换验证
- 性能分析工具

### 🎨 可视化支持
- 三角形网格绘制
- 坐标标签显示
- 路径和范围可视化
- 邻居连接可视化
- 自定义颜色配置

### 📈 性能分析
- 缓存命中率统计
- 内存使用监控
- 性能基准测试

### ✅ 验证测试
- 坐标转换正确性验证
- 邻居查找验证
- 距离计算验证

## 项目结构

```
TriangleCoordinateSystem/
├── Core/                           # 核心接口和常量
│   ├── ITriangleCoordinate.cs     # 三角形坐标统一接口
│   └── TriangleConstants.cs       # 三角形网格常量定义
├── Coordinates/                    # 坐标系统实现
│   ├── TriangleAxialCoordinate.cs # 轴向坐标系统
│   ├── TriangleCubeCoordinate.cs  # 立方坐标系统
│   └── TriangleOffsetCoordinate.cs# 偏移坐标系统
├── Utils/                          # 工具类
│   ├── TriangleConverter.cs       # 坐标转换工具
│   ├── TriangleOptimization.cs    # 性能优化工具
│   └── TriangleDebugger.cs        # 调试和可视化工具
├── TriangleCoordinateManager.cs    # 坐标系统管理器
└── README.md                       # 项目文档
```

## 使用场景

### 🎮 游戏类型
- **策略游戏** - 回合制策略、战棋类游戏
- **塔防游戏** - 三角形网格的塔防布局
- **拼图游戏** - 三角形拼图、消除类游戏
- **模拟游戏** - 城市建设、资源管理
- **RPG游戏** - 地图系统、战斗系统

### 🗺️ 地图系统
- 三角形地形网格
- 不规则地形表示
- 高密度网格布局
- 复杂地形建模

### 🎯 游戏机制
- 移动和路径规划
- 区域控制和影响范围
- 资源分布和管理
- 战斗范围计算

## 快速开始

### 1. 基本设置

```csharp
// 在场景中添加 TriangleCoordinateManager
var manager = gameObject.AddComponent<TriangleCoordinateManager>();
manager.TriangleSize = 1.0f;
manager.GridOrigin = Vector3.zero;
```

### 2. 创建坐标

```csharp
// 创建轴向坐标
var axial = manager.CreateAxial(2, -1);

// 创建立方坐标
var cube = manager.CreateCube(2, -1, -1);

// 创建偏移坐标
var offset = manager.CreateOffset(3, 1);
```

### 3. 坐标转换

```csharp
// 世界坐标转三角形坐标
Vector3 worldPos = new Vector3(1.5f, 0.866f, 0);
var triangleCoord = manager.WorldToAxial(worldPos);

// 三角形坐标转世界坐标
Vector3 worldPosition = manager.CoordinateToWorld(triangleCoord);
```

### 4. 邻居查找

```csharp
// 获取边邻居
var neighbors = manager.GetNeighbors(axial);

// 获取顶点邻居
var vertexNeighbors = manager.GetVertexNeighbors(axial);
```

### 5. 距离和路径

```csharp
// 计算距离
int distance = manager.GetDistance(axial1, axial2);

// 获取路径
var path = manager.GetPath(start, end);

// 获取范围内坐标
var range = manager.GetCoordinatesInRange(center, 3);
```

### 6. 可视化

```csharp
// 绘制三角形
manager.DrawTriangle(coordinate);

// 绘制路径
manager.DrawPath(path, Color.green);

// 绘制范围
manager.DrawRange(center, radius, Color.yellow);
```

## 功能特性

### 三角形朝向
- **向上三角形** (Upward) - 顶点朝上
- **向下三角形** (Downward) - 顶点朝下
- 自动朝向检测和处理

### 邻居方向
- **EdgeRight** - 右边邻居
- **EdgeBottomLeft** - 左下邻居  
- **EdgeBottomRight** - 右下邻居
- 精确的方向计算

### 坐标验证
- 立方坐标约束验证 (x + y + z = 0)
- 坐标范围检查
- 输入参数验证

## 性能优化

### 缓存配置
```csharp
// 启用缓存
manager.CacheEnabled = true;

// 预热缓存
manager.WarmupCache(center, radius);

// 获取缓存统计
string stats = manager.GetCacheStats();

// 清理缓存
manager.ClearCaches();
```

### 批量操作
```csharp
// 批量坐标转换
var coordinates = new List<TriangleAxialCoordinate>();
var worldPositions = TriangleOptimization.BatchWorldPositions(coordinates, triangleSize);

// 批量距离计算
var distances = TriangleOptimization.BatchDistances(fromCoords, toCoords);
```

## 调试和可视化

### 调试模式
```csharp
// 启用调试
TriangleDebugger.DebugEnabled = true;

// 验证坐标
bool isValid = manager.ValidateCoordinate(coordinate);

// 输出坐标信息
manager.LogCoordinateInfo(coordinate);
```

### 可视化配置
```csharp
// 配置可视化
TriangleDebugger.ShowGrid = true;
TriangleDebugger.ShowLabels = true;
TriangleDebugger.ShowTriangleFill = false;
TriangleDebugger.GridColor = Color.gray;
TriangleDebugger.UpwardTriangleColor = Color.blue;
TriangleDebugger.DownwardTriangleColor = Color.red;
```

## 最佳实践

### 1. 统一接口设计
- 使用 `ITriangleCoordinate` 接口进行编程
- 避免直接依赖具体的坐标类型
- 利用接口的多态性

### 2. 混合使用策略
- 根据具体需求选择合适的坐标系统
- 轴向坐标适合大多数计算
- 立方坐标适合复杂的几何运算
- 偏移坐标适合数组索引

### 3. 调试技巧
- 使用可视化工具验证逻辑
- 启用调试模式查看详细信息
- 利用坐标验证功能确保正确性

### 4. 性能优化
- 在频繁计算场景中启用缓存
- 使用批量操作处理大量数据
- 定期清理缓存避免内存泄漏
- 合理配置对象池大小

## 注意事项

### 坐标系统选择
- **轴向坐标** - 简单直观，适合大多数场景
- **立方坐标** - 数学运算方便，适合复杂计算
- **偏移坐标** - 类似传统网格，适合数组操作

### 性能考虑
- 缓存会占用额外内存
- 对象池需要合理配置大小
- 批量操作适合大数据量场景

### 精度问题
- 浮点数比较使用 EPSILON
- 世界坐标转换可能有精度损失
- 建议使用整数坐标进行逻辑计算

## 未来计划

### 功能扩展
- [ ] A* 路径查找算法
- [ ] 地形高度支持
- [ ] 多层网格系统
- [ ] 网格变形支持

### 性能优化
- [ ] SIMD 指令优化
- [ ] 多线程支持
- [ ] GPU 计算支持
- [ ] 内存池优化

### 工具改进
- [ ] Unity Editor 集成
- [ ] 可视化编辑器
- [ ] 性能分析器
- [ ] 自动化测试

---

**三角形网格坐标系统** 为Unity游戏开发提供了完整的三角形网格解决方案，具有高性能、易用性和可扩展性的特点。无论是简单的拼图游戏还是复杂的策略游戏，都能提供强大的支持。