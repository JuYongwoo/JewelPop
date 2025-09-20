using UnityEngine;

public abstract class BlockChild : MonoBehaviour
{

    [HideInInspector]
    private BlockType blockType; //GameManage에서 오브젝트 생성과 함께 JSON에서 받아온 타입이 대입

    public void SetBlockType(BlockType blockType)
    {
        this.blockType = blockType;
    }
    public BlockType GetBlockType()
    {
        return blockType;
    }

    public abstract void DestroySelf();// 블럭종류마다 파괴 모션이 다르므로 자식에서 구현 강제화


    protected void turnoff() // 삭제하지 않고 파괴 연출을 위해 게임에 지장 없도록 기능만 끈다.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.SetParent(null);// 부모 벗어나고 neighbor감지 안되도록
        GetComponent<Collider2D>().enabled = false; //터치 안되도록
    }
}
