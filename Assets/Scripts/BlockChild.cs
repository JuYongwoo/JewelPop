using System.Collections;
using UnityEngine;



public abstract class BlockChild : MonoBehaviour
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

    public abstract void DestroySelf();// ���������� �ı� ����� �ٸ��Ƿ� �ڽĿ��� ���� ����ȭ


    protected void turnoff() // �������� �ʰ� �ı� ������ ���� ���ӿ� ���� ������ ��ɸ� ����.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.SetParent(null);// �θ� ����� neighbor���� �ȵǵ���
        GetComponent<Collider2D>().enabled = false; //��ġ �ȵǵ���
    }
}
