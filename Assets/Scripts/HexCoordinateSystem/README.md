# 六边形坐标系统 (Hex Coordinate System)

这是一个完整的Unity六边形坐标系统实现，基于最佳实践设计，提供高性能、易用且功能完整的六边形网格坐标管理解决方案。

## 功能特性

### 🎯 核心功能
- **多种坐标系统支持**：轴向坐标、立方坐标、偏移坐标（奇数列/偶数列）、双宽坐标
- **统一接口设计**：`IHexCoordinate` 接口确保所有坐标系统的一致性
- **高效转换**：各坐标系统间的快速转换
- **距离计算**：精确的六边形距离计算
- **路径查找**：两点间的直线路径计算
- **邻居查找**：获取指定坐标的相邻坐标
- **范围查询**：获取指定范围内的所有坐标

### ⚡ 性能优化
- **对象池管理**：减少GC压力，提高性能
- **坐标缓存**：缓存频繁计算的结果
- **批量操作**：支持批量坐标转换
- **内存优化**：智能的内存管理策略

### 🛠️ 开发工具
- **调试工具**：详细的调试信息输出
- **可视化支持**：Scene视图中的网格和路径可视化
- **性能分析**：内置的性能基准测试
- **验证测试**：坐标转换正确性验证

## 文件结构

```
HexCoordinateSystem/
├── Core/                           # 核心接口和常量
│   ├── IHexCoordinate.cs          # 统一坐标接口
│   └── HexConstants.cs            # 六边形常量定义
├── Coordinates/                    # 坐标系统实现
│   ├── AxialCoordinate.cs         # 轴向坐标系统
│   ├── CubeCoordinate.cs          # 立方坐标系统
│   ├── OffsetCoordinate.cs        # 偏移坐标系统
│   └── DoubledCoordinate.cs       # 双宽坐标系统
├── Utils/                          # 工具类
│   ├── HexOptimization.cs         # 对象池和缓存管理
│   ├── HexConverter.cs            # 坐标转换工具
│   └── HexDebugger.cs             # 调试和可视化工具
├── Examples/                       # 示例代码
│   └── HexCoordinateExample.cs    # 使用示例
└── HexCoordinateManager.cs        # 统一管理器
```

## 快速开始

### 1. 基础设置

在场景中创建一个空的GameObject，添加 `HexCoordinateManager` 组件：

```csharp
// 获取管理器实例
var hexManager = HexCoordinateManager.Instance;

// 配置六边形大小
hexManager.HexSize = 1.0f;

// 设置是否使用尖顶六边形（默认为true）
hexManager.UsePointyTop = true;

// 启用缓存和对象池优化
hexManager.EnableCaching = true;
hexManager.EnableObjectPool = true;
```

### 2. 创建坐标

```csharp
// 创建轴向坐标
var axial = hexManager.CreateAxial(2, -1);

// 创建立方坐标
var cube = hexManager.CreateCube(2, -1, -1);

// 创建偏移坐标
var offset = hexManager.CreateOffsetOddQ(2, 0);

// 创建双宽坐标
var doubled = hexManager.CreateDoubled(3, -1);
```

### 3. 坐标转换

```csharp
// 世界坐标转六边形坐标
Vector3 worldPos = new Vector3(1.5f, 0, 2.6f);
var hexCoord = hexManager.WorldToAxial(worldPos);

// 六边形坐标转世界坐标
var worldPosition = hexManager.AxialToWorld(hexCoord);

// 不同坐标系统间转换
var axial = new AxialCoordinate(2, -1);
var cube = axial.ToCube();
var offset = OffsetCoordinateOddQ.FromAxial(axial);
var doubled = axial.ToDoubled();
```

### 4. 距离和路径

```csharp
var start = hexManager.CreateAxial(0, 0);
var end = hexManager.CreateAxial(3, 2);

// 计算距离
int distance = hexManager.GetDistance(start, end);

// 获取路径
var path = hexManager.GetLinePath(start, end);

// 使用完后归还到对象池
hexManager.ReturnAxialList(path);
```

### 5. 邻居和范围查询

```csharp
var center = hexManager.CreateAxial(0, 0);

// 获取邻居坐标
var neighbors = hexManager.GetNeighbors(center);

// 获取范围内所有坐标
var coordinatesInRange = hexManager.GetCoordinatesInRange(center, 3);

// 记得归还列表
hexManager.ReturnAxialList(neighbors);
hexManager.ReturnAxialList(coordinatesInRange);
```

## 坐标系统详解

### 轴向坐标 (Axial Coordinate)
- **格式**：(q, r)
- **特点**：最常用，计算简单
- **适用场景**：大多数六边形网格应用

