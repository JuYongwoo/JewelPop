using System.Collections;
using UnityEngine;



public class Basic : BlockChild
{


    public override void DestroySelf() //�Ϲ� ���� �ı� ���
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