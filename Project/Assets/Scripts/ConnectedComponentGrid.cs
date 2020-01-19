using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 查看区域连通性
/// * 鼠标左键：添加障碍
/// * 鼠标右键：移除障碍
/// </summary>
public class ConnectedComponentGrid : BaseGrid
{
    private const int c_obstacle = -1;

    readonly UnionFind m_unionFind = new UnionFind();
    short[][] m_map;

    readonly Dictionary<int, Color> m_areaToColor = new Dictionary<int, Color>();
    readonly List<Color> m_colorPool = new List<Color>();

    void Start()
    {
        m_map = new short[m_widthCount][];
        for(int x = 0; x < m_widthCount; x++)
        {
            m_map[x] = new short[m_heightCount];
            for (int y = 0; y < m_heightCount; y++)
                m_map[x][y] = 0;
        }

        InitColorPool();
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector2Int tilePos = GetMouseTilePos();
            if(IsValidTilePos(tilePos))
            {
                short origin = m_map[tilePos.x][tilePos.y];
                if(origin != c_obstacle)
                {
                    m_map[tilePos.x][tilePos.y] = c_obstacle;
                    UpdateConnectComponent();
                }
            }
        }
        else if(Input.GetMouseButton(1))
        {
            Vector2Int tilePos = GetMouseTilePos();
            if (IsValidTilePos(tilePos))
            {
                short origin = m_map[tilePos.x][tilePos.y];
                if (origin == c_obstacle)
                {
                    m_map[tilePos.x][tilePos.y] = 0;
                    UpdateConnectComponent();
                }
            }
        }
    }

    /// <summary>
    /// 更新连通分量
    /// </summary>
    void UpdateConnectComponent()
    {
        //算法思路
        //1. 按行扫描
        //2. 找到可行的结点
        //3. 查看该可行结点的左下邻居是否有islandId，有则用该id，没有则新建一个id
        //4. 如果邻居的id不同，则随意用其中一个，并把这些不同的id标记为相同

        m_unionFind.Clear();

        short ccCount = 1;
        for (int x = 0; x < m_widthCount; x++)
        {
            for (int y = 0; y < m_heightCount; y++)
            {
                if (m_map[x][y] == c_obstacle)
                    continue;

                short leftValue = 0;
                short downValue = 0;

                int left = x - 1;
                if (left >= 0)
                {
                    if (m_map[left][y] != c_obstacle)
                        leftValue = m_map[left][y];
                }

                int down = y - 1;
                if (down >= 0)
                {
                    if (m_map[x][down] != c_obstacle)
                        downValue = m_map[x][down];
                }

                if (leftValue == 0 && downValue == 0)
                {
                    m_map[x][y] = ccCount++;
                }
                else if (leftValue == 0 && downValue != 0)
                {
                    m_map[x][y] = downValue;
                }
                else if (leftValue != 0 && downValue == 0)
                {
                    m_map[x][y] = leftValue;
                }
                else
                {
                    m_map[x][y] = leftValue;
                    m_unionFind.Union(leftValue, downValue);
                }
            }
        }
    }

    #region 显示
    void InitColorPool()
    {
        float b = 0.7f;
        m_colorPool.Add(new Color(b, b, b));
        m_colorPool.Add(new Color(b, 0, 0));
        m_colorPool.Add(new Color(0, b, 0));
        m_colorPool.Add(new Color(0, 0, b));
        m_colorPool.Add(new Color(b, b, 0));
        m_colorPool.Add(new Color(b, 0, b));
        m_colorPool.Add(new Color(0, b, b));
    }

    protected override void Draw()
    {
        m_areaToColor.Clear();

        GL.Begin(GL.QUADS);
        for (int x = 0; x < m_widthCount; x++)
        {
            for (int y = 0; y < m_heightCount; y++)
            {
                GL.Color(CalcColor(x, y));
                DrawQuad(new Vector2Int(x, y));
            }
        }
        GL.End();
    }

    Color CalcColor(int x, int y)
    {
        int value = m_map[x][y];
        if (value == c_obstacle)
            return Color.black;

        int areaId = m_unionFind.Find(value);
        if (!m_areaToColor.TryGetValue(areaId, out Color c))
        {
            int index = m_areaToColor.Count;
            if (index < m_colorPool.Count)
            {
                c = m_colorPool[index];
            }
            else
            {
                c = Color.white;
                Debug.LogError("颜色池用完了，默认用白色");
            }

            m_areaToColor[areaId] = c;
        }

        return c;
    }
    #endregion
}