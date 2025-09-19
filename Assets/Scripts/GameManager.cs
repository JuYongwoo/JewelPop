using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class GameManager
{

    private Dictionary<(int, int), BlockParent> board = new Dictionary<(int, int), BlockParent>();
    private JSONVars jsonVars;

    private const float xStep = 0.6f;
    private const float yStep = 0.7f;

    private bool _isSwapping = false;



    (int dy, int dx)[] oddXDirections = new(int dy, int dx)[] {
        //�� �ΰ����� base�� ���� ���� 3��
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
        //JSON �б�
        jsonVars = JsonUtility.FromJson<JSONVars>(ManagerObject.instance.resourceManager.mapJSON.text); //JSON ������ JSONVars�� �о�´�
        
        //�� ����
        SetBlocks();

        //��ġ �߾�����
        MoveMiddleBlockToOrigin();

        //�׼� intermediate
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
            //JSON�� �ۼ��Ǿ��ִ� �׸��� ��ǥ�� ���� ������Ʈ(BlockParent ������Ʈ�� �ִ�) ���� & ��ųʸ� ����
            //�θ� ������Ʈ
            board.Add((grid.y, grid.x),Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab).GetComponent<BlockParent>());
            board[(grid.y, grid.x)].name = $"y{grid.y}x{grid.x}"; //�̸�
            board[(grid.y, grid.x)].setPositionYX((grid.y, grid.x)); //�׸��� ��ǥ
            board[(grid.y, grid.x)].setUnityPositionYX(grid.x % 2 == 1 ? (-grid.y * yStep + yStep * 0.5f, grid.x * xStep) : (-grid.y * yStep, grid.x * xStep)); //����Ƽ ��ǥ, �������, Ȧ�� X�� ���� ��ĭ


            //�ڽ� ������Ʈ(���)
            GameObject child = Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[grid.type], board[(grid.y, grid.x)].transform);
            child.GetComponent<BlockChild>().SetBlockType(grid.type); // ���⼭ Ÿ���� ����

        }
    }



    private void MoveMiddleBlockToOrigin()
    {

        // �����߽�
        Vector2 centroid = Vector2.zero;
        foreach (var go in board.Values)
            centroid += (Vector2)go.transform.localPosition;
        centroid /= board.Count;

        // �߾� ��� Ž��
        BlockParent midBlock = board.Values.OrderBy(blockParent => Vector2.SqrMagnitude((Vector2)blockParent.transform.localPosition - centroid)).First();

        // ������ ����
        Vector3 offset = -midBlock.transform.localPosition;
        foreach (var go in board.Values)
            go.transform.localPosition += offset;
    }

    private void BlockChange(GameObject startArg, GameObject nextArg)
    {
        if (_isSwapping) return;

        if (!GetNeighbors(startArg.transform.parent.GetComponent<BlockParent>().getPosition()).Contains(nextArg.transform.parent.GetComponent<BlockParent>().getPosition()))
        {
            Debug.Log(GetNeighbors(startArg.transform.parent.GetComponent<BlockParent>().getPosition()) + "     " + nextArg.transform.parent.GetComponent<BlockParent>().getPosition());
            return; //�̿��� �ƴϸ� ����
        }


        _isSwapping = true;
        ManagerObject.instance.StartCoroutine(SwapBlockChild(startArg.transform.parent.gameObject, nextArg.transform.parent.gameObject)); //Ŭ���� ���� ������Ʈ�� �θ������Ʈ
        //TODO ���� �ٲٴ°� �ƴ϶� �ϳ��� �̵��ϴ°�����, ���⼱ ���ΰ� �̵��ؼ� ���ڰ� �ѹ���, �� �ι� �����ϵ��� �ؾ���
    }


    private IEnumerator SwapBlockChild(GameObject startBlockGO, GameObject nextBlockGO) //�ڽ� ������Ʈ���� �ٲ����
    {

        // �� �θ� �ؿ� �ڽ��� ���� ��� �� ��ȯ �Ұ�
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

        ClearAll3Chains();

    }


    private IEnumerator MoveBlockChild(GameObject startBlockGO, GameObject nextBlockGO) //�ڽ� ������Ʈ���� �ٲ����
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




        ClearAll3Chains();

    }




    private List<(int y, int x)> GetNeighbors((int y, int x) baseYX)
    {

        List<(int y, int x)> neighbors = new List<(int y, int x)>();

        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i++)
        {
            if (board.ContainsKey((baseYX.y + directions[i].dy, baseYX.x + directions[i].dx))
                && board[(baseYX.y + directions[i].dy, baseYX.x + directions[i].dx)].transform.childCount != 0)
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



            // �ڽ� ������ ��ŵ
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
                    if(board[n].transform.GetChild(0).GetComponent<Joker>() != null) //blockparent�� type���� �˻��ص� ����
                    {
                        board[n].transform.GetChild(0).GetComponent<Joker>().motionStart();//�ߺ� ����ǵ� �÷��� ����Ͽ� ����
                        //��Ŀ�� ���� bool on���� �ٲ��
                    }
                }
            }

        }
        /////��Ŀ Ȯ��
        ///
        ////////





        return burstables;
    }

    private bool IsValid((int y, int x) pos)
    {
        return board.ContainsKey(pos) ? true : false;
    }


    private void ClearAll3Chains()
    {

        List<(int y, int x)> dels = new List<(int y, int x)>();
        foreach(var grid in board)
            foreach(var del in CheckIsBurstable(grid.Key))
                dels.Add(del);


        foreach (var a in dels)
            board[a].transform.GetChild(0).GetComponent<BlockChild>().DestroySelf();
    }


    private void dropAllBlocks() //�߷¿� ���� y+ ĭ�� ��� +�� �̵���Ų��.
    {

    }


}
