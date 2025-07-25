# 四边形网格坐标系统 - 可视化验证指南

## 🎯 快速开始

### 最简单的方式
1. 在Unity中打开项目
2. 使用菜单 **Grid System → Open Demo Scene**
3. 点击播放按钮 ▶️
4. 开始交互验证！

### 或者创建新场景
1. 使用菜单 **Grid System → Create Visualization Scene**
2. 点击播放按钮 ▶️
3. 享受验证过程！

## 🎮 交互说明

### 鼠标操作
- **左键点击**: 选择网格位置
- **第一次点击**: 选择起始位置，显示邻居
- **第二次点击**: 选择目标位置，显示路径和距离

### 键盘快捷键
- **N键**: 切换邻居显示/隐藏
- **P键**: 切换路径显示/隐藏  
- **C键**: 清除所有选择

### UI控制面板
- 右上角有功能开关
- 可以切换网格、坐标、距离计算等显示
- 左上角显示实时信息

## 🔍 验证内容

### ✅ 坐标转换验证
- 直角坐标 ↔ 世界坐标 ↔ 索引坐标
- 实时显示在UI面板中
- 自动测试转换的正确性

### ✅ 邻居关系验证
- 4邻居（上下左右）
- 8邻居（包含对角线）
- 边界处理正确性
- 蓝色框高亮显示

### ✅ 距离计算验证
- 曼哈顿距离：|x1-x2| + |y1-y2|
- 欧几里得距离：√[(x1-x2)² + (y1-y2)²]
- 切比雪夫距离：max(|x1-x2|, |y1-y2|)
- 数学关系验证

### ✅ 路径查找验证
- Bresenham直线算法
- 支持对角线移动
- 紫色线条显示路径
- 箭头指示方向

### ✅ 范围查询验证
- 矩形、圆形、曼哈顿、切比雪夫范围
- 黄色框显示查询结果
- 可调节查询半径

### ✅ 边界检查验证
- 坐标有效性检查
- 边界外坐标处理
- 自动边界裁剪

## 🧪 自动化测试

### 启动时自动测试
系统会在启动时自动运行所有功能测试：
- 坐标转换测试
- 邻居查找测试
- 距离计算测试
- 路径查找测试
- 范围查询测试
- 边界检查测试

### 手动测试
使用菜单 **Grid System → Run Tests** 手动运行测试

### 性能测试
使用菜单 **Grid System → Performance Test** 运行性能基准测试

## 🎨 可视化元素

### 颜色说明
- **白色线条**: 网格线
- **绿色框**: 鼠标悬停位置
- **红色框**: 第一个选中位置
- **青色框**: 第二个选中位置
- **蓝色框**: 邻居位置
- **紫色线条**: 路径
- **黄色框**: 范围查询结果

### UI信息
- **左上角**: 实时坐标和距离信息
- **右上角**: 控制说明和功能开关
- **网格标签**: 显示坐标和索引信息

## 🛠️ 自定义配置

### 网格大小
```csharp
_gridWidth = 10;    // 网格宽度
_gridHeight = 10;   // 网格高度
_cellSize = 1.0f;   // 单元大小
```

### 显示选项
- 显示网格线
- 显示坐标标签
- 显示单元编号
- 显示距离计算
- 显示范围查询

## 📊 验证结果

### 成功指标
- ✅ 所有坐标转换往返一致
- ✅ 邻居数量符合预期（边界处理正确）
- ✅ 距离计算符合数学关系
- ✅ 路径连续且最优
- ✅ 范围查询结果准确
- ✅ 边界检查有效

### 控制台输出
查看Unity控制台获取详细的测试结果和性能数据。

## 🚀 高级功能

### 连续测试模式
启用连续测试可以持续验证系统稳定性：
```csharp
_runContinuousTests = true;
_testInterval = 2.0f;  // 每2秒运行一次测试
```

### 性能监控
系统会自动监控关键操作的性能：
- 坐标转换速度
- 邻居查找速度
- 距离计算速度

### 预设配置
使用编辑器工具创建不同规模的网格预设：
- 小网格 (5x5)
- 中等网格 (10x10)
- 大网格 (15x15)
- 密集网格 (20x20)

## 🔧 故障排除

### 常见问题
1. **看不到网格**: 确保在Scene视图中启用了Gizmos
2. **鼠标不响应**: 确保在Game视图中点击
3. **测试不运行**: 检查控制台错误信息
4. **性能问题**: 减少网格大小或关闭部分显示选项

### 验证安装
使用菜单 **Grid System → Validate Installation** 检查所有文件是否完整。

## 📚 文档和支持

- **详细文档**: `README_Visualization.md`
- **代码示例**: `Examples/` 目录
- **编辑器工具**: **Grid System** 菜单
- **自动测试**: 启动时运行或手动触发

## 🎉 验证完成

当你看到以下内容时，说明验证成功：
- ✅ 网格正确显示
- ✅ 鼠标交互正常
- ✅ 所有测试通过
- ✅ 控制台无错误
- ✅ 功能符合预期

恭喜！你的四边形网格坐标系统已经完全可用！🎊

---

**提示**: 这个可视化系统不仅用于验证，也是学习和理解网格坐标系统的绝佳工具。通过交互式的方式，你可以直观地理解各种坐标转换、距离计算和空间查询的工作原理。