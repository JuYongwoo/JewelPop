using UnityEngine;



public class BlockBase : MonoBehaviour
{

    private int y;
    private int x;
    private string blockType;

    public void setGridPosition(int y, int x) //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        this.y = y;
        this.x = x;

    }


    public (int, int) getPosition() //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        return (y, x);
    }


    public bool IsSameColor(BlockBase other) //���� ���ؼ� ��� �ı�, �ٽ� �Լ�
    {
        return other != null && other.blockType == blockType;
    }
}
