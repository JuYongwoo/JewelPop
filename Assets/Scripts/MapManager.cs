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
            board[grid.y][grid.x].GetOrAddComponent<BlockParent>().setPosition((grid.y, grid.x)); //��Ŀ�� ������ ��� ���� BlockBase ����
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

        if (!getNeighbors(startArg.GetComponent<BlockParent>().getPosition()).Contains(nextArg.GetComponent<BlockParent>().getPosition()))
        {
            Debug.Log(getNeighbors(startArg.GetComponent<BlockParent>().getPosition()) + "     " + nextArg.GetComponent<BlockParent>().getPosition());
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

            string type0 = board[baseYX.y][baseYX.x].transform.GetChild(0).GetComponent<BlockChild>().getBlockType();
            string type1 = board[p1.y][p1.x].transform.GetChild(0).GetComponent<BlockChild>().getBlockType();
            string type2 = board[p2.y][p2.x].transform.GetChild(0).GetComponent<BlockChild>().getBlockType();

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



}
