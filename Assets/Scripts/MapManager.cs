using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;



public class MapManager
{


    private List<List<GameObject>> board = new List<List<GameObject>>();
    private JSONVars jsonVars;

    private const float xStep = 0.6f;
    private const float yStep = 0.7f;



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
        setBlocks();

        //��ġ �߾�����
        MoveMiddleBlockToOrigin();

        //�׼� intermediate
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
            // �� Ȯ��
            while (board.Count <= grid.y)
                board.Add(new List<GameObject>());

            // �� Ȯ��
            while (board[grid.y].Count <= grid.x)
                board[grid.y].Add(null);

            // �θ� ������Ʈ ������ ����
            if (board[grid.y][grid.x] == null)
            {
                board[grid.y][grid.x] = Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab);
            }
            
            
            GameObject block = Object.Instantiate(ManagerObject.instance.resourceManager.blockPrefabs[grid.type], board[grid.y][grid.x].transform);
            board[grid.y][grid.x].GetOrAddComponent<BlockBase>().setPosition((grid.y, grid.x)); //��Ŀ�� ������ ��� ���� BlockBase ����
            board[grid.y][grid.x].GetOrAddComponent<BlockBase>().setBlockType(grid.type); // < �ڽ� ��ȯ�� �ݵ�� ���� �ٲ������
            //TODO ��Ŀ�� ���� Ŭ���� �ȵǴ� �������� ����ũ ��ü�� �ٸ��� ������ ���� �ڵ� �ʿ� X

            Vector3 lp;




            //����Ƽ�� y��ǥ�� ���� ���� ���� ���� ������ �迭 �� 11�� ������� �Ʒ��� ����������
            if (grid.x % 2 == 1) //Ȧ�� x��
            {
                lp = new Vector2(grid.x * xStep, -grid.y * yStep + yStep * 0.5f); //(�� ĭ �ø���)

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

        // �����߽�
        Vector2 centroid = Vector2.zero;
        foreach (var go in allBlocks)
            centroid += (Vector2)go.transform.localPosition;
        centroid /= allBlocks.Count;

        // �߾� ��� Ž��
        GameObject midBlock = allBlocks
            .OrderBy(go => Vector2.SqrMagnitude((Vector2)go.transform.localPosition - centroid))
            .First();

        // ������ ����
        Vector3 offset = -midBlock.transform.localPosition;
        foreach (var go in allBlocks)
            go.transform.localPosition += offset;
    }
    private bool _isSwapping = false;

    private void blockChange(GameObject startArg, GameObject nextArg)
    {
        if (_isSwapping) return;

        if (!getNeighbors(startArg.GetComponent<BlockBase>().getPosition()).Contains(nextArg.GetComponent<BlockBase>().getPosition()))
        {
            Debug.Log(getNeighbors(startArg.GetComponent<BlockBase>().getPosition()) + "     " + nextArg.GetComponent<BlockBase>().getPosition());
            return; //�̿��� �ƴϸ� ����
        }


        _isSwapping = true;
        ManagerObject.instance.StartCoroutine(SwapBlocksBetweenParents(startArg, nextArg)); //������ Ŭ���� ���� ������Ʈ�� �θ������Ʈ
    }

    private IEnumerator SwapBlocksBetweenParents(GameObject startBlockGO, GameObject nextBlockGO) //�ڽ� ������Ʈ���� �ٲ����
    {

        float t = 0f, speed = 5f, snap2 = 0.01f * 0.01f;
        while (true)
        {
            t += Time.deltaTime * speed; if (t > 1f) t = 1f;

            startBlockGO.transform.GetChild(0).position = Vector3.Lerp(startBlockGO.transform.position, nextBlockGO.transform.position, t);
            nextBlockGO.transform.GetChild(0).position = Vector3.Lerp(nextBlockGO.transform.position, startBlockGO.transform.position, t);

            if ((startBlockGO.transform.GetChild(0).position - nextBlockGO.transform.position).sqrMagnitude <= snap2 &&
                (nextBlockGO.transform.GetChild(0).position - startBlockGO.transform.position).sqrMagnitude <= snap2)
                break;
            yield return null;
        }

        // �θ� ��ȯ: �ݵ�� "��"��, world ����
        startBlockGO.transform.GetChild(0).SetParent(nextBlockGO.transform, true); //setparent�ؼ� ���� �ѱ����, ���� �߰��Ȱ��� �ι�° �ڽ����� �߰��Ǳ� ������ �Ʒ����� getchild(0)��ȿ
        nextBlockGO.transform.GetChild(0).SetParent(startBlockGO.transform, true);

        // �� �߽����� ����
        startBlockGO.transform.GetChild(0).localPosition = Vector3.zero;
        nextBlockGO.transform.GetChild(0).localPosition = Vector3.zero;



        _isSwapping = false;
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
            if (board[baseYX.y][baseYX.x] == null || board[p1.y][p1.x] == null || board[p2.y][p2.x] == null) continue;

            string type0 = board[baseYX.y][baseYX.x].GetComponent<BlockBase>().getBlockType();
            string type1 = board[p1.y][p1.x].GetComponent<BlockBase>().getBlockType();
            string type2 = board[p2.y][p2.x].GetComponent<BlockBase>().getBlockType();

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
        return pos.y >= 0 && pos.y < board.Count &&
               pos.x >= 0 && pos.x < board[pos.y].Count;
    }


    private void clearAll3Chains()
    {
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; j++)
            {
                Debug.Log("������ �� ��� �� = " + checkIsBurstable((i, j)).Count);
            }
        }
    }

    private void dropAllBlockesAndSpawn()
    {
        //�Ʒ��� �� ���� �Ʒ��� �ű�� �������� ���ο� ���� �߰��ȴ�.
    }




    // ���� ��� üũ (�龦���� �� ���� ����)
    private bool InBounds(List<List<GameObject>> b, int y, int x)
    {
        return y >= 0 && y < b.Count && x >= 0 && x < b[y].Count && b[y][x] != null;
    }

    // odd-q(Ȧ�� ���� �Ʒ���) ���� �̿� �� ĭ �̵�
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

    // �� �������� ���� �� �� �� �̾�������(���� ĭ ����)
    private int RunLen(List<List<GameObject>> b, int y, int x, int dir)
    {
        int cnt = 0;
        var cur = b[y][x]?.GetComponent<BlockBase>();
        int cx = x, cy = y;

        while (true)
        {
            (cx, cy) = StepOddQ(cx, cy, dir);
            if (!InBounds(b, cy, cx)) break;
            var nb = b[cy][cx]?.GetComponent<BlockBase>();
            if (nb == null || !cur.IsSameColor(nb)) break;
            cnt++;
        }
        return cnt;
    }

    // ���� 3��(E-W, NE-SW, NW-SE)���� 3�� �̻� ���� �˻�
    private bool checkIsThere3Chain(List<List<GameObject>> b)
    {
        if (b == null || b.Count == 0) return false;

        // �� ����: (��/��) ���� ��
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
                var cur = b[y][x]?.GetComponent<BlockBase>();
                if (cur == null) continue;

                foreach (var ax in axes)
                {
                    int len = 1; // ���� ĭ ����
                    len += RunLen(b, y, x, ax[0]);
                    len += RunLen(b, y, x, ax[1]);
                    if (len >= 3) return true;
                }
            }
        }
        return false;
    }


}
