# 距离计算对比演示

## 📐 三种距离的详细说明

### 1. 曼哈顿距离 (Manhattan Distance)
**定义**: 两点之间沿坐标轴方向的距离之和  
**公式**: `|x1 - x2| + |y1 - y2|`  
**别名**: 城市街区距离、出租车距离、L1距离  

**特点**:
- 只能沿水平或垂直方向移动（4方向）
- 形成菱形的等距离区域
- 计算简单，性能最优
- 适用于网格移动限制的场景

**应用场景**:
- 网格游戏中的移动（如俄罗斯方块）
- 城市导航（只能沿街道行走）
- 回合制策略游戏
- 路径规划算法

### 2. 欧几里得距离 (Euclidean Distance)
**定义**: 两点之间的直线距离  
**公式**: `√[(x1 - x2)² + (y1 - y2)²]`  
**别名**: 直线距离、L2距离  

**特点**:
- 最直观的距离概念
- 形成圆形的等距离区域
- 允许任意方向移动
- 计算复杂度较高（涉及开方运算）

**应用场景**:
- 物理模拟
- 射击游戏中的射程计算
- 自由移动的游戏
- 碰撞检测

### 3. 切比雪夫距离 (Chebyshev Distance)
**定义**: 两点坐标差值的最大值  
**公式**: `max(|x1 - x2|, |y1 - y2|)`  
**别名**: 棋盘距离、国王距离、L∞距离  

**特点**:
- 允许对角线移动（8方向）
- 形成正方形的等距离区域
- 计算简单，只需比较大小
- 适用于允许对角移动的场景

**应用场景**:
- 国际象棋中国王的移动
- 策略游戏中的8方向移动
- 图像处理中的邻域操作
- 某些路径查找算法

## 🎯 数值对比示例

假设从原点 (0,0) 到点 (3,2) 的距离：

| 距离类型 | 计算过程 | 结果 |
|---------|---------|------|
| 曼哈顿 | \|3-0\| + \|2-0\| | 5 |
| 欧几里得 | √[(3-0)² + (2-0)²] | √13 ≈ 3.61 |
| 切比雪夫 | max(\|3-0\|, \|2-0\|) | max(3,2) = 3 |

## 🎮 移动模式对比

### 曼哈顿距离 - 4方向移动
```
  ↑
← ● →
  ↓
```
只能上下左右移动，形成菱形区域

### 欧几里得距离 - 自由移动
```
↖ ↑ ↗
← ● →
↙ ↓ ↘
```
可以任意方向移动，形成圆形区域

### 切比雪夫距离 - 8方向移动
```
↖ ↑ ↗
← ● →
↙ ↓ ↘
```
可以8个方向移动，但每步距离相等，形成正方形区域

## 🔍 可视化演示使用说明

### 快速开始
1. 打开 Unity 项目
2. 加载场景 `Assets/Scenes/DistanceComparisonDemo.unity`
3. 点击播放按钮
4. 观察Scene视图中的距离可视化

### 交互操作
- **鼠标点击**: 在场景中点击可以更改中心点位置
- **Scene视图**: 可以看到三种距离的可视化图形
- **GUI面板**: 显示实时的距离计算结果

### 可视化元素
- **红色菱形**: 曼哈顿距离等距线
- **绿色圆形**: 欧几里得距离等距线  
- **蓝色正方形**: 切比雪夫距离等距线
- **黄色球体**: 当前中心点
- **灰色网格**: 坐标网格

### 参数调整
在Inspector面板中可以调整：
- `Grid Width/Height`: 网格大小
- `Cell Size`: 网格单元大小
- `Center Point`: 中心点坐标
- `Max Distance`: 最大显示距离
- `Show Manhattan/Euclidean/Chebyshev`: 控制显示哪种距离
- 各种颜色设置

## 💻 代码实现

在您的网格系统中，三种距离都已经实现：

```csharp
// 曼哈顿距离
public int ManhattanDistanceTo(IGridCoordinate other)
{
    var otherCartesian = other.ToCartesian();
    return Mathf.Abs(x - otherCartesian.X) + Mathf.Abs(y - otherCartesian.Y);
}

// 欧几里得距离
public float EuclideanDistanceTo(IGridCoordinate other)
{
    var otherCartesian = other.ToCartesian();
    int dx = x - otherCartesian.X;
    int dy = y - otherCartesian.Y;
    return Mathf.Sqrt(dx * dx + dy * dy);
}

// 切比雪夫距离
public int ChebyshevDistanceTo(IGridCoordinate other)
{
    var otherCartesian = other.ToCartesian();
    return Mathf.Max(Mathf.Abs(x - otherCartesian.X), Mathf.Abs(y - otherCartesian.Y));
}
```

## 🎯 选择建议

### 何时使用曼哈顿距离
- 网格限制移动的游戏
- 需要高性能的场景
- 模拟城市街道导航
- 回合制策略游戏

### 何时使用欧几里得距离
- 物理真实性要求高的场景
- 自由移动的游戏
- 射程和碰撞计算
- 需要精确距离的场合

### 何时使用切比雪夫距离
- 允许对角移动的游戏
- 棋类游戏
- 需要8方向等距的场景
- 某些AI寻路算法

## 🔧 性能对比

| 距离类型 | 计算复杂度 | 性能 | 精确度 |
|---------|-----------|------|--------|
| 曼哈顿 | O(1) | 最快 | 网格精确 |
| 切比雪夫 | O(1) | 快 | 8方向精确 |
| 欧几里得 | O(1) | 较慢 | 最精确 |

## 📚 扩展阅读

- **Minkowski距离**: 这三种距离都是Minkowski距离的特例
  - p=1: 曼哈顿距离
  - p=2: 欧几里得距离  
  - p=∞: 切比雪夫距离

- **应用领域**:
  - 机器学习中的距离度量
  - 图像处理中的像素距离
  - 数据挖掘中的相似性计算
  - 游戏AI中的决策算法