```csharp
var axial = new AxialCoordinate(2, -1);
var worldPos = axial.ToWorldPosition();
var neighbors = axial.GetNeighbors();
```

### 立方坐标 (Cube Coordinate)
- **格式**：(x, y, z) 其中 x + y + z = 0
- **特点**：对称性好，某些算法更简单
- **适用场景**：复杂的几何计算

```csharp
var cube = new CubeCoordinate(2, -1, -1);
var rotated = CubeCoordinate.Rotate(cube, 1); // 旋转60度
var reflected = CubeCoordinate.Reflect(cube, 0); // 反射
```

### 偏移坐标 (Offset Coordinate)
- **格式**：(col, row)
- **特点**：类似传统网格，易于理解
- **适用场景**：与传统2D数组结合使用

```csharp
// 奇数列偏移
var offsetOddQ = new OffsetCoordinateOddQ(2, 1);

// 偶数列偏移
var offsetEvenQ = new OffsetCoordinateEvenQ(2, 0);
```

### 双宽坐标 (Doubled Coordinate)
- **格式**：(col, row) 坐标值放大2倍
- **特点**：避免浮点运算，高精度
- **适用场景**：需要精确整数计算的场合

```csharp
var doubled = new DoubledCoordinate(4, -1);
var isValid = DoubledCoordinate.IsValidDoubledCoordinate(doubled);
```

## 性能优化

### 对象池使用

```csharp
// 获取列表（从对象池）
var list = HexObjectPool.GetAxialList(6);

// 使用列表
list.Add(new AxialCoordinate(1, 0));

// 归还列表到对象池
HexObjectPool.ReturnAxialList(list);
```

### 缓存管理

```csharp
// 创建缓存实例
var cache = new HexCoordinateCache();

// 使用缓存获取转换结果
var cube = cache.GetOrComputeCube(axial);
var worldPos = cache.GetOrComputeWorldPosition(axial);

// 清理缓存
cache.ClearCache();
```

## 调试和可视化

### 调试信息输出

```csharp
// 输出坐标详细信息
HexDebugger.LogCoordinateInfo(coordinate, "测试坐标");

// 输出路径信息
HexDebugger.LogPathInfo(path, "移动路径");

// 输出距离信息
HexDebugger.LogDistanceInfo(start, end);
```

### 性能测试

```csharp
// 运行转换性能测试
HexDebugger.BenchmarkConversions(10000);

// 运行对象池性能测试
HexDebugger.BenchmarkObjectPool(1000);

// 验证坐标转换正确性
HexDebugger.ValidateConversions(100);
```

### Scene视图可视化

```csharp
// 在OnDrawGizmos中绘制网格
HexDebugger.DrawHexGrid(center, radius, hexSize, Color.white);

// 绘制路径
HexDebugger.DrawPath(path, hexSize, Color.red);

// 绘制单个六边形
HexDebugger.DrawHexagon(worldPos, hexSize, Color.yellow);
```

## 最佳实践

### 1. 混合使用策略
- 内部计算使用轴向坐标
- 存储使用偏移坐标
- 复杂几何计算使用立方坐标
- 高精度计算使用双宽坐标

### 2. 性能优化
- 启用对象池减少GC压力
- 使用缓存避免重复计算
- 批量操作提高效率
- 及时归还对象到池中

### 3. 内存管理
- 使用完列表后及时归还对象池
- 定期清理缓存
- 避免创建大量临时对象

### 4. 调试技巧
- 使用可视化工具验证逻辑
- 运行验证测试确保正确性
- 监控性能统计信息
- 启用调试模式获取详细信息

## 示例场景

查看 `HexCoordinateExample.cs` 了解完整的使用示例，包括：

- 基础坐标操作
- 坐标系统转换
- 距离和路径计算
- 邻居和范围查询
- 性能测试和验证
- 交互式坐标获取
- 可视化展示

## 注意事项

1. **坐标有效性**：立方坐标必须满足 x + y + z = 0
2. **浮点精度**：使用 `HexConstants.EPSILON` 进行浮点比较
3. **对象池限制**：对象池有大小限制，避免无限增长
4. **缓存管理**：定期清理缓存避免内存泄漏
5. **线程安全**：当前实现不是线程安全的

## 扩展建议

- 添加A*寻路算法
- 实现六边形地图编辑器
- 支持不规则六边形网格
- 添加更多几何算法
- 实现序列化支持

---

这个六边形坐标系统提供了完整、高效、易用的解决方案，适用于各种六边形网格应用场景。通过合理使用各种坐标系统和优化功能，可以构建高性能的六边形网格游戏和应用。