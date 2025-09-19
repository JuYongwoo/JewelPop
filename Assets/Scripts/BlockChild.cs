using UnityEngine;



public class BlockChild : MonoBehaviour
{

    private string blockType = "";


    public void setBlockType(string blockType)
    {
        this.blockType = blockType;
    }
    public string getBlockType()
    {
        return blockType;
    }
}
