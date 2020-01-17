using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 绘制圆
/// * 鼠标左键：圆心位置
/// * 鼠标右键：控制半径
/// </summary>
public class CircleGrid : BaseGrid
{
    [Range(0, 1)]
    public float m_radius = 0.3f;
    public Material m_circleMat;

    Vector2Int m_center;

    void Start()
    {
        m_center = new Vector2Int(m_widthCount / 2, m_heightCount / 2);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            m_center = GetMouseTilePos();
        }
        else if(Input.GetMouseButton(1))
        {
            float cx = (float)m_center.x / m_widthCount;
            float mx = Input.mousePosition.x / Screen.width;
            m_radius = Mathf.Abs(mx - cx);
        }
    }

    protected override void Draw()
    {
        base.Draw();

        GL.Begin(GL.QUADS);
        m_circleMat.SetPass(0);
        //List<Vector2Int> points = CircleByBoundingBox(m_center, m_radius, TileWidth);
        List<Vector2Int> points = CircleByBoundingCircle(m_center, m_radius, TileWidth);
        for (int i = 0; i < points.Count; i++)
            DrawQuad(points[i]);
        GL.End();
    }

    /// <summary>
    /// 算出圆所在的包围盒，然后算出遍历里面的点看哪些在半径里
    /// </summary>
    /// <param name="centerTile">圆心的格子坐标（实际坐标指格子左下角）</param>
    public static List<Vector2Int> CircleByBoundingBox(Vector2Int centerTile, float radius, float tileSize)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int radiusCount = Mathf.CeilToInt(radius / tileSize);
        float sqrRadius = radius * radius;
        for(int dx = -radiusCount; dx <= radiusCount; dx++)
        {
            for(int dy = -radiusCount; dy <= radiusCount; dy++)
            {
                Vector2 relativePos = new Vector2(dx * tileSize, dy * tileSize);
                float sqrDistance = relativePos.sqrMagnitude;
                if (sqrDistance <= sqrRadius)
                    result.Add(centerTile + new Vector2Int(dx, dy));
            }
        }

        return result;
    }

    /// <summary>
    /// 利用勾股定理来算出左右两端的x值
    /// 参考 https://www.redblobgames.com/grids/circle-drawing/
    /// </summary>
    public static List<Vector2Int> CircleByBoundingCircle(Vector2Int centerTile, float radius, float tileSize)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int radiusCount = Mathf.CeilToInt(radius / tileSize);
        int sqr = radiusCount * radiusCount;
        int top = centerTile.y + radiusCount;
        int bottom = centerTile.y - radiusCount;

        for(int y = bottom; y <= top; y++)
        {
            int dy = y - centerTile.y;
            int dx = Mathf.FloorToInt(Mathf.Sqrt(sqr - dy * dy));
            int left = centerTile.x - dx;
            int right = centerTile.x + dx;
            for (int x = left; x <= right; x++)
                result.Add(new Vector2Int(x, y));
        }

        return result;
    }
}