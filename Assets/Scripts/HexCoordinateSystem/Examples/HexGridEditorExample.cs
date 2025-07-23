using System.Collections.Generic;
using UnityEngine;
using HexCoordinateSystem.Core;
using HexCoordinateSystem.Coordinates;

namespace HexCoordinateSystem.Examples
{
    /// <summary>
    /// 六边形网格编辑器示例
    /// 展示如何使用双循环初始化六边形网格，以及奇数列和偶数列偏移坐标的区别
    /// </summary>
    public class HexGridEditorExample : MonoBehaviour
    {
        [Header("网格配置")]
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 8;
        [SerializeField] private float hexSize = 1.0f;
        [SerializeField] private GameObject hexPrefab;
        
        [Header("偏移坐标类型")]
        [SerializeField] private OffsetType offsetType = OffsetType.OddQ;
        
        [Header("可视化")]
        [SerializeField] private bool showCoordinates = true;
        [SerializeField] private bool showWorldPositions = false;
        [SerializeField] private Color oddColumnColor = Color.white;
        [SerializeField] private Color evenColumnColor = Color.gray;
        
        private Dictionary<Vector2Int, GameObject> hexObjects = new Dictionary<Vector2Int, GameObject>();
        private Dictionary<Vector2Int, IHexCoordinate> hexCoordinates = new Dictionary<Vector2Int, IHexCoordinate>();
        
        public enum OffsetType
        {
            OddQ,   // 奇数列偏移
            EvenQ   // 偶数列偏移
        }
        
        void Start()
        {
            GenerateHexGrid();
        }
        
        /// <summary>
        /// 生成六边形网格
        /// </summary>
        [ContextMenu("生成六边形网格")]
        public void GenerateHexGrid()
        {
            ClearGrid();
            
            Debug.Log($"=== 六边形偏移坐标系统说明 ===");
            Debug.Log($"当前使用: {offsetType}");
            Debug.Log($"网格大小: {gridWidth} x {gridHeight}");
            
            // 使用双循环生成六边形网格
            for (int col = 0; col < gridWidth; col++)
            {
                for (int row = 0; row < gridHeight; row++)
                {
                    CreateHexAtPosition(col, row);
                }
            }
            
            ExplainOffsetDifferences();
        }
        
        /// <summary>
        /// 在指定位置创建六边形
        /// </summary>
        /// <param name="col">列坐标</param>
        /// <param name="row">行坐标</param>
        private void CreateHexAtPosition(int col, int row)
        {
            IHexCoordinate hexCoord;
            Vector3 worldPos;
            
            // 根据偏移类型创建对应的坐标
            switch (offsetType)
            {
                case OffsetType.OddQ:
                    var oddQCoord = new OffsetCoordinateOddQ(col, row);
                    hexCoord = oddQCoord;
                    worldPos = oddQCoord.ToWorldPosition(hexSize);
                    break;
                    
                case OffsetType.EvenQ:
                    var evenQCoord = new OffsetCoordinateEvenQ(col, row);
                    hexCoord = evenQCoord;
                    worldPos = evenQCoord.ToWorldPosition(hexSize);
                    break;
                    
                default:
                    return;
            }
            
            // 创建六边形对象
            GameObject hexObj = CreateHexObject(worldPos, col, row);
            
            // 存储引用
            Vector2Int gridPos = new Vector2Int(col, row);
            hexObjects[gridPos] = hexObj;
            hexCoordinates[gridPos] = hexCoord;
            
            // 设置颜色区分奇偶列
            SetHexColor(hexObj, col);
            
            // 添加坐标信息
            if (showCoordinates)
            {
                AddCoordinateLabel(hexObj, hexCoord, col, row);
            }
        }
        
