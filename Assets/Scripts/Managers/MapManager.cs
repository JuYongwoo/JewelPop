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

    public bool isInMotion = false; //���� �����̴� ������
    public bool isChanged { get; set; }//���� ���°� �ٲ� ��������



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
        ManagerObject.instance.actionManager.inputBlockChangeAction = InputBlockChangeEvent;
        ManagerObject.instance.actionManager.setIsInMotion = (a) => { isInMotion = a; };
        ManagerObject.instance.actionManager.setIsBoardChanged = (a) => { isChanged = a; };

    }


    public void OnUpdate()
    {

        if (isChanged)
        {
            DropAllBlocks(); //�̵��κ��� ����



            if (!isInMotion) //��� ���� �ƴҶ��� ����&�ı�
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

                if(dels.Count == 0 && tops.Count == 0) isChanged = false; //������ �ı��� �� ���� ������ ������ ����

            }
        }
    }


    private void SetBlocks()
    {
        foreach (var grid in jsonVars.grids)
        {
            //JSON�� �ۼ��Ǿ��ִ� �׸��� ��ǥ�� ���� ������Ʈ(BlockParent ������Ʈ�� �ִ�) ���� & ��ųʸ� ����
            //�θ� ������Ʈ
            board.Add((grid.y, grid.x),UnityEngine.Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab).GetComponent<BlockParent>());
            board[(grid.y, grid.x)].name = $"y{grid.y}x{grid.x}"; //�̸�
            board[(grid.y, grid.x)].SetGridPositionYX((grid.y, grid.x)); //�׸��� ��ǥ
            board[(grid.y, grid.x)].SetUnityPositionYX(grid.x % 2 == 1 ? (-grid.y * yStep + yStep * 0.5f, grid.x * xStep) : (-grid.y * yStep, grid.x * xStep)); //����Ƽ ��ǥ, �������, Ȧ�� X�� ���� ��ĭ


            //�ڽ� ������Ʈ(���)
            GameObject child = UnityEngine.Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[grid.type], board[(grid.y, grid.x)].transform);
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
        Vector2 offset = -midBlock.transform.localPosition;
        foreach (var go in board.Values)
            go.transform.localPosition += new Vector3(offset.x, offset.y, 0);
    }

    private void InputBlockChangeEvent(GameObject startChild, GameObject endChild)
    {

        if (!GetNeighbors(startChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()).Contains(endChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()))
        {
            return; //�̿��� �ƴϸ� ����
        }






        //TODO JYW
        //�̵��غôµ� CheckIsBurstable(���ο� ��ġ) �� ������ 0�̸� moveandback
        //�̵��غôµ� ������ 1�̻��̸� move







        var startParentTransform = startChild.transform.parent;
        var endParentTransform = endChild.transform.parent;

        startChild.GetComponent<IMoveAndDesroyable>().MoveAndBack(endParentTransform);
        //endChild.GetComponent<IMoveAndDesroyable>().Move(startParentTransform);


        //moveTo(startChild.transform.parent.gameObject, endChild.transform.parent.gameObject, true);
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
        HashSet<ISpecial> specials = new HashSet<ISpecial>(); //�ߺ� ���� ����, �� ���

        foreach (var a in dels)
        {
            if (IsValid(a))
            {


                foreach (var n in GetNeighbors(a))
                {
                    if (board[n].transform.GetChild(0).GetComponent<ISpecial>() != null) //blockparent�� type���� �˻��ص� ����
                    {
                        specials.Add(board[n].transform.GetChild(0).GetComponent<ISpecial>());

                        //��Ŀ�� ���� bool on���� �ٲ��
                    }
                }


                board[a].transform.GetChild(0).GetComponent<IMoveAndDesroyable>().DestroySelf();
            }
        }



        foreach (var special in specials)
        {
            special.SpecialMotion();
        }


    }


    private void AddNewBlocks(List<(int y, int x)> tops)
    {

        string[] str = new string[] {"r", "p", "pp", "o", "r", "y" };
        // tops�� �ִ� ��ġ�� �� ��� ��ȯ
        foreach (var pos in tops)
        {
            var rd = str[UnityEngine.Random.Range(0, str.Length)];
            GameObject child = UnityEngine.Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[rd], board[pos].transform);
            child.GetComponent<BlockChild>().SetBlockType(rd); // ���⼭ Ÿ���� ����
        }


    }
    private List<(int y, int x)> checkEmptyTops()
    {
        List<(int y, int x)> tops = new List<(int y, int x)>();
        foreach (var key in board.Keys)
        {
            if (IsTop(key) && board[key].transform.childCount == 0) //������ �ְ� �ڽ��� ������
            {
                tops.Add(key);
            }
        }
        return tops;
    }

    private void DropAllBlocks()
    {
        // y�� ū ������ �˻� (������ �Ʒ��� �������� ������)

        
        var keys = board.Keys.OrderByDescending(k => k.Item1).ToList(); //��������, y���� ���� ���� �Ʒ��� ��ġ // �Ʒ����� ���� Ž��

        foreach (var key in keys)
        {
            if (!IsValid(key)) continue; //���� ������ isvalid�� �ڽ� �ִ��� Ȯ��

            var block = board[key];
            int y = key.Item1;
            int x = key.Item2;

            int newY = y;

            // ������ �� �� �ִ� ��ŭ ����(�Ʒ�ĭ�� �ڽ� ������Ʈ�� ������)
            while (board.ContainsKey((newY+1, x)) && board[(newY+1, x)].transform.childCount==0)
            {
                newY++;
            }

            if (newY != y)
            {
                block.transform.GetChild(0).GetComponent<IMoveAndDesroyable>().Move(board[(newY, x)].transform); //�Ʒ��� �̵�
            }
        }


        //ClearAll3Chains(); //�ٽ� ��������� �귯����.
    }


    private bool IsValid((int y, int x) pos) //�����̰� �μ��� ���� CommonBlock�� �ش�
    {
        if (board.ContainsKey(pos)
            && board[pos].transform.childCount != 0
            && board[pos].GetComponent<IMoveAndDesroyable>() == null) return true; //��ȿ�� �˻�
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



            //// �ڽ� ������ ��ŵ
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




        /////��Ŀ Ȯ��
        ///
        ////////





        return burstables;
    }


}
