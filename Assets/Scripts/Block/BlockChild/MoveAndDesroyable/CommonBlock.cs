using System;
using System.Collections;
using UnityEngine;

public class CommonBlock : BlockChild, IMoveAndDesroyable
{
    [SerializeField]
    private float speed = 2f;

    public void DestroySelf()
    {

        Instantiate(AppManager.instance.resourceManager.blockCrushFxPrefabsHandles[Enum.Parse<BlockCrushFXPrefabs>("BlockCrush_" + GetBlockType())].Result, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Move(Transform targetParent)
    {
        var aPos = new Vector3(transform.parent.position.x, transform.transform.parent.position.y, transform.position.z);
        var bPos = new Vector3(targetParent.position.x, targetParent.position.y, transform.position.z);
        StartCoroutine(MoveCoroutine(targetParent, aPos, bPos));
    }

    private IEnumerator MoveCoroutine(Transform targetParent, Vector3 startPos, Vector3 endPos)
    {
        transform.SetParent(targetParent, true);
        float t = 0f;
        float endDist = 0.01f * 0.01f;

        AppManager.instance.actionManager.setIsInMotionM(true);
        AppManager.instance.actionManager.setIsBoardChangedM(true);

        while (true)
        {
            if (transform == null) yield break;
            t += Time.deltaTime * speed;
            if (t > 1f) t = 1f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            if ((transform.position - endPos).sqrMagnitude <= endDist) break;
            yield return null;
        }

        AppManager.instance.actionManager.setIsInMotionM(false);
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

        AppManager.instance.actionManager.setIsInMotionM(true);
        AppManager.instance.actionManager.setIsBoardChangedM(true);

        while (true)
        {
            if (transform == null) yield break;
            t += Time.deltaTime * speed;
            if (t > 1f) t = 1f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            if ((transform.position - endPos).sqrMagnitude <= endDist)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (transform == null) yield break;
            t2 += Time.deltaTime * speed;
            if (t2 > 1f) t2 = 1f;
            transform.position = Vector3.Lerp(endPos, startPos, t2);
            if ((transform.position - startPos).sqrMagnitude <= endDist) break;
            yield return null;
        }

        AppManager.instance.actionManager.setIsInMotionM(false);
        transform.position = startPos;
    }

    public void Turnoff()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.SetParent(null);
        GetComponent<Collider2D>().enabled = false;
    }
}
