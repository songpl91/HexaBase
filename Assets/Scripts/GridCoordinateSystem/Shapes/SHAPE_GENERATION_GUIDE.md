# 网格形状生成系统使用指南

## 概述

网格形状生成系统为格子系统提供了强大的自定义图案生成功能，支持多种几何形状的坐标数据生成，包括十字形、菱形、圆形、椭圆形、长方形、正方形、扇形等。

## 核心组件

### 1. GridShapeGenerator（形状生成器）
- **功能**: 生成各种几何形状的坐标集合
- **位置**: `Assets/Scripts/GridCoordinateSystem/Shapes/GridShapeGenerator.cs`
- **特点**: 静态类，提供纯函数式的形状生成方法

### 2. ShapeParameters（形状参数）
- **功能**: 配置形状的各种参数
- **支持参数**:
  - 形状类型（ShapeType）
  - 填充模式（FillMode）
  - 中心点坐标
  - 尺寸参数（半径、宽度、高度、厚度）
  - 角度参数（起始角度、结束角度、角数）

### 3. ShapePatternDemo（形状演示器）
- **功能**: 可视化展示和交互测试各种形状
- **位置**: `Assets/Scripts/GridCoordinateSystem/Examples/ShapePatternDemo.cs`
- **场景**: `Assets/Scenes/ShapePatternDemo.unity`

## 支持的形状类型

### 基础形状

#### 1. 圆形（Circle）
```csharp
// 创建填充圆形
var circleParams = ShapeParameters.CreateCircle(
    center: new CartesianCoordinate(10, 10),
    radius: 5,
    fillMode: FillMode.Filled
);
var coordinates = GridShapeGenerator.GenerateShape(circleParams);
```

**特点**:
- 基于欧几里得距离计算
- 支持填充、轮廓、空心三种模式
- 适用于范围查询、爆炸效果等

#### 2. 矩形（Rectangle）
```csharp
// 创建矩形
var rectParams = ShapeParameters.CreateRectangle(
    center: new CartesianCoordinate(10, 10),
    width: 8,
    height: 6,
    fillMode: FillMode.Filled
);
var coordinates = GridShapeGenerator.GenerateShape(rectParams);
```

**特点**:
- 支持任意宽高比
- 可设置填充模式和边框厚度
- 适用于建筑物、区域划分等

#### 3. 菱形（Diamond）
```csharp
// 创建菱形
var diamondParams = new ShapeParameters
{
    shapeType = ShapeType.Diamond,
    center = new CartesianCoordinate(10, 10),
    radius = 4,
    fillMode = FillMode.Filled
};
var coordinates = GridShapeGenerator.GenerateShape(diamondParams);
```

**特点**:
- 基于曼哈顿距离计算
- 45度旋转的正方形
- 适用于移动范围、影响区域等

#### 4. 十字形（Cross）
```csharp
// 创建十字形
var crossParams = ShapeParameters.CreateCross(
    center: new CartesianCoordinate(10, 10),
    radius: 6,
    thickness: 2
);
var coordinates = GridShapeGenerator.GenerateShape(crossParams);
```

**特点**:
- 水平和垂直线的组合
- 可调节厚度
- 适用于十字攻击、道路交叉等

### 高级形状

#### 5. 椭圆形（Ellipse）
```csharp
// 创建椭圆
var ellipseParams = new ShapeParameters
{
    shapeType = ShapeType.Ellipse,
    center = new CartesianCoordinate(10, 10),
    width = 12,
    height = 8,
    fillMode = FillMode.Filled
};
var coordinates = GridShapeGenerator.GenerateShape(ellipseParams);
```

**特点**:
- 支持不同的长轴和短轴
- 基于椭圆方程计算
- 适用于椭圆形区域、轨道等

#### 6. 扇形（Sector）
```csharp
// 创建扇形
var sectorParams = ShapeParameters.CreateSector(
    center: new CartesianCoordinate(10, 10),
    radius: 6,
    startAngle: 0,      // 起始角度（度）
    endAngle: 90,       // 结束角度（度）
    fillMode: FillMode.Filled
);
var coordinates = GridShapeGenerator.GenerateShape(sectorParams);
```

**特点**:
- 支持任意角度范围
- 基于极坐标计算
- 适用于视野范围、攻击扇形等

#### 7. 三角形（Triangle）
```csharp
// 创建三角形
var triangleParams = new ShapeParameters
{
    shapeType = ShapeType.Triangle,
    center = new CartesianCoordinate(10, 10),
    radius = 6,
    fillMode = FillMode.Filled
};
var coordinates = GridShapeGenerator.GenerateShape(triangleParams);
```

