using System.Collections;
using UnityEngine;



public class CommonBlock : BlockChild, IMoveAndDesroyable
{

    [SerializeField]
    private float speed = 2f; //이동 속도


    public void DestroySelf() //일반 블럭의 파괴 모션
    {
        Instantiate(ManagerObject.instance.resourceManager.blockCrushFxPrefabs["BlockCrush_"+ GetBlockType()], transform.position, Quaternion.identity); //파괴 이펙트 소환 후 자신 파괴
        DestroyImmediate(gameObject);

    }


    public void Move(Transform targetParent)
    {

        var aPos = new Vector3(transform.parent.position.x, transform.transform.parent.position.y, transform.position.z);
        var bPos = new Vector3(targetParent.position.x, targetParent.position.y, transform.position.z);

        StartCoroutine(moveCoroutine(targetParent, aPos, bPos));
    }


    // 자식 Transform을 직접 받아서 이동 (부모에서 다시 GetChild(0) 하지 않음)
    private IEnumerator moveCoroutine(Transform targetParent, Vector3 startPos, Vector3 endPos)
    {
        transform.SetParent(targetParent, true);
        float t = 0f;
        float endDist = 0.01f * 0.01f;

        ManagerObject.instance.actionManager.setIsInMotion(true);
        ManagerObject.instance.actionManager.setIsBoardChanged(true);

        while (true)
        {
            if (transform == null) yield break; //파괴된 경우
            t += Time.deltaTime * speed;
            if(t > 1f) t = 1f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
//            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if ((transform.position - endPos).sqrMagnitude <= endDist) break;
            yield return null;
        }

        ManagerObject.instance.actionManager.setIsInMotion(false);
        transform.position = endPos;

    }

    public void moveAndBack(Transform targetParent)
    {
        var aPos = new Vector3(transform.parent.position.x, transform.transform.parent.position.y, transform.position.z);
        var bPos = new Vector3(targetParent.position.x, targetParent.position.y, transform.position.z);
        StartCoroutine(moveAndBackCoroutine(targetParent, aPos, bPos));
    }

    private IEnumerator moveAndBackCoroutine(Transform targetParent, Vector3 startPos, Vector3 endPos)
    {
        float t = 0f;
        float endDist = 0.01f * 0.01f;

        ManagerObject.instance.actionManager.setIsInMotion(true);
        ManagerObject.instance.actionManager.setIsBoardChanged(true);

        while (true)
        {
            if (transform == null) yield break; //파괴된 경우
            t += Time.deltaTime * speed;
            if (t > 1f) t = 1f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            //            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if ((transform.position - endPos).sqrMagnitude <= endDist) break;
            yield return null;
        }

        while (true)
        {
            if (transform == null) yield break; //파괴된 경우
            t += Time.deltaTime * speed;
            if (t > 1f) t = 1f;
            transform.position = Vector3.Lerp(endPos, startPos, t);
            //            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if ((transform.position - startPos).sqrMagnitude <= endDist) break;
            yield return null;
        }



        ManagerObject.instance.actionManager.setIsInMotion(false);
        transform.position = endPos;

    }


}