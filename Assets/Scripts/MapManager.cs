using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class MapManager
{

    private Dictionary<(int, int), BlockParent> board = new Dictionary<(int, int), BlockParent>();
    private JSONVars jsonVars;

    private const float xStep = 0.6f;
    private const float yStep = 0.7f;




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
        SetBlocks();

        //위치 중앙으로
        MoveMiddleBlockToOrigin();

        //액션 intermediate
        ManagerObject.instance.actionManager.blockChangeAction = BlockChange;

    }

    public void OnStart()
    {
        ClearAll3Chains();
    }

    private void SetBlocks()
    {
        foreach (var grid in jsonVars.grids)
        {
            //JSON에 작성되어있는 그리드 좌표에 맞춰 오브젝트(BlockParent 컴포넌트가 있는) 생성 & 딕셔너리 매핑
            //부모 오브젝트
            board.Add((grid.y, grid.x),Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab).GetComponent<BlockParent>());
            board[(grid.y, grid.x)].name = $"y{grid.y}x{grid.x}"; //이름
            board[(grid.y, grid.x)].SetGridPositionYX((grid.y, grid.x)); //그리드 좌표
            board[(grid.y, grid.x)].SetUnityPositionYX(grid.x % 2 == 1 ? (-grid.y * yStep + yStep * 0.5f, grid.x * xStep) : (-grid.y * yStep, grid.x * xStep)); //유니티 좌표, 지그재그, 홀수 X는 위로 반칸


            //자식 오브젝트(블록)
            GameObject child = Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[grid.type], board[(grid.y, grid.x)].transform);
            child.GetComponent<BlockChild>().SetBlockType(grid.type); // 여기서 타입을 설정

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
        Vector2 offset = -midBlock.transform.localPosition;
        foreach (var go in board.Values)
            go.transform.localPosition += new Vector3(offset.x, offset.y, 0);
    }

    private void BlockChange(GameObject startArg, GameObject nextArg)
    {

        if (!GetNeighbors(startArg.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()).Contains(nextArg.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()))
        {
            Debug.Log(GetNeighbors(startArg.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()) + "     " + nextArg.transform.parent.GetComponent<BlockParent>().GetGridPositionYX());
            return; //이웃이 아니면 리턴
        }

        ManagerObject.instance.StartCoroutine(SwapBlockChild(startArg.transform.parent.gameObject, nextArg.transform.parent.gameObject)); //클릭된 게임 오브젝트는 부모오브젝트
        //TODO 서로 바꾸는게 아니라 하나가 이동하는것으로, 여기선 서로가 이동해서 각자가 한번씩, 총 두번 실행하도록 해야함
    }


    private IEnumerator SwapBlockChild(GameObject startBlockGO, GameObject nextBlockGO) //자식 오브젝트끼리 바꿔야함
    {
        //움직이기 전 위치 기록, 서로의 목적지는 이 변수만을 사용
        Vector3 startInit = new Vector3(startBlockGO.transform.position.x, startBlockGO.transform.position.y, startBlockGO.transform.GetChild(0).position.z); //서로의 이동 시작 당시 위치, z축은 자식 기준
        Vector3 nextInit = new Vector3(nextBlockGO.transform.position.x, nextBlockGO.transform.position.y, nextBlockGO.transform.GetChild(0).position.z);

        //함수 호출하자마자 부모 변경, 연출은 그 뒤에
        startBlockGO.transform.GetChild(0).SetParent(nextBlockGO.transform, true);
        nextBlockGO.transform.GetChild(0).SetParent(startBlockGO.transform, true);



        //연출: 위치는 그대로이나 부모가 바뀐 상태// start부모는 next->start방향으로Lerp
        float t = 0f, speed = 5f, snap2 = 0.01f * 0.01f;
        while (true)
        {
            t += Time.deltaTime * speed; if (t > 1f) t = 1f;

            nextBlockGO.transform.GetChild(0).position = Vector3.Lerp(startInit, nextInit, t);
            startBlockGO.transform.GetChild(0).position = Vector3.Lerp(nextInit, startInit, t);

            if ((startBlockGO.transform.GetChild(0).position - startInit).sqrMagnitude <= snap2
                && (nextBlockGO.transform.GetChild(0).position - nextInit).sqrMagnitude <= snap2)
                break;
            yield return null;
        }

        //자식의 위치를 부모의 원점으로 맞춰준다
        startBlockGO.transform.GetChild(0).position = startInit;
        nextBlockGO.transform.GetChild(0).position = nextInit;




        ClearAll3Chains();

    }


    private IEnumerator MoveBlockChild(GameObject startBlockGO, GameObject nextBlockGO) //자식 오브젝트끼리 바꿔야함
    {


        //움직이기 전 위치 기록, 서로의 목적지는 이 변수만을 사용
        Vector3 startInit = new Vector3(startBlockGO.transform.position.x, startBlockGO.transform.position.y, startBlockGO.transform.GetChild(0).position.z); //서로의 이동 시작 당시 위치, z축은 자식 기준
        Vector3 nextInit = new Vector3(nextBlockGO.transform.position.x, nextBlockGO.transform.position.y, startBlockGO.transform.GetChild(0).position.z);

        //함수 호출하자마자 부모 변경, 연출은 그 뒤에
        startBlockGO.transform.GetChild(0).SetParent(nextBlockGO.transform, true);



        //연출: 위치는 그대로이나 부모가 바뀐 상태// start부모는 next->start방향으로Lerp
        float t = 0f, speed = 5f, snap2 = 0.01f * 0.01f;
        while (true)
        {
            t += Time.deltaTime * speed; if (t > 1f) t = 1f;

            nextBlockGO.transform.GetChild(0).position = Vector3.Lerp(startInit, nextInit, t);

            if ((nextBlockGO.transform.GetChild(0).position - nextInit).sqrMagnitude <= snap2)
                break;
            yield return null;
        }

        //자식의 위치를 부모의 원점으로 맞춰준다
        nextBlockGO.transform.GetChild(0).position = nextInit;




        ClearAll3Chains();

    }




    private List<(int y, int x)> GetNeighbors((int y, int x) baseYX)
    {

        List<(int y, int x)> neighbors = new List<(int y, int x)>();

        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i++)
        {
            if (IsValid((baseYX.y + directions[i].dy, baseYX.x + directions[i].dx)))
                neighbors.Add((baseYX.y + directions[i].dy, baseYX.x + directions[i].dx));

        }

        return neighbors;

    }


    private List<(int y, int x)> CheckIsBurstable((int y, int x) baseYX)
    {
        List<(int y, int x)> burstables = new List<(int y, int x)>();
        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i += 2)
        {
            var p1 = (y: baseYX.y + directions[i].dy, x: baseYX.x + directions[i].dx);
            var p2 = (y: baseYX.y + directions[i + 1].dy, x: baseYX.x + directions[i + 1].dx);

            if (!IsValid(baseYX) || !IsValid(p1) || !IsValid(p2)) continue;
            if (board[(baseYX.y, baseYX.x)] == null || board[(p1.y, p1.x)] == null || board[(p2.y, p2.x)] == null) continue;



            // 자식 없으면 스킵
            if (board[(baseYX.y, baseYX.x)].transform.childCount == 0
                || board[(p1.y, p1.x)].transform.childCount == 0
                || board[(p2.y, p2.x)].transform.childCount == 0) continue;

            string type0 = board[(baseYX.y, baseYX.x)].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();
            string type1 = board[(p1.y, p1.x)].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();
            string type2 = board[(p2.y, p2.x)].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();

            if (type0.Equals(type1) && type0.Equals(type2))
            {
                burstables.Add(baseYX);
                burstables.Add(p1);
                burstables.Add(p2);
            }
        }




        if(burstables.Count != 0)
        {
            foreach(var a in burstables)
            {
                foreach(var n in GetNeighbors(a))
                {
                    if(board[n].transform.GetChild(0).GetComponent<Joker>() != null) //blockparent의 type으로 검사해도 무방
                    {
                        board[n].transform.GetChild(0).GetComponent<Joker>().motionStart();//중복 실행되도 플래그 사용하여 무방
                        //조커의 상태 bool on으로 바꿔야
                    }
                }
            }

        }
        /////조커 확인
        ///
        ////////





        return burstables;
    }

    private bool IsValid((int y, int x) pos)
    {
        if (board.ContainsKey(pos) && board[pos].transform.childCount != 0) return true; //맵에 등록 & 자식오브젝트가 있는가
        else return false;

    }


    private void ClearAll3Chains()
    {

        List<(int y, int x)> dels = new List<(int y, int x)>();
        foreach(var grid in board)
            foreach(var del in CheckIsBurstable(grid.Key))
                dels.Add(del);


        foreach (var a in dels)
            board[a].transform.GetChild(0).GetComponent<BlockChild>().DestroySelf();

        DropAllBlocks();
    }


    private void DropAllBlocks()
    {
        // y가 큰 블럭부터 검사 (위에서 아래로 내려오기 때문에)

        
        var keys = board.Keys.OrderByDescending(k => k.Item1).ToList(); //내림차순, y값이 높을 수록 아래에 위치 // 아래에서 위로 탐색

        foreach (var key in keys)
        {
            if (!IsValid(key)) continue; //시작 지점은 isvalid로 자식 있는지 확인

            var block = board[key];
            int y = key.Item1;
            int x = key.Item2;

            int newY = y;

            // 밑으로 갈 수 있는 만큼 내림(아래칸의 자식 오브젝트가 없으면)
            while (board.ContainsKey((newY+1, x)) && board[(newY+1, x)].transform.childCount==0)
            {
                newY++;
            }

            if (newY != y)
            {

                // 코루틴 실행 → 블럭을 실제로 움직임
                ManagerObject.instance.StartCoroutine(MoveBlockChild(block.gameObject, board[(newY, x)].gameObject));
            }
        }
    }



}