**特点**:
- 等边三角形
- 支持填充和轮廓模式
- 适用于箭头指示、三角形区域等

#### 8. 六边形（Hexagon）
```csharp
// 创建六边形
var hexParams = new ShapeParameters
{
    shapeType = ShapeType.Hexagon,
    center = new CartesianCoordinate(10, 10),
    radius = 5,
    fillMode = FillMode.Filled
};
var coordinates = GridShapeGenerator.GenerateShape(hexParams);
```

**特点**:
- 正六边形
- 适用于六边形网格、蜂窝结构等

#### 9. 星形（Star）
```csharp
// 创建星形
var starParams = new ShapeParameters
{
    shapeType = ShapeType.Star,
    center = new CartesianCoordinate(10, 10),
    radius = 6,
    points = 5,         // 星形角数
    fillMode = FillMode.Filled
};
var coordinates = GridShapeGenerator.GenerateShape(starParams);
```

**特点**:
- 可调节角数（3-12角）
- 内外半径自动计算
- 适用于装饰图案、特殊标记等

#### 10. 直线（Line）
```csharp
// 创建直线
var lineParams = new ShapeParameters
{
    shapeType = ShapeType.Line,
    center = new CartesianCoordinate(10, 10),
    radius = 8,         // 线段长度的一半
    startAngle = 45     // 线段角度
};
var coordinates = GridShapeGenerator.GenerateShape(lineParams);
```

**特点**:
- 任意角度的直线段
- 基于中心点和角度生成
- 适用于射线、路径等

## 填充模式

### 1. Filled（填充）
- 生成形状内部的所有坐标点
- 适用于实体区域、完整覆盖等场景

### 2. Outline（轮廓）
- 仅生成形状边界的坐标点
- 适用于边框显示、轮廓检测等场景

### 3. Hollow（空心）
- 生成外边界和内边界之间的坐标点
- 可通过thickness参数控制厚度
- 适用于环形区域、边框带等场景

## 使用示例

### 基础用法
```csharp
using GridCoordinateSystem.Shapes;
using GridCoordinateSystem.Coordinates;

// 1. 创建形状参数
var shapeParams = new ShapeParameters
{
    shapeType = ShapeType.Circle,
    center = new CartesianCoordinate(0, 0),
    radius = 5,
    fillMode = FillMode.Filled
};

// 2. 生成坐标
var coordinates = GridShapeGenerator.GenerateShape(shapeParams);

// 3. 使用坐标
foreach (var coord in coordinates)
{
    Debug.Log($"坐标: ({coord.X}, {coord.Y})");
    // 在此坐标放置游戏对象、标记等
}
```

### 批量生成多个形状
```csharp
// 创建多个形状参数
var shapes = new ShapeParameters[]
{
    ShapeParameters.CreateCircle(new CartesianCoordinate(0, 0), 3),
    ShapeParameters.CreateRectangle(new CartesianCoordinate(10, 0), 6, 4),
    ShapeParameters.CreateCross(new CartesianCoordinate(20, 0), 4)
};

// 批量生成
var allCoordinates = new List<CartesianCoordinate>();
foreach (var shape in shapes)
{
    var coords = GridShapeGenerator.GenerateShape(shape);
    allCoordinates.AddRange(coords);
}
```

### 动态形状生成
```csharp
public class DynamicShapeExample : MonoBehaviour
{
    [SerializeField] private ShapeParameters shapeParams;
    private List<CartesianCoordinate> currentShape;
    
    void Update()
    {
        // 根据游戏状态动态调整形状
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shapeParams.radius++;
            RegenerateShape();
        }
    }
    
    void RegenerateShape()
    {
        currentShape = GridShapeGenerator.GenerateShape(shapeParams);
        // 更新可视化或游戏逻辑
        UpdateVisualization();
    }
}
```

## 演示系统使用

### 快速开始
1. 打开场景：`Assets/Scenes/ShapePatternDemo.unity`
2. 运行游戏
3. 使用鼠标和键盘交互

### 交互控制
- **鼠标点击**: 设置形状中心点
- **空格键**: 切换到下一个演示形状
- **R键**: 重新生成当前形状
- **G键**: 切换网格显示
- **F键**: 切换填充模式（填充/轮廓/空心）
- **+/-键**: 调整形状大小
- **数字键1-9,0**: 快速切换形状类型
- **自动演示按钮**: 开启/关闭自动演示模式

### 数据导出
- 点击"导出坐标数据"按钮
- 坐标数据将输出到控制台和文件
- 文件保存在`Application.persistentDataPath`目录

## 性能优化建议

