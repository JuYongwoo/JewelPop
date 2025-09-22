using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct YX
{
    public YX(int y, int x) {
        this.y = y;
        this.x = x;
    }
    public int y;
    public int x;
}

public class MapManager
{

    private Dictionary<YX, BlockParent> board = new Dictionary<YX, BlockParent>();
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
        ManagerObject.instance.actionManager.getIsInMotion = () => { return isInMotion; };
        ManagerObject.instance.actionManager.setIsInMotion = (a) => { isInMotion = a; };
        ManagerObject.instance.actionManager.getIsBoardChanged = () => { return isChanged; };
        ManagerObject.instance.actionManager.setIsBoardChanged = (a) => { isChanged = a; };

    }


    public void OnUpdate()
    {

        if (isChanged)
        {
            DropAllBlocks(); //�̵��κ��� ����



            if (!isInMotion) //��� ���� �ƴҶ��� ����&�ı�
            {
                var dels = checkAllChains();
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
            YX yx = new YX(grid.y, grid.x);
            board.Add(yx, UnityEngine.Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab).GetComponent<BlockParent>());
            board[yx].name = $"y{grid.y}x{grid.x}"; //�̸�
            board[yx].SetGridPositionYX(yx); //�׸��� ��ǥ
            board[yx].SetUnityPositionYX(grid.x % 2 == 1 ? (-grid.y * yStep + yStep * 0.5f, grid.x * xStep) : (-grid.y * yStep, grid.x * xStep)); //����Ƽ ��ǥ, �������, Ȧ�� X�� ���� ��ĭ


            //�ڽ� ������Ʈ(���)
            GameObject child = UnityEngine.Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[grid.type], board[yx].transform);
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
        YX startYX = startChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX();
        YX endYX = endChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX();
        if (!GetNeighbors(startChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()).Contains(endChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()))
        {
            return; //�̿��� �ƴϸ� ����
        }


        Dictionary<YX, BlockParent> fakeBoard = new Dictionary<YX, BlockParent>(board); //���� ����

        // ��� ��ġ�� ��ȯ �ùķ��̼�
        (fakeBoard[startChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()], fakeBoard[endChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()])
            = (fakeBoard[endChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()], fakeBoard[startChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX()]);

        HashSet<YX> cates = new HashSet<YX>();

        foreach (var n in GetNeighbors(startChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX(), fakeBoard))
        {
            cates.Add(n); //���������� �̿��� �߰�
        }
        foreach (var n in GetNeighbors(endChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX(), fakeBoard))
        {
            cates.Add(n); //���������� �̿��� �߰�
        }


        //�� �ĺ����� �߿� 3������ ������ ���� �ִ°�?
        bool canBurst = false;

        foreach (var c in cates)
        {
            var bursts = CheckIsBurstable(c, fakeBoard);
            if (bursts.Count != 0)
            {
                canBurst = true;
                break;
            }
        }



        //TODO JYW
        //�̵��غôµ� CheckIsBurstable(���ο� ��ġ�� �� neighbors) �� ������ 0�̸� moveandback
        //�̵��غôµ� ������ 1�̻��̸� move
        //�̵��� �� �� ��¥ ���带 �ϳ� �����






        var startParentTransform = startChild.transform.parent;
        var endParentTransform = endChild.transform.parent;


        if (canBurst)
        {
            startChild.GetComponent<IMoveAndDesroyable>().Move(endParentTransform);
            endChild.GetComponent<IMoveAndDesroyable>().Move(startParentTransform);

        }
        else if (!canBurst)
        {
            startChild.GetComponent<IMoveAndDesroyable>().MoveAndBack(endParentTransform);
            endChild.GetComponent<IMoveAndDesroyable>().MoveAndBack(startParentTransform);
        }


        //moveTo(startChild.transform.parent.gameObject, endChild.transform.parent.gameObject, true);
    }




    private List<YX> checkAllChains()
    {
        List<YX> dels = new List<YX>();
        foreach (var grid in board)
            foreach (var del in CheckIsBurstable(grid.Key))
                dels.Add(del);


        return dels;
    }


    private void DestroyBlocks(List<YX> dels)
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


    private void AddNewBlocks(List<YX> tops)
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
    private List<YX> checkEmptyTops()
    {
        List<YX> tops = new List<YX>();
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

        
        var keys = board.Keys.OrderByDescending(k => k.y).ToList(); //��������, y���� ���� ���� �Ʒ��� ��ġ // �Ʒ����� ���� Ž��

        foreach (var key in keys)
        {
            if (!IsValid(key)) continue; //���� ������ isvalid�� �ڽ� �ִ��� Ȯ��

            var block = board[key];
            int y = key.y;
            int x = key.x;

            int newY = y;

            // ������ �� �� �ִ� ��ŭ ����(�Ʒ�ĭ�� �ڽ� ������Ʈ�� ������)
            while (board.ContainsKey(new YX(newY+1, x)) && board[new YX(newY + 1, x)].transform.childCount==0)
            {
                newY++;
            }

            if (newY != y)
            {
                block.transform.GetChild(0).GetComponent<IMoveAndDesroyable>().Move(board[new YX(newY, x)].transform); //�Ʒ��� �̵�
            }
        }


        //ClearAll3Chains(); //�ٽ� ��������� �귯����.
    }


    private bool IsValid(YX pos, Dictionary<YX, BlockParent> pBoard = null) //�����̰� �μ��� ���� CommonBlock�� �ش�
    {
        if(pBoard == null) pBoard = board;

        if (pBoard.ContainsKey(pos)
            && pBoard[pos].transform.childCount != 0
            && pBoard[pos].GetComponent<IMoveAndDesroyable>() == null) return true; //��ȿ�� �˻�
        else return false;

    }

    private bool IsTop(YX yx)
    {
        if (board.ContainsKey(new YX(yx.y - 1, yx.x)))
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private List<YX> GetNeighbors(YX baseYX, Dictionary<YX, BlockParent> pBoard = null)
    {
        if (pBoard == null) pBoard = board;

        List<YX> neighbors = new List<YX>();

        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i++)
        {
            YX newYX = new YX(baseYX.y + directions[i].dy, baseYX.x + directions[i].dx);
            if (IsValid(newYX))
                neighbors.Add(newYX);

        }

        return neighbors;

    }


    private HashSet<YX> CheckIsBurstable(YX baseYX, Dictionary<YX, BlockParent> pBoard = null)
    {
        if(pBoard == null) pBoard = board;

        HashSet<YX> burstables = new HashSet<YX>();
        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i += 2)
        {
            YX p1 = new YX(baseYX.y + directions[i].dy, baseYX.x + directions[i].dx);
            YX p2 = new YX(baseYX.y + directions[i + 1].dy, baseYX.x + directions[i + 1].dx);

            if (!IsValid(baseYX) || !IsValid(p1) || !IsValid(p2)) continue;
            if (pBoard[baseYX] == null || pBoard[p1] == null || pBoard[p2] == null) continue;



            //// �ڽ� ������ ��ŵ
            //if (board[(baseYX.y, baseYX.x)].transform.childCount == 0
            //    || board[(p1.y, p1.x)].transform.childCount == 0
            //    || board[(p2.y, p2.x)].transform.childCount == 0) continue;

            var type0 = pBoard[baseYX].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();
            var type1 = pBoard[p1].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();
            var type2 = pBoard[p2].transform.GetChild(0).GetComponent<BlockChild>().GetBlockType();

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
