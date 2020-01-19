using UnityEngine;
using System.Collections.Generic;

public enum LineAlgorithm
{
    Bresenhams,
    Lerp,
    WalkGrid,
    Supercover
}

/// <summary>
/// 绘制直线
/// * 鼠标左键：直线起点
/// * 鼠标右键：直线终点
/// </summary>
public class LineGrid : BaseGrid
{
    readonly Color c_startColor = new Color(1, 1, 0);
    readonly Color c_endColor = new Color(0, 0, 1);
    readonly Color c_lineColor = new Color(1, 0.5f, 0.5f);

    public LineAlgorithm m_algorithm;

    Vector2Int m_start;
    Vector2Int m_end;

    void Start()
    {
        m_start = new Vector2Int(0, m_heightCount / 2);
        m_end = new Vector2Int(m_widthCount - 1, m_heightCount / 2);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            m_start = GetMouseTilePos();
        else if (Input.GetMouseButton(1))
            m_end = GetMouseTilePos();
    }

    private List<Vector2Int> GetLinePoints()
    {
        switch(m_algorithm)
        {
            case LineAlgorithm.Bresenhams:
                return LineByBresenhams(m_start, m_end);
            case LineAlgorithm.Lerp:
                return LineByLerp(m_start, m_end);
            case LineAlgorithm.WalkGrid:
                return LineByWalkGrid(m_start, m_end);
            case LineAlgorithm.Supercover:
                return LineBySupercover(m_start, m_end);
            default:
                Debug.LogError($"No LineAlgorithm={m_algorithm}");
                return LineBySupercover(m_start, m_end);
        }
    }

    protected override void Draw()
    {
        base.Draw();

        GL.Begin(GL.QUADS);
        GL.Color(c_lineColor);
        List<Vector2Int> points = GetLinePoints();
        for (int i = 0; i < points.Count; i++)
            DrawQuad(points[i]);

        GL.Color(c_startColor);
        DrawQuad(m_start);

        GL.Color(c_endColor);
        DrawQuad(m_end);
        GL.End();

        GL.Begin(GL.LINES);
        GL.Vertex3((m_start.x+0.5f) * TileWidth, (m_start.y+0.5f) * TileHeight, 0);
        GL.Vertex3((m_end.x+0.5f) * TileWidth, (m_end.y+0.5f) * TileHeight, 0);
        GL.End();
    }

    /// <summary>
    /// Bresenhams
    /// </summary>
    /// <param name="start">直线起点</param>
    /// <param name="end">直线终点</param>
    /// <returns>直线覆盖的格子</returns>
    public static List<Vector2Int> LineByBresenhams(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int dx = end.x - start.x;
        int dy = end.y - start.y;
        int ux = dx > 0 ? 1 : -1;
        int uy = dy > 0 ? 1 : -1;
        int x = start.x;
        int y = start.y;
        int eps = 0;
        dx = Mathf.Abs(dx);
        dy = Mathf.Abs(dy);

        if (dx > dy)
        {
            for (x = start.x; x != end.x; x += ux)
            {
                result.Add(new Vector2Int(x, y));

                eps += dy;
                if ((eps << 1) >= dx)
                {
                    y += uy;
                    eps -= dx;
                }
            }
        }
        else
        {
            for (y = start.y; y != end.y; y += uy)
            {
                result.Add(new Vector2Int(x, y));

                eps += dx;
                if ((eps << 1) >= dy)
                {
                    x += ux;
                    eps -= dy;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 利用插值生成直线 （和DDA算法类似）
    /// 参考 https://www.redblobgames.com/grids/line-drawing.html
    /// </summary>
    public static List<Vector2Int> LineByLerp(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int nx = Mathf.Abs(end.x - start.x);
        int ny = Mathf.Abs(end.y - start.y);
        int n = Mathf.Max(nx, ny);
        for(int step = 0; step <= n; step++)
        {
            float t = (n == 0 ? 0 : (float)step / n);
            Vector2 p = Utils.Lerp(start, end, t);
            result.Add(Vector2Int.RoundToInt(p));
        }

        return result;
    }

    /// <summary>
    /// 利用走格子生成直线（不能走斜角）
    /// 参考 https://www.redblobgames.com/grids/line-drawing.html
    /// </summary>
    public static List<Vector2Int> LineByWalkGrid(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int dx = end.x - start.x;
        int dy = end.y - start.y;
        int nx = Mathf.Abs(dx);
        int ny = Mathf.Abs(dy);
        int signX = dx > 0 ? 1 : -1;
        int signY = dy > 0 ? 1 : -1;

        Vector2Int p = new Vector2Int(start.x, start.y);
        for(int ix = 0, iy = 0; ix < nx || iy < ny;)
        {
            if((0.5f + ix) / nx < (0.5f + iy) / ny)
            {
                p.x += signX;
                ix++;
            }
            else
            {
                p.y += signY;
                iy++;
            }

            result.Add(p);
        }

        return result;
    }

    /// <summary>
    /// 在WalkGrid的基础上多了斜线处理
    /// 参考 https://www.redblobgames.com/grids/line-drawing.html
    /// </summary>
    public static List<Vector2Int> LineBySupercover(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int dx = end.x - start.x;
        int dy = end.y - start.y;
        int nx = Mathf.Abs(dx);
        int ny = Mathf.Abs(dy);
        int signX = dx > 0 ? 1 : -1;
        int signY = dy > 0 ? 1 : -1;

        Vector2Int p = new Vector2Int(start.x, start.y);
        for (int ix = 0, iy = 0; ix < nx || iy < ny;)
        {
            float px = (0.5f + ix) / nx;
            float py = (0.5f + iy) / ny;

            //if (px == py) //因为直接比较浮点不好，所以用下面这种形式
            if((1+2*ix) * ny == (1+2*iy)*nx)
            {
                p.x += signX;
                p.y += signY;
                ix++;
                iy++;
            }
            else if (px < py)
            {
                p.x += signX;
                ix++;
            }
            else
            {
                p.y += signY;
                iy++;
            }

            result.Add(p);
        }

        return result;
    }
}