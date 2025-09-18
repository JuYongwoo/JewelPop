using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MapManager
{
    private List<List<GameObject>> board = new List<List<GameObject>>();
    private JSONVars jsonVars;

    private const float xStep = 0.6f;
    private const float yStep = 0.7f;

    public void OnAwake()
    {
        //JSON 읽기
        jsonVars = JsonUtility.FromJson<JSONVars>(ManagerObject.instance.resourceManager.mapJSON.text); //JSON 파일을 JSONVars로 읽어온다
        
        setBlocks();
        MoveMiddleBlockToOrigin(); //전체 위치 옮겨서 중앙으로 끌고온다

        ManagerObject.instance.actionManager.blockChangeAction = blockChange;

    }

    private void setBlocks()
    {
        foreach (var grid in jsonVars.grids)
        {
            // 행 확장
            while (board.Count <= grid.y)
                board.Add(new List<GameObject>());

            // 열 확장
            while (board[grid.y].Count <= grid.x)
                board[grid.y].Add(null);

            // 부모 오브젝트 없으면 생성
            if (board[grid.y][grid.x] == null)
            {
                board[grid.y][grid.x] = Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab);
                Object.Instantiate(ManagerObject.instance.resourceManager.blockBackPrefab, board[grid.y][grid.x].transform);
            }
            
            
            GameObject block = Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[grid.type], board[grid.y][grid.x].transform);
            block.GetComponent<Block>().setGridPosition(grid.y, grid.x);


            Vector3 lp;




            //유니티는 y좌표가 오를 수록 위로 가기 때문에 배열 상 11시 방향부터 아래로 내려오도록
            if (grid.x % 2 == 1) //홀수 x들
            {
                lp = new Vector2(grid.x * xStep, -grid.y * yStep + yStep * 0.5f); //(반 칸 올린다)

            }
            else
            {
                lp = new Vector2(grid.x * xStep, -grid.y * yStep);

            }
           
            board[grid.y][grid.x].transform.localPosition = lp;
            board[grid.y][grid.x].name = $"r{grid.y}c{grid.x}";
        }
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
    private bool _isSwapping = false;

    private void blockChange(GameObject startArg, GameObject nextArg)
    {
        if (_isSwapping) return;

        var startCell = ResolveCell(startArg.transform);
        var nextCell = ResolveCell(nextArg.transform);

        var startBlockTf = GetDirectBlockChild(startCell);
        var nextBlockTf = GetDirectBlockChild(nextCell);
        if (startBlockTf == null || nextBlockTf == null) return;

        _isSwapping = true;
        ManagerObject.instance.StartCoroutine(
            SwapBlocksBetweenParents(startBlockTf.gameObject, nextBlockTf.gameObject, startCell, nextCell));
    }

    private IEnumerator SwapBlocksBetweenParents(
        GameObject startBlockGO, GameObject nextBlockGO,
        Transform startCell, Transform nextCell)
    {
        Vector3 startFrom = startBlockGO.transform.position;
        Vector3 nextFrom = nextBlockGO.transform.position;
        Vector3 startGoal = nextCell.TransformPoint(Vector3.zero);
        Vector3 nextGoal = startCell.TransformPoint(Vector3.zero);

        float t = 0f, speed = 5f, snap2 = 0.01f * 0.01f;
        while (true)
        {
            t += Time.deltaTime * speed; if (t > 1f) t = 1f;

            startBlockGO.transform.position = Vector3.Lerp(startFrom, startGoal, t);
            nextBlockGO.transform.position = Vector3.Lerp(nextFrom, nextGoal, t);

            if ((startBlockGO.transform.position - startGoal).sqrMagnitude <= snap2 &&
                (nextBlockGO.transform.position - nextGoal).sqrMagnitude <= snap2)
                break;
            yield return null;
        }

        // 부모 교환: 반드시 "셀"로, world 유지
        startBlockGO.transform.SetParent(nextCell, true);
        nextBlockGO.transform.SetParent(startCell, true);

        // 셀 중심으로 스냅
        startBlockGO.transform.localPosition = Vector3.zero;
        nextBlockGO.transform.localPosition = Vector3.zero;

        // Block이 항상 Back 위로 오게
        startBlockGO.transform.SetAsLastSibling();
        nextBlockGO.transform.SetAsLastSibling();

        _isSwapping = false;
    }




    // 전달된 트랜스폼이 블록이면 그 부모(셀)로 정규화
    private Transform ResolveCell(Transform t)
    {
        return t.GetComponent<Block>() != null ? t.parent : t;
    }

    // 셀의 "직계 자식" 중 Block 달린 것만 반환
    private Transform GetDirectBlockChild(Transform cell)
    {
        foreach (Transform ch in cell)
            if (ch.GetComponent<Block>() != null) return ch;
        return null;
    }



    private bool checkIsThere3Chain()
    {
        //모든 블록을 돌며 연속 3라인이 있는지 확인한다.

        return false;
    }

    private void clearAll3Chains()
    {
        //모든 연속 3라인을 없앤다.
    }

    private void dropAllBlockesAndSpawn()
    {
        //아래가 빈 블럭은 아래로 옮기고 맨위에는 새로운 블럭이 추가된다.
    }




    // 보드 경계 체크 (들쑥날쑥 행 길이 대응)
    private bool InBounds(List<List<GameObject>> b, int y, int x)
    {
        return y >= 0 && y < b.Count && x >= 0 && x < b[y].Count && b[y][x] != null;
    }

    // odd-q(홀수 열이 아래로) 기준 이웃 한 칸 이동
    // dir: 0=E, 1=W, 2=NE, 3=SW, 4=NW, 5=SE
    private (int nx, int ny) StepOddQ(int x, int y, int dir)
    {
        bool isOdd = (x & 1) == 1;
        switch (dir)
        {
            case 0: return (x + 1, y);                     // E
            case 1: return (x - 1, y);                     // W
            case 2: return isOdd ? (x, y - 1) : (x + 1, y - 1); // NE
            case 3: return isOdd ? (x, y + 1) : (x - 1, y + 1); // SW
            case 4: return isOdd ? (x - 1, y - 1) : (x, y - 1);        // NW
            case 5: return isOdd ? (x + 1, y + 1) : (x, y + 1);        // SE
            default: return (x, y);
        }
    }

    // 한 방향으로 같은 색 몇 개 이어지는지(현재 칸 제외)
    private int RunLen(List<List<GameObject>> b, int y, int x, int dir)
    {
        int cnt = 0;
        var cur = b[y][x]?.GetComponent<Block>();
        int cx = x, cy = y;

        while (true)
        {
            (cx, cy) = StepOddQ(cx, cy, dir);
            if (!InBounds(b, cy, cx)) break;
            var nb = b[cy][cx]?.GetComponent<Block>();
            if (nb == null || !cur.IsSameColor(nb)) break;
            cnt++;
        }
        return cnt;
    }

    // 육각 3축(E-W, NE-SW, NW-SE)으로 3개 이상 연속 검사
    private bool checkIsThere3Chain(List<List<GameObject>> b)
    {
        if (b == null || b.Count == 0) return false;

        // 축 정의: (정/역) 방향 쌍
        int[][] axes = new int[][]
        {
        new int[]{ 0, 1 }, // E-W
        new int[]{ 2, 3 }, // NE-SW
        new int[]{ 4, 5 }, // NW-SE
        };

        for (int y = 0; y < b.Count; y++)
        {
            for (int x = 0; x < b[y].Count; x++)
            {
                var cur = b[y][x]?.GetComponent<Block>();
                if (cur == null) continue;

                foreach (var ax in axes)
                {
                    int len = 1; // 현재 칸 포함
                    len += RunLen(b, y, x, ax[0]);
                    len += RunLen(b, y, x, ax[1]);
                    if (len >= 3) return true;
                }
            }
        }
        return false;
    }


}
