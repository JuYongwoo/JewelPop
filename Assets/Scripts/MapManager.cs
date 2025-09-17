using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    private HexBlock[,] board;
    private Transform boardRoot;
    private LevelDef level;

    private const float xStep = 0.9f;
    private const float yStep = 0.78f;

    // ��ġ �� �߾Ӻ��(=���� �����߽ɿ� ���� ����� ���)�� 0,0���� �̵��ϱ� ���� ����
    private readonly List<(Transform t, Vector3 lp)> placed = new List<(Transform, Vector3)>();

    public void OnAwake()
    {
        var res = ManagerObject.instance.resourceManager;
        if (res == null || res.mapJSON == null || string.IsNullOrEmpty(res.mapJSON.text)) return;

        level = JsonUtility.FromJson<LevelDef>(res.mapJSON.text);

        var rootObj = GameObject.Find("BoardRoot");
        if (rootObj == null) rootObj = new GameObject("BoardRoot");
        boardRoot = rootObj.transform;

        for (int i = boardRoot.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(boardRoot.GetChild(i).gameObject);

        board = new HexBlock[level.width, level.height];
        placed.Clear();

        BuildBoardFromJson(level);

        // �߾� ����� (0,0)�� �ε��� ������ ���� (�� ������ �״��)
        MoveMiddleBlockToOrigin();
    }

    private void BuildBoardFromJson(LevelDef def)
    {
        var res = ManagerObject.instance.resourceManager;
        if (res.blockPrefab == null) return;

        foreach (CellDef cell in def.cells)
        {
            int r = cell.r, c = cell.c;
            if (!InBounds(c, r, def.width, def.height)) continue;

            Vector3 lp = HexToLocal(c, r);

            GameObject go = Object.Instantiate(res.blockPrefab, boardRoot);
            go.transform.localPosition = lp;
            go.name = $"r{r}c{c}";

            var b = go.GetComponent<HexBlock>();
            if (b != null && cell.color >= 0) b.SetColor(cell.color);

            board[c, r] = b;
            placed.Add((go.transform, lp));
        }
    }

    // ������ �����߽��� ���ϰ�, �� ���� ���� ����� ����� (0,0)���� �̵���Ű�� ������ ����
    private void MoveMiddleBlockToOrigin()
    {
        if (placed.Count == 0) return;

        // 1) �����߽�(����)
        Vector2 centroid = Vector2.zero;
        foreach (var p in placed) centroid += (Vector2)p.lp;
        centroid /= placed.Count;

        // 2) �����߽ɿ� ���� ����� ��� ����(= �߾� ���)
        int midIdx = 0;
        float best = float.PositiveInfinity;
        for (int i = 0; i < placed.Count; i++)
        {
            float d = Vector2.SqrMagnitude((Vector2)placed[i].lp - centroid);
            if (d < best) { best = d; midIdx = i; }
        }

        Vector3 midPos = placed[midIdx].lp;   // �߾Ӻ���� ���� ������ǥ

        // 3) ��� �ڽ��� ������ǥ���� midPos��ŭ ���� �߾Ӻ���� (0,0)����
        Vector3 offset = -midPos;
        for (int i = 0; i < placed.Count; i++)
        {
            var tr = placed[i].t;
            tr.localPosition += offset;
        }
    }

    private static bool InBounds(int x, int y, int w, int h)
    {
        return x >= 0 && x < w && y >= 0 && y < h;
    }

    // odd-r ������ �� ���� ��ǥ
    private static Vector3 HexToLocal(int x, int y)
    {
        float xPos = x * xStep + ((y % 2 == 1) ? xStep * 0.5f : 0f);
        float yPos = y * yStep;
        return new Vector3(xPos, yPos, 0f);
    }
}
