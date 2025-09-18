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
        //JSON �б�
        level = JsonUtility.FromJson<JSONVars>(ManagerObject.instance.resourceManager.mapJSON.text);
        
        setBlocks();
        setBlocksBack();
        MoveMiddleBlockToOrigin();
    }

    private void setBlocks()
    {
        foreach (var cell in level.grids)
        {
            // �� Ȯ��
            while (board.Count <= cell.y)
                board.Add(new List<GameObject>());

            // �� Ȯ��
            while (board[cell.y].Count <= cell.x)
                board[cell.y].Add(null);

            // �θ� ������Ʈ ������ ����
            if (board[cell.y][cell.x] == null)
            {
                board[cell.y][cell.x] = Object.Instantiate(ManagerObject.instance.resourceManager.blockParentObjectPrefab);
                Object.Instantiate(ManagerObject.instance.resourceManager.blockBackPrefab, board[cell.y][cell.x].transform);
            }
            // ��� ���� (�θ� = parentObject)
            Object.Instantiate(
                ManagerObject.instance.resourceManager.blockPrefab,
                board[cell.y][cell.x].transform
            );

            // ��ǥ/�̸� ����
            Vector3 lp = new Vector3(
                cell.x * xStep,
                -cell.y * yStep + ((cell.x % 2 == 1) ? yStep * 0.5f : 0f), //����Ƽ�� y��ǥ�� ���� ���� ���� ���� ������ �迭 �� 11�� ������� �Ʒ��� ����������
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





}
