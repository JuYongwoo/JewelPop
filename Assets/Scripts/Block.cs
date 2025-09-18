using UnityEngine;



public class Block : MonoBehaviour
{

    private int y;
    private int x;
    private BlockColor colorIndex;

    public void setPosition(int y, int x) //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        this.y = y;
        this.x = x;

    }


    public (int, int) getPosition() //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        return (y, x);
    }

    public void SetColor(BlockColor color)
    {
        colorIndex = color;
        if (ManagerObject.instance.resourceManager.blockTextures != null
            && (int)color >= 0
            && (int)color < ManagerObject.instance.resourceManager.blockTextures.GetObjects<Sprite>().Length)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = ManagerObject.instance.resourceManager.blockTextures.GetObject<Sprite>((int)color);
        }


    }

    public bool IsSameColor(Block other) //���� ���ؼ� ��� �ı�, �ٽ� �Լ�
    {
        return other != null && other.colorIndex == colorIndex;
    }
}
