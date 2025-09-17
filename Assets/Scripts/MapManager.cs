using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    private HexBlock[,] board;
    private Transform boardRoot;
    private LevelDef level;

    private const float xStep = 0.9f;
    private const float yStep = 0.78f;

    // 배치 후 중앙블록(=보드 무게중심에 가장 가까운 블록)을 0,0으로 이동하기 위해 보관
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

        // 중앙 블록을 (0,0)에 두도록 오프셋 적용 (맵 구조는 그대로)
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

    // 보드의 무게중심을 구하고, 그 점에 가장 가까운 블록을 (0,0)으로 이동시키는 오프셋 적용
    private void MoveMiddleBlockToOrigin()
    {
        if (placed.Count == 0) return;

        // 1) 무게중심(로컬)
        Vector2 centroid = Vector2.zero;
        foreach (var p in placed) centroid += (Vector2)p.lp;
        centroid /= placed.Count;

        // 2) 무게중심에 가장 가까운 블록 선택(= 중앙 블록)
        int midIdx = 0;
        float best = float.PositiveInfinity;
        for (int i = 0; i < placed.Count; i++)
        {
            float d = Vector2.SqrMagnitude((Vector2)placed[i].lp - centroid);
            if (d < best) { best = d; midIdx = i; }
        }

        Vector3 midPos = placed[midIdx].lp;   // 중앙블록의 현재 로컬좌표

        // 3) 모든 자식의 로컬좌표에서 midPos만큼 빼서 중앙블록을 (0,0)으로
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

    // odd-r 오프셋 → 로컬 좌표
    private static Vector3 HexToLocal(int x, int y)
    {
        float xPos = x * xStep + ((y % 2 == 1) ? xStep * 0.5f : 0f);
        float yPos = y * yStep;
        return new Vector3(xPos, yPos, 0f);
    }
}
