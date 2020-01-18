﻿using UnityEngine;
using System;

public class BaseGrid : MonoBehaviour {
    public int m_widthCount = 10;
    public int m_heightCount = 10;

    public Material m_mat;

    protected float TileWidth { get { return 1.0f / m_widthCount; } }

    protected float VisualWidth { get { return 0.9f / m_widthCount; } } //中间留一些格子间的间隔

    protected float TileHeight { get { return 1.0f / m_heightCount; } }

    protected float VisualHeight { get { return 0.9f / m_heightCount; } }

    protected virtual void OnRenderObject()
    {
        GL.PushMatrix();
        GL.LoadOrtho();

        Draw();
        
        GL.PopMatrix();
    }
    
    protected virtual void Draw()
    {
        GL.Begin(GL.QUADS);
        m_mat.SetPass(0);

        DrawGrid();

        GL.End();
    }

    void DrawGrid()
    {
        float w = TileWidth;
        float vw = VisualWidth;
        float h = TileHeight;
        float vh = VisualHeight;

        for(int x = 0; x < m_widthCount; x++)
        {
            for(int y = 0; y < m_heightCount; y++)
            {
                Vector2 pos = new Vector2(x * w, y * h);
                Vector2 size = new Vector2(vw, vh);
                DrawQuad(pos, size);
            }
        }
    }

    protected void DrawQuad(Vector2Int tilePos)
    {
        Vector2 pos = new Vector2(tilePos.x * TileWidth, tilePos.y * TileHeight);
        Vector2 size = new Vector2(TileWidth * 0.9f, TileHeight * 0.9f);
        DrawQuad(pos, size);
    }

    protected static void DrawQuad(Vector2 pos, Vector2 size)
    {
        GL.Vertex3(pos.x, pos.y, 0);
        GL.Vertex3(pos.x, pos.y + size.y, 0);
        GL.Vertex3(pos.x + size.x, pos.y + size.y, 0);
        GL.Vertex3(pos.x + size.x, pos.y, 0);
    }

    protected bool IsValidTilePos(Vector2Int tilePos)
    {
        return tilePos.x >= 0 && tilePos.x < m_widthCount &&
                    tilePos.y >= 0 && tilePos.y < m_heightCount;
    }

    protected Vector2Int GetMouseTilePos()
    {
        float x = Input.mousePosition.x / Screen.width;
        float y = Input.mousePosition.y / Screen.height;
        return ScreenPosToTilePos(new Vector2(x, y));
    }

    protected Vector2Int ScreenPosToTilePos(Vector2 pos)
    {
        int ix = (int)(pos.x * m_widthCount);
        int iy = (int)(pos.y * m_heightCount);
        return new Vector2Int(ix, iy);
    }

    protected Vector2 TilePosToScreenPos(Vector2Int tilePos)
    {
        return new Vector2(tilePos.x * TileWidth, tilePos.y * TileHeight);
    }
}