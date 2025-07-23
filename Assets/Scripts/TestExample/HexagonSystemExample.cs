using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 六边形网格系统完整实现示例
/// 从零开始构建六边形坐标转换和网格管理系统
/// </summary>
public class HexagonSystemExample : MonoBehaviour
{
    #region 六边形几何常量定义
    
    /// <summary>
    /// 六边形边长（标准化为1）
    /// </summary>
    public const float HEX_SIDE_LENGTH = 1.0f;
    
    /// <summary>
    /// 六边形高度 = √3 * 边长
    /// 这是六边形从顶点到底边的距离
    /// </summary>
    public static readonly float HEX_HEIGHT = Mathf.Sqrt(3.0f) * HEX_SIDE_LENGTH;
    
    /// <summary>
    /// 六边形宽度 = 2 * 边长
    /// 这是六边形从左顶点到右顶点的距离
    /// </summary>
    public static readonly float HEX_WIDTH = 2.0f * HEX_SIDE_LENGTH;
    
    /// <summary>
    /// 水平间距 = 1.5 * 边长
    /// 相邻列六边形中心点的水平距离
    /// </summary>
    public static readonly float HEX_HORIZONTAL_SPACING = 1.5f * HEX_SIDE_LENGTH;
    
    /// <summary>
    /// 垂直间距 = √3 * 边长
    /// 相邻行六边形中心点的垂直距离
    /// </summary>
    public static readonly float HEX_VERTICAL_SPACING = HEX_HEIGHT;
    
    #endregion
    
    #region 六边形坐标结构定义
    
    /// <summary>
    /// 六边形坐标结构
    /// 使用偏移坐标系统（Offset Coordinates）
    /// </summary>
    [System.Serializable]
    public struct HexCoordinate
    {
        public int q; // 列坐标（水平方向）
        public int r; // 行坐标（垂直方向）
        
        public HexCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
        
        public override string ToString()
        {
            return $"({q}, {r})";
        }
        
        public override bool Equals(object obj)
        {
            if (obj is HexCoordinate other)
            {
                return q == other.q && r == other.r;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return q.GetHashCode() ^ (r.GetHashCode() << 2);
        }
    }
    
    #endregion
    
    #region 核心坐标转换方法
    
    /// <summary>
    /// 步骤1：基础的六边形坐标到世界坐标转换
    /// 这是最核心的转换函数
    /// </summary>
    /// <param name="hexCoord">六边形坐标</param>
    /// <returns>世界坐标</returns>
    public static Vector3 HexToWorldBasic(HexCoordinate hexCoord)
    {
        // 计算X坐标：每列间距为1.5倍边长
        float worldX = hexCoord.q * HEX_HORIZONTAL_SPACING;
        
        // 计算Y坐标：需要考虑奇偶列的偏移
        float worldY = hexCoord.r * HEX_VERTICAL_SPACING;
        
        // 奇数列需要向上偏移半个六边形高度
        if (hexCoord.q % 2 != 0)
        {
            worldY += HEX_VERTICAL_SPACING * 0.5f;
        }
        
        return new Vector3(worldX, worldY, 0);
    }
    
    /// <summary>
    /// 步骤2：优化版本的坐标转换（与原项目保持一致）
    /// 使用调整后的R坐标来简化计算
    /// </summary>
    /// <param name="q">列坐标</param>
    /// <param name="r">行坐标</param>
    /// <returns>世界坐标</returns>
    public static Vector3 HexToWorldOptimized(int q, int r)
    {
        // 调整R坐标，将奇偶列的偏移预先计算
        int adjustedR = r - q / 2;
        
        // X坐标计算
        float x = 1.5f * q;
        
        // Y坐标计算：使用调整后的R值
        float y = HEX_HEIGHT * (adjustedR + q / 2.0f);
        
        // 处理负数奇数列的特殊情况
        if (q < 0 && q % 2 != 0)
        {
            y += HEX_HEIGHT;
        }
        
        return new Vector3(x, y, 0);
    }
    
    /// <summary>
    /// 步骤3：世界坐标转换为六边形坐标（逆向转换）
    /// </summary>
    /// <param name="worldPos">世界坐标</param>
    /// <returns>六边形坐标</returns>
    public static HexCoordinate WorldToHex(Vector3 worldPos)
    {
        // 计算Q坐标（列）
        int q = Mathf.RoundToInt(worldPos.x / HEX_HORIZONTAL_SPACING);
        
        // 计算R坐标（行），需要考虑奇偶列偏移
        float adjustedY = worldPos.y;
        if (q % 2 != 0)
        {
            adjustedY -= HEX_VERTICAL_SPACING * 0.5f;
        }
        int r = Mathf.RoundToInt(adjustedY / HEX_VERTICAL_SPACING);
        
        return new HexCoordinate(q, r);
    }
    
