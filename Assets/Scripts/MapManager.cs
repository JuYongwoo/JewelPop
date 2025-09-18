using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MapManager
{
    private List<List<GameObject>> board = new List<List<GameObject>>();
    private JSONVars level;

    private const float xStep = 0.6f;
    private const float yStep = 0.7f;

    private int maxX =-1;
    private int maxY = -1;

    public void OnAwake()
    {
        //JSON 읽기
        level = JsonUtility.FromJson<JSONVars>(ManagerObject.instance.resourceManager.mapJSON.text);
        
        setBlocks();
        setBlocksBack();
        MoveMiddleBlockToOrigin();
    }

    private void setBlocks()
    {
        foreach (var cell in level.grids)
        {
            // 행 확장
            while (board.Count <= cell.y)
                board.Add(new List<GameObject>());

            // 열 확장
            while (board[cell.y].Count <= cell.x)
                board[cell.y].Add(null);

            // 부모 오브젝트 없으면 생성
            if (board[cell.y][cell.x] == null)
            {
                board[cell.y][cell.x] = Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab);
                Object.Instantiate(ManagerObject.instance.resourceManager.blockBackPrefab, board[cell.y][cell.x].transform);
            }
            // 블록 생성 (부모 = parentObject)
            Object.Instantiate(
                ManagerObject.instance.resourceManager.blockPrefab,
                board[cell.y][cell.x].transform
            );

            // 좌표/이름 지정
            Vector3 lp = new Vector3(
                cell.x * xStep,
                -cell.y * yStep + ((cell.x % 2 == 1) ? yStep * 0.5f : 0f), //유니티는 y좌표가 오를 수록 위로 가기 때문에 배열 상 11시 방향부터 아래로 내려오도록
                0f
            );
            board[cell.y][cell.x].transform.localPosition = lp;
            board[cell.y][cell.x].name = $"r{cell.y}c{cell.x}";
        }
    }

    private void setBlocksBack()
    {

    }


    private void MoveMiddleBlockToOrigin()
    {
        var allBlocks = board.SelectMany(row => row).Where(go => go != null).ToList();
        if (allBlocks.Count == 0) return;

        // 무게중심
        Vector2 centroid = Vector2.zero;
        foreach (var go in allBlocks)
            centroid += (Vector2)go.transform.localPosition;
        centroid /= allBlocks.Count;

        // 중앙 블록 탐색
        GameObject midBlock = allBlocks
            .OrderBy(go => Vector2.SqrMagnitude((Vector2)go.transform.localPosition - centroid))
            .First();

        // 오프셋 적용
        Vector3 offset = -midBlock.transform.localPosition;
        foreach (var go in allBlocks)
            go.transform.localPosition += offset;
    }





}