        /// <summary>
        /// 创建六边形对象
        /// </summary>
        private GameObject CreateHexObject(Vector3 position, int col, int row)
        {
            GameObject hexObj;
            
            if (hexPrefab != null)
            {
                hexObj = Instantiate(hexPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                // 如果没有预制体，创建一个简单的立方体
                hexObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hexObj.transform.position = position;
                hexObj.transform.parent = transform;
                hexObj.transform.localScale = Vector3.one * hexSize * 0.8f;
            }
            
            hexObj.name = $"Hex_{offsetType}_{col}_{row}";
            return hexObj;
        }
        
        /// <summary>
        /// 设置六边形颜色
        /// </summary>
        private void SetHexColor(GameObject hexObj, int col)
        {
            var renderer = hexObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                var material = new Material(Shader.Find("Standard"));
                material.color = (col % 2 == 0) ? evenColumnColor : oddColumnColor;
                renderer.material = material;
            }
        }
        
        /// <summary>
        /// 添加坐标标签
        /// </summary>
        private void AddCoordinateLabel(GameObject hexObj, IHexCoordinate hexCoord, int col, int row)
        {
            // 创建文本对象显示坐标信息
            var textObj = new GameObject("CoordinateLabel");
            textObj.transform.parent = hexObj.transform;
            textObj.transform.localPosition = Vector3.up * 0.6f;
            
            var textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = $"({col},{row})";
            textMesh.fontSize = 20;
            textMesh.color = Color.black;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            
            if (showWorldPositions)
            {
                var axial = hexCoord.ToAxial();
                textMesh.text += $"\nAxial({axial.q},{axial.r})";
            }
        }
        
        /// <summary>
        /// 解释偏移坐标的区别
        /// </summary>
        private void ExplainOffsetDifferences()
        {
            Debug.Log("\n=== 为什么需要两种偏移坐标系统？ ===");
            Debug.Log("1. 六边形网格的特殊性：");
            Debug.Log("   - 六边形网格不是完美的矩形网格");
            Debug.Log("   - 每一列的六边形相对于相邻列有半个单位的偏移");
            Debug.Log("   - 这种偏移可以向上也可以向下");
            
            Debug.Log("\n2. 奇数列偏移 (OddQ)：");
            Debug.Log("   - 奇数列（1,3,5...）向上偏移半个单位");
            Debug.Log("   - 偶数列（0,2,4...）保持原位");
            Debug.Log("   - 转换公式: r = axial.r + (axial.q - (axial.q & 1)) / 2");
            
            Debug.Log("\n3. 偶数列偏移 (EvenQ)：");
            Debug.Log("   - 偶数列（0,2,4...）向上偏移半个单位");
            Debug.Log("   - 奇数列（1,3,5...）保持原位");
            Debug.Log("   - 转换公式: r = axial.r + (axial.q + (axial.q & 1)) / 2");
            
            Debug.Log("\n4. 使用场景：");
            Debug.Log("   - 选择哪种取决于你的具体需求和视觉偏好");
            Debug.Log("   - 两种系统在数学上等价，只是表示方式不同");
            Debug.Log("   - 重要的是在整个项目中保持一致");
        }
        
        /// <summary>
        /// 清空网格
        /// </summary>
        [ContextMenu("清空网格")]
        public void ClearGrid()
        {
            foreach (var hexObj in hexObjects.Values)
            {
                if (hexObj != null)
                {
                    DestroyImmediate(hexObj);
                }
            }
            
            hexObjects.Clear();
            hexCoordinates.Clear();
        }
        
        /// <summary>
        /// 切换偏移类型并重新生成
        /// </summary>
        [ContextMenu("切换偏移类型")]
        public void ToggleOffsetType()
        {
            offsetType = (offsetType == OffsetType.OddQ) ? OffsetType.EvenQ : OffsetType.OddQ;
            GenerateHexGrid();
        }
        
        /// <summary>
        /// 演示坐标转换
        /// </summary>
        [ContextMenu("演示坐标转换")]
        public void DemonstrateCoordinateConversion()
        {
            Debug.Log("\n=== 坐标转换演示 ===");
            
            // 选择几个示例坐标进行转换
            var testPositions = new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                new Vector2Int(2, 1)
            };
            
