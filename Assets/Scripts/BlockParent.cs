using UnityEngine;



public class BlockParent : MonoBehaviour
{

    private (int y, int x) yx;

    public void setPositionYX((int y, int x) yx) //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        this.yx = yx;

    }

    public void setUnityPositionYX((float y, float x) yx)
    {
        this.gameObject.transform.localPosition = new Vector2(yx.x, yx.y);
    }


    public (int, int) getPosition() //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        return yx;
    }



}
