using UnityEngine;



public class BlockBase : MonoBehaviour
{

    private (int y, int x) yx;
    private string blockType = "";

    public void setPosition((int y, int x) yx) //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
    {
        this.yx = yx;

    }


    public (int, int) getPosition() //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
    {
        return yx;
    }


    public bool IsSameColor(BlockBase other) //색을 통해서 블록 파괴, 핵심 함수
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
