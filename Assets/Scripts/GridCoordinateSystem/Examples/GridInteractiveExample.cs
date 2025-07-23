using UnityEngine;
using GridCoordinateSystem.Coordinates;
using GridCoordinateSystem.Utils;

namespace GridCoordinateSystem.Examples
{
    /// <summary>
    /// 交互式网格示例
    /// 展示鼠标交互和实时网格操作
    /// </summary>
    public class GridInteractiveExample : MonoBehaviour
    {
        [Header("交互设置")]
        [SerializeField] private bool _enableInteraction = true;
        [SerializeField] private KeyCode _placeObjectKey = KeyCode.Space;
        [SerializeField] private KeyCode _removeObjectKey = KeyCode.X;
        [SerializeField] private KeyCode _clearAllKey = KeyCode.C;
        
        [Header("可视化对象")]
        [SerializeField] private GameObject _gridObjectPrefab;
        [SerializeField] private GameObject _highlightPrefab;
        [SerializeField] private Transform _objectContainer;
        
        [Header("交互反馈")]
        [SerializeField] private Color _hoverColor = Color.yellow;
        [SerializeField] private Color _selectedColor = Color.red;
        [SerializeField] private Color _placedColor = Color.green;
        
        private GridCoordinateManager _gridManager;
        private GridDebugger _gridDebugger;
        private Camera _mainCamera;
        
        // 交互状态
        private CartesianCoordinate _currentHoverPosition;
        private CartesianCoordinate _selectedPosition;
        private GameObject _hoverHighlight;
        private GameObject _selectedHighlight;
        
        // 放置的对象
        private System.Collections.Generic.Dictionary<CartesianCoordinate, GameObject> _placedObjects;
        
        private void Awake()
        {
            _gridManager = GridCoordinateManager.Instance;
            _gridDebugger = FindObjectOfType<GridDebugger>();
            _mainCamera = Camera.main;
            
            if (_mainCamera == null)
                _mainCamera = FindObjectOfType<Camera>();
                
            _placedObjects = new System.Collections.Generic.Dictionary<CartesianCoordinate, GameObject>();
            
            // 创建容器
            if (_objectContainer == null)
            {
                var containerGO = new GameObject("Grid Objects");
                _objectContainer = containerGO.transform;
            }
        }
        
        private void Start()
        {
            CreateHighlights();
            
            if (_gridDebugger != null)
            {
                _gridDebugger.EnableDebug = true;
                _gridDebugger.ShowGrid = true;
            }
        }
        
        private void Update()
        {
            if (!_enableInteraction || _gridManager == null || _mainCamera == null)
                return;
                
            UpdateHoverPosition();
            HandleInput();
            UpdateHighlights();
        }
        
        /// <summary>
        /// 更新鼠标悬停位置
        /// </summary>
        private void UpdateHoverPosition()
        {
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            
            _currentHoverPosition = _gridManager.WorldToCartesian(mouseWorldPos);
        }
        
        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleInput()
        {
            // 鼠标左键选择
            if (Input.GetMouseButtonDown(0))
            {
                SelectPosition(_currentHoverPosition);
            }
            
            // 放置对象
            if (Input.GetKeyDown(_placeObjectKey))
            {
                PlaceObject(_currentHoverPosition);
            }
            
            // 移除对象
            if (Input.GetKeyDown(_removeObjectKey))
            {
                RemoveObject(_currentHoverPosition);
            }
            
            // 清除所有对象
            if (Input.GetKeyDown(_clearAllKey))
            {
                ClearAllObjects();
            }
        }
        
        /// <summary>
        /// 选择位置
        /// </summary>
        /// <param name="position">位置</param>
        private void SelectPosition(CartesianCoordinate position)
        {
            _selectedPosition = position;
            
            Debug.Log($"选中位置: {position}");
            Debug.Log($"世界坐标: {_gridManager.CartesianToWorld(position)}");
            Debug.Log($"索引坐标: {_gridManager.CartesianToIndex(position)}");
            
            // 显示邻居信息
            var neighbors4 = _gridManager.GetNeighbors4(position);
            var neighbors8 = _gridManager.GetNeighbors8(position);
            
            Debug.Log($"4邻居: {string.Join(", ", neighbors4)}");
            Debug.Log($"8邻居: {string.Join(", ", neighbors8)}");
            
            // 如果有调试器，设置路径显示
            if (_gridDebugger != null && _placedObjects.Count > 0)
            {
                // 找到最近的放置对象并显示路径
                CartesianCoordinate? nearestObject = null;
                float minDistance = float.MaxValue;
                
                foreach (var placedPos in _placedObjects.Keys)
                {
                    float distance = position.EuclideanDistance(placedPos);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestObject = placedPos;
                    }
                }
                
                if (nearestObject.HasValue)
                {
                    var path = _gridManager.GetLinePath(position, nearestObject.Value);
                    _gridDebugger.SetPath(path.ToArray());
                }
            }
        }
        
