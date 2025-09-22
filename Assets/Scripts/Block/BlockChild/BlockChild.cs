using UnityEngine;

public abstract class BlockChild : MonoBehaviour
{

    [HideInInspector]
    private string blockType; //GameManage���� ������Ʈ ������ �Բ� JSON���� �޾ƿ� Ÿ���� ����

    public void SetBlockType(string blockType)
    {
        this.blockType = blockType;
    }
    public string GetBlockType()
    {
        return blockType;
    }

    protected void Turnoff() // �ı� ������ ���� ���ӿ� ���� ������ �������� �ʰ� ��ɸ� ���� �Լ�.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.SetParent(null);// �θ� ����� neighbor���� �ȵǵ���
        GetComponent<Collider2D>().enabled = false; //��ġ �ȵǵ���
    }
}
