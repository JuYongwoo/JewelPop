using System.Collections;
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

    public void destroySelf()
    {
        StartCoroutine(blockCrushMotion());
    }

    IEnumerator blockCrushMotion()
    {

        Instantiate(ManagerObject.instance.resourceManager.blockCrush, transform);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);

    }
}
