using System.Collections;
using System.Net.Sockets;
using UnityEngine;



public class CommonBlock : BlockChild, IMoveAndDesroyable
{


    public void DestroySelf() //일반 블럭의 파괴 모션
    {
        GameObject go = Instantiate(ManagerObject.instance.resourceManager.blockCrushFxPrefab, transform.position, Quaternion.identity); //파괴 이펙트 소환 후 자신 파괴
        DestroyImmediate(gameObject);

    }



    public void move(Transform targetParent)
    {

        var aPos = new Vector3(transform.parent.position.x, transform.transform.parent.position.y, transform.position.z);
        var bPos = new Vector3(targetParent.position.x, targetParent.position.y, transform.position.z);

        StartCoroutine(moveEnu(targetParent, aPos, bPos));
    }


    // 자식 Transform을 직접 받아서 이동 (부모에서 다시 GetChild(0) 하지 않음)
    private IEnumerator moveEnu(Transform targetParent, Vector3 startPos, Vector3 endPos)
    {
        transform.SetParent(targetParent, true);
        float t = 0f, speed = 5f, snap2 = 0.01f * 0.01f;

        ManagerObject.instance.actionManager.setIsInMotion(true);
        ManagerObject.instance.actionManager.setIsBoardChanged(true);

        while (true)
        {
            if (transform == null) yield break; //파괴된 경우
            t += Time.deltaTime * speed; if (t > 1f) t = 1f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            if ((transform.position - endPos).sqrMagnitude <= snap2) break;
            yield return null;
        }

        ManagerObject.instance.actionManager.setIsInMotion(false);
        transform.position = endPos;

    }

}