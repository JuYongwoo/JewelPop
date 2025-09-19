using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;



public class MapManager
{


    private Dictionary<(int, int), BlockParent> board = new Dictionary<(int, int), BlockParent>();
    private JSONVars jsonVars;

    private const float xStep = 0.6f;
    private const float yStep = 0.7f;

    private bool _isSwapping = false;



    (int dy, int dx)[] oddXDirections = new(int dy, int dx)[] {
        //각 두개마다 base가 들어가면 연속 3블럭
        (-1, -1),
        (0, 1),


        (-1, 1),
        (0, -1),

        (-1, 0),
        (1, 0),
    };

    (int dy, int dx)[] evenXDirections = new (int dy, int dx)[] {
        (1, -1),
        (0, 1),


        (1, 1),
        (0, -1),

        (1, 0),
        (-1, 0),
    };


    public void OnAwake()
    {
        //JSON 읽기
        jsonVars = JsonUtility.FromJson<JSONVars>(ManagerObject.instance.resourceManager.mapJSON.text); //JSON 파일을 JSONVars로 읽어온다
        
        //맵 제작
        setBlocks();

        //위치 중앙으로
        MoveMiddleBlockToOrigin();

        //액션 intermediate
        ManagerObject.instance.actionManager.blockChangeAction = blockChange;

    }

    public void OnStart()
    {
        clearAll3Chains();
    }

