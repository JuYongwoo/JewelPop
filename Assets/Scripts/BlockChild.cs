using System.Collections;
using UnityEngine;



public class BlockChild : MonoBehaviour
{

    [HideInInspector]
    private string blockType = ""; //GameManage���� ������Ʈ ������ �Բ� JSON���� �޾ƿ� Ÿ���� ����

    public void SetBlockType(string blockType)
    {
        this.blockType = blockType;
    }
    public string GetBlockType()
    {
        return blockType;
    }

    public virtual void DestroySelf() // ������ �ı� ����� �ٸ� �� �����Ƿ� virtual
    {
        Destroy(gameObject);
    }

    public void turnoff() // �������� �ʰ� �ı� ������ ���� ���ӿ� ���� ������ ��ɸ� ����.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.SetParent(null);// �θ� ����� neighbor���� �ȵǵ���
        GetComponent<Collider2D>().enabled = false; //��ġ �ȵǵ���
    }
}
