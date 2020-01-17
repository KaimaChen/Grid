using UnityEngine;
using System.Collections.Generic;

public enum CircleAlgorithm
{
    Simple,
    BresenhamFloat,
    BresenhamInt,
}

/// <summary>
/// 圆（不填充）
/// * 鼠标左键：圆心位置
/// * 鼠标右键：控制半径
/// </summary>
public class CircleGrid : BaseCircleGrid
{
    public CircleAlgorithm m_algorithm;

    protected override List<Vector2Int> GetCirclePoints()
    {
        //半径占的格子数
        int radiusCount = Mathf.CeilToInt(m_radius / TileWidth);

        switch(m_algorithm)
        {
            case CircleAlgorithm.Simple:
                return CircleBySimple(m_center, radiusCount);
            case CircleAlgorithm.BresenhamFloat:
                return CircleByBresenhamFloat(m_center, radiusCount);
            case CircleAlgorithm.BresenhamInt:
                return CircleByBresenhamInt(m_center, radiusCount);
            default:
                Debug.LogError($"No CircleAlgorithm={m_algorithm}");
                return CircleBySimple(m_center, radiusCount);
        }
    }

    /// <summary>
    /// 利用对称性
    /// 参考 http://www.sunshine2k.de/coding/java/Bresenham/RasterisingLinesCircles.pdf
    /// </summary>
    public static List<Vector2Int> CircleBySimple(Vector2Int c, int r)
    {
        List<Vector2Int> result = new List<Vector2Int>()
        {
            new Vector2Int(c.x, c.y + r),
            new Vector2Int(c.x, c.y - r),
            new Vector2Int(c.x + r, c.y),
            new Vector2Int(c.x - r, c.y)
        };

        int sqr = r * r;
        int x = 1;
        int y = Mathf.RoundToInt(Mathf.Sqrt(sqr - x * x));
        while(x <= y)
        {
            result.Add(new Vector2Int(c.x + x, c.y + y));
            result.Add(new Vector2Int(c.x + x, c.y - y));
            result.Add(new Vector2Int(c.x - x, c.y + y));
            result.Add(new Vector2Int(c.x - x, c.y - y));

            result.Add(new Vector2Int(c.x + y, c.y + x));
            result.Add(new Vector2Int(c.x + y, c.y - x));
            result.Add(new Vector2Int(c.x - y, c.y + x));
            result.Add(new Vector2Int(c.x - y, c.y - x));

            x += 1;
            y = Mathf.RoundToInt(Mathf.Sqrt(sqr - x * x));
        }

        return result;
    }

    /// <summary>
    /// Bresenham（带浮点运算）
    /// 参考 http://www.sunshine2k.de/coding/java/Bresenham/RasterisingLinesCircles.pdf
    /// </summary>
    public static List<Vector2Int> CircleByBresenhamFloat(Vector2Int c, int r)
    {
        List<Vector2Int> result = new List<Vector2Int>()
        {
            new Vector2Int(c.x, c.y + r),
            new Vector2Int(c.x, c.y - r),
            new Vector2Int(c.x + r, c.y),
            new Vector2Int(c.x - r, c.y)
        };

        int x = 0;
        int y = r;
        float d = 1.25f - r;
        while(x < y)
        {
            if(d < 0)
            {
                d = d + 2 * x + 3;
                x += 1;
            }
            else
            {
                d = d + 2 * (x - y) + 5;
                x += 1;
                y -= 1;
            }

            result.Add(new Vector2Int(c.x + x, c.y + y));
            result.Add(new Vector2Int(c.x + x, c.y - y));
            result.Add(new Vector2Int(c.x - x, c.y + y));
            result.Add(new Vector2Int(c.x - x, c.y - y));

            result.Add(new Vector2Int(c.x + y, c.y + x));
            result.Add(new Vector2Int(c.x + y, c.y - x));
            result.Add(new Vector2Int(c.x - y, c.y + x));
            result.Add(new Vector2Int(c.x - y, c.y - x));
        }

        return result;
    }

    /// <summary>
    /// Bresenham（不带浮点运算）
    /// 参考 http://www.sunshine2k.de/coding/java/Bresenham/RasterisingLinesCircles.pdf
    /// </summary>
    public static List<Vector2Int> CircleByBresenhamInt(Vector2Int c, int r)
    {
        List<Vector2Int> result = new List<Vector2Int>()
        {
            new Vector2Int(c.x, c.y + r),
            new Vector2Int(c.x, c.y - r),
            new Vector2Int(c.x + r, c.y),
            new Vector2Int(c.x - r, c.y)
        };

        int x = 0;
        int y = r;
        int d = 1 - r; //这里去掉了浮点
        while (x < y)
        {
            if (d < 0)
            {
                d = d + 2 * x + 3;
                x += 1;
            }
            else
            {
                d = d + 2 * (x - y) + 5;
                x += 1;
                y -= 1;
            }

            result.Add(new Vector2Int(c.x + x, c.y + y));
            result.Add(new Vector2Int(c.x + x, c.y - y));
            result.Add(new Vector2Int(c.x - x, c.y + y));
            result.Add(new Vector2Int(c.x - x, c.y - y));

            result.Add(new Vector2Int(c.x + y, c.y + x));
            result.Add(new Vector2Int(c.x + y, c.y - x));
            result.Add(new Vector2Int(c.x - y, c.y + x));
            result.Add(new Vector2Int(c.x - y, c.y - x));
        }

        return result;
    }
}