using UnityEngine;



public class BlockParent : MonoBehaviour
{

    private (int y, int x) yx;

    public void setPosition((int y, int x) yx) //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        this.yx = yx;

    }


    public (int, int) getPosition() //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        return yx;
    }



}
