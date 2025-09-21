using System.Collections;
using UnityEngine;



public class Basic : BlockChild
{


    public override void DestroySelf() //일반 블럭의 파괴 모션
    {
        GameObject go = Instantiate(ManagerObject.instance.resourceManager.blockCrush); //파괴 이펙트 소환 후 자신 파괴
        go.transform.position = transform.position;
        DestroyImmediate(gameObject);

    }

}