    #endregion
    
    #region 六边形邻居计算
    
    /// <summary>
    /// 六边形的6个基础方向偏移
    /// 顺序：上、下、右、左
    /// </summary>
    private static readonly Vector2Int[] BaseDirections = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // 上
        new Vector2Int(0, -1),  // 下
        new Vector2Int(1, 0),   // 右
        new Vector2Int(-1, 0),  // 左
    };
    
    /// <summary>
    /// 步骤4：获取六边形的所有相邻坐标
    /// </summary>
    /// <param name="hexCoord">目标六边形坐标</param>
    /// <returns>6个相邻六边形的坐标列表</returns>
    public static List<HexCoordinate> GetHexNeighbors(HexCoordinate hexCoord)
    {
        List<HexCoordinate> neighbors = new List<HexCoordinate>(6);
        
        // 添加基础4个方向
        foreach (var dir in BaseDirections)
        {
            neighbors.Add(new HexCoordinate(hexCoord.q + dir.x, hexCoord.r + dir.y));
        }
        
        // 根据列的奇偶性添加对角方向
        if (hexCoord.q % 2 != 0)
        {
            // 奇数列：添加左上、右上
            neighbors.Add(new HexCoordinate(hexCoord.q - 1, hexCoord.r + 1)); // 左上
            neighbors.Add(new HexCoordinate(hexCoord.q + 1, hexCoord.r + 1)); // 右上
        }
        else
        {
            // 偶数列：添加左下、右下
            neighbors.Add(new HexCoordinate(hexCoord.q - 1, hexCoord.r - 1)); // 左下
            neighbors.Add(new HexCoordinate(hexCoord.q + 1, hexCoord.r - 1)); // 右下
        }
        
        return neighbors;
    }
    
    #endregion
    
    #region 六边形网格管理
    
    /// <summary>
    /// 六边形网格数据结构
    /// </summary>
    [System.Serializable]
    public class HexGrid
    {
        /// <summary>
        /// 网格数据字典，存储每个坐标对应的数据
        /// </summary>
        private Dictionary<HexCoordinate, GameObject> hexTiles = new Dictionary<HexCoordinate, GameObject>();
        
        /// <summary>
        /// 网格范围
        /// </summary>
        public int gridRadius = 5;
        
        /// <summary>
        /// 步骤5：初始化六边形网格
        /// </summary>
        /// <param name="radius">网格半径</param>
        /// <param name="hexPrefab">六边形预制体</param>
        /// <param name="parent">父物体</param>
        public void InitializeGrid(int radius, GameObject hexPrefab, Transform parent)
        {
            gridRadius = radius;
            
            // 生成六边形网格
            for (int q = -radius; q <= radius; q++)
            {
                for (int r = -radius; r <= radius; r++)
                {
                    // 检查是否在六边形范围内
                    if (IsWithinHexRadius(q, r, radius))
                    {
                        CreateHexTile(new HexCoordinate(q, r), hexPrefab, parent);
                    }
                }
            }
        }
        
