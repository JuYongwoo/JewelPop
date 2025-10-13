using UnityEngine;

public abstract class BlockChild : MonoBehaviour, PooledObejct
{

    [HideInInspector]
    private BlockPrefabs blockType; //GameManage에서 오브젝트 생성과 함께 JSON에서 받아온 타입이 대입

    public void PoolStart() { }

    public void PoolDestroy() { }
    
    public void SetBlockType(BlockPrefabs blockType)
    {
        this.blockType = blockType;
    }
    public BlockPrefabs GetBlockType()
    {
        return blockType;
    }


}
