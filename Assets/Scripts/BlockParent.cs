using UnityEngine;



public class BlockParent : MonoBehaviour
{

    private (int y, int x) yx;

    public void setPositionYX((int y, int x) yx) //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
    {
        this.yx = yx;

    }

    public void setUnityPositionYX((float y, float x) yx)
    {
        this.gameObject.transform.localPosition = new Vector2(yx.x, yx.y);
    }


    public (int, int) getPosition() //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
    {
        return yx;
    }



}