### 1. 缓存结果
```csharp
// 缓存常用形状
private static Dictionary<string, List<CartesianCoordinate>> shapeCache = 
    new Dictionary<string, List<CartesianCoordinate>>();

public static List<CartesianCoordinate> GetCachedShape(ShapeParameters parameters)
{
    string key = $"{parameters.shapeType}_{parameters.radius}_{parameters.fillMode}";
    
    if (!shapeCache.ContainsKey(key))
    {
        shapeCache[key] = GridShapeGenerator.GenerateShape(parameters);
    }
    
    return new List<CartesianCoordinate>(shapeCache[key]);
}
```

### 2. 预计算常用形状
```csharp
// 在游戏启动时预计算
void PrecomputeCommonShapes()
{
    var commonShapes = new[]
    {
        ShapeParameters.CreateCircle(CartesianCoordinate.Zero, 3),
        ShapeParameters.CreateCircle(CartesianCoordinate.Zero, 5),
        ShapeParameters.CreateRectangle(CartesianCoordinate.Zero, 4, 4),
        // ... 更多常用形状
    };
    
    foreach (var shape in commonShapes)
    {
        GridShapeGenerator.GenerateShape(shape);
    }
}
```

### 3. 异步生成大型形状
```csharp
public async Task<List<CartesianCoordinate>> GenerateShapeAsync(ShapeParameters parameters)
{
    return await Task.Run(() => GridShapeGenerator.GenerateShape(parameters));
}
```

## 扩展开发

### 添加自定义形状
1. 在`ShapeType`枚举中添加新类型
2. 在`GridShapeGenerator.GenerateShape`中添加case分支
3. 实现对应的生成方法

```csharp
// 示例：添加心形
case ShapeType.Heart:
    return GenerateHeart(parameters);

private static List<CartesianCoordinate> GenerateHeart(ShapeParameters parameters)
{
    var coordinates = new List<CartesianCoordinate>();
    // 心形生成逻辑
    return coordinates;
}
```

### 自定义填充算法
```csharp
public static List<CartesianCoordinate> GenerateCustomPattern(
    CartesianCoordinate center, 
    Func<CartesianCoordinate, bool> predicate)
{
    var coordinates = new List<CartesianCoordinate>();
    
    // 在指定范围内测试每个点
    for (int x = center.X - 10; x <= center.X + 10; x++)
    {
        for (int y = center.Y - 10; y <= center.Y + 10; y++)
        {
            var coord = new CartesianCoordinate(x, y);
            if (predicate(coord))
            {
                coordinates.Add(coord);
            }
        }
    }
    
    return coordinates;
}
```

## 应用场景

### 游戏开发
- **技能范围**: 法术、攻击的作用范围
- **移动区域**: 角色可移动的区域标记
- **建筑放置**: 建筑物占用的网格区域
- **地形生成**: 程序化地形的基础形状

### 算法可视化
- **路径规划**: 起点、终点、障碍物的标记
- **图论算法**: 节点连接关系的可视化
- **数据结构**: 树、图等结构的空间表示

### 教育工具
- **几何教学**: 各种几何形状的直观展示
- **数学概念**: 坐标系、函数图像等
- **编程教学**: 算法逻辑的可视化演示

## 常见问题

### Q: 如何生成不规则形状？
A: 可以组合多个基础形状，或使用自定义predicate函数：
```csharp
// 组合形状
var circle = GridShapeGenerator.GenerateShape(circleParams);
var rectangle = GridShapeGenerator.GenerateShape(rectParams);
var combined = circle.Union(rectangle).ToList();

// 自定义形状
var custom = GenerateCustomPattern(center, coord => 
    Math.Sin(coord.X * 0.5) > coord.Y * 0.1);
```

### Q: 如何优化大型形状的生成性能？
A: 
1. 使用缓存避免重复计算
2. 异步生成避免阻塞主线程
3. 分块生成大型形状
4. 使用更高效的算法（如Bresenham算法绘制圆形）

### Q: 如何处理形状超出网格边界？
A: 在生成后进行边界检查：
```csharp
var coordinates = GridShapeGenerator.GenerateShape(parameters);
var validCoordinates = coordinates.Where(coord => 
    coord.X >= 0 && coord.X < gridWidth && 
    coord.Y >= 0 && coord.Y < gridHeight).ToList();
```

## 总结

网格形状生成系统提供了完整的解决方案，用于在格子系统中生成各种自定义图案。通过灵活的参数配置和多样的形状类型，可以满足大部分游戏开发和算法可视化的需求。系统设计注重性能和可扩展性，支持自定义形状和填充算法的开发。