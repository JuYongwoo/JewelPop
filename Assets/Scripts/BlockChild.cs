using System.Collections;
using UnityEngine;



public class BlockChild : MonoBehaviour
{

    [HideInInspector]
    private string blockType = ""; //GameManage에서 오브젝트 생성과 함께 JSON에서 받아온 타입이 대입

    public void SetBlockType(string blockType)
    {
        this.blockType = blockType;
    }
    public string GetBlockType()
    {
        return blockType;
    }

    public virtual void DestroySelf() // 블럭마다 파괴 모션이 다를 수 있으므로 virtual
    {
        Destroy(gameObject);
    }

    public void turnoff() // 삭제하지 않고 파괴 연출을 위해 게임에 지장 없도록 기능만 끈다.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.SetParent(null);// 부모 벗어나고 neighbor감지 안되도록
        GetComponent<Collider2D>().enabled = false; //터치 안되도록
    }
}
