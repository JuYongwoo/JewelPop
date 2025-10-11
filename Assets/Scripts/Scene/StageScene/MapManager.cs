using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct YX
{
    public YX(int y, int x)
    {
        this.y = y;
        this.x = x;
    }
    public int y;
    public int x;
}

public class MapManager
{
    private enum BoardFSMState
    {
        DroppingEnter,
        DroppingUpdate,
        DroppingExit,

        DestroyingEnter,
        DestroyingUpdate,
        DestroyingExit,

        SpawningEnter,
        SpawningUpdate,
        SpawningExit
    }

    private BoardFSMState fsmState;
    private int _lastDelsCount = 0;
    private int _lastTopsCount = 0;

    private void ChangeState(BoardFSMState next)
    {
        fsmState = next;
    }

    public void OnUpdate()
    {
        if (!isChanged) return;

        switch (fsmState)
        {
            case BoardFSMState.DroppingEnter:
                ChangeState(BoardFSMState.DroppingUpdate);
                break;

            case BoardFSMState.DroppingUpdate:
                DropAllBlocks();
                if (!isInMotion)
                    ChangeState(BoardFSMState.DroppingExit);
                break;

            case BoardFSMState.DroppingExit:
                ChangeState(BoardFSMState.DestroyingEnter);
                break;

            case BoardFSMState.DestroyingEnter:
                ChangeState(BoardFSMState.DestroyingUpdate);
                break;

            case BoardFSMState.DestroyingUpdate:
                var dels = checkAllChains();
                _lastDelsCount = dels.Count;
                if (_lastDelsCount != 0)
                    DestroyBlocks(dels);
                ChangeState(BoardFSMState.DestroyingExit);
                break;

            case BoardFSMState.DestroyingExit:
                ChangeState(BoardFSMState.SpawningEnter);
                break;

            case BoardFSMState.SpawningEnter:
                ChangeState(BoardFSMState.SpawningUpdate);
                break;

            case BoardFSMState.SpawningUpdate:
                var tops = checkEmptyTops();
                _lastTopsCount = tops.Count;
                if (_lastTopsCount != 0)
                    AddNewBlocks(tops);

                if (_lastDelsCount == 0 && _lastTopsCount == 0)
                    isChanged = false;

                ChangeState(BoardFSMState.SpawningExit);
                break;

            case BoardFSMState.SpawningExit:
                ChangeState(BoardFSMState.DroppingEnter);
                break;
        }
    }

    private Dictionary<YX, BlockParent> board = new Dictionary<YX, BlockParent>();

    private const float xStep = 0.6f;
    private const float yStep = 0.7f;

    public bool isInMotion = false;
    public bool isChanged { get; set; }

    (int dy, int dx)[] oddXDirections = new (int, int)[] {
        (-1, -1), (0, 1),
        (-1, 1), (0, -1),
        (-1, 0), (1, 0),
    };

    (int dy, int dx)[] evenXDirections = new (int, int)[] {
        (1, -1), (0, 1),
        (1, 1), (0, -1),
        (1, 0), (-1, 0),
    };

    public void Init(JSONVars JSONvars)
    {
        SetBlocks(JSONvars);
        MoveMiddleBlockToOrigin();

        // FSM 초기 상태
        ChangeState(BoardFSMState.DroppingEnter);

        GameManager.instance.actionManager.inputBlockChangeAction -= InputBlockChangeEvent;
        GameManager.instance.actionManager.inputBlockChangeAction += InputBlockChangeEvent;

        GameManager.instance.actionManager.getIsInMotion -= getIsInMotion;
        GameManager.instance.actionManager.getIsInMotion += getIsInMotion;

        GameManager.instance.actionManager.setIsInMotion -= setIsInMotion;
        GameManager.instance.actionManager.setIsInMotion += setIsInMotion;

        GameManager.instance.actionManager.getIsBoardChanged -= getIsBoardChanged;
        GameManager.instance.actionManager.getIsBoardChanged += getIsBoardChanged;

        GameManager.instance.actionManager.setIsBoardChanged -= setIsBoardChanged;
        GameManager.instance.actionManager.setIsBoardChanged += setIsBoardChanged;
    }

