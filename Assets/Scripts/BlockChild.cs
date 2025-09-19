using UnityEngine;



public class BlockChild : MonoBehaviour
{

    public string blockType = "";


    public void setBlockType(string blockType)
    {
        this.blockType = blockType;
    }
    public string getBlockType()
    {
        return blockType;
    }
}
