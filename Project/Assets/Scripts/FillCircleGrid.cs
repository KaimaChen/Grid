using UnityEngine;
using System.Collections.Generic;

public enum FillCircleAlgorithm
{
    BoundingBox,
    BoundingCircle,
}

/// <summary>
/// 圆（填充）
/// * 鼠标左键：圆心位置
/// * 鼠标右键：控制半径
/// </summary>
public class FillCircleGrid : BaseCircleGrid
{
    public FillCircleAlgorithm m_algorithm;
    protected override List<Vector2Int> GetCirclePoints()
    {
        switch(m_algorithm)
        {
            case FillCircleAlgorithm.BoundingBox:
                return CircleByBoundingBox(m_center, m_radius, TileWidth);
            case FillCircleAlgorithm.BoundingCircle:
                return CircleByBoundingCircle(m_center, m_radius, TileWidth);
            default:
                Debug.LogError($"No FillCircleAlgorithm={m_algorithm}");
                return CircleByBoundingCircle(m_center, m_radius, TileWidth);
        }
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