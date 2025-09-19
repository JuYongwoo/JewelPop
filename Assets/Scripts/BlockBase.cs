using UnityEngine;



public class BlockBase : MonoBehaviour
{

    private (int y, int x) yx;
    private string blockType = "";

    public void setPosition((int y, int x) yx) //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        this.yx = yx;

    }


    public (int, int) getPosition() //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        return yx;
    }


    public bool IsSameColor(BlockBase other) //���� ���ؼ� ��� �ı�, �ٽ� �Լ�
    {
        return other != null && other.blockType == blockType;
    }

    public void setBlockType(string blockType)
    {

    }
    public string getBlockType()
    {
        return blockType;
    }
}
