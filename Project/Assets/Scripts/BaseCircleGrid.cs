using UnityEngine;
using System.Collections.Generic;

public abstract class BaseCircleGrid : BaseGrid
{
    [Range(0, 1)]
    public float m_radius = 0.3f;
    public Material m_circleMat;

    protected Vector2Int m_center;

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
        else if (Input.GetMouseButton(1))
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
        List<Vector2Int> points = GetCirclePoints();
        for (int i = 0; i < points.Count; i++)
            DrawQuad(points[i]);
        GL.End();
    }

    protected abstract List<Vector2Int> GetCirclePoints();
}