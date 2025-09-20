using UnityEngine;

public abstract class BlockChild : MonoBehaviour
{

    [HideInInspector]
    private BlockType blockType; //GameManage���� ������Ʈ ������ �Բ� JSON���� �޾ƿ� Ÿ���� ����

    public void SetBlockType(BlockType blockType)
    {
        this.blockType = blockType;
    }
    public BlockType GetBlockType()
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