    public void OnDestroy()
    {
        GameManager.instance.actionManager.inputBlockChangeAction -= InputBlockChangeEvent;
        GameManager.instance.actionManager.getIsInMotion -= getIsInMotion;
        GameManager.instance.actionManager.setIsInMotion -= setIsInMotion;
        GameManager.instance.actionManager.getIsBoardChanged -= getIsBoardChanged;
        GameManager.instance.actionManager.setIsBoardChanged -= setIsBoardChanged;
    }

    private bool getIsInMotion()
    {
        return isInMotion;
    }

    private void setIsInMotion(bool a)
    {
        isInMotion = a;
    }

    private bool getIsBoardChanged()
    {
        return isChanged;
    }

    private void setIsBoardChanged(bool a)
    {
        isChanged = a;
    }


    private void SetBlocks(JSONVars jsonVars)
    {
        foreach (var grid in jsonVars.grids)
        {
            YX yx = new YX(grid.y, grid.x);
            board.Add(yx, GameManager.instance.poolManager.Spawn(GameManager.instance.resourceManager.blockParentObjectHandle.Result).GetComponent<BlockParent>());
            board[yx].name = $"y{grid.y}x{grid.x}";
            board[yx].SetGridPositionYX(yx);
            board[yx].SetUnityPositionYX(grid.x % 2 == 1
                ? (-grid.y * yStep + yStep * 0.5f, grid.x * xStep)
                : (-grid.y * yStep, grid.x * xStep));

            GameObject child = GameManager.instance.poolManager.Spawn(GameManager.instance.resourceManager.blockPrefabsHandles[Enum.Parse<BlockPrefabs>(grid.type)].Result, board[yx].transform);
            child.GetComponent<BlockChild>().SetBlockType(Enum.Parse<BlockPrefabs>(grid.type));
        }
    }

    private void MoveMiddleBlockToOrigin()
    {
        Vector2 centroid = Vector2.zero;
        foreach (var go in board.Values)
            centroid += (Vector2)go.transform.localPosition;
        centroid /= board.Count;

        BlockParent midBlock = board.Values
            .OrderBy(blockParent => Vector2.SqrMagnitude((Vector2)blockParent.transform.localPosition - centroid))
            .First();

        Vector2 offset = -midBlock.transform.localPosition;
        foreach (var go in board.Values)
            go.transform.localPosition += new Vector3(offset.x, offset.y, 0);
    }

    private void InputBlockChangeEvent(GameObject startChild, GameObject endChild)
    {
        YX startYX = startChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX();
        YX endYX = endChild.transform.parent.GetComponent<BlockParent>().GetGridPositionYX();
        if (!GetNeighbors(startYX).Contains(endYX))
            return;

        Dictionary<YX, BlockParent> fakeBoard = new Dictionary<YX, BlockParent>(board);

        (fakeBoard[startYX], fakeBoard[endYX]) = (fakeBoard[endYX], fakeBoard[startYX]);

        HashSet<YX> cates = new HashSet<YX>();
        foreach (var n in GetNeighbors(startYX, fakeBoard)) cates.Add(n);
        foreach (var n in GetNeighbors(endYX, fakeBoard)) cates.Add(n);

        bool canBurst = false;
        foreach (var c in cates)
        {
            var bursts = CheckIsBurstable(c, fakeBoard);
            if (bursts.Count != 0) { canBurst = true; break; }
        }

        var startParentTransform = startChild.transform.parent;
        var endParentTransform = endChild.transform.parent;

        if (canBurst)
        {
            startChild.GetComponent<IMoveAndDestroyable>().Move(endParentTransform);
            endChild.GetComponent<IMoveAndDestroyable>().Move(startParentTransform);
        }
        else
        {
            startChild.GetComponent<IMoveAndDestroyable>().MoveAndBack(endParentTransform);
            endChild.GetComponent<IMoveAndDestroyable>().MoveAndBack(startParentTransform);
        }
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
        HashSet<ISpecial> specials = new HashSet<ISpecial>();

        foreach (var a in dels)
        {
            if (IsValid(a))
            {
                foreach (var n in GetNeighbors(a))
                {
                    if (board[n].transform.GetChild(0).GetComponent<ISpecial>() != null)
                        specials.Add(board[n].transform.GetChild(0).GetComponent<ISpecial>());
                }

                board[a].transform.GetChild(0).GetComponent<IMoveAndDestroyable>().DestroySelf();
            }
        }

        foreach (var special in specials)
            special.SpecialMotion();
    }

