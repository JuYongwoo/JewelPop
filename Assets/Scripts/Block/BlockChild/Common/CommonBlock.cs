using System;
using System.Collections;
using UnityEngine;

public class CommonBlock : BlockChild, IMoveAndDesroyable
{
    [SerializeField]
    private float speed = 2f; //이동 속도

    public void DestroySelf() //일반 블럭의 파괴 모션
    {
        Instantiate(ManagerObject.instance.resourceManager.blockCrushFxPrefabs[Enum.Parse<BlockCrushFXPrefabs>("BlockCrush_" + GetBlockType())], transform.position, Quaternion.identity); //파괴 이펙트 소환 후 자신 파괴
        AudioSource.PlayClipAtPoint(ManagerObject.instance.resourceManager.gamsSFXPrefabs[SFX.Block3SFX], transform.position);
        DestroyImmediate(gameObject);
    }

    public void Move(Transform targetParent)
    {
        var aPos = new Vector3(transform.parent.position.x, transform.transform.parent.position.y, transform.position.z);
        var bPos = new Vector3(targetParent.position.x, targetParent.position.y, transform.position.z);
        StartCoroutine(MoveCoroutine(targetParent, aPos, bPos));
    }

    // 자식 Transform을 직접 받아서 이동 (부모에서 다시 GetChild(0) 하지 않음)
    private IEnumerator MoveCoroutine(Transform targetParent, Vector3 startPos, Vector3 endPos)
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
            if (t > 1f) t = 1f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            //            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if ((transform.position - endPos).sqrMagnitude <= endDist) break;
            yield return null;
        }

        ManagerObject.instance.actionManager.setIsInMotion(false);
        transform.position = endPos;
    }

    public void MoveAndBack(Transform targetParent)
    {
        var aPos = new Vector3(transform.parent.position.x, transform.transform.parent.position.y, transform.position.z);
        var bPos = new Vector3(targetParent.position.x, targetParent.position.y, transform.position.z);
        StartCoroutine(MoveAndBackCoroutine(targetParent, aPos, bPos));
    }

    private IEnumerator MoveAndBackCoroutine(Transform targetParent, Vector3 startPos, Vector3 endPos)
    {
        float t = 0f;
        float t2 = 0f;
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
            if ((transform.position - endPos).sqrMagnitude <= endDist)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (transform == null) yield break; //파괴된 경우
            t2 += Time.deltaTime * speed;
            if (t2 > 1f) t2 = 1f;
            transform.position = Vector3.Lerp(endPos, startPos, t2);
            //            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if ((transform.position - startPos).sqrMagnitude <= endDist) break;
            yield return null;
        }

        ManagerObject.instance.actionManager.setIsInMotion(false);
        transform.position = startPos;
    }

    public void Turnoff() // 파괴 연출을 위해 게임에 지장 없도록 삭제하지 않고 기능만 끄는 함수.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.SetParent(null);// 부모 벗어나고 neighbor감지 안되도록
        GetComponent<Collider2D>().enabled = false; //터치 안되도록
    }
}