    private void setBlocks()
    {
        foreach (var grid in jsonVars.grids)
        {
            board.Add((grid.y, grid.x),Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab).GetOrAddComponent<BlockParent>());
            board[(grid.y, grid.x)].name = $"y{grid.y}x{grid.x}";

            Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[grid.type], board[(grid.y, grid.x)].transform);
            
            
            board[(grid.y, grid.x)].setPositionYX((grid.y, grid.x)); //그리드 좌표
            board[(grid.y, grid.x)].setUnityPositionYX(grid.x % 2 == 1 ? (-grid.y * yStep + yStep * 0.5f, grid.x * xStep) : (-grid.y * yStep, grid.x * xStep)); //지그재그, 홀수 X는 위로 반칸
        }
    }



    private void MoveMiddleBlockToOrigin()
    {

        // 무게중심
        Vector2 centroid = Vector2.zero;
        foreach (var go in board.Values)
            centroid += (Vector2)go.transform.localPosition;
        centroid /= board.Count;

        // 중앙 블록 탐색
        BlockParent midBlock = board.Values.OrderBy(blockParent => Vector2.SqrMagnitude((Vector2)blockParent.transform.localPosition - centroid)).First();

        // 오프셋 적용
        Vector3 offset = -midBlock.transform.localPosition;
        foreach (var go in board.Values)
            go.transform.localPosition += offset;
    }

    private void blockChange(GameObject startArg, GameObject nextArg)
    {
        if (_isSwapping) return;

        if (!getNeighbors(startArg.transform.parent.GetComponent<BlockParent>().getPosition()).Contains(nextArg.transform.parent.GetComponent<BlockParent>().getPosition()))
        {
            Debug.Log(getNeighbors(startArg.transform.parent.GetComponent<BlockParent>().getPosition()) + "     " + nextArg.transform.parent.GetComponent<BlockParent>().getPosition());
            return; //이웃이 아니면 리턴
        }


        _isSwapping = true;
        ManagerObject.instance.StartCoroutine(swapBlockChild(startArg.transform.parent.gameObject, nextArg.transform.parent.gameObject)); //클릭된 게임 오브젝트는 부모오브젝트
        //TODO 서로 바꾸는게 아니라 하나가 이동하는것으로, 여기선 서로가 이동해서 각자가 한번씩, 총 두번 실행하도록 해야함
    }


    private IEnumerator swapBlockChild(GameObject startBlockGO, GameObject nextBlockGO) //자식 오브젝트끼리 바꿔야함
    {

        // 두 부모 밑에 자식이 없는 경우 → 교환 불가
        if (startBlockGO.transform.childCount == 0 || nextBlockGO.transform.childCount == 0)
        {
            _isSwapping = false;
            yield break;
        }

        float t = 0f, speed = 5f, snap2 = 0.01f * 0.01f;
        while (true)
        {
            t += Time.deltaTime * speed; if (t > 1f) t = 1f;

            startBlockGO.transform.GetChild(0).position = Vector3.Lerp(startBlockGO.transform.position, nextBlockGO.transform.position, t);
            nextBlockGO.transform.GetChild(0).position = Vector3.Lerp(nextBlockGO.transform.position, startBlockGO.transform.position, t);

            if ((startBlockGO.transform.GetChild(0).position - nextBlockGO.transform.position).sqrMagnitude <= snap2
                && (nextBlockGO.transform.GetChild(0).position - startBlockGO.transform.position).sqrMagnitude <= snap2)
                break;
            yield return null;
        }


        UnityEngine.Transform go1 = startBlockGO.transform.GetChild(0);
        go1.SetParent(nextBlockGO.transform, true);
        go1.localPosition = Vector3.zero;


        UnityEngine.Transform go2 = nextBlockGO.transform.GetChild(0);
        go2.SetParent(startBlockGO.transform, true);
        go2.localPosition = Vector3.zero;



        _isSwapping = false;

        clearAll3Chains();

    }


    private IEnumerator moveBlockChild(GameObject startBlockGO, GameObject nextBlockGO) //자식 오브젝트끼리 바꿔야함
    {

        float t = 0f, speed = 5f, snap2 = 0.01f * 0.01f;
        while (true)
        {
            t += Time.deltaTime * speed; if (t > 1f) t = 1f;

            startBlockGO.transform.GetChild(0).position = Vector3.Lerp(startBlockGO.transform.position, nextBlockGO.transform.position, t);

            if ((startBlockGO.transform.GetChild(0).position - nextBlockGO.transform.position).sqrMagnitude <= snap2)
                break;
            yield return null;
        }


        UnityEngine.Transform go = startBlockGO.transform.GetChild(0);
        go.SetParent(nextBlockGO.transform, true);
        go.localPosition = Vector3.zero;




        clearAll3Chains();

    }




    private List<(int y, int x)> getNeighbors((int y, int x) baseYX)
    {

        List<(int y, int x)> neighbors = new List<(int y, int x)>();

        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i++)
        {
            neighbors.Add((baseYX.y + directions[i].dy, baseYX.x + directions[i].dx));

        }

        return neighbors;

    }


    private List<(int y, int x)> checkIsBurstable((int y, int x) baseYX)
    {
        List<(int y, int x)> burstables = new List<(int y, int x)>();
        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i += 2)
        {
            var p1 = (y: baseYX.y + directions[i].dy, x: baseYX.x + directions[i].dx);
            var p2 = (y: baseYX.y + directions[i + 1].dy, x: baseYX.x + directions[i + 1].dx);

            if (!isValid(baseYX) || !isValid(p1) || !isValid(p2)) continue;
            if (board[(baseYX.y, baseYX.x)] == null || board[(p1.y, p1.x)] == null || board[(p2.y, p2.x)] == null) continue;



            // 자식 없으면 스킵
            if (board[(baseYX.y, baseYX.x)].transform.childCount == 0
                || board[(p1.y, p1.x)].transform.childCount == 0
                || board[(p2.y, p2.x)].transform.childCount == 0) continue;

            string type0 = board[(baseYX.y, baseYX.x)].transform.GetChild(0).GetComponent<BlockChild>().getBlockType();
            string type1 = board[(p1.y, p1.x)].transform.GetChild(0).GetComponent<BlockChild>().getBlockType();
            string type2 = board[(p2.y, p2.x)].transform.GetChild(0).GetComponent<BlockChild>().getBlockType();

            if (type0.Equals(type1) && type0.Equals(type2))
            {
                burstables.Add(baseYX);
                burstables.Add(p1);
                burstables.Add(p2);
            }
        }

        return burstables;
    }

    private bool isValid((int y, int x) pos)
    {
        return board.ContainsKey(pos) ? true : false;
    }


    private void clearAll3Chains()
    {

        List<(int y, int x)> dels = new List<(int y, int x)>();
        foreach(var grid in board)
            foreach(var del in checkIsBurstable(grid.Key))
                dels.Add(del);


        foreach (var a in dels)
            board[a].transform.GetChild(0).GetComponent<BlockChild>().destroySelf();
    }


    private void dropAllBlocks() //중력에 의해 y+ 칸이 비면 +로 이동시킨다.
    {

    }


}