    private void AddNewBlocks(List<YX> tops)
    {
        foreach (var pos in tops)
        {
            var rd = (BlockPrefabs)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BlockPrefabs)).Length-1); // 조커 제외 끝 1 감소
            GameObject child = GameManager.instance.poolManager.Spawn(GameManager.instance.resourceManager.blockPrefabsHandles[rd].Result, board[pos].transform);
            child.GetComponent<BlockChild>().SetBlockType(rd);
        }
    }

    private List<YX> checkEmptyTops()
    {
        List<YX> tops = new List<YX>();
        foreach (var key in board.Keys)
        {
            if (IsTop(key) && board[key].transform.childCount == 0)
                tops.Add(key);
        }
        return tops;
    }

    private void DropAllBlocks()
    {
        var keys = board.Keys.OrderByDescending(k => k.y).ToList();
        foreach (var key in keys)
        {
            if (!IsValid(key)) continue;

            var block = board[key];
            int y = key.y;
            int x = key.x;
            int newY = y;

            while (board.ContainsKey(new YX(newY + 1, x)) && board[new YX(newY + 1, x)].transform.childCount == 0)
            {
                newY++;
            }

            if (newY != y)
            {
                block.transform.GetChild(0).GetComponent<IMoveAndDestroyable>().Move(board[new YX(newY, x)].transform);
            }
        }
    }

    private bool IsValid(YX pos, Dictionary<YX, BlockParent> pBoard = null)
    {
        if (pBoard == null) pBoard = board;

        return pBoard.ContainsKey(pos)
            && pBoard[pos].transform.childCount != 0
            && pBoard[pos].GetComponent<IMoveAndDestroyable>() == null;
    }

    private bool IsTop(YX yx)
    {
        return !board.ContainsKey(new YX(yx.y - 1, yx.x));
    }

    private List<YX> GetNeighbors(YX baseYX, Dictionary<YX, BlockParent> pBoard = null)
    {
        if (pBoard == null) pBoard = board;
        List<YX> neighbors = new List<YX>();

        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i++)
        {
            YX newYX = new YX(baseYX.y + directions[i].dy, baseYX.x + directions[i].dx);
            if (IsValid(newYX, pBoard))
                neighbors.Add(newYX);
        }

        return neighbors;
    }

    private HashSet<YX> CheckIsBurstable(YX baseYX, Dictionary<YX, BlockParent> pBoard = null)
    {
        if (pBoard == null) pBoard = board;

        HashSet<YX> burstables = new HashSet<YX>();
        (int dy, int dx)[] directions = (baseYX.x % 2 == 1) ? oddXDirections : evenXDirections;

        for (int i = 0; i < directions.Length; i += 2)
        {
            YX p1 = new YX(baseYX.y + directions[i].dy, baseYX.x + directions[i].dx);
            YX p2 = new YX(baseYX.y + directions[i + 1].dy, baseYX.x + directions[i + 1].dx);

            if (!IsValid(baseYX, pBoard) || !IsValid(p1, pBoard) || !IsValid(p2, pBoard))
                continue;

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
        return burstables;
    }
}
