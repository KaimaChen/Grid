using UnityEngine;
using System;

/// <summary>
/// 并查集
/// </summary>
public class UnionFind
{
    const int c_reserveSize = 1000; //数组初始大小
    const int c_addSize = 100; //每次数组改变大小时会增加的大小

    int[] mParent; //用数组而不是List，是为了加快速度，因为List需要经常去加item来保证Index对的上

    /// <summary>
    /// 指定容量构造
    /// </summary>
    /// <param name="capacity"></param>
    public UnionFind(int capacity = c_reserveSize)
    {
        mParent = new int[capacity];
        //初始化数据
        Clear();
    }

    public void Clear()
    {
        for (int i = 0; i < mParent.Length; i++)
        {
            mParent[i] = i;
        }
    }

    /// <summary>
    /// 查找元素的根（路径压缩版）
    /// </summary>
    public int Find(int x)
    {
        if (mParent[x] == x)
        {
            return x;
        }
        else
        {
            int result = Find(mParent[x]);
            mParent[x] = result;
            return result;
        }
    }

    /// <summary>
    /// 把两者合并到同一个集合里
    /// </summary>
    public void Union(int p, int q)
    {
        //调整mParent的大小
        int bigger = Mathf.Max(p, q);
        if (bigger >= mParent.Length)
        {
            int length = Mathf.Max(bigger + 1, mParent.Length + c_addSize);

            int[] newParent = new int[length];
            Array.Copy(mParent, newParent, mParent.Length);
            for (int i = mParent.Length; i < newParent.Length; i++)
            {
                newParent[i] = i;
            }
            mParent = newParent;
        }

        int pRoot = Find(p);
        int qRoot = Find(q);
        if (pRoot == qRoot)
            return;

        mParent[pRoot] = qRoot;
    }

    /// <summary>
    /// 查询两者是否连通
    /// </summary>
    public bool IsConnected(int p, int q)
    {
        return Find(p) == Find(q);
    }
}