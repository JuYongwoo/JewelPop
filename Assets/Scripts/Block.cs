using UnityEngine;



public class Block : MonoBehaviour
{

    private int y;
    private int x;
    private string blockType;

    public void setGridPosition(int y, int x) //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
    {
        this.y = y;
        this.x = x;

    }


    public (int, int) getPosition() //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
    {
        return (y, x);
    }


    public bool IsSameColor(Block other) //색을 통해서 블록 파괴, 핵심 함수
    {
        return other != null && other.blockType == blockType;
    }
}