        /// <summary>
        /// 放置对象
        /// </summary>
        /// <param name="position">位置</param>
        private void PlaceObject(CartesianCoordinate position)
        {
            if (_gridObjectPrefab == null)
            {
                Debug.LogWarning("未设置网格对象预制体！");
                return;
            }
            
            // 检查是否已经有对象
            if (_placedObjects.ContainsKey(position))
            {
                Debug.Log($"位置 {position} 已经有对象了！");
                return;
            }
            
            // 检查边界
            if (!_gridManager.IsInBounds(position))
            {
                Debug.Log($"位置 {position} 超出边界！");
                return;
            }
            
            // 创建对象
            Vector3 worldPos = _gridManager.CartesianToWorld(position);
            GameObject obj = Instantiate(_gridObjectPrefab, worldPos, Quaternion.identity, _objectContainer);
            
            // 设置颜色
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = _placedColor;
            }
            
            _placedObjects[position] = obj;
            
            Debug.Log($"在位置 {position} 放置了对象");
        }
        
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="position">位置</param>
        private void RemoveObject(CartesianCoordinate position)
        {
            if (_placedObjects.TryGetValue(position, out GameObject obj))
            {
                DestroyImmediate(obj);
                _placedObjects.Remove(position);
                
                Debug.Log($"移除了位置 {position} 的对象");
            }
            else
            {
                Debug.Log($"位置 {position} 没有对象可移除");
            }
        }
        
        /// <summary>
        /// 清除所有对象
        /// </summary>
        private void ClearAllObjects()
        {
            foreach (var obj in _placedObjects.Values)
            {
                if (obj != null)
                    DestroyImmediate(obj);
            }
            
            _placedObjects.Clear();
            
            if (_gridDebugger != null)
                _gridDebugger.ClearPath();
                
            Debug.Log("清除了所有对象");
        }
        
        /// <summary>
        /// 创建高亮显示
        /// </summary>
        private void CreateHighlights()
        {
            if (_highlightPrefab != null)
            {
                // 悬停高亮
                _hoverHighlight = Instantiate(_highlightPrefab, Vector3.zero, Quaternion.identity, _objectContainer);
                _hoverHighlight.name = "Hover Highlight";
                var hoverRenderer = _hoverHighlight.GetComponent<Renderer>();
                if (hoverRenderer != null)
                    hoverRenderer.material.color = _hoverColor;
                    
                // 选中高亮
                _selectedHighlight = Instantiate(_highlightPrefab, Vector3.zero, Quaternion.identity, _objectContainer);
                _selectedHighlight.name = "Selected Highlight";
                var selectedRenderer = _selectedHighlight.GetComponent<Renderer>();
                if (selectedRenderer != null)
                    selectedRenderer.material.color = _selectedColor;
            }
        }
        
        /// <summary>
        /// 更新高亮显示
        /// </summary>
        private void UpdateHighlights()
        {
            // 更新悬停高亮
            if (_hoverHighlight != null)
            {
                Vector3 hoverWorldPos = _gridManager.CartesianToWorld(_currentHoverPosition);
                _hoverHighlight.transform.position = hoverWorldPos;
                _hoverHighlight.SetActive(_gridManager.IsInBounds(_currentHoverPosition));
            }
            
            // 更新选中高亮
            if (_selectedHighlight != null)
            {
                if (_selectedPosition != null && _gridManager.IsInBounds(_selectedPosition))
                {
                    Vector3 selectedWorldPos = _gridManager.CartesianToWorld(_selectedPosition);
                    _selectedHighlight.transform.position = selectedWorldPos;
                    _selectedHighlight.SetActive(true);
                }
                else
                {
                    _selectedHighlight.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 在GUI中显示帮助信息
        /// </summary>
        private void OnGUI()
        {
            if (!_enableInteraction) return;
            
            string helpText = $"交互式网格示例\n" +
                             $"左键: 选择位置\n" +
                             $"{_placeObjectKey}: 放置对象\n" +
                             $"{_removeObjectKey}: 移除对象\n" +
                             $"{_clearAllKey}: 清除所有\n" +
                             $"\n当前悬停: {_currentHoverPosition}\n" +
                             $"已放置对象: {_placedObjects.Count}";
            
            GUI.Box(new Rect(10, Screen.height - 150, 200, 140), helpText);
        }
    }
}