        /// <summary>
        /// 检查坐标是否在六边形范围内
        /// </summary>
        private bool IsWithinHexRadius(int q, int r, int radius)
        {
            // 使用立方坐标系统的距离计算
            int s = -q - r; // 第三个坐标
            return Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s)) <= radius;
        }
        
        /// <summary>
        /// 创建单个六边形瓦片
        /// </summary>
        private void CreateHexTile(HexCoordinate coord, GameObject prefab, Transform parent)
        {
            Vector3 worldPos = HexToWorldOptimized(coord.q, coord.r);
            GameObject hexTile = Object.Instantiate(prefab, worldPos, Quaternion.identity, parent);
            hexTile.name = $"Hex_{coord}";
            
            // 存储到字典中
            hexTiles[coord] = hexTile;
            
            // 可以在这里添加六边形的特定组件或数据
            var hexComponent = hexTile.GetComponent<HexTileComponent>();
            if (hexComponent == null)
            {
                hexComponent = hexTile.AddComponent<HexTileComponent>();
            }
            hexComponent.Initialize(coord);
        }
        
        /// <summary>
        /// 获取指定坐标的六边形对象
        /// </summary>
        public GameObject GetHexTile(HexCoordinate coord)
        {
            hexTiles.TryGetValue(coord, out GameObject tile);
            return tile;
        }
        
        /// <summary>
        /// 获取所有六边形坐标
        /// </summary>
        public List<HexCoordinate> GetAllCoordinates()
        {
            return new List<HexCoordinate>(hexTiles.Keys);
        }
    }
    
    #endregion
    
    #region 示例使用方法
    
    [Header("网格配置")]
    public int gridRadius = 3;
    public GameObject hexPrefab;
    
    private HexGrid hexGrid;
    
    /// <summary>
    /// 步骤6：Unity中的使用示例
    /// </summary>
    void Start()
    {
        // 创建六边形网格
        CreateHexagonGrid();
        
        // 演示坐标转换
        DemonstrateCoordinateConversion();
        
        // 演示邻居查找
        DemonstrateNeighborFinding();
    }
    
    /// <summary>
    /// 创建六边形网格
    /// </summary>
    private void CreateHexagonGrid()
    {
        if (hexPrefab == null)
        {
            Debug.LogError("请设置六边形预制体！");
            return;
        }
        
        hexGrid = new HexGrid();
        hexGrid.InitializeGrid(gridRadius, hexPrefab, transform);
        
        Debug.Log($"创建了 {hexGrid.GetAllCoordinates().Count} 个六边形瓦片");
    }
    
    /// <summary>
    /// 演示坐标转换
    /// </summary>
    private void DemonstrateCoordinateConversion()
    {
        // 测试几个坐标点
        HexCoordinate[] testCoords = {
            new HexCoordinate(0, 0),
            new HexCoordinate(1, 0),
            new HexCoordinate(0, 1),
            new HexCoordinate(-1, 1),
            new HexCoordinate(2, -1)
        };
        
        Debug.Log("=== 坐标转换演示 ===");
        foreach (var coord in testCoords)
        {
            Vector3 worldPos = HexToWorldOptimized(coord.q, coord.r);
            HexCoordinate backToHex = WorldToHex(worldPos);
            
            Debug.Log($"六边形坐标 {coord} -> 世界坐标 {worldPos} -> 转换回 {backToHex}");
        }
    }
    
    /// <summary>
    /// 演示邻居查找
    /// </summary>
    private void DemonstrateNeighborFinding()
    {
        HexCoordinate center = new HexCoordinate(0, 0);
        List<HexCoordinate> neighbors = GetHexNeighbors(center);
        
        Debug.Log($"=== 邻居查找演示 ===");
        Debug.Log($"中心坐标 {center} 的邻居：");
        for (int i = 0; i < neighbors.Count; i++)
        {
            Debug.Log($"  邻居 {i + 1}: {neighbors[i]}");
        }
    }
    
    #endregion
    
    #region 调试和可视化
    
    /// <summary>
    /// 在Scene视图中绘制网格线
    /// </summary>
    void OnDrawGizmos()
    {
        if (hexGrid == null) return;
        
        Gizmos.color = Color.yellow;
        
        // 绘制每个六边形的中心点
        foreach (var coord in hexGrid.GetAllCoordinates())
        {
            Vector3 worldPos = HexToWorldOptimized(coord.q, coord.r);
            Gizmos.DrawWireSphere(worldPos, 0.1f);
            
            // 绘制坐标标签（仅在选中时）
            if (Selection.activeGameObject == gameObject)
            {
                UnityEditor.Handles.Label(worldPos + Vector3.up * 0.5f, coord.ToString());
            }
        }
    }
    
    #endregion
}

/// <summary>
/// 六边形瓦片组件
/// 附加到每个六边形对象上，存储其坐标信息
/// </summary>
public class HexTileComponent : MonoBehaviour
{
    [Header("六边形信息")]
    public HexagonSystemExample.HexCoordinate coordinate;
    
    /// <summary>
    /// 初始化六边形瓦片
    /// </summary>
    public void Initialize(HexagonSystemExample.HexCoordinate coord)
    {
        coordinate = coord;
    }
    
    /// <summary>
    /// 获取相邻的六边形瓦片
    /// </summary>
    public List<HexTileComponent> GetNeighborTiles()
    {
        List<HexTileComponent> neighborTiles = new List<HexTileComponent>();
        List<HexagonSystemExample.HexCoordinate> neighbors = HexagonSystemExample.GetHexNeighbors(coordinate);
        
        foreach (var neighborCoord in neighbors)
        {
            // 在场景中查找对应的六边形对象
            GameObject neighborObj = GameObject.Find($"Hex_{neighborCoord}");
            if (neighborObj != null)
            {
                HexTileComponent neighborTile = neighborObj.GetComponent<HexTileComponent>();
                if (neighborTile != null)
                {
                    neighborTiles.Add(neighborTile);
                }
            }
        }
        
        return neighborTiles;
    }
    
    /// <summary>
    /// 鼠标点击事件处理
    /// </summary>
    void OnMouseDown()
    {
        Debug.Log($"点击了六边形: {coordinate}");
        
        // 高亮显示邻居
        List<HexTileComponent> neighbors = GetNeighborTiles();
        Debug.Log($"邻居数量: {neighbors.Count}");
        foreach (var neighbor in neighbors)
        {
            Debug.Log($"  邻居: {neighbor.coordinate}");
        }
    }
}