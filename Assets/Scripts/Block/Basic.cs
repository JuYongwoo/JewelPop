using System.Collections;
using UnityEngine;



public class Basic : BlockChild
{


    public override void DestroySelf() //�Ϲ� ���� �ı� ���
    {
        GameObject go = Instantiate(ManagerObject.instance.resourceManager.blockCrush); //�ı� ����Ʈ ��ȯ �� �ڽ� �ı�
        go.transform.position = transform.position;
        DestroyImmediate(gameObject);

    }

}