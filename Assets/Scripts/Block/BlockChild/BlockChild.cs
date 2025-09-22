using UnityEngine;

public abstract class BlockChild : MonoBehaviour
{

    [HideInInspector]
    private string blockType; //GameManage에서 오브젝트 생성과 함께 JSON에서 받아온 타입이 대입

    public void SetBlockType(string blockType)
    {
        this.blockType = blockType;
    }
    public string GetBlockType()
    {
        return blockType;
    }

    protected void Turnoff() // 파괴 연출을 위해 게임에 지장 없도록 삭제하지 않고 기능만 끄는 함수.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.SetParent(null);// 부모 벗어나고 neighbor감지 안되도록
        GetComponent<Collider2D>().enabled = false; //터치 안되도록
    }
}
