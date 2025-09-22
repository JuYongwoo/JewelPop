using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MapManager
{

    private Dictionary<(int, int), BlockParent> board = new Dictionary<(int, int), BlockParent>();
    private JSONVars jsonVars;

    private const float xStep = 0.6f;
    private const float yStep = 0.7f;

    public bool isInMotion = false; //블럭이 움직이는 중인지
    public bool isChanged { get; set; }//보드 상태가 바뀐 상태인지



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
        ManagerObject.instance.actionManager.inputBlockChangeAction = InputBlockChangeEvent;
        ManagerObject.instance.actionManager.setIsInMotion = (a) => { isInMotion = a; };
        ManagerObject.instance.actionManager.setIsBoardChanged = (a) => { isChanged = a; };

    }


    public void OnUpdate()
    {

        if (isChanged)
        {
            DropAllBlocks(); //이동부분을 관장



            if (!isInMotion) //모션 중이 아닐때만 생성&파괴
            {
                var dels = checkChains();
                if (dels.Count != 0)
                {
                    DestroyBlocks(dels);
                }

                var tops = checkEmptyTops();
                if (tops.Count != 0)
                {
                    AddNewBlocks(tops);
                }

                if(dels.Count == 0 && tops.Count == 0) isChanged = false; //생성도 파괴도 할 것이 없으면 안정된 상태

            }
        }
    }


    private void SetBlocks()
    {
        foreach (var grid in jsonVars.grids)
        {
            //JSON에 작성되어있는 그리드 좌표에 맞춰 오브젝트(BlockParent 컴포넌트가 있는) 생성 & 딕셔너리 매핑
            //부모 오브젝트
            board.Add((grid.y, grid.x),UnityEngine.Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab).GetComponent<BlockParent>());
            board[(grid.y, grid.x)].name = $"y{grid.y}x{grid.x}"; //이름
            board[(grid.y, grid.x)].SetGridPositionYX((grid.y, grid.x)); //그리드 좌표
            board[(grid.y, grid.x)].SetUnityPositionYX(grid.x % 2 == 1 ? (-grid.y * yStep + yStep * 0.5f, grid.x * xStep) : (-grid.y * yStep, grid.x * xStep)); //유니티 좌표, 지그재그, 홀수 X는 위로 반칸


            //자식 오브젝트(블록)
            GameObject child = UnityEngine.Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[grid.type], board[(grid.y, grid.x)].transform);
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

    private void InputBlockChangeEvent(GameObject startChild, GameObject endChild)
    {

        if (!GetNeighbors(startChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()).Contains(endChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()))
        {
            return; //이웃이 아니면 리턴
        }


        //var startParentTransform = startChild.transform.parent;
        //var endParentTransform = endChild.transform.parent;

        //startChild.GetComponent<Basic>().move(endParentTransform);
        //endChild.GetComponent<Basic>().move(startParentTransform);


        moveTo(startChild.transform.parent.gameObject, endChild.transform.parent.gameObject, true);
    }

    public void moveTo(GameObject startArg, GameObject nextArg, bool isSwap = false) //반드시 이 함수를 통해 이동
    {


        var aPos = new Vector3(startArg.transform.position.x, startArg.transform.position.y, startArg.transform.GetChild(0).position.z);
        var bPos = new Vector3(nextArg.transform.position.x, nextArg.transform.position.y, startArg.transform.GetChild(0).position.z);

        ManagerObject.instance.StartCoroutine(MoveChild(startArg.transform.GetChild(0), nextArg.transform, aPos, bPos));
        if (isSwap)
        {
            ManagerObject.instance.StartCoroutine(MoveChild(nextArg.transform.GetChild(0), startArg.transform, bPos, aPos));

        }

    }


    // 자식 Transform을 직접 받아서 이동 (부모에서 다시 GetChild(0) 하지 않음)
    private IEnumerator MoveChild(UnityEngine.Transform child, UnityEngine.Transform targetParent, Vector3 startPos, Vector3 endPos)
    {
        child.SetParent(targetParent, true);
        float t = 0f, speed = 5f, snap2 = 0.01f * 0.01f;
        
        isInMotion = true; //모션중
        isChanged = true; // 어딘가 움직였다는 것은 보드 상태가 변했다는 것을 의미
        
        while (true)
        {
            if(child == null) yield break; //파괴된 경우
            t += Time.deltaTime * speed; if (t > 1f) t = 1f;
            child.position = Vector3.Lerp(startPos, endPos, t);
            if ((child.position - endPos).sqrMagnitude <= snap2) break;
            yield return null;
        }

        child.position = endPos;

        isInMotion = false;
    }




    private List<(int y, int x)> checkChains()
    {
        List<(int y, int x)> dels = new List<(int y, int x)>();
        foreach (var grid in board)
            foreach (var del in CheckIsBurstable(grid.Key))
                dels.Add(del);


        return dels;
    }


    private void DestroyBlocks(List<(int y, int x)> dels)
    {
        HashSet<SpecialBlock> specials = new HashSet<SpecialBlock>(); //중복 참조 방지, 셋 사용

        foreach (var a in dels)
        {
            if (IsValid(a))
            {


                foreach (var n in GetNeighbors(a))
                {
                    if (board[n].transform.GetChild(0).GetComponent<SpecialBlock>() != null) //blockparent의 type으로 검사해도 무방
                    {
                        specials.Add(board[n].transform.GetChild(0).GetComponent<SpecialBlock>());

                        //조커의 상태 bool on으로 바꿔야
                    }
                }


                board[a].transform.GetChild(0).GetComponent<CommonBlockInterface>().DestroySelf();
            }
        }



        foreach (var special in specials)
        {
            special.specialMotion();
        }


    }


    private void AddNewBlocks(List<(int y, int x)> tops)
    {

        string[] str = new string[] {"r", "p", "pp", "o", "r", "y" };
        // tops에 있는 위치에 새 블록 소환
        foreach (var pos in tops)
        {
            var rd = str[UnityEngine.Random.Range(0, str.Length)];
            GameObject child = UnityEngine.Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[rd], board[pos].transform);
            child.GetComponent<BlockChild>().SetBlockType(rd); // 여기서 타입을 설정
        }


    }
    private List<(int y, int x)> checkEmptyTops()
    {
        List<(int y, int x)> tops = new List<(int y, int x)>();
        foreach (var key in board.Keys)
        {
            if (IsTop(key) && board[key].transform.childCount == 0) //맨위에 있고 자식이 없으면
            {
                tops.Add(key);
            }
        }
        return tops;
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
                moveTo(block.gameObject, board[(newY, x)].gameObject);
            }
        }


        //ClearAll3Chains(); //다시 재귀적으로 흘러간다.
    }


    private bool IsValid((int y, int x) pos) //움직이고 부수는 블럭은 CommonBlock만 해당
    {
        if (board.ContainsKey(pos)
            && board[pos].transform.childCount != 0
            && board[pos].GetComponent<CommonBlockInterface>() == null) return true; //유효성 검사
        else return false;

    }

    private bool IsTop((int y, int x) yx)
    {
        if (board.ContainsKey((yx.y - 1, yx.x)))
        {
            return false;
        }
        else
        {
            return true;
        }
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


    private HashSet<(int y, int x)> CheckIsBurstable((int y, int x) baseYX)
    {
        HashSet<(int y, int x)> burstables = new HashSet<(int y, int x)>();
        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i += 2)
        {
            var p1 = (y: baseYX.y + directions[i].dy, x: baseYX.x + directions[i].dx);
            var p2 = (y: baseYX.y + directions[i + 1].dy, x: baseYX.x + directions[i + 1].dx);

            if (!IsValid(baseYX) || !IsValid(p1) || !IsValid(p2)) continue;
            if (board[(baseYX.y, baseYX.x)] == null || board[(p1.y, p1.x)] == null || board[(p2.y, p2.x)] == null) continue;



            //// 자식 없으면 스킵
            //if (board[(baseYX.y, baseYX.x)].transform.childCount == 0
            //    || board[(p1.y, p1.x)].transform.childCount == 0
            //    || board[(p2.y, p2.x)].transform.childCount == 0) continue;

            var type0 = board[(baseYX.y, baseYX.x)].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();
            var type1 = board[(p1.y, p1.x)].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();
            var type2 = board[(p2.y, p2.x)].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();

            if (type0.Equals(type1) && type0.Equals(type2))
            {
                burstables.Add(baseYX);
                burstables.Add(p1);
                burstables.Add(p2);
            }
        }




        /////조커 확인
        ///
        ////////





        return burstables;
    }


}