            foreach (var pos in testPositions)
            {
                // 创建两种偏移坐标
                var oddQ = new OffsetCoordinateOddQ(pos.x, pos.y);
                var evenQ = new OffsetCoordinateEvenQ(pos.x, pos.y);
                
                // 转换为轴向坐标
                var oddQAxial = oddQ.ToAxial();
                var evenQAxial = evenQ.ToAxial();
                
                // 获取世界坐标
                var oddQWorld = oddQ.ToWorldPosition(hexSize);
                var evenQWorld = evenQ.ToWorldPosition(hexSize);
                
                Debug.Log($"偏移坐标 ({pos.x},{pos.y}):");
                Debug.Log($"  OddQ -> Axial({oddQAxial.q},{oddQAxial.r}) -> World({oddQWorld.x:F2},{oddQWorld.z:F2})");
                Debug.Log($"  EvenQ -> Axial({evenQAxial.q},{evenQAxial.r}) -> World({evenQWorld.x:F2},{evenQWorld.z:F2})");
            }
        }
        
        /// <summary>
        /// 演示邻居查找
        /// </summary>
        [ContextMenu("演示邻居查找")]
        public void DemonstrateNeighborFinding()
        {
            if (hexCoordinates.Count == 0)
            {
                Debug.LogWarning("请先生成网格");
                return;
            }
            
            Debug.Log("\n=== 邻居查找演示 ===");
            
            // 选择中心位置的六边形
            var centerPos = new Vector2Int(gridWidth / 2, gridHeight / 2);
            
            if (hexCoordinates.TryGetValue(centerPos, out var centerCoord))
            {
                Debug.Log($"中心坐标: {centerCoord}");
                
                var neighbors = centerCoord.GetNeighbors();
                Debug.Log($"邻居数量: {neighbors.Count}");
                
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Debug.Log($"  邻居 {i}: {neighbors[i]}");
                }
                
                // 高亮显示邻居
                HighlightNeighbors(centerPos);
            }
        }
        
        /// <summary>
        /// 高亮显示邻居
        /// </summary>
        private void HighlightNeighbors(Vector2Int centerPos)
        {
            // 重置所有颜色
            foreach (var kvp in hexObjects)
            {
                SetHexColor(kvp.Value, kvp.Key.x);
            }
            
            // 高亮中心
            if (hexObjects.TryGetValue(centerPos, out var centerObj))
            {
                var renderer = centerObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
            }
            
            // 高亮邻居
            if (hexCoordinates.TryGetValue(centerPos, out var centerCoord))
            {
                var neighbors = centerCoord.GetNeighbors();
                
                foreach (var neighbor in neighbors)
                {
                    // 找到对应的网格位置
                    foreach (var kvp in hexCoordinates)
                    {
                        if (kvp.Value.ToAxial().Equals(neighbor.ToAxial()))
                        {
                            if (hexObjects.TryGetValue(kvp.Key, out var neighborObj))
                            {
                                var renderer = neighborObj.GetComponent<Renderer>();
                                if (renderer != null)
                                {
                                    renderer.material.color = Color.yellow;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        
        void OnDrawGizmos()
        {
            if (hexCoordinates.Count > 0)
            {
                // 绘制网格连接线
                Gizmos.color = Color.cyan;
                
                foreach (var kvp in hexCoordinates)
                {
                    var coord = kvp.Value;
                    var worldPos = coord.ToWorldPosition(hexSize);
                    
                    var neighbors = coord.GetNeighbors();
                    foreach (var neighbor in neighbors)
                    {
                        var neighborWorldPos = neighbor.ToWorldPosition(hexSize);
                        Gizmos.DrawLine(worldPos, neighborWorldPos);
                    }
                }
            }
        }
    }
}