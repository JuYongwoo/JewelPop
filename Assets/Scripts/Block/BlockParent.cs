using UnityEngine;

public class BlockParent : MonoBehaviour
{

    private (int y, int x) yx;

    public void SetGridPositionYX((int y, int x) yx) //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
    {
        this.yx = yx;

    }

    public (int, int) GetGridPositionYX() // //
    {
        return yx;
    }


    public void SetUnityPositionYX((float y, float x) yx)
    {
        this.gameObject.transform.localPosition = new Vector2(yx.x, yx.y);
    }



}
