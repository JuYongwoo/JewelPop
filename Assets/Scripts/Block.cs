using UnityEngine;



public class Block : MonoBehaviour
{

    private int y;
    private int x;
    private BlockColor colorIndex;

    public void setPosition(int y, int x) //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
    {
        this.y = y;
        this.x = x;

    }


    public (int, int) getPosition() //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
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

    public bool IsSameColor(Block other) //색을 통해서 블록 파괴, 핵심 함수
    {
        return other != null && other.colorIndex == colorIndex;
    }
}
