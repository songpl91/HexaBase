using UnityEngine;

/// <summary>
/// 六边形预制体创建工具
/// 用于快速创建六边形网格所需的基础预制体
/// </summary>
public class HexPrefabCreator : MonoBehaviour
{
    [Header("六边形配置")]
    [Tooltip("六边形的边长")]
    public float hexSize = 1.0f;
    
    [Tooltip("六边形的颜色")]
    public Color hexColor = Color.white;
    
    [Tooltip("是否显示边框")]
    public bool showBorder = true;
    
    [Tooltip("边框颜色")]
    public Color borderColor = Color.black;
    
    [Tooltip("边框宽度")]
    public float borderWidth = 0.05f;
    
    /// <summary>
    /// 创建六边形预制体
    /// 在编辑器中调用此方法来生成六边形预制体
    /// </summary>
    [ContextMenu("创建六边形预制体")]
    public void CreateHexPrefab()
    {
        // 创建根对象
        GameObject hexPrefab = new GameObject("HexTile");
        
        // 添加六边形渲染组件
        CreateHexRenderer(hexPrefab);
        
        // 添加碰撞器
        CreateHexCollider(hexPrefab);
        
        // 添加六边形组件
        hexPrefab.AddComponent<HexTileComponent>();
        
        Debug.Log("六边形预制体创建完成！");
    }
    
    /// <summary>
    /// 创建六边形渲染器
    /// </summary>
    private void CreateHexRenderer(GameObject parent)
    {
        // 创建六边形网格
        Mesh hexMesh = CreateHexMesh();
        
        // 添加MeshFilter和MeshRenderer
        MeshFilter meshFilter = parent.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = parent.AddComponent<MeshRenderer>();
        
        meshFilter.mesh = hexMesh;
        
        // 创建材质
        Material hexMaterial = new Material(Shader.Find("Standard"));
        hexMaterial.color = hexColor;
        hexMaterial.name = "HexMaterial";
        
        meshRenderer.material = hexMaterial;
        
        // 如果需要边框，创建边框对象
        if (showBorder)
        {
            CreateHexBorder(parent);
        }
    }
    
    /// <summary>
    /// 创建六边形网格
    /// </summary>
    private Mesh CreateHexMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "HexMesh";
        
        // 计算六边形顶点
        Vector3[] vertices = new Vector3[7]; // 中心点 + 6个顶点
        Vector2[] uv = new Vector2[7];
        int[] triangles = new int[18]; // 6个三角形，每个3个顶点
        
        // 中心点
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);
        
        // 六边形的6个顶点
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * hexSize;
            float y = Mathf.Sin(angle) * hexSize;
            
            vertices[i + 1] = new Vector3(x, y, 0);
            
            // UV坐标
            uv[i + 1] = new Vector2(
                0.5f + Mathf.Cos(angle) * 0.5f,
                0.5f + Mathf.Sin(angle) * 0.5f
            );
        }
        
        // 创建三角形
        for (int i = 0; i < 6; i++)
        {
            int triangleIndex = i * 3;
            triangles[triangleIndex] = 0; // 中心点
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = (i + 1) % 6 + 1;
        }
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    /// <summary>
    /// 创建六边形边框
    /// </summary>
    private void CreateHexBorder(GameObject parent)
    {
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(parent.transform);
        borderObj.transform.localPosition = Vector3.zero;
        
        LineRenderer lineRenderer = borderObj.AddComponent<LineRenderer>();
        
        // 配置LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        // lineRenderer.color = borderColor;
        lineRenderer.startWidth = borderWidth;
        lineRenderer.endWidth = borderWidth;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        
        // 设置六边形边框的点
        Vector3[] borderPoints = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * hexSize;
            float y = Mathf.Sin(angle) * hexSize;
            borderPoints[i] = new Vector3(x, y, 0.01f); // 稍微向前偏移避免Z-fighting
        }
        
        lineRenderer.positionCount = 6;
        lineRenderer.SetPositions(borderPoints);
    }
    
    /// <summary>
    /// 创建六边形碰撞器
    /// </summary>
    private void CreateHexCollider(GameObject parent)
    {
        // 使用PolygonCollider2D创建精确的六边形碰撞
        PolygonCollider2D collider = parent.AddComponent<PolygonCollider2D>();
        
        // 设置六边形碰撞器的点
        Vector2[] colliderPoints = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * hexSize;
            float y = Mathf.Sin(angle) * hexSize;
            colliderPoints[i] = new Vector2(x, y);
        }
        
        collider.points = colliderPoints;
    }
    
    /// <summary>
    /// 在Scene视图中预览六边形
    /// </summary>
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // 绘制六边形轮廓
            Gizmos.color = hexColor;
            Vector3[] hexPoints = new Vector3[6];
            
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * hexSize;
                float y = Mathf.Sin(angle) * hexSize;
                hexPoints[i] = transform.position + new Vector3(x, y, 0);
            }
            
            // 绘制六边形边框
            for (int i = 0; i < 6; i++)
            {
                Gizmos.DrawLine(hexPoints[i], hexPoints[(i + 1) % 6]);
            }
            
            // 绘制中心点
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
}