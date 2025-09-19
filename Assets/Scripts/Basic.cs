using System.Collections;
using UnityEngine;



public class Basic : BlockChild
{


    public override void DestroySelf() //일반 블럭의 파괴 모션
    {
        StartCoroutine(BlockCrushMotion());
    }

    private IEnumerator BlockCrushMotion()
    {
        turnoff();
        Instantiate(ManagerObject.instance.resourceManager.blockCrush, transform);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);

    